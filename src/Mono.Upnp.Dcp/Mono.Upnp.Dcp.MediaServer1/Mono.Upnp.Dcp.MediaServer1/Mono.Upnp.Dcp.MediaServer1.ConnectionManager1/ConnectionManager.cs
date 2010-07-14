// 
// ConnectionManager.cs
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

using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.MediaServer1.ConnectionManager1
{
    public abstract class ConnectionManager
    {
        public static readonly ServiceType ServiceType = new ServiceType ("schemas-upnp-org", "ConnectionManager", new Version (1, 0));
        
        [UpnpAction]
        public virtual void GetProtocolInfo ([UpnpArgumentAttribute ("Source")]
                                              [UpnpRelatedStateVariable ("SourceProtocolInfo")] out string source,
                                              [UpnpArgument ("Sink")]
                                              [UpnpRelatedStateVariable ("SinkProtocolInfo")] out string sink)
        {
            GetProtocolInfoCore (out source, out sink);
        }
        
        protected abstract void GetProtocolInfoCore (out string source, out string sink);
        
        [UpnpAction]
        public virtual void GetCurrentConnectionIDs ([UpnpArgument ("ConnectionIDs"), UpnpRelatedStateVariable ("CurrentConnectionIDs")] out string connectionIds)
        {
            connectionIds = CurrentConnectionIDs;
        }
        
        protected abstract string CurrentConnectionIDs { get; }
        
        [UpnpAction]
        public virtual void GetCurrentConnectionInfo ([UpnpArgument ("ConnectionID")] int connectionId,
                                                      [UpnpArgument ("ResID")] out int resId,
                                                      [UpnpArgument ("AVTransportID")] out int avTransportId,
                                                      [UpnpArgument ("ProtocolInfo")] out string protocolInfo,
                                                      [UpnpArgument ("PeerConnectionManager")]
                                                      [UpnpRelatedStateVariable ("A_ARG_TYPE_ConnectionManager")] out string peerConnectionManager,
                                                      [UpnpArgument ("PeerConnectionID")]
                                                      [UpnpRelatedStateVariable ("A_ARG_TYPE_ConnectionID")] out int peerConnectionId,
                                                      [UpnpArgument ("Direction")] out Direction direction,
                                                      [UpnpArgument ("Status")] out ConnectionStatus status)
        {
            GetCurrentConnectionInfoCore (connectionId, out resId, out avTransportId, out protocolInfo,
                out peerConnectionManager, out peerConnectionId, out direction, out status);
        }
        
        protected abstract void GetCurrentConnectionInfoCore (int connectionId, out int resId, out int avTransportId, out string protocolInfo,
                                                              out string peerConnectionManager, out int peerConnectionId,
                                                              out Direction direction, out ConnectionStatus status);
    }
}
