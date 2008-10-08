// ConnectionManager1.cs auto-generated at 10/9/2008 2:49:26 AM by Sharpener

using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.MediaServer1
{
    public class ConnectionManager1 : Service
    {
        internal ConnectionManager1 (Client client, string deviceId, IEnumerable<string> locations)
            : base (client, deviceId, locations, ConnectionManager1Factory.ServiceType)
        {
        }

        internal ConnectionManager1 (Device device, XmlReader reader, WebHeaderCollection headers)
            : base (device, reader, headers)
        {
        }

        public void GetProtocolInfo (out string source, out string sink)
        {
            Action action = Actions["GetProtocolInfo"];
            action.Invoke ();
            source = action.OutArguments["Source"].Value;
            sink = action.OutArguments["Sink"].Value;
        }

        public bool CanPrepareForConnection { get { return Actions.ContainsKey("PrepareForConnection"); } }
        public void PrepareForConnection (string remoteProtocolInfo, string peerConnectionManager, int peerConnectionID, Direction direction, out string connectionID, out string aVTransportID, out string rcsID)
        {
            if (!CanPrepareForConnection) throw new NotImplementedException ();
            Action action = Actions["PrepareForConnection"];
            action.InArguments["RemoteProtocolInfo"].Value = remoteProtocolInfo;
            action.InArguments["PeerConnectionManager"].Value = peerConnectionManager;
            action.InArguments["PeerConnectionID"].Value = peerConnectionID.ToString ();
            action.InArguments["Direction"].Value = direction.ToString ();
            action.Invoke ();
            connectionID = action.OutArguments["ConnectionID"].Value;
            aVTransportID = action.OutArguments["AVTransportID"].Value;
            rcsID = action.OutArguments["RcsID"].Value;
        }

        public bool CanConnectionComplete { get { return Actions.ContainsKey("ConnectionComplete"); } }
        public void ConnectionComplete (int connectionID)
        {
            if (!CanConnectionComplete) throw new NotImplementedException ();
            Action action = Actions["ConnectionComplete"];
            action.InArguments["ConnectionID"].Value = connectionID.ToString ();
            action.Invoke ();
        }

        public string GetCurrentConnectionIDs ()
        {
            Action action = Actions["GetCurrentConnectionIDs"];
            action.Invoke ();
            return action.OutArguments["ConnectionIDs"].Value;
        }

        public void GetCurrentConnectionInfo (int connectionID, out string rcsID, out string aVTransportID, out string protocolInfo, out string peerConnectionManager, out string peerConnectionID, out string direction, out string status)
        {
            Action action = Actions["GetCurrentConnectionInfo"];
            action.InArguments["ConnectionID"].Value = connectionID.ToString ();
            action.Invoke ();
            rcsID = action.OutArguments["RcsID"].Value;
            aVTransportID = action.OutArguments["AVTransportID"].Value;
            protocolInfo = action.OutArguments["ProtocolInfo"].Value;
            peerConnectionManager = action.OutArguments["PeerConnectionManager"].Value;
            peerConnectionID = action.OutArguments["PeerConnectionID"].Value;
            direction = action.OutArguments["Direction"].Value;
            status = action.OutArguments["Status"].Value;
        }

        public event EventHandler<StateVariableChangedArgs<string>> SourceProtocolInfoChanged {
            add { StateVariables["SourceProtocolInfo"].Changed += value; }
            remove { StateVariables["SourceProtocolInfo"].Changed -= value; }
        }

        public event EventHandler<StateVariableChangedArgs<string>> SinkProtocolInfoChanged {
            add { StateVariables["SinkProtocolInfo"].Changed += value; }
            remove { StateVariables["SinkProtocolInfo"].Changed -= value; }
        }

        public event EventHandler<StateVariableChangedArgs<string>> CurrentConnectionIDsChanged {
            add { StateVariables["CurrentConnectionIDs"].Changed += value; }
            remove { StateVariables["CurrentConnectionIDs"].Changed -= value; }
        }

        protected override void VerifyContract ()
        {
            base.VerifyContract ();
            if (!Actions.ContainsKey ("GetProtocolInfo")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type {1} but it does not have the required action GetProtocolInfo.", Id, Type));
            if (!Actions.ContainsKey ("GetCurrentConnectionIDs")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type {1} but it does not have the required action GetCurrentConnectionIDs.", Id, Type));
            if (!Actions.ContainsKey ("GetCurrentConnectionInfo")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type {1} but it does not have the required action GetCurrentConnectionInfo.", Id, Type));
        }
    }
}