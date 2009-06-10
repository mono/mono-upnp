// 
// ServerTests.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Peterson
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Mono.Ssdp.Tests
{
    [TestFixture]
    public class ServerTests
    {
        const string address = "239.255.255.250";
        static readonly IPAddress ipaddress = IPAddress.Parse (address);
        static readonly IPEndPoint endpoint = new IPEndPoint (ipaddress, 1900);
        
        readonly object mutex = new object ();
        
        static Socket CreateMulticastSocket ()
        {
            var socket = CreateSocket ();
            socket.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            socket.SetSocketOption (SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 4);
            socket.SetSocketOption (SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption (ipaddress, 0));
            return socket;
        }
        
        static Socket CreateSocket ()
        {
            var socket = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            return socket;
        }
        
        [Test]
        public void AnnouncementTest ()
        {
//            using (var socket = CreateSocket ()) {
//                using (var server = new Server ()) {
//                    socket.Bind (endpoint);
//                    var buffer = new byte[1024];
//                    socket.BeginReceiveFrom (buffer, 0, buffer.Length, SocketFlags.None, ref endpoint,
//                        result => {
//                            lock (mutex) {
//                                socket.EndReceiveFrom (result, ref endpoint);
//                                var datagram = Encoding.ASCII.GetString (buffer, 0, buffer.Length);
//                                Assert.IsTrue (datagram.StartsWith ("NOTIFY * HTTP/1.1\r\n"));
//                                Monitor.Pulse (mutex);
//                            }
//                        }, null
//                    );
//                }
//            }
        }
    }
}
