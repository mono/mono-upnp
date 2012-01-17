// ConnectionManagerController.cs auto-generated at 1/18/2009 9:31:12 PM by Sharpener

using System;
using System.Collections.Generic;

using Mono.Upnp;
using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.MediaServer1.ConnectionManager1
{
    public class ConnectionManagerController
    {
        readonly ServiceController controller;
        
        public ConnectionManagerController (ServiceAnnouncement announcement)
        {
            if (announcement == null) throw new ArgumentNullException ("announcement");
            controller = announcement.GetService ().GetController ();
            Verify ();
        }

        public void GetProtocolInfo (out string source, out string sink)
        {
            var action_result = controller.Actions["GetProtocolInfo"].Invoke ();
            source = action_result["Source"];
            sink = action_result["Sink"];
        }

        public bool CanPrepareForConnection { get { return controller.Actions.ContainsKey ("PrepareForConnection"); } }
        public void PrepareForConnection (string remoteProtocolInfo, string peerConnectionManager, int peerConnectionID, Direction direction, out string connectionID, out string aVTransportID, out string rcsID)
        {
            if (!CanPrepareForConnection) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (4);
            in_arguments.Add ("RemoteProtocolInfo", remoteProtocolInfo);
            in_arguments.Add ("PeerConnectionManager", peerConnectionManager);
            in_arguments.Add ("PeerConnectionID", peerConnectionID.ToString ());
            in_arguments.Add ("Direction", direction.ToString ());
            var action_result = controller.Actions["PrepareForConnection"].Invoke (in_arguments);
            connectionID = action_result["ConnectionID"];
            aVTransportID = action_result["AVTransportID"];
            rcsID = action_result["RcsID"];
        }

        public bool CanConnectionComplete { get { return controller.Actions.ContainsKey ("ConnectionComplete"); } }
        public void ConnectionComplete (int connectionID)
        {
            if (!CanConnectionComplete) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("ConnectionID", connectionID.ToString ());
            controller.Actions["ConnectionComplete"].Invoke (in_arguments);
        }

        public string GetCurrentConnectionIDs ()
        {
            var action_result = controller.Actions["GetCurrentConnectionIDs"].Invoke ();
            return action_result["ConnectionIDs"];
        }

        public void GetCurrentConnectionInfo (int connectionID, out string rcsID, out string aVTransportID, out string protocolInfo, out string peerConnectionManager, out string peerConnectionID, out string direction, out string status)
        {
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("ConnectionID", connectionID.ToString ());
            var action_result = controller.Actions["GetCurrentConnectionInfo"].Invoke (in_arguments);
            rcsID = action_result["RcsID"];
            aVTransportID = action_result["AVTransportID"];
            protocolInfo = action_result["ProtocolInfo"];
            peerConnectionManager = action_result["PeerConnectionManager"];
            peerConnectionID = action_result["PeerConnectionID"];
            direction = action_result["Direction"];
            status = action_result["Status"];
        }

//        public event EventHandler<StateVariableChangedArgs<string>> SourceProtocolInfoChanged
//        {
//            add { controller.StateVariables["SourceProtocolInfo"].Changed += value; }
//            remove { controller.StateVariables["SourceProtocolInfo"].Changed -= value; }
//        }
//
//        public event EventHandler<StateVariableChangedArgs<string>> SinkProtocolInfoChanged
//        {
//            add { controller.StateVariables["SinkProtocolInfo"].Changed += value; }
//            remove { controller.StateVariables["SinkProtocolInfo"].Changed -= value; }
//        }
//
//        public event EventHandler<StateVariableChangedArgs<string>> CurrentConnectionIDsChanged
//        {
//            add { controller.StateVariables["CurrentConnectionIDs"].Changed += value; }
//            remove { controller.StateVariables["CurrentConnectionIDs"].Changed -= value; }
//        }

        void Verify ()
        {
//            if (!controller.Actions.ContainsKey ("GetProtocolInfo")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ConnectionManager:1 but it does not have the required action GetProtocolInfo.", controller.Description.Id));
//            if (!controller.Actions.ContainsKey ("GetCurrentConnectionIDs")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ConnectionManager:1 but it does not have the required action GetCurrentConnectionIDs.", controller.Description.Id));
//            if (!controller.Actions.ContainsKey ("GetCurrentConnectionInfo")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ConnectionManager:1 but it does not have the required action GetCurrentConnectionInfo.", controller.Description.Id));
//            if (!controller.StateVariables.ContainsKey ("SourceProtocolInfo")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ConnectionManager:1 but it does not have the required state variable SourceProtocolInfo.", controller.Description.Id));
//            if (!controller.StateVariables.ContainsKey ("SinkProtocolInfo")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ConnectionManager:1 but it does not have the required state variable SinkProtocolInfo.", controller.Description.Id));
//            if (!controller.StateVariables.ContainsKey ("CurrentConnectionIDs")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ConnectionManager:1 but it does not have the required state variable CurrentConnectionIDs.", controller.Description.Id));
        }
    }
}