// ContentDirectoryController.cs auto-generated at 1/18/2009 9:31:12 PM by Sharpener

using System;
using System.Collections.Generic;

using Mono.Upnp;
using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public class ContentDirectoryController
    {
        readonly ServiceController controller;
        
        public ContentDirectoryController (ServiceController controller)
        {
            if (controller == null) throw new ArgumentNullException ("controller");
            
            this.controller = controller;
            Verify ();
        }
        
        public ServiceController ServiceController {
            get { return controller; }
        }

        public string GetSearchCapabilities ()
        {
            var action_result = controller.Actions["GetSearchCapabilities"].Invoke (2);
            return action_result["SearchCaps"];
        }

        public string GetSortCapabilities ()
        {
            var action_result = controller.Actions["GetSortCapabilities"].Invoke (2);
            return action_result["SortCaps"];
        }

        public string GetSystemUpdateId ()
        {
            var action_result = controller.Actions["GetSystemUpdateID"].Invoke (2);
            return action_result["Id"];
        }

        public string Browse (string objectId, BrowseFlag browseFlag, string filter, uint startingIndex, uint requestedCount, string sortCriteria, out uint numberReturned, out uint totalMatches, out uint updateId)
        {
            if (browseFlag < BrowseFlag.BrowseMetadata || browseFlag > BrowseFlag.BrowseDirectChildren)
                throw new ArgumentOutOfRangeException ("browseFlag");
            
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (6);
            in_arguments.Add ("ObjectID", objectId);
            in_arguments.Add ("BrowseFlag", browseFlag.ToString ());
            in_arguments.Add ("Filter", filter);
            in_arguments.Add ("StartingIndex", startingIndex.ToString ());
            in_arguments.Add ("RequestedCount", requestedCount.ToString ());
            in_arguments.Add ("SortCriteria", sortCriteria);
            var action_result = controller.Actions["Browse"].Invoke (in_arguments, 2);
            numberReturned = uint.Parse (action_result["NumberReturned"]);
            totalMatches = uint.Parse (action_result["TotalMatches"]);
            updateId = uint.Parse (action_result["UpdateID"]);
            return action_result["Result"];
        }

        public bool CanSearch {
            get { return controller.Actions.ContainsKey ("Search"); }
        }
        
        public string Search (string containerId, string searchCriteria, string filter, uint startingIndex, uint requestedCount, string sortCriteria, out uint numberReturned, out uint totalMatches, out uint updateId)
        {
            if (!CanSearch) throw new NotImplementedException ();
            
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (6);
            in_arguments.Add ("ContainerID", containerId);
            in_arguments.Add ("SearchCriteria", searchCriteria);
            in_arguments.Add ("Filter", filter);
            in_arguments.Add ("StartingIndex", startingIndex.ToString ());
            in_arguments.Add ("RequestedCount", requestedCount.ToString ());
            in_arguments.Add ("SortCriteria", sortCriteria);
            var action_result = controller.Actions["Search"].Invoke (in_arguments, 2);
            numberReturned = uint.Parse (action_result["NumberReturned"]);
            totalMatches = uint.Parse (action_result["TotalMatches"]);
            updateId = uint.Parse (action_result["UpdateID"]);
            return action_result["Result"];
        }

        public bool CanCreateObject {
            get { return controller.Actions.ContainsKey ("CreateObject"); }
        }
        
        public string CreateObject (string containerId, string elements)
        {
            if (!CanCreateObject) throw new NotImplementedException ();
            
            var in_arguments = new Dictionary<string, string> (2);
            in_arguments.Add ("ContainerID", containerId);
            in_arguments.Add ("Elements", elements);
            var action_result = controller.Actions["CreateObject"].Invoke (in_arguments);
            return action_result["Result"];
        }

        public bool CanDestroyObject {
            get { return controller.Actions.ContainsKey ("DestroyObject"); }
        }
        
        public void DestroyObject (string objectId)
        {
            if (!CanDestroyObject) throw new NotImplementedException ();
            
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("ObjectID", objectId);
            controller.Actions["DestroyObject"].Invoke (in_arguments);
        }

        public bool CanUpdateObject {
            get { return controller.Actions.ContainsKey ("UpdateObject"); }
        }
        
        public void UpdateObject (string objectId, string currentTagValue, string newTagValue)
        {
            if (!CanUpdateObject) throw new NotImplementedException ();
            
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (3);
            in_arguments.Add ("ObjectID", objectId);
            in_arguments.Add ("CurrentTagValue", currentTagValue);
            in_arguments.Add ("NewTagValue", newTagValue);
            controller.Actions["UpdateObject"].Invoke (in_arguments);
        }

        public bool CanImportResource {
            get { return controller.Actions.ContainsKey ("ImportResource"); }
        }
        
        public string ImportResource (Uri sourceUri, Uri destinationUri)
        {
            if (!CanImportResource) throw new NotImplementedException ();
            
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (2);
            in_arguments.Add ("SourceURI", sourceUri.ToString ());
            in_arguments.Add ("DestinationURI", destinationUri.ToString ());
            var action_result = controller.Actions["ImportResource"].Invoke (in_arguments);
            return action_result["TransferID"];
        }

        public bool CanExportResource {
            get { return controller.Actions.ContainsKey ("ExportResource"); }
        }
        
        public string ExportResource (Uri sourceUri, Uri destinationUri)
        {
            if (!CanExportResource) throw new NotImplementedException ();
            
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (2);
            in_arguments.Add ("SourceURI", sourceUri.ToString ());
            in_arguments.Add ("DestinationURI", destinationUri.ToString ());
            var action_result = controller.Actions["ExportResource"].Invoke (in_arguments);
            return action_result["TransferID"];
        }

        public bool CanStopTransferResource {
            get { return controller.Actions.ContainsKey ("StopTransferResource"); }
        }
        
        public void StopTransferResource (uint transferId)
        {
            if (!CanStopTransferResource) throw new NotImplementedException ();
            
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("TransferID", transferId.ToString ());
            controller.Actions["StopTransferResource"].Invoke (in_arguments);
        }

        public bool CanGetTransferProgress {
            get { return controller.Actions.ContainsKey ("GetTransferProgress"); }
        }
        
        public void GetTransferProgress (uint transferId, out string transferStatus, out string transferLength, out string transferTotal)
        {
            if (!CanGetTransferProgress) throw new NotImplementedException ();
            
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("TransferID", transferId.ToString ());
            var action_result = controller.Actions["GetTransferProgress"].Invoke (in_arguments);
            transferStatus = action_result["TransferStatus"];
            transferLength = action_result["TransferLength"];
            transferTotal = action_result["TransferTotal"];
        }

        public bool CanDeleteResource {
            get { return controller.Actions.ContainsKey ("DeleteResource"); }
        }
        
        public void DeleteResource (Uri resourceUri)
        {
            if (!CanDeleteResource) throw new NotImplementedException ();
            
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("ResourceURI", resourceUri.ToString ());
            controller.Actions["DeleteResource"].Invoke (in_arguments);
        }

        public bool CanCreateReference {
            get { return controller.Actions.ContainsKey ("CreateReference"); }
        }
        
        public string CreateReference (string containerId, string objectId)
        {
            if (!CanCreateReference) throw new NotImplementedException ();
            
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (2);
            in_arguments.Add ("ContainerID", containerId);
            in_arguments.Add ("ObjectID", objectId);
            var action_result = controller.Actions["CreateReference"].Invoke (in_arguments);
            return action_result["NewID"];
        }

//        public bool HasTransferIds { get { return controller.StateVariables.ContainsKey ("TransferIDs"); } }
//        public event EventHandler<StateVariableChangedArgs<string>> TransferIdsChanged {
//            add {
//                if (!HasTransferIds) return;
//                controller.StateVariables["TransferIDs"].Changed += value;
//            }
//            remove {
//                if (!HasTransferIds) return;
//                controller.StateVariables["TransferIDs"].Changed -= value;
//            }
//        }
//
//        public event EventHandler<StateVariableChangedArgs<string>> SystemUpdateIdChanged {
//            add { controller.StateVariables["SystemUpdateID"].Changed += value; }
//            remove { controller.StateVariables["SystemUpdateID"].Changed -= value; }
//        }
//
//        public bool HasContainerUpdateIds { get { return controller.StateVariables.ContainsKey ("ContainerUpdateIDs"); } }
//        public event EventHandler<StateVariableChangedArgs<string>> ContainerUpdateIdsChanged {
//            add {
//                if (!HasContainerUpdateIds) return;
//                controller.StateVariables["ContainerUpdateIDs"].Changed += value;
//            }
//            remove {
//                if (!HasContainerUpdateIds) return;
//                controller.StateVariables["ContainerUpdateIDs"].Changed -= value;
//            }
//        }

        void Verify ()
        {
//            if (!controller.Actions.ContainsKey ("GetSearchCapabilities"))
//                throw new UpnpDeserializationException (string.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required action GetSearchCapabilities.", controller.Description.Id));
//            if (!controller.Actions.ContainsKey ("GetSortCapabilities"))
//                throw new UpnpDeserializationException (string.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required action GetSortCapabilities.", controller.Description.Id));
//            if (!controller.Actions.ContainsKey ("GetSystemUpdateID"))
//                throw new UpnpDeserializationException (string.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required action GetSystemUpdateID.", controller.Description.Id));
//            if (!controller.Actions.ContainsKey ("Browse"))
//                throw new UpnpDeserializationException (string.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required action Browse.", controller.Description.Id));
//            if (!controller.StateVariables.ContainsKey ("SystemUpdateID"))
//                throw new UpnpDeserializationException (string.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required state variable SystemUpdateID.", controller.Description.Id));
        }
    }
}