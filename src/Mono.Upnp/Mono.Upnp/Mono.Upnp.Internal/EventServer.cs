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
using System.Threading;
using System.Xml;

using Mono.Upnp.Control;

namespace Mono.Upnp.Internal
{
    sealed class EventServer : UpnpServer
    {
        class Subscription
        {
            public readonly Uri Callback;
            public readonly string Sid;
            public volatile uint Seq;
            public volatile int ConnectFailures;
            public uint TimeoutId;
            
            public Subscription (Uri callback, string sid)
            {
                this.Callback = callback;
                this.Sid = sid;
            }
        }
        
        readonly static Encoding utf8 = new UTF8Encoding (false);
        
        readonly IEnumerable<StateVariable> state_variables;
        volatile bool started;
        
        readonly object subscription_mutex = new object ();
        readonly Dictionary<string, Subscription> subscribers = new Dictionary<string, Subscription> ();
        readonly TimeoutDispatcher dispatcher = new TimeoutDispatcher ();
        
        readonly object publish_mutex = new object ();
        readonly List<StateVariable> updated_state_variables = new List<StateVariable> ();
        Thread publish_thread;

        public EventServer (IEnumerable<StateVariable> stateVariables, Uri url)
            : base (url)
        {
            this.state_variables = stateVariables;
        }
        
        public void QueueUpdate (StateVariable stateVariable)
        {
            lock (publish_mutex) {
                if (!updated_state_variables.Contains (stateVariable)) {
                    updated_state_variables.Add (stateVariable);
                    Monitor.Pulse (publish_mutex);
                }
            }
        }
        
        public override void Start ()
        {
            started = true;
            publish_thread = new Thread (new ThreadStart (Publish));
            publish_thread.Start ();
            base.Start ();
        }
        
        public override void Stop ()
        {
            started = false;
            lock (publish_mutex) {
                Monitor.Pulse (publish_mutex);
            }
            base.Stop ();
        }
        
        void Publish ()
        {
            lock (publish_mutex) {
                Monitor.Wait (publish_mutex);
                
                while (started) {
                    var count = 0;
                    do {
                        // FIXME what if code updates a state variable constantly at more than 1Hz?
                        // We would never broadcast. We need to handle that.
                        count = updated_state_variables.Count;
                        Monitor.Exit (publish_mutex);
                        Thread.Sleep (TimeSpan.FromSeconds (1));
                        Monitor.Enter (publish_mutex);
                    } while (count != updated_state_variables.Count);
                    
                    PublishUpdates (updated_state_variables);
                    updated_state_variables.Clear ();
                    
                    Monitor.Wait (publish_mutex);
                }
            }
        }

        public void PublishUpdates (IEnumerable<StateVariable> stateVariables)
        {
            lock (subscription_mutex) {
                foreach (var subscriber in subscribers.Values) {
                    PublishUpdates (subscriber, stateVariables);
                }
            }
        }

        void PublishUpdates (Subscription subscriber, IEnumerable<StateVariable> stateVariables)
        {
            var request = (HttpWebRequest)WebRequest.Create (subscriber.Callback);
            request.Method = "NOTIFY";
            request.ContentType = "text/xml";
            request.Headers.Add ("NT", "upnp:event");
            request.Headers.Add ("NTS", "upnp:propchange");
            request.Headers.Add ("SID", subscriber.Sid);
            request.Headers.Add ("SEQ", subscriber.Seq.ToString ());
            request.KeepAlive = false;
            subscriber.Seq++;
            
            using (var stream = request.GetRequestStream ()) {
                using (var writer = XmlWriter.Create (stream, new XmlWriterSettings { Encoding = utf8 })) {
                    writer.WriteStartDocument ();
                    writer.WriteStartElement ("e", "propertyset", Protocol.EventSchema);
                    foreach (var state_variable in stateVariables) {
                        writer.WriteStartAttribute ("property", Protocol.EventSchema);
                        writer.WriteElementString (state_variable.Name, state_variable.Value);
                        writer.WriteEndElement ();
                    }
                    writer.WriteEndElement ();
                }
            }
            
            request.BeginGetResponse (async => {
                try {
                    request.EndGetResponse (async).Close ();
                    Interlocked.Exchange (ref subscriber.ConnectFailures, 0);
                } catch (Exception e) {
                    Log.Exception (string.Format ("There was a problem publishing updates to subscription {0}.", subscriber.Sid), e);
                    
                    Interlocked.Increment (ref subscriber.ConnectFailures);
                    if (subscriber.ConnectFailures == 2) {
                        lock (subscription_mutex) {
                            if (subscribers.ContainsKey (subscriber.Sid)) {
                                subscribers.Remove (subscriber.Sid);
                                
                                Log.Information (string.Format (
                                    "Subscription {0} failed multiple times and was removed.", subscriber.Sid));
                            }
                        }
                    }
                }
            }, null);
        }

        protected override void HandleContext (HttpListenerContext context)
        {
            lock (subscription_mutex) {
                var method = context.Request.HttpMethod.ToUpper ();
                if (method == "SUBSCRIBE") {
                    var callback = context.Request.Headers["CALLBACK"];
                    if (callback != null) {
                        Subscribe (context, callback);
                    } else {
                        Renew (context);
                    }
                } else if (method == "UNSUBSCRIBE") {
                    Unsubscribe (context);
                }
            }
        }
        
        void Subscribe (HttpListenerContext context, string callback)
        {
            var url_string = callback.Length < 3 ? null : callback.Substring (1, callback.Length - 2);
            
            if (url_string == null || !Uri.IsWellFormedUriString (url_string, UriKind.Absolute)) {
                Log.Error (string.Format (
                    "The subscription request from {0} to {1} provides an illegal callback: {2}",
                    context.Request.RemoteEndPoint, context.Request.Url, callback));
                return; 
            }
            
            var url = new Uri (url_string);
            
            if (url.Scheme != Uri.UriSchemeHttp) {
                Log.Error (string.Format (
                    "The callback for the subscription request from {0} to {1} is not HTTP: {2}",
                    context.Request.RemoteEndPoint, context.Request.Url, url));
                return;
            }
            
            var uuid = string.Format ("uuid:{0}", Guid.NewGuid ());
            var subscriber = new Subscription (url, uuid);
            HandleSubscription (context, subscriber);
            subscribers.Add (uuid, subscriber);
            context.Response.Close ();
            
            Log.Information (string.Format (
                "{0} from {1} subscribed to {2} as {3}.",
                subscriber.Callback, context.Request.RemoteEndPoint, context.Request.Url, subscriber.Sid));
            
            PublishUpdates (subscriber, state_variables);
        }
        
        void Renew (HttpListenerContext context)
        {
            var sid = context.Request.Headers["SID"];
            
            if (sid == null) {
                Log.Error (string.Format (
                    "A subscription request from {0} to {1} provided neither a CALLBACK nor a SID.",
                    context.Request.RemoteEndPoint, context.Request.Url));
                return;
            } else if (!subscribers.ContainsKey (sid)) {
                Log.Error (string.Format (
                    "A renewal request from {0} to {1} was for subscription {2} which does not exist.",
                    context.Request.RemoteEndPoint, context.Request.Url, sid));
                return;
            }
            
            HandleSubscription (context, subscribers[sid]);
            
            Log.Information (string.Format ("Subscription {0} was renewed.", sid));
        }
        
        void Unsubscribe (HttpListenerContext context)
        {
            var sid = context.Request.Headers["SID"];
            if (sid == null) {
                Log.Error (string.Format (
                    "An unsubscribe request from {0} to {1} did not provide a SID.",
                    context.Request.RemoteEndPoint, context.Request.Url));
                return;
            } else if (!subscribers.ContainsKey (sid)) {
                Log.Error (string.Format (
                    "An unsubscribe request from {0} to {1} was for subscription {2} which does not exist.",
                    context.Request.RemoteEndPoint, context.Request.Url, sid));
                return;
            }
            
            dispatcher.Remove (subscribers[sid].TimeoutId);
            subscribers.Remove (sid);
            
            Log.Information (string.Format ("Subscription {0} was canceled.", sid));
        }

        void HandleSubscription (HttpListenerContext context, Subscription subscriber)
        {
            dispatcher.Remove (subscriber.TimeoutId);
            var timeout = context.Request.Headers["TIMEOUT"] ?? "Second-1800";
            if (timeout != "infinite") {
                int time;
                if (timeout.Length > 7 && int.TryParse (timeout.Substring (7), out time)) {
                    subscriber.TimeoutId = dispatcher.Add (TimeSpan.FromSeconds (time), OnTimeout, subscriber.Sid);
                } else {
                    Log.Error (string.Format (
                        "Subscription {0} ({1}) from {2} to {3} has an illegal TIMEOUT value: {4}",
                        subscriber.Sid, subscriber.Callback, context.Request.RemoteEndPoint, context.Request.Url, timeout));
                    // TODO throw
                }
            }
            context.Response.AddHeader ("DATE", DateTime.Now.ToString ("r"));
            context.Response.AddHeader ("SERVER", Protocol.UserAgent);
            context.Response.AddHeader ("SID", subscriber.Sid);
            context.Response.AddHeader ("TIMEOUT", timeout);
            context.Response.StatusCode = 200;
            context.Response.StatusDescription = "OK";
        }
        
        bool OnTimeout (object state, ref TimeSpan interval)
        {
            lock (subscription_mutex) {
                subscribers.Remove ((string)state);
                
                Log.Information (string.Format ("Subscription {0} expired and was removed.", state));
                
                return false;
            }
        }
    }
}
