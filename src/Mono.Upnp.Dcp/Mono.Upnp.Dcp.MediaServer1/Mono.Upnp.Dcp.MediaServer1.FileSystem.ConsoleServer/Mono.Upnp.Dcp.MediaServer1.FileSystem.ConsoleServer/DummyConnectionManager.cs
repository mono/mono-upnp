// 
// DummyConnectionManager.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Thomas
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

using Mono.Upnp.Dcp.MediaServer1.ConnectionManager1;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem.ConsoleServer
{
    public class DummyConnectionManager : ConnectionManager
    {
        protected override string CurrentConnectionIDs {
            get {
                throw new System.NotImplementedException ();
            }
        }
        
        protected override void GetProtocolInfoCore (out string source, out string sink)
        {
            throw new System.NotImplementedException ();
        }


        protected override void GetCurrentConnectionInfoCore (int connectionId, out int resId, out int avTransportId, out string protocolInfo, out string peerConnectionManager, out int peerConnectionId, out Mono.Upnp.Dcp.MediaServer1.ConnectionManager1.Direction direction, out Mono.Upnp.Dcp.MediaServer1.ConnectionManager1.ConnectionStatus status)
        {
            throw new System.NotImplementedException ();
        }
    }
}
