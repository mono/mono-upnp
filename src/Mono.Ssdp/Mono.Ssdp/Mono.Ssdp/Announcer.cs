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
        static readonly Random random = new Random ();

        readonly object mutex = new object ();

        public string Location { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        ushort max_age = Protocol.DefaultMaxAge;
        public ushort MaxAge {
            get { return max_age; }
            set { max_age = System.Math.Max (Protocol.DefaultMaxAge, value); }
        }

        volatile uint announcement_timeout_id;

        TimeSpan GetInterval ()
        {
            return TimeSpan.FromMilliseconds (random.Next (MaxAge * 1000 / 2));
        }

        static TimeSpan GetInterval (ushort mx)
        {
            // FIXME lame
            return TimeSpan.FromMilliseconds (random.Next (System.Math.Min (Protocol.MaxMX * 1000, mx * 1000)));
        }

        readonly Server server;
        public Server Server {
            get { return server; }
        }

        public bool Started { get; private set; }
        
        public bool IsDisposed {
            get { return server.IsDisposed; }
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

                if (Started) {
                    throw new InvalidOperationException ("The announcer is already running. Cancel it first.");
                }

                if (!server.Started) {
                    server.Start (false);
                }

                AnnounceAlive ();

                announcement_timeout_id = server.Dispatcher.Add (GetInterval (), OnAnnounceTimeout);
                Started = true;
            }
        }

        public void Stop ()
        {
            lock (mutex) {
                CheckDisposed ();

                if (announcement_timeout_id == 0) {
                    return;
                }

                var handle = StopAsync ();
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

        void OnByeBye (IAsyncResult asyncResult)
        {
            var socket = (SsdpSocket)asyncResult.AsyncState;
            try {
                socket.EndSendTo (asyncResult);
            } catch {
            }
            Started = false;
        }

        internal void Stop (WaitHandle wait)
        {
        }

        bool OnAnnounceTimeout (object state, ref TimeSpan interval)
        {
            lock (mutex) {
                AnnounceAlive ();
                interval = GetInterval ();
                return true;
            }
        }

        void AnnounceAlive ()
        {
            Server.AnnounceSocket.BeginSendTo (Protocol.CreateAliveNotify (Location, Type, Name, MaxAge), OnFinished);
        }

        internal void Respond (IPEndPoint endPoint, ushort mx)
        {
            var interval = GetInterval (mx);
            Server.Dispatcher.Add (interval, OnRespondTimeout, endPoint);
        }

        bool OnRespondTimeout (object state, ref TimeSpan interval)
        {
            lock (mutex) {
                if (announcement_timeout_id != 0) {
                    server.RespondSocket.BeginSendTo (Protocol.CreateAliveResponse (Location, Type, Name, MaxAge), OnFinished, (IPEndPoint)state);
                }
                return false;
            }
        }

        void OnFinished (IAsyncResult asyncResult)
        {
            var socket = (SsdpSocket)asyncResult.AsyncState;
            socket.EndSendTo (asyncResult);
        }

        void CheckDisposed ()
        {
            if (IsDisposed) {
                throw new ObjectDisposedException ("Announcer has been Disposed");
            }
        }
    }
}
