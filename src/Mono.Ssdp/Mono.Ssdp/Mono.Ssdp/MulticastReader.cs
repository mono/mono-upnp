//
// MulticastReader.cs
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

using Mono.Ssdp.Internal;

namespace Mono.Ssdp
{
    public abstract class MulticastReader
    {
        internal MulticastReader ()
        {
        }

        internal void AsyncReadResult (SsdpSocket socket)
        {
            AsyncReadResult(new AsyncReceiveBuffer (socket));
        }

        void AsyncReadResult (AsyncReceiveBuffer buffer)
        {
            try {
                buffer.Socket.BeginReceiveFrom (buffer, OnAsyncResultReceived);
            } catch (ObjectDisposedException) {
                // Socket disposed while we were receiving from it... just ignore this
            }
        }
        
        void OnAsyncResultReceived (IAsyncResult asyncResult)
        {
            var buffer = (AsyncReceiveBuffer)asyncResult.AsyncState;

            try {
                buffer.BytesReceived = buffer.Socket.EndReceiveFrom (asyncResult, ref buffer.SenderEndPoint);
            } catch (ObjectDisposedException) {
                // Socket already disposed... just ignore this and exit
                return;
            }

            if (OnAsyncResultReceived (buffer)) {
                AsyncReadResult (buffer);
            } else {
                buffer.Socket.Close ();
            }
        }
        
        internal virtual bool OnAsyncResultReceived (AsyncReceiveBuffer result)
        {
            return false;
        }
    }
}
