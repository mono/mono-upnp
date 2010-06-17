//
// RequestListener.cs
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
using System.Net;
using System.Threading;

namespace Mono.Ssdp.Internal
{
    struct Request
    {
        public readonly IPEndPoint EndPoint;
        public readonly string ST;
        public readonly ushort MX;
        
        public Request (IPEndPoint endPoint, string st, ushort mx)
        {
            EndPoint = endPoint;
            ST = st;
            MX = mx;
        }
    }

    class RequestListener : MulticastReader, IDisposable
    {
        readonly object mutex = new object ();
        readonly Server server;
        SsdpSocket socket;

        public RequestListener (Server server)
        {
            this.server = server;
        }

        public void Start ()
        {
            lock (mutex) {
                Stop ();

                socket = new MulticastSsdpSocket (server.NetworkInterfaceInfo);
                socket.Bind ();
                AsyncReadResult (socket);
            }
        }

        public void Stop ()
        {
            lock (mutex) {
                if (socket != null) {
                    socket.Close ();
                    socket = null;
                }
            }
        }

        internal override bool OnAsyncResultReceived (AsyncReceiveBuffer result)
        {
            try {
                var dgram = HttpDatagram.Parse (result.Buffer);
                if (dgram == null || dgram.Type != HttpDatagramType.MSearch) {
                    return true;
                }

                var st = dgram.Headers.Get ("ST");
                var mx = dgram.Headers.Get ("MX");
                var man = dgram.Headers.Get ("Man");

                if (string.IsNullOrEmpty (st) || string.IsNullOrEmpty (mx) || man != Protocol.SsdpDiscoverMan) {
                    return true;
                }

                server.RequestRecieved (new Request (result.SenderIPEndPoint, st, UInt16.Parse(mx)));
            } catch (Exception e) {
                Log.Exception ("Invalid HTTPMU/M-SEARCH datagram", e);
            }

            return true;
        }

        public void Dispose ()
        {
            // TODO more here
            Stop ();
        }
    }
}
