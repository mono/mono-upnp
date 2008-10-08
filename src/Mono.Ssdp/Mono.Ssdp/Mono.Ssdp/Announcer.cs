//
// Announcer.cs
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
    public class Announcer
    {
        private static readonly Random random = new Random ();

        private bool disposed;
        private readonly object mutex = new object ();

        private string location;
        public string Location {
            get { return location; }
            set { location = value; }
        }

        private string type;
        public string Type {
            get { return type; }
            set { type = value; }
        }

        private string name;
        public string Name {
            get { return name; }
            set { name = value; }
        }

        private ushort max_age = Protocol.DefaultMaxAge;
        public ushort MaxAge {
            get { return max_age; }
            set { max_age = Math.Max (Protocol.DefaultMaxAge, value); }
        }

        private volatile uint announcement_timeout_id;

        private TimeSpan GetInterval ()
        {
            return TimeSpan.FromMilliseconds (random.Next (MaxAge * 1000 / 2));
        }

        private static TimeSpan GetInterval (ushort mx)
        {
            // FIXME lame
            return TimeSpan.FromMilliseconds (random.Next (Math.Min (Protocol.MaxMX * 1000, mx * 1000)));
        }

        private Server server;
        public Server Server {
            get { return server; }
        }

        private volatile bool started;
        public bool Started {
            get { return started; }
        }

        internal Announcer (Server server, string name, string type, string location)
        {
            this.server = server;
            Name = name;
            Type = type;
            Location = location;
        }

        public void Start ()
        {
            lock (mutex) {
                CheckDisposed ();

                if (started) {
                    throw new InvalidOperationException ("The announcer is already running. Cancel it first.");
                }

                if (!server.Started) {
                    server.Start (false);
                }

                AnnounceAlive ();

                announcement_timeout_id = server.Dispatcher.Add (GetInterval (), OnAnnounceTimeout);
                started = true;
            }
        }

        public void Stop ()
        {
            lock (mutex) {
                CheckDisposed ();

                if (announcement_timeout_id == 0) {
                    return;
                }

                WaitHandle handle = StopAsync ();
                handle.WaitOne ();
            }
        }

        internal WaitHandle StopAsync ()
        {
            lock (mutex) {
                if (announcement_timeout_id == 0) {
                    return new ManualResetEvent (true);
                }

                server.Dispatcher.Remove (announcement_timeout_id);
                announcement_timeout_id = 0;
                return server.AnnounceSocket.BeginSendTo (Protocol.CreateByeByeNotify (Type, Name), OnByeBye).AsyncWaitHandle;
            }
        }

        private void OnByeBye (IAsyncResult asyncResult)
        {
            SsdpSocket socket = (SsdpSocket)asyncResult.AsyncState;
            try {
                socket.EndSendTo (asyncResult);
            } catch {
            }
            started = false;
        }

        internal void Stop (WaitHandle wait)
        {
        }

        private bool OnAnnounceTimeout (object state, ref TimeSpan interval)
        {
            lock (mutex) {
                AnnounceAlive ();
                interval = GetInterval ();
                return true;
            }
        }

        private void AnnounceAlive ()
        {
            server.AnnounceSocket.BeginSendTo (Protocol.CreateAliveNotify (Location, Type, Name, MaxAge), OnFinished);
        }

        internal void Respond (IPEndPoint endPoint, ushort mx)
        {
            TimeSpan interval = GetInterval (mx);
            server.Dispatcher.Add (interval, OnRespondTimeout, endPoint);
        }

        private bool OnRespondTimeout (object state, ref TimeSpan interval)
        {
            lock (mutex) {
                if (announcement_timeout_id != 0) {
                    server.RespondSocket.BeginSendTo (Protocol.CreateAliveResponse (Location, Type, Name, MaxAge), OnFinished, (IPEndPoint)state);
                }
                return false;
            }
        }

        private void OnFinished (IAsyncResult asyncResult)
        {
            SsdpSocket socket = (SsdpSocket)asyncResult.AsyncState;
            socket.EndSendTo (asyncResult);
        }

        private void CheckDisposed ()
        {
            if (disposed) {
                throw new ObjectDisposedException ("Announcer has been Disposed");
            }
        }
    }
}
