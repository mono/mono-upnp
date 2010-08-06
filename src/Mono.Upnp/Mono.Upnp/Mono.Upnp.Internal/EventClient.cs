//
// EventClient.cs
//
// Author:
//   Scott Thomas <lunchtimemama@gmail.com>
//
// Copyright (C) 2008 S&S Black Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Net;
using System.Net.Sockets;
using System.Xml;

using Mono.Upnp.Control;

namespace Mono.Upnp.Internal
{
    class EventClient : IDisposable
    {
        static readonly Random random = new Random ();
        static int id;

        readonly IMap<string, StateVariable> state_variables;
        readonly Uri url;
        readonly TimeoutDispatcher dispatcher = new TimeoutDispatcher ();
        readonly string prefix = GeneratePrefix ();
        
        volatile bool started;
        volatile bool confidently_subscribed;
        
        HttpListener listener;
        uint expire_timeout_id;
        uint renew_timeout_id;
        string subscription_uuid;
        int ref_count;

        public EventClient (IMap<string, StateVariable> stateVariable, Uri url)
        {
            this.state_variables = stateVariable;
            this.url = url;
        }

        static string GeneratePrefix ()
        {
            // FIXME configure the network interface
            foreach (var address in Dns.GetHostAddresses (Dns.GetHostName ())) {
                if (address.AddressFamily == AddressFamily.InterNetwork) {
                    return string.Format (
                        "http://{0}:{1}/upnp/event-subscriber/{2}/", address, random.Next (1024, 5000), id++);
                }
            }
            
            return null;
        }
        
        public void Ref ()
        {
            if (ref_count == 0) {
                Start ();
            }
            ref_count++;
        }
        
        public void Unref ()
        {
            ref_count--;
            if (ref_count == 0) {
                Stop ();
            }
        }

        public void Start ()
        {
            if (started) {
                return;
            }
            StartListening ();
            Subscribe ();

            started = true;
        }

        void StartListening ()
        {
            if (listener == null) {
                listener = new HttpListener { IgnoreWriteExceptions = true };
                listener.Prefixes.Add (prefix);
            }
            lock (listener) {
                listener.Start ();
                listener.BeginGetContext (OnGotContext, null);
            }
        }

        void OnGotContext (IAsyncResult asyncResult)
        {
            lock (listener) {
                if (!listener.IsListening) {
                    return;
                }
                var context = listener.EndGetContext (asyncResult);
                try {
                    using (var stream = context.Request.InputStream) {
                        using (var reader = XmlReader.Create (stream)) {
                            if (reader.MoveToContent () != XmlNodeType.Element) {
                                Log.Warning ("The event update has no root XML element.");
                            } else if (reader.LocalName != "propertyset"
                                && reader.NamespaceURI != Protocol.EventSchema)
                            {
                                Log.Warning ("The event update has no propertyset.");
                            } else if (!reader.ReadToDescendant ("property", Protocol.EventSchema)) {
                                Log.Warning ("The event update has an empty propertyset.");
                            } else {
                                do {
                                    reader.Read ();
                                    StateVariable state_variable;
                                    if (state_variables.TryGetValue (reader.Name, out state_variable)) {
                                        state_variable.Value = reader.ReadElementContentAsString ();
                                    } else {
                                        Log.Warning (string.Format (
                                            "{0} published an event update to {1} " +
                                            "which includes unknown state variable {2}.",
                                            context.Request.RemoteEndPoint, context.Request.Url, reader.Name));
                                    }
                                } while (reader.ReadToNextSibling ("property", Protocol.EventSchema));
                            }
                        }
                    }
                } catch (Exception e) {
                    Log.Exception (string.Format ("There was a problem processing an event update from {0} to {1}.",
                        context.Request.RemoteEndPoint, context.Request.Url), e);
                }
                context.Response.Close ();
                listener.BeginGetContext (OnGotContext, null);
            }
        }

        void StopListening ()
        {
            lock (listener) {
                listener.Stop ();
            }
        }

        void Subscribe ()
        {
            var request = WebRequest.Create (url);
            request.Method = "SUBSCRIBE";
            request.Headers.Add ("USERAGENT", Protocol.UserAgent);
            request.Headers.Add ("CALLBACK", string.Format ("<{0}>", prefix));
            request.Headers.Add ("NT", "upnp:event");
            request.Headers.Add ("TIMEOUT", "Second-1800");
            
            lock (this) {
                request.BeginGetResponse (OnSubscribeResponse, request);
                expire_timeout_id = dispatcher.Add (TimeSpan.FromSeconds (30), OnSubscribeTimeout, request);
                confidently_subscribed = false;
            }
        }

        bool OnSubscribeTimeout (object state, ref TimeSpan interval)
        {
            lock (this) {
                expire_timeout_id = 0;
                if (!confidently_subscribed) {
                    var request = (WebRequest)state;
                    request.Abort ();
                    Stop ();
                    // TODO retry
                    Log.Error ("Failed to subscribe or renew. The server did not respond in 30 seconds.");
                    //controller.Description.CheckDisposed ();
                }
            }
            return false;
        }

        void OnSubscribeResponse (IAsyncResult asyncResult)
        {
            lock (this) {
                if (expire_timeout_id != 0) {
                    dispatcher.Remove (expire_timeout_id);
                }
                var request = (WebRequest)asyncResult.AsyncState;
                try {
                    using (var response = (HttpWebResponse)request.EndGetResponse (asyncResult)) {
                        if (response.StatusCode == HttpStatusCode.GatewayTimeout) {
                            throw new WebException ("", WebExceptionStatus.Timeout);
                        } else if (response.StatusCode != HttpStatusCode.OK) {
                            throw new WebException ();
                        } else if (!started) {
                            return;
                        }
                        confidently_subscribed = true;
                        subscription_uuid = response.Headers["SID"];
                        var timeout_header = response.Headers["TIMEOUT"];
                        if (timeout_header == "infinate") {
                            return;
                        }
                        var timeout = TimeSpan.FromSeconds (double.Parse (timeout_header.Substring (7)));
                        if (timeout > TimeSpan.FromMinutes (2)) {
                            timeout -= TimeSpan.FromMinutes (2);
                        }
                        renew_timeout_id = dispatcher.Add (timeout, OnRenewTimeout);
                    }
                } catch (WebException e) {
                    Stop ();
                    // TODO more info
                    Log.Exception (new UpnpException ("Failed to subscribe or renew.", e));
                    if (e.Status == WebExceptionStatus.Timeout) {
                        //controller.Description.CheckDisposed ();
                    }
                }
            }
        }

        bool OnRenewTimeout (object state, ref TimeSpan interval)
        {
            lock (this) {
                renew_timeout_id = 0;
                if (started) {
                    Renew ();
                }
                return false;
            }
        }

        void Renew ()
        {
            lock (this) {
                var request = WebRequest.Create (url);
                request.Method = "SUBSCRIBE";
                request.Headers.Add ("SID", subscription_uuid);
                request.Headers.Add ("TIMEOUT", "Second-1800");
                request.BeginGetResponse (OnSubscribeResponse, request);
                expire_timeout_id = dispatcher.Add (TimeSpan.FromSeconds (30), OnSubscribeTimeout, request);
                confidently_subscribed = false;
            }
        }

        void Unsubscribe ()
        {
            lock (this) {
                var request = WebRequest.Create (url);
                request.Method = "UNSUBSCRIBE";
                request.Headers.Add ("SID", subscription_uuid);
                request.BeginGetResponse (OnUnsubscribeResponse, request);
                confidently_subscribed = false;
            }
        }

        void OnUnsubscribeResponse (IAsyncResult asyncResult)
        {
            try {
                ((WebRequest)asyncResult).EndGetResponse (asyncResult).Close ();
            } catch {
            }
        }

        public void Stop ()
        {
            if (!started) {
                return;
            }

            if (renew_timeout_id != 0) {
                dispatcher.Remove (renew_timeout_id);
                renew_timeout_id = 0;
            }

            if (expire_timeout_id != 0) {
                dispatcher.Remove (expire_timeout_id);
                expire_timeout_id = 0;
            }

            lock (this) {
                if (confidently_subscribed) {
                    Unsubscribe ();
                }
            }
            
            StopListening ();

            started = false;
        }

        public void Dispose ()
        {
            Stop ();
            lock (listener) {
                listener.Close ();
            }
        }
    }
}
