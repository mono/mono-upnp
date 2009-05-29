//
// EventServer.cs
//
// Author:
//   Scott Peterson <lunchtimemama@gmail.com>
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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

using Mono.Upnp.Control;

namespace Mono.Upnp.Internal
{
    class EventServer : UpnpServer
    {
        class Subscription
        {
            public Uri Callback;
            public string Sid;
            public uint TimeoutId;
            public uint Seq;
            public int ConnectFailures;

            public Subscription (Uri callback, string uuid)
            {
                Callback = callback;
                Sid = uuid;
            }
        }

        private readonly ServiceController controller;
        private readonly object mutex = new object ();
        private readonly Dictionary<string, Subscription> subscribers = new Dictionary<string, Subscription> ();
        private readonly TimeoutDispatcher dispatcher = new TimeoutDispatcher ();

        public EventServer (ServiceController service, Uri url)
            : base (url)
        {
            this.controller = service;
        }

        private readonly Stack<Subscription> dead_subscribers = new Stack<Subscription> ();

        public void PublishUpdates ()
        {
            lock (mutex) {
                foreach (Subscription subscriber in subscribers.Values) {
                    try {
                        PublishUpdates (subscriber);
                        subscriber.ConnectFailures = 0;
                    } catch (WebException e) {
                        // TODO this for all exception types?
                        if (e.Status == WebExceptionStatus.ConnectFailure) {
                            subscriber.ConnectFailures++;
                        }
                        if (subscriber.ConnectFailures == 2) {
                            dead_subscribers.Push (subscriber);
                        }
                    } catch {
                    }
                }
                while (dead_subscribers.Count > 0) {
                    subscribers.Remove (dead_subscribers.Pop ().Sid);
                }
            }
        }

        private void PublishUpdates (Subscription subscriber)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create (subscriber.Callback);
            request.Method = "NOTIFY";
            request.ContentType = "text/xml";
            request.Headers.Add ("NT", "upnp:event");
            request.Headers.Add ("NTS", "upnp:propchange");
            request.Headers.Add ("SID", subscriber.Sid);
            request.Headers.Add ("SEQ", subscriber.Seq.ToString ());
            request.KeepAlive = false;
            subscriber.Seq++;
            Stream stream = request.GetRequestStream ();
            XmlWriter writer = XmlWriter.Create (stream);
            writer.WriteStartDocument ();
            writer.WriteStartElement ("e", "propertyset", Protocol.EventUrn);
            foreach (StateVariable state_variable in controller.StateVariables.Values) {
                if (state_variable.SendsEvents) {
                    writer.WriteStartElement ("property", Protocol.EventUrn);
                    writer.WriteStartElement (state_variable.Name);
                    //writer.WriteValue (state_variable.Value);
                    writer.WriteEndElement ();
                    writer.WriteEndElement ();
                }
            }
            writer.WriteEndElement ();
            writer.WriteEndDocument ();
            writer.Close ();
            stream.Close ();
            request.BeginGetResponse (OnGotResponse, request);
        }

        private void OnGotResponse (IAsyncResult async)
        {
            HttpWebRequest request = (HttpWebRequest)async.AsyncState;
            try {
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse (async);
                response.Close ();
            } catch {
            }
        }

        protected override void HandleContext (HttpListenerContext context)
        {
            lock (mutex) {
                if (context.Request.HttpMethod.ToUpper () == "SUBSCRIBE") {
                    string callback = context.Request.Headers["CALLBACK"];
                    if (callback != null) {
                        string uuid = GenerateUuid ();
                        // TODO try/catch
                        Subscription subscriber = new Subscription (new Uri (callback.Substring (1, callback.Length - 2)), uuid);
                        subscribers.Add (uuid, subscriber);
                        HandleSubscription (context, subscriber);
                        context.Response.Close ();
                        PublishUpdates (subscriber);
                    } else {
                        string sid = context.Request.Headers["SID"];
                        if (sid == null || !subscribers.ContainsKey (sid)) {
                            // TODO stuff here
                        }
                        HandleSubscription (context, subscribers[sid]);
                        context.Response.Close ();
                    }
                } else if (context.Request.HttpMethod.ToUpper () == "UNSUBSCRIBE") {
                    string sid = context.Request.Headers["SID"];
                    if (sid == null || !subscribers.ContainsKey (sid)) {
                        // TODO stuff here
                    }
                    dispatcher.Remove (subscribers[sid].TimeoutId);
                    subscribers.Remove (sid);
                    context.Response.Close ();
                }
            }
        }

        private bool OnTimeout (object state, ref TimeSpan interval)
        {
            lock (mutex) {
                subscribers.Remove ((string)state);
                return false;
            }
        }

        private void HandleSubscription (HttpListenerContext context, Subscription subscriber)
        {
            dispatcher.Remove (subscriber.TimeoutId);
            string timeout = context.Request.Headers["TIMEOUT"] ?? "Second-1800";
            TimeSpan time = timeout == "infinate" ? TimeSpan.MaxValue : TimeSpan.FromSeconds (int.Parse (timeout.Substring (7)));
            subscriber.TimeoutId = dispatcher.Add (time, OnTimeout, subscriber.Sid);
            context.Response.AddHeader ("DATE", DateTime.Now.ToString ("r"));
            context.Response.AddHeader ("SERVER", Protocol.UserAgent);
            context.Response.AddHeader ("SID", subscriber.Sid);
            context.Response.AddHeader ("TIMEOUT", timeout);
            context.Response.StatusCode = 200;
            context.Response.StatusDescription = "OK";
        }

        private static readonly Random random = new Random ();
        private static string GenerateUuid ()
        {
            StringBuilder builder = new StringBuilder (37);
            builder.Append ("uuid:");
            for (int i = 0; i < 32; i++) {
                int r = random.Next (65, 100);
                if (r > 90) {
                    builder.Append (r - 90);
                } else {
                    builder.Append ((char)r);
                }
            }
            return builder.ToString ();
        }
    }
}
