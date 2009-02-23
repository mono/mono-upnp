// ConnectionManager1.cs auto-generated at 10/5/2008 8:36:31 AM by Sharpener

using System;

using Mono.Upnp.Server;

namespace Mono.Upnp.Dcp.MediaServer1
{
    public abstract class ConnectionManager1 : Service
    {
        protected ConnectionManager1 (string id)
            : base (new ServiceType ("schemas-upnp-org", "ConnectionManager", new Version (1, 0)), id)
        {
        }
        [UpnpAction]
        public void GetProtocolInfo ([UpnpArgument ("Source")]out string source, [UpnpArgument ("Sink")]out string sink)
        {
            GetProtocolInfoCore (out source, out sink);
        }

        public abstract void GetProtocolInfoCore (out string source, out string sink);

        [UpnpAction]
        public void PrepareForConnection ([UpnpArgument ("RemoteProtocolInfo")]string remoteProtocolInfo, [UpnpArgument ("PeerConnectionManager")]string peerConnectionManager, [UpnpArgument ("PeerConnectionID")]int peerConnectionID, [UpnpArgument ("Direction")]Direction direction, [UpnpArgument ("ConnectionID")]out int connectionID, [UpnpArgument ("AVTransportID")]out int aVTransportID, [UpnpArgument ("RcsID")]out int rcsID)
        {
            PrepareForConnectionCore (remoteProtocolInfo, peerConnectionManager, peerConnectionID, direction, out connectionID, out aVTransportID, out rcsID);
        }

        public abstract void PrepareForConnectionCore (string remoteProtocolInfo, string peerConnectionManager, int peerConnectionID, Direction direction, out int connectionID, out int aVTransportID, out int rcsID);

        [UpnpAction]
        public void ConnectionComplete ([UpnpArgument ("ConnectionID")]int connectionID)
        {
            ConnectionCompleteCore (connectionID);
        }

        public abstract void ConnectionCompleteCore (int connectionID);

        [UpnpAction]
        public void GetCurrentConnectionIDs ([UpnpArgument ("ConnectionIDs")]out string connectionIDs)
        {
            GetCurrentConnectionIDsCore (out connectionIDs);
        }

        public abstract void GetCurrentConnectionIDsCore (out string connectionIDs);

        [UpnpAction]
        public void GetCurrentConnectionInfo ([UpnpArgument ("ConnectionID")]int connectionID, [UpnpArgument ("RcsID")]out int rcsID, [UpnpArgument ("AVTransportID")]out int aVTransportID, [UpnpArgument ("ProtocolInfo")]out string protocolInfo, [UpnpArgument ("PeerConnectionManager")]out string peerConnectionManager, [UpnpArgument ("PeerConnectionID")]out int peerConnectionID, [UpnpArgument ("Direction")]out Direction direction, [UpnpArgument ("Status")]out ConnectionStatus status)
        {
            GetCurrentConnectionInfoCore (connectionID, out rcsID, out aVTransportID, out protocolInfo, out peerConnectionManager, out peerConnectionID, out direction, out status);
        }

        public abstract void GetCurrentConnectionInfoCore (int connectionID, out int rcsID, out int aVTransportID, out string protocolInfo, out string peerConnectionManager, out int peerConnectionID, out Direction direction, out ConnectionStatus status);

        [UpnpStateVariable ("SourceProtocolInfo")]
        public event EventHandler<StateVariableChangedArgs<string>> SourceProtocolInfoChanged;
        protected string SourceProtocolInfo {
            set {
                EventHandler<StateVariableChangedArgs<string>> handler = SourceProtocolInfoChanged;
                if (handler != null) {
                    handler (this, new StateVariableChangedArgs<string> (value));
                }
            }
        }

        [UpnpStateVariable ("SinkProtocolInfo")]
        public event EventHandler<StateVariableChangedArgs<string>> SinkProtocolInfoChanged;
        protected string SinkProtocolInfo {
            set {
                EventHandler<StateVariableChangedArgs<string>> handler = SinkProtocolInfoChanged;
                if (handler != null) {
                    handler (this, new StateVariableChangedArgs<string> (value));
                }
            }
        }

        [UpnpStateVariable ("CurrentConnectionIDs")]
        public event EventHandler<StateVariableChangedArgs<string>> CurrentConnectionIDsChanged;
        protected string CurrentConnectionIDs {
            set {
                EventHandler<StateVariableChangedArgs<string>> handler = CurrentConnectionIDsChanged;
                if (handler != null) {
                    handler (this, new StateVariableChangedArgs<string> (value));
                }
            }
        }

    }
}