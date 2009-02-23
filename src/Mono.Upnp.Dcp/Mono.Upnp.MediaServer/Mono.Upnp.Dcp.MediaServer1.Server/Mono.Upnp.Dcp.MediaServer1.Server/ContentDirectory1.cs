// ContentDirectory1.cs auto-generated at 10/4/2008 7:14:37 AM by Sharpener

using System;

using Mono.Upnp.Server;

namespace Mono.Upnp.Dcp.MediaServer1
{
    public abstract class ContentDirectory1 : Service
    {
        protected ContentDirectory1 (string id)
            : base (new ServiceType ("ContentDirectory", new Version (1, 0)), id)
        {
        }

        [UpnpAction]
        public void GetSearchCapabilities ([UpnpArgument ("SearchCaps")]out string searchCaps)
        {
            GetSearchCapabilitiesCore (out searchCaps);
        }

        public abstract void GetSearchCapabilitiesCore (out string searchCaps);

        [UpnpAction]
        public void GetSortCapabilities ([UpnpArgument ("SortCaps")]out string sortCaps)
        {
            GetSortCapabilitiesCore (out sortCaps);
        }

        public abstract void GetSortCapabilitiesCore (out string sortCaps);

        [UpnpAction]
        public void GetSystemUpdateID ([UpnpArgument ("Id")]out uint id)
        {
            GetSystemUpdateIDCore (out id);
        }

        public abstract void GetSystemUpdateIDCore (out uint id);

        [UpnpAction]
        public void Browse ([UpnpArgument ("ObjectID")]string objectID, [UpnpArgument ("BrowseFlag")]BrowseFlag browseFlag, [UpnpArgument ("Filter")]string filter, [UpnpArgument ("StartingIndex")]uint startingIndex, [UpnpArgument ("RequestedCount")]uint requestedCount, [UpnpArgument ("SortCriteria")]string sortCriteria, [UpnpArgument ("Result")]out string result, [UpnpArgument ("NumberReturned")]out uint numberReturned, [UpnpArgument ("TotalMatches")]out uint totalMatches, [UpnpArgument ("UpdateID")]out uint updateID)
        {
            BrowseCore (objectID, browseFlag, filter, startingIndex, requestedCount, sortCriteria, out result, out numberReturned, out totalMatches, out updateID);
        }

        public abstract void BrowseCore (string objectID, BrowseFlag browseFlag, string filter, uint startingIndex, uint requestedCount, string sortCriteria, out string result, out uint numberReturned, out uint totalMatches, out uint updateID);

        [UpnpAction]
        public void Search ([UpnpArgument ("ContainerID")]string containerID, [UpnpArgument ("SearchCriteria")]string searchCriteria, [UpnpArgument ("Filter")]string filter, [UpnpArgument ("StartingIndex")]uint startingIndex, [UpnpArgument ("RequestedCount")]uint requestedCount, [UpnpArgument ("SortCriteria")]string sortCriteria, [UpnpArgument ("Result")]out string result, [UpnpArgument ("NumberReturned")]out uint numberReturned, [UpnpArgument ("TotalMatches")]out uint totalMatches, [UpnpArgument ("UpdateID")]out uint updateID)
        {
            SearchCore (containerID, searchCriteria, filter, startingIndex, requestedCount, sortCriteria, out result, out numberReturned, out totalMatches, out updateID);
        }

        public abstract void SearchCore (string containerID, string searchCriteria, string filter, uint startingIndex, uint requestedCount, string sortCriteria, out string result, out uint numberReturned, out uint totalMatches, out uint updateID);

        [UpnpAction]
        public void CreateObject ([UpnpArgument ("ContainerID")]string containerID, [UpnpArgument ("Elements")]string elements, [UpnpArgument ("ObjectID")]out string objectID, [UpnpArgument ("Result")]out string result)
        {
            CreateObjectCore (containerID, elements, out objectID, out result);
        }

        public abstract void CreateObjectCore (string containerID, string elements, out string objectID, out string result);

        [UpnpAction]
        public void DestroyObject ([UpnpArgument ("ObjectID")]string objectID)
        {
            DestroyObjectCore (objectID);
        }

        public abstract void DestroyObjectCore (string objectID);

        [UpnpAction]
        public void UpdateObject ([UpnpArgument ("ObjectID")]string objectID, [UpnpArgument ("CurrentTagValue")]string currentTagValue, [UpnpArgument ("NewTagValue")]string newTagValue)
        {
            UpdateObjectCore (objectID, currentTagValue, newTagValue);
        }

        public abstract void UpdateObjectCore (string objectID, string currentTagValue, string newTagValue);

        [UpnpAction]
        public void ImportResource ([UpnpArgument ("SourceURI")]Uri sourceURI, [UpnpArgument ("DestinationURI")]Uri destinationURI, [UpnpArgument ("TransferID")]out uint transferID)
        {
            ImportResourceCore (sourceURI, destinationURI, out transferID);
        }

        public abstract void ImportResourceCore (Uri sourceURI, Uri destinationURI, out uint transferID);

        [UpnpAction]
        public void ExportResource ([UpnpArgument ("SourceURI")]Uri sourceURI, [UpnpArgument ("DestinationURI")]Uri destinationURI, [UpnpArgument ("TransferID")]out uint transferID)
        {
            ExportResourceCore (sourceURI, destinationURI, out transferID);
        }

        public abstract void ExportResourceCore (Uri sourceURI, Uri destinationURI, out uint transferID);

        [UpnpAction]
        public void StopTransferResource ([UpnpArgument ("TransferID")]uint transferID)
        {
            StopTransferResourceCore (transferID);
        }

        public abstract void StopTransferResourceCore (uint transferID);

        [UpnpAction]
        public void GetTransferProgress ([UpnpArgument ("TransferID")]uint transferID, [UpnpArgument ("TransferStatus")]out TransferStatus transferStatus, [UpnpArgument ("TransferLength")]out string transferLength, [UpnpArgument ("TransferTotal")]out string transferTotal)
        {
            GetTransferProgressCore (transferID, out transferStatus, out transferLength, out transferTotal);
        }

        public abstract void GetTransferProgressCore (uint transferID, out TransferStatus transferStatus, out string transferLength, out string transferTotal);

        [UpnpAction]
        public void DeleteResource ([UpnpArgument ("ResourceURI")]Uri resourceURI)
        {
            DeleteResourceCore (resourceURI);
        }

        public abstract void DeleteResourceCore (Uri resourceURI);

        [UpnpAction]
        public void CreateReference ([UpnpArgument ("ContainerID")]string containerID, [UpnpArgument ("ObjectID")]string objectID, [UpnpArgument ("NewID")]out string newID)
        {
            CreateReferenceCore (containerID, objectID, out newID);
        }

        public abstract void CreateReferenceCore (string containerID, string objectID, out string newID);

        [UpnpStateVariable ("TransferIDs")]
        public event EventHandler<StateVariableChangedArgs<string>> TransferIDsChanged;
        protected string TransferIDs {
            set {
                EventHandler<StateVariableChangedArgs<string>> handler = TransferIDsChanged;
                if (handler != null) {
                    handler (this, new StateVariableChangedArgs<string> (value));
                }
            }
        }

        [UpnpStateVariable ("SystemUpdateID")]
        public event EventHandler<StateVariableChangedArgs<uint>> SystemUpdateIDChanged;
        protected uint SystemUpdateID {
            set {
                EventHandler<StateVariableChangedArgs<uint>> handler = SystemUpdateIDChanged;
                if (handler != null) {
                    handler (this, new StateVariableChangedArgs<uint> (value));
                }
            }
        }

        [UpnpStateVariable ("ContainerUpdateIDs")]
        public event EventHandler<StateVariableChangedArgs<string>> ContainerUpdateIDsChanged;
        protected string ContainerUpdateIDs {
            set {
                EventHandler<StateVariableChangedArgs<string>> handler = ContainerUpdateIDsChanged;
                if (handler != null) {
                    handler (this, new StateVariableChangedArgs<string> (value));
                }
            }
        }

    }
}