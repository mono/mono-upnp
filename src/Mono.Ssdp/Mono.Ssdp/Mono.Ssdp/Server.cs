//
// Server.cs
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
using System.Net;
using System.Threading;

using Mono.Ssdp.Internal;

namespace Mono.Ssdp
{
    public class Server : IDisposable
    {
        readonly object mutex = new object ();
        readonly string default_location;

        bool disposed;

        readonly RequestListener request_listener;
        readonly Dictionary<string, Announcer> announcers;

        public bool Started { get; private set; }
        
        public bool IsDisposed {
            get { return disposed; }
        }

        readonly TimeoutDispatcher dispatcher = new TimeoutDispatcher ();
        internal TimeoutDispatcher Dispatcher {
            get { return dispatcher; }
        }

        internal SsdpSocket AnnounceSocket { get; private set; }

        internal SsdpSocket RespondSocket { get; private set; }

        public Server ()
        {
            request_listener = new RequestListener (this);
            announcers = new Dictionary<string, Announcer> ();
        }

        public Server (string location)
            : this ()
        {
            default_location = location;
        }

        public Announcer Announce (string type, string name)
        {
            return Announce (type, name, default_location, true);
        }

        public Announcer Announce (string type, string name, string location)
        {
            return Announce (type, name, location, true);
        }

        public Announcer Announce (string type, string name, bool autoStart)
        {
            return Announce (type, name, default_location, autoStart);
        }

        public Announcer Announce (string type, string name, string location, bool autoStart)
        {
            lock (mutex) {
                CheckDisposed ();

                if (type == null) {
                    throw new ArgumentNullException ("type");
                }
                if (name == null) {
                    throw new ArgumentNullException ("name");
                }
                if (location == null) {
                    throw new ArgumentNullException ("location");
                }

                Announcer announcer;
                if (announcers.TryGetValue (name, out announcer)) {
                    if (!announcer.Started && autoStart) {
                        announcer.Start ();
                    }
                    return announcer;
                }

                announcer = new Announcer (this, name, type, location);
                announcers.Add (name, announcer);

                if (autoStart) {
                    announcer.Start ();
                }

                return announcer;
            }
        }

        public void Start ()
        {
            Start (true);
        }

        public void Start (bool startAnnouncers)
        {
            lock (mutex) {
                CheckDisposed ();

                if (Started) {
                    throw new InvalidOperationException ("The Server is already started.");
                }

                Started = true;
                request_listener.Start ();
                AnnounceSocket = new SsdpSocket ();
                AnnounceSocket.Bind (new IPEndPoint (IPAddress.Any, 0));
                RespondSocket = new SsdpSocket (false);
                RespondSocket.Bind (new IPEndPoint (IPAddress.Any, Protocol.Port));

                if (startAnnouncers) {
                    foreach (Announcer announcer in announcers.Values) {
                        if (!announcer.Started) {
                            announcer.Start ();
                        }
                    }
                }
            }
        }

        public void Stop ()
        {
            lock (mutex) {
                CheckDisposed ();

                if (!Started) {
                    return;
                }

                var handles = new WaitHandle[announcers.Count];
                int i = 0;
                foreach (var announcer in announcers.Values) {
                    handles[i++] = announcer.StopAsync ();
                }

                request_listener.Stop ();
                RespondSocket.Close ();
                RespondSocket = null;
                WaitHandle.WaitAll (handles);
                AnnounceSocket.Close ();
                AnnounceSocket = null;
                Started = false;
            }
        }

        internal void RequestRecieved (Request request)
        {
            foreach (var announcer in Match (request.ST)) {
                announcer.Respond (request.EndPoint, request.MX);
            }
        }

        IEnumerable<Announcer> Match (string st)
        {
            if (st == Protocol.SsdpAll) {
                return announcers.Values;
            } else {
                return MatchTypeOrName (st);
            }
        }

        IEnumerable<Announcer> MatchTypeOrName (string st)
        {
            if (st.StartsWith ("uuid:")) {
                foreach (var announcer in announcers.Values) {
                    if (announcer.Name.StartsWith (st)) {
                        yield return announcer;
                    }
                }
            } else {
                foreach (var announcer in announcers.Values) {
                    if (announcer.Type == st) {
                        yield return announcer;
                    }
                }
            }
        }

        void CheckDisposed ()
        {
            if (disposed) {
                throw new ObjectDisposedException ("Browser has been Disposed");
            }
        }

        public void Dispose ()
        {
            lock (mutex) {
                if (disposed) {
                    return;
                }

                Stop ();
                request_listener.Dispose ();
                dispatcher.Dispose ();
                disposed = true;
            }
        }
    }
}
