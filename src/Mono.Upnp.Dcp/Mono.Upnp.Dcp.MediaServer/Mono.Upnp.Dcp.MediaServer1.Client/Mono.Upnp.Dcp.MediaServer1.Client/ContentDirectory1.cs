// ContentDirectory1.cs auto-generated at 10/9/2008 2:49:26 AM by Sharpener

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.MediaServer1
{
    public class ContentDirectory1 : Service
    {
        internal ContentDirectory1 (Client client, string deviceId, IEnumerable<string> locations)
            : base (client, deviceId, locations, ContentDirectory1Factory.ServiceType)
        {
        }

        internal ContentDirectory1 (Device device, XmlReader reader, WebHeaderCollection headers)
            : base (device, reader, headers)
        {
        }

        public string GetSearchCapabilities ()
        {
            Action action = Actions["GetSearchCapabilities"];
            action.Invoke ();
            return action.OutArguments["SearchCaps"].Value;
        }

        public string GetSortCapabilities ()
        {
            Action action = Actions["GetSortCapabilities"];
            action.Invoke ();
            return action.OutArguments["SortCaps"].Value;
        }

        public string GetSystemUpdateID ()
        {
            Action action = Actions["GetSystemUpdateID"];
            action.Invoke ();
            return action.OutArguments["Id"].Value;
        }

        public void Browse (string objectID, BrowseFlag browseFlag, string filter, uint startingIndex, uint requestedCount, string sortCriteria, out string result, out string numberReturned, out string totalMatches, out string updateID)
        {
            Action action = Actions["Browse"];
            action.InArguments["ObjectID"].Value = objectID;
            action.InArguments["BrowseFlag"].Value = browseFlag.ToString ();
            action.InArguments["Filter"].Value = filter;
            action.InArguments["StartingIndex"].Value = startingIndex.ToString ();
            action.InArguments["RequestedCount"].Value = requestedCount.ToString ();
            action.InArguments["SortCriteria"].Value = sortCriteria;
            action.Invoke ();
            result = action.OutArguments["Result"].Value;
            numberReturned = action.OutArguments["NumberReturned"].Value;
            totalMatches = action.OutArguments["TotalMatches"].Value;
            updateID = action.OutArguments["UpdateID"].Value;
        }

        public bool CanSearch { get { return Actions.ContainsKey("Search"); } }
        public void Search (string containerID, string searchCriteria, string filter, uint startingIndex, uint requestedCount, string sortCriteria, out string result, out string numberReturned, out string totalMatches, out string updateID)
        {
            if (!CanSearch) throw new NotImplementedException ();
            Action action = Actions["Search"];
            action.InArguments["ContainerID"].Value = containerID;
            action.InArguments["SearchCriteria"].Value = searchCriteria;
            action.InArguments["Filter"].Value = filter;
            action.InArguments["StartingIndex"].Value = startingIndex.ToString ();
            action.InArguments["RequestedCount"].Value = requestedCount.ToString ();
            action.InArguments["SortCriteria"].Value = sortCriteria;
            action.Invoke ();
            result = action.OutArguments["Result"].Value;
            numberReturned = action.OutArguments["NumberReturned"].Value;
            totalMatches = action.OutArguments["TotalMatches"].Value;
            updateID = action.OutArguments["UpdateID"].Value;
        }

        public bool CanCreateObject { get { return Actions.ContainsKey("CreateObject"); } }
        public void CreateObject (string containerID, string elements, out string objectID, out string result)
        {
            if (!CanCreateObject) throw new NotImplementedException ();
            Action action = Actions["CreateObject"];
            action.InArguments["ContainerID"].Value = containerID;
            action.InArguments["Elements"].Value = elements;
            action.Invoke ();
            objectID = action.OutArguments["ObjectID"].Value;
            result = action.OutArguments["Result"].Value;
        }

        public bool CanDestroyObject { get { return Actions.ContainsKey("DestroyObject"); } }
        public void DestroyObject (string objectID)
        {
            if (!CanDestroyObject) throw new NotImplementedException ();
            Action action = Actions["DestroyObject"];
            action.InArguments["ObjectID"].Value = objectID;
            action.Invoke ();
        }

        public bool CanUpdateObject { get { return Actions.ContainsKey("UpdateObject"); } }
        public void UpdateObject (string objectID, string currentTagValue, string newTagValue)
        {
            if (!CanUpdateObject) throw new NotImplementedException ();
            Action action = Actions["UpdateObject"];
            action.InArguments["ObjectID"].Value = objectID;
            action.InArguments["CurrentTagValue"].Value = currentTagValue;
            action.InArguments["NewTagValue"].Value = newTagValue;
            action.Invoke ();
        }

        public bool CanImportResource { get { return Actions.ContainsKey("ImportResource"); } }
        public string ImportResource (Uri sourceURI, Uri destinationURI)
        {
            if (!CanImportResource) throw new NotImplementedException ();
            Action action = Actions["ImportResource"];
            action.InArguments["SourceURI"].Value = sourceURI.ToString ();
            action.InArguments["DestinationURI"].Value = destinationURI.ToString ();
            action.Invoke ();
            return action.OutArguments["TransferID"].Value;
        }

        public bool CanExportResource { get { return Actions.ContainsKey("ExportResource"); } }
        public string ExportResource (Uri sourceURI, Uri destinationURI)
        {
            if (!CanExportResource) throw new NotImplementedException ();
            Action action = Actions["ExportResource"];
            action.InArguments["SourceURI"].Value = sourceURI.ToString ();
            action.InArguments["DestinationURI"].Value = destinationURI.ToString ();
            action.Invoke ();
            return action.OutArguments["TransferID"].Value;
        }

        public bool CanStopTransferResource { get { return Actions.ContainsKey("StopTransferResource"); } }
        public void StopTransferResource (uint transferID)
        {
            if (!CanStopTransferResource) throw new NotImplementedException ();
            Action action = Actions["StopTransferResource"];
            action.InArguments["TransferID"].Value = transferID.ToString ();
            action.Invoke ();
        }

        public bool CanGetTransferProgress { get { return Actions.ContainsKey("GetTransferProgress"); } }
        public void GetTransferProgress (uint transferID, out string transferStatus, out string transferLength, out string transferTotal)
        {
            if (!CanGetTransferProgress) throw new NotImplementedException ();
            Action action = Actions["GetTransferProgress"];
            action.InArguments["TransferID"].Value = transferID.ToString ();
            action.Invoke ();
            transferStatus = action.OutArguments["TransferStatus"].Value;
            transferLength = action.OutArguments["TransferLength"].Value;
            transferTotal = action.OutArguments["TransferTotal"].Value;
        }

        public bool CanDeleteResource { get { return Actions.ContainsKey("DeleteResource"); } }
        public void DeleteResource (Uri resourceURI)
        {
            if (!CanDeleteResource) throw new NotImplementedException ();
            Action action = Actions["DeleteResource"];
            action.InArguments["ResourceURI"].Value = resourceURI.ToString ();
            action.Invoke ();
        }

        public bool CanCreateReference { get { return Actions.ContainsKey("CreateReference"); } }
        public string CreateReference (string containerID, string objectID)
        {
            if (!CanCreateReference) throw new NotImplementedException ();
            Action action = Actions["CreateReference"];
            action.InArguments["ContainerID"].Value = containerID;
            action.InArguments["ObjectID"].Value = objectID;
            action.Invoke ();
            return action.OutArguments["NewID"].Value;
        }

        public bool HasTransferIDs { get { return StateVariables.ContainsKey ("TransferIDs"); } }
        public event EventHandler<StateVariableChangedArgs<string>> TransferIDsChanged {
            add {
                if (!HasTransferIDs) throw new NotImplementedException ();
                StateVariables["TransferIDs"].Changed += value;
            }
            remove {
                if (!HasTransferIDs) throw new NotImplementedException ();
                StateVariables["TransferIDs"].Changed -= value;
            }
        }

        public event EventHandler<StateVariableChangedArgs<string>> SystemUpdateIDChanged {
            add { StateVariables["SystemUpdateID"].Changed += value; }
            remove { StateVariables["SystemUpdateID"].Changed -= value; }
        }

        public bool HasContainerUpdateIDs { get { return StateVariables.ContainsKey ("ContainerUpdateIDs"); } }
        public event EventHandler<StateVariableChangedArgs<string>> ContainerUpdateIDsChanged {
            add {
                if (!HasContainerUpdateIDs) throw new NotImplementedException ();
                StateVariables["ContainerUpdateIDs"].Changed += value;
            }
            remove {
                if (!HasContainerUpdateIDs) throw new NotImplementedException ();
                StateVariables["ContainerUpdateIDs"].Changed -= value;
            }
        }

        protected override Type DeserializeDataType (string dataType)
        {
            //if (Regex.Matches (dataType, "string
            return base.DeserializeDataType (dataType);
        }

        protected override void VerifyContract ()
        {
            base.VerifyContract ();
            if (!Actions.ContainsKey ("GetSearchCapabilities")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type {1} but it does not have the required action GetSearchCapabilities.", Id, Type));
            if (!Actions.ContainsKey ("GetSortCapabilities")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type {1} but it does not have the required action GetSortCapabilities.", Id, Type));
            if (!Actions.ContainsKey ("GetSystemUpdateID")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type {1} but it does not have the required action GetSystemUpdateID.", Id, Type));
            if (!Actions.ContainsKey ("Browse")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type {1} but it does not have the required action Browse.", Id, Type));
            if (!StateVariables.ContainsKey ("TransferIDs")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type {1} but it does not have the required state variable TransferIDs.", Id, Type));
            if (!StateVariables.ContainsKey ("ContainerUpdateIDs")) throw new UpnpDeserializationException (String.Format ("The service {0} claims to be of type {1} but it does not have the required state variable ContainerUpdateIDs.", Id, Type));
        }
    }
}