//
// SsdpSocket.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2008 Novell, Inc.
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

namespace Mono.Ssdp.Internal
{
    class SsdpSocket : Socket
    {
        /// <summary>
        /// Winsock ioctl code which will disable ICMP errors from being propagated to a UDP socket.
        /// This can occur if a UDP packet is sent to a valid destination but there is no socket
        /// registered to listen on the given port.
        /// </summary>
        /// http://msdn.microsoft.com/en-us/library/cc242275.aspx
        /// http://msdn.microsoft.com/en-us/library/bb736550(VS.85).aspx
        /// 0x9800000C == 2550136844 (uint) == -1744830452 (int) == 0x9800000C
        const int SIO_UDP_CONNRESET = -1744830452;

        static readonly IPEndPoint ssdp_send_point = new IPEndPoint (Protocol.IPAddress, Protocol.Port);
        
        readonly IPEndPoint ssdp_receive_point;
        
        public SsdpSocket (IPAddress address)
            : base (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            ssdp_receive_point = new IPEndPoint (address, Protocol.Port);
            SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // set socket to disregard ICMP errors.
            this.IOControl((IOControlCode)(SIO_UDP_CONNRESET), new byte[] { 0, 0, 0, 0 }, null);
        }
        
        public IAsyncResult BeginSendTo (byte [] data, AsyncCallback callback)
        {
            return BeginSendTo (data, callback, ssdp_send_point);
        }

        public IAsyncResult BeginSendTo (byte[] data, AsyncCallback callback, IPEndPoint endPoint)
        {
            return BeginSendTo (data, 0, data.Length, SocketFlags.None, endPoint, callback, this);
        }
        
        public IAsyncResult BeginReceiveFrom (AsyncReceiveBuffer buffer, AsyncCallback callback)
        {
            return base.BeginReceiveFrom (buffer.Buffer, 0, buffer.Buffer.Length, SocketFlags.None, 
                ref buffer.SenderEndPoint, callback, buffer);
        }
        
        public void Bind ()
        {
            Bind (ssdp_receive_point);
        }
    }
}
