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

using Mono.Ssdp.Internal;

namespace Mono.Ssdp
{
    public class Server : IDisposable
	{
        private readonly object mutex = new object ();
        private RequestListener request_listener;
        private Dictionary<string, Announcer> announcers;

        private bool started;
        public bool Started {
            get { return started; }
        }

        private TimeoutDispatcher dispatcher = new TimeoutDispatcher ();
        internal TimeoutDispatcher Dispatcher {
            get { return dispatcher; }
        }

        private int active_announcer_count;
        internal int ActiveAnnouncerCount {
            get { lock (mutex) { return active_announcer_count; } }
            set { lock (mutex) { active_announcer_count = value; } }
        }

        private SsdpSocket announceSocket;
        internal SsdpSocket AnnounceSocket {
            get { return announceSocket; }
        }

        private SsdpSocket respondSocket;
        internal SsdpSocket RespondSocket {
            get { return respondSocket; }
        }

        public event EventHandler Stopped;

        public Server ()
        {
            request_listener = new RequestListener (this);
            announcers = new Dictionary<string, Announcer> ();
        }

        public Announcer Announce (string type, string name, string location)
        {
            return Announce (type, name, location, true);
        }

        public Announcer Announce (string type, string name, string location, bool autoStart)
        {
            lock (mutex) {
                CheckDisposed ();

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

                started = true;
                request_listener.Start ();
                announceSocket = new SsdpSocket ();
                respondSocket = new SsdpSocket (false);

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

                if (ActiveAnnouncerCount == 0) {
                    HardStop ();
                } else {
                    foreach (Announcer announcer in announcers.Values) {
                        announcer.Stopped += AnnouncerStopped;
                        announcer.Stop ();
                    }
                }
            }
        }

        private void AnnouncerStopped (object sender, EventArgs args)
        {
            lock (mutex) {
                ((Announcer)sender).Stopped -= AnnouncerStopped;
                if (ActiveAnnouncerCount == 0) {
                    HardStop ();
                }
            }
        }

        private void HardStop ()
        {
            announceSocket.Close ();
            announceSocket = null;
            respondSocket.Close ();
            respondSocket = null;
            started = false;

            OnStopped ();
        }

        protected virtual void OnStopped ()
        {
            EventHandler stopped = Stopped;
            if (stopped != null) {
                stopped (this, EventArgs.Empty);
            }
        }

        internal void RequestRecieved (Request request)
        {
            foreach (Announcer announcer in Match (request.ST)) {
                announcer.Respond (request.EndPoint, request.MX);
            }
        }

        private IEnumerable<Announcer> Match (string st)
        {
            if (st == Protocol.SsdpAll) {
                return announcers.Values;
            } else {
                return MatchTypeOrName (st);
            }
        }

        private IEnumerable<Announcer> MatchTypeOrName (string st)
        {
            if (st.StartsWith ("uuid:")) {
                foreach (Announcer announcer in announcers.Values) {
                    if (announcer.Name.StartsWith (st)) {
                        yield return announcer;
                    }
                }
            } else {
                foreach (Announcer announcer in announcers.Values) {
                    if (announcer.Type == st) {
                        yield return announcer;
                    }
                }
            }
        }

        private void CheckDisposed ()
        {
            //if (disposed) {
            //    throw new ObjectDisposedException ("Browser has been Disposed");
            //}
        }

        public void Dispose ()
        {
            throw new NotImplementedException ();
        }
    }
}
