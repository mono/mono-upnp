// ContentDirectory1.cs auto-generated at 1/18/2009 9:31:12 PM by Sharpener

using System;
using System.Collections.Generic;

using Mono.Upnp.Discovery;
using Mono.Upnp.Description;
using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.MediaServer1
{
    public class ContentDirectory1
    {
        public static readonly ServiceType ServiceType = new ServiceType ("urn:schemas-upnp-org:service:ContentDirectory:1");
        readonly ServiceController controller;
        public ContentDirectory1 (ServiceAnnouncement announcement)
        {
            if (announcement == null) throw new ArgumentNullException ("announcement");
            ServiceDescription description = announcement.GetDescription ();
            controller = description.GetController ();
            if (controller == null) throw new UpnpDeserializationException (string.Format ("{0} has no controller.", description));
            Verify ();
        }

        public string GetSearchCapabilities ()
        {
            ActionResult action_result = controller.Actions["GetSearchCapabilities"].Invoke ();
            return action_result.OutValues["SearchCaps"];
        }

        public string GetSortCapabilities ()
        {
            ActionResult action_result = controller.Actions["GetSortCapabilities"].Invoke ();
            return action_result.OutValues["SortCaps"];
        }

        public string GetSystemUpdateID ()
        {
            ActionResult action_result = controller.Actions["GetSystemUpdateID"].Invoke ();
            return action_result.OutValues["Id"];
        }

        public void Browse (string objectID, BrowseFlag browseFlag, string filter, uint startingIndex, uint requestedCount, string sortCriteria, out string result, out string numberReturned, out string totalMatches, out string updateID)
        {
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (6);
            in_arguments.Add ("ObjectID", objectID);
            in_arguments.Add ("BrowseFlag", browseFlag.ToString ());
            in_arguments.Add ("Filter", filter);
            in_arguments.Add ("StartingIndex", startingIndex.ToString ());
            in_arguments.Add ("RequestedCount", requestedCount.ToString ());
            in_arguments.Add ("SortCriteria", sortCriteria);
            ActionResult action_result = controller.Actions["Browse"].Invoke (in_arguments);
            result = action_result.OutValues["Result"];
            numberReturned = action_result.OutValues["NumberReturned"];
            totalMatches = action_result.OutValues["TotalMatches"];
            updateID = action_result.OutValues["UpdateID"];
        }

        public bool CanSearch { get { return controller.Actions.ContainsKey ("Search"); } }
        public void Search (string containerID, string searchCriteria, string filter, uint startingIndex, uint requestedCount, string sortCriteria, out string result, out string numberReturned, out string totalMatches, out string updateID)
        {
            if (!CanSearch) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (6);
            in_arguments.Add ("ContainerID", containerID);
            in_arguments.Add ("SearchCriteria", searchCriteria);
            in_arguments.Add ("Filter", filter);
            in_arguments.Add ("StartingIndex", startingIndex.ToString ());
            in_arguments.Add ("RequestedCount", requestedCount.ToString ());
            in_arguments.Add ("SortCriteria", sortCriteria);
            ActionResult action_result = controller.Actions["Search"].Invoke (in_arguments);
            result = action_result.OutValues["Result"];
            numberReturned = action_result.OutValues["NumberReturned"];
            totalMatches = action_result.OutValues["TotalMatches"];
            updateID = action_result.OutValues["UpdateID"];
        }

        public bool CanCreateObject { get { return controller.Actions.ContainsKey ("CreateObject"); } }
        public void CreateObject (string containerID, string elements, out string objectID, out string result)
        {
            if (!CanCreateObject) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (2);
            in_arguments.Add ("ContainerID", containerID);
            in_arguments.Add ("Elements", elements);
            ActionResult action_result = controller.Actions["CreateObject"].Invoke (in_arguments);
            objectID = action_result.OutValues["ObjectID"];
            result = action_result.OutValues["Result"];
        }

        public bool CanDestroyObject { get { return controller.Actions.ContainsKey ("DestroyObject"); } }
        public void DestroyObject (string objectID)
        {
            if (!CanDestroyObject) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("ObjectID", objectID);
            ActionResult action_result = controller.Actions["DestroyObject"].Invoke (in_arguments);
        }

        public bool CanUpdateObject { get { return controller.Actions.ContainsKey ("UpdateObject"); } }
        public void UpdateObject (string objectID, string currentTagValue, string newTagValue)
        {
            if (!CanUpdateObject) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (3);
            in_arguments.Add ("ObjectID", objectID);
            in_arguments.Add ("CurrentTagValue", currentTagValue);
            in_arguments.Add ("NewTagValue", newTagValue);
            ActionResult action_result = controller.Actions["UpdateObject"].Invoke (in_arguments);
        }

        public bool CanImportResource { get { return controller.Actions.ContainsKey ("ImportResource"); } }
        public string ImportResource (Uri sourceURI, Uri destinationURI)
        {
            if (!CanImportResource) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (2);
            in_arguments.Add ("SourceURI", sourceURI.ToString ());
            in_arguments.Add ("DestinationURI", destinationURI.ToString ());
            ActionResult action_result = controller.Actions["ImportResource"].Invoke (in_arguments);
            return action_result.OutValues["TransferID"];
        }

        public bool CanExportResource { get { return controller.Actions.ContainsKey ("ExportResource"); } }
        public string ExportResource (Uri sourceURI, Uri destinationURI)
        {
            if (!CanExportResource) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (2);
            in_arguments.Add ("SourceURI", sourceURI.ToString ());
            in_arguments.Add ("DestinationURI", destinationURI.ToString ());
            ActionResult action_result = controller.Actions["ExportResource"].Invoke (in_arguments);
            return action_result.OutValues["TransferID"];
        }

        public bool CanStopTransferResource { get { return controller.Actions.ContainsKey ("StopTransferResource"); } }
        public void StopTransferResource (uint transferID)
        {
            if (!CanStopTransferResource) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("TransferID", transferID.ToString ());
            ActionResult action_result = controller.Actions["StopTransferResource"].Invoke (in_arguments);
        }

        public bool CanGetTransferProgress { get { return controller.Actions.ContainsKey ("GetTransferProgress"); } }
        public void GetTransferProgress (uint transferID, out string transferStatus, out string transferLength, out string transferTotal)
        {
            if (!CanGetTransferProgress) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("TransferID", transferID.ToString ());
            ActionResult action_result = controller.Actions["GetTransferProgress"].Invoke (in_arguments);
            transferStatus = action_result.OutValues["TransferStatus"];
            transferLength = action_result.OutValues["TransferLength"];
            transferTotal = action_result.OutValues["TransferTotal"];
        }

        public bool CanDeleteResource { get { return controller.Actions.ContainsKey ("DeleteResource"); } }
        public void DeleteResource (Uri resourceURI)
        {
            if (!CanDeleteResource) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("ResourceURI", resourceURI.ToString ());
            ActionResult action_result = controller.Actions["DeleteResource"].Invoke (in_arguments);
        }

        public bool CanCreateReference { get { return controller.Actions.ContainsKey ("CreateReference"); } }
        public string CreateReference (string containerID, string objectID)
        {
            if (!CanCreateReference) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (2);
            in_arguments.Add ("ContainerID", containerID);
            in_arguments.Add ("ObjectID", objectID);
            ActionResult action_result = controller.Actions["CreateReference"].Invoke (in_arguments);
            return action_result.OutValues["NewID"];
        }

        public bool HasTransferIDs { get { return controller.StateVariables.ContainsKey ("TransferIDs"); } }
        public event EventHandler<StateVariableChangedArgs<string>> TransferIDsChanged {
            add {
                if (!HasTransferIDs) return;
                controller.StateVariables["TransferIDs"].Changed += value;
            }
            remove {
                if (!HasTransferIDs) return;
                controller.StateVariables["TransferIDs"].Changed -= value;
            }
        }

        public event EventHandler<StateVariableChangedArgs<string>> SystemUpdateIDChanged {
            add { controller.StateVariables["SystemUpdateID"].Changed += value; }
            remove { controller.StateVariables["SystemUpdateID"].Changed -= value; }
        }

        public bool HasContainerUpdateIDs { get { return controller.StateVariables.ContainsKey ("ContainerUpdateIDs"); } }
        public event EventHandler<StateVariableChangedArgs<string>> ContainerUpdateIDsChanged {
            add {
                if (!HasContainerUpdateIDs) return;
                controller.StateVariables["ContainerUpdateIDs"].Changed += value;
            }
            remove {
                if (!HasContainerUpdateIDs) return;
                controller.StateVariables["ContainerUpdateIDs"].Changed -= value;
            }
        }

        void Verify ()
        {
            if (!controller.Actions.ContainsKey ("GetSearchCapabilities")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required action GetSearchCapabilities.", controller.Description.Id));
            if (!controller.Actions.ContainsKey ("GetSortCapabilities")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required action GetSortCapabilities.", controller.Description.Id));
            if (!controller.Actions.ContainsKey ("GetSystemUpdateID")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required action GetSystemUpdateID.", controller.Description.Id));
            if (!controller.Actions.ContainsKey ("Browse")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required action Browse.", controller.Description.Id));
            if (!controller.StateVariables.ContainsKey ("SystemUpdateID")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required state variable SystemUpdateID.", controller.Description.Id));
        }
    }
}