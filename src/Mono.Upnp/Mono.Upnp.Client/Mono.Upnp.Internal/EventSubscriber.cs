//
// EventSubscriber.cs
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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using Mono.Upnp.Control;

namespace Mono.Upnp.Internal
{
	internal class EventSubscriber : IDisposable
	{
		static readonly Random random = new Random ();
        static int id;

        readonly object mutex = new object ();
        ServiceController controller;
        TimeoutDispatcher dispatcher = new TimeoutDispatcher();
        bool started;
        bool confidently_subscribed;
        uint expire_timeout_id;
        uint renew_timeout_id;
        HttpListener listener;
        string prefix;

        string delivery_url;
        string subscription_uuid;

        public EventSubscriber (ServiceController serviceController)
        {
            controller = serviceController;
            // TODO relocate this stuff?
            prefix = GeneratePrefix ();
            delivery_url = String.Format ("<{0}>", prefix);
        }

        static string GeneratePrefix ()
        {
			// TODO handle if we don't find the address
			var address = (from a in Dns.GetHostAddresses (Dns.GetHostName ())
						   where a.AddressFamily == AddressFamily.InterNetwork
						   select a).First ();
			
			return string.Format (
				"http://{0}:{1}/upnp/event-subscriber/{2}/", address, random.Next (1024, 5000), id++);
        }

        public void Start ()
        {
            lock (mutex) {
                if (started) {
                    return;
                }
                StartListening ();
                Subscribe ();

                started = true;
            }
        }

        void StartListening ()
        {
            if (listener == null) {
                listener = new HttpListener ();
                listener.Prefixes.Add (prefix);
            }
            listener.Start ();
            listener.BeginGetContext (OnGotContext, listener);
        }

        void OnGotContext (IAsyncResult asyncResult)
        {
            var listener = (HttpListener)asyncResult.AsyncState;
            var context = listener.EndGetContext (asyncResult);
            try {
                controller.DeserializeEvents (context.Request);
            } catch {
                // TODO log
            }
            context.Response.Close ();
            listener.BeginGetContext (OnGotContext, listener);
        }

        void StopListening ()
        {
            listener.Stop ();
        }

        void Subscribe ()
        {
            lock (mutex) {
                var request = WebRequest.Create (controller.Description.EventUrl);
                request.Method = "SUBSCRIBE";
                request.Headers.Add ("CALLBACK", delivery_url);
                request.Headers.Add ("NT", "upnp:event");
                request.BeginGetResponse (OnSubscribeResponse, request);
                expire_timeout_id = dispatcher.Add (TimeSpan.FromSeconds (30), OnSubscribeTimeout, request);
                confidently_subscribed = false;
            }
        }

        bool OnSubscribeTimeout (object state, ref TimeSpan interval)
        {
            lock (mutex) {
                expire_timeout_id = 0;
                if (!confidently_subscribed) {
                    var request = (WebRequest)state;
                    request.Abort ();
                    Stop ();
                    // TODO retry
                    Log.Exception (new UpnpException ("Failed to subscribe or renew. The server did not respond in 30 seconds."));
                    controller.Description.CheckDisposed ();
                }
                return false;
            }
        }

        void OnSubscribeResponse (IAsyncResult asyncResult)
        {
            lock (mutex) {
                if (expire_timeout_id != 0) {
                    dispatcher.Remove (expire_timeout_id);
                }
                var request = (WebRequest)asyncResult.AsyncState;
                try {
                    var response = (HttpWebResponse)request.EndGetResponse (asyncResult);
                    if (response.StatusCode == HttpStatusCode.GatewayTimeout) {
                        throw new WebException ("", WebExceptionStatus.Timeout);
                    } else if (response.StatusCode != HttpStatusCode.OK) {
                        throw new WebException ();
                    } else if (!started) {
                        response.Close ();
                        return;
                    }
                    subscription_uuid = response.Headers["SID"];
                    var timeout_header = response.Headers["TIMEOUT"];
                    if (timeout_header == "infinate") {
                        return;
                    }
                    var timeout = TimeSpan.FromSeconds (Double.Parse (timeout_header.Substring (7)));
                    timeout -= TimeSpan.FromMinutes (1);
                    renew_timeout_id = dispatcher.Add (timeout, OnRenewTimeout);
                    confidently_subscribed = true;
                    response.Close ();
                } catch (WebException e) {
                    Stop ();
                    // TODO more info
                    Log.Exception (new UpnpException ("Failed to subscribe or renew.", e));
                    if (e.Status == WebExceptionStatus.Timeout) {
                        controller.Description.CheckDisposed ();
                    }
                }
            }
        }

        bool OnRenewTimeout (object state, ref TimeSpan interval)
        {
            lock (mutex) {
                if (started) {
                    Renew ();
                }
                renew_timeout_id = 0;
                return false;
            }
        }

        void Renew ()
        {
            lock (mutex) {
                var request = WebRequest.Create (controller.Description.EventUrl);
                request.Method = "SUBSCRIBE";
                request.Headers.Add ("SID", subscription_uuid);
                request.BeginGetResponse (OnSubscribeResponse, request);
                expire_timeout_id = dispatcher.Add (TimeSpan.FromSeconds (30), OnSubscribeTimeout, request);
                confidently_subscribed = false;
            }
        }

        void Unsubscribe ()
        {
            lock (mutex) {
                var request = WebRequest.Create (controller.Description.EventUrl);
                request.Method = "UNSUBSCRIBE";
                request.Headers.Add ("SID", subscription_uuid);
                request.BeginGetResponse (OnUnsubscribeResponse, request);
                confidently_subscribed = false;
            }
        }

        void OnUnsubscribeResponse (IAsyncResult asyncResult)
        {
            try {
                var response = ((WebRequest)asyncResult).EndGetResponse (asyncResult);
                response.Close ();
            } catch {
            }
        }

        public void Stop ()
        {
            lock (mutex) {
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

                if (confidently_subscribed) {
                    Unsubscribe ();
                }
                StopListening ();

                started = false;
            }
        }

        public void Dispose ()
        {
            Stop ();
            listener.Close ();
        }
	}
}
