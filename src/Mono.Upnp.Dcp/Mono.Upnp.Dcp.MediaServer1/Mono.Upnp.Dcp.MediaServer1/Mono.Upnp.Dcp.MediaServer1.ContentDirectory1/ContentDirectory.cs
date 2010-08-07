// 
// ContentDirectory.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Peterson
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

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public abstract class ContentDirectory : IDisposable
    {
        bool is_started;
        ulong system_id;
        
        protected virtual void OnSystemUpdate ()
        {
            if (is_started) {
                system_id++;
            }
        }
        
        public virtual void Start ()
        {
            is_started = true;
        }
        
        public virtual void Stop ()
        {
            is_started = false;
        }

        public bool IsStarted {
            get { return is_started; }
        }
        
        public static readonly ServiceType ServiceType = new ServiceType (
            "schemas-upnp-org", "ContentDirectory", new Version (1, 0));
        
        [UpnpAction]
        public virtual void GetSearchCapabilities ([UpnpArgument ("SearchCaps")]
                                                   [UpnpRelatedStateVariable ("SearchCapabilities")]
                                                   out string searchCapabilities)
        {
            searchCapabilities = SearchCapabilities;
        }
        
        protected abstract string SearchCapabilities { get; }
        
        [UpnpAction]
        public virtual void GetSortCapabilities ([UpnpArgument ("SortCaps")]
                                                 [UpnpRelatedStateVariable ("SortCapabilities")]
                                                 out string sortCapabilities)
        {
            sortCapabilities = SortCapabilities;
        }
        
        protected abstract string SortCapabilities { get; }
        
        [UpnpAction ("GetSystemUpdateID")]
        public virtual void GetSystemUpdateId ([UpnpArgument ("Id")]
                                               [UpnpRelatedStateVariable ("SystemUpdateID")]
                                               out string systemUpdateId)
        {
            systemUpdateId = GetSystemUpdateId ();
        }
        
        protected virtual string GetSystemUpdateId ()
        {
            return system_id.ToString ();
        }
        
        [UpnpAction]
        public virtual void Browse ([UpnpArgument ("ObjectID")] string objectId,
                                    [UpnpArgument ("BrowseFlag")] BrowseFlag browseFlag,
                                    [UpnpArgument ("Filter")] string filter,
                                    [UpnpArgument ("StartingIndex")] int startingIndex,
                                    [UpnpArgument ("RequestedCount")] int requestCount,
                                    [UpnpArgument ("SortCriteria")] string sortCriteria,
                                    [UpnpArgument ("Result")] out string result,
                                    [UpnpArgument ("NumberReturned")] out int numberReturned,
                                    [UpnpArgument ("TotalMatches")] out int totalMatches,
                                    [UpnpArgument ("UpdateID")] out string updateId)
        {
            result = Browse (objectId, browseFlag, filter, startingIndex, requestCount,
                sortCriteria, out numberReturned, out totalMatches, out updateId);
        }
        
        protected abstract string Browse (string objectId, BrowseFlag browseFlag, string filter, int startIndex,
                                          int requestCount, string sortCriteria, out int numberReturned,
                                          out int totalMatches, out string updateId);
        
        [UpnpAction (OmitUnless = "CanSearch")]
        public virtual void Search ([UpnpArgument ("ContainerID")] string containerId,
                                    [UpnpArgument ("SearchCriteria")] string searchCriteria,
                                    [UpnpArgument ("Filter")] string filter,
                                    [UpnpArgument ("StartingIndex")] int startingIndex,
                                    [UpnpArgument ("RequestedCount")] int requestCount,
                                    [UpnpArgument ("SortCriteria")] string sortCriteria,
                                    [UpnpArgument ("Result")] out string result,
                                    [UpnpArgument ("NumberReturned")] out int numberReturned,
                                    [UpnpArgument ("TotalMatches")] out int totalMatches,
                                    [UpnpArgument ("UpdateID")] out string updateId)
        {
            result = Search (containerId, QueryParser.Parse (searchCriteria), filter, startingIndex,
                requestCount, sortCriteria, out numberReturned, out totalMatches, out updateId);
        }
        
        public virtual bool CanSearch {
            get { return false; }
        }
        
        protected virtual string Search (string containerId,
                                         Query query,
                                         string filter,
                                         int startingIndex,
                                         int requestCount,
                                         string sortCriteria,
                                         out int numberReturned,
                                         out int totalMatches,
                                         out string updateId)
        {
            throw new NotImplementedException ();
        }
        
        public virtual bool CanCreateObject {
            get { return false; }
        }
        
        [UpnpAction (OmitUnless = "CanCreateObject")]
        public virtual void CreateObject ([UpnpArgument ("ContainerID")] string containerId,
                                          [UpnpArgument ("Elements"), UpnpRelatedStateVariable ("A_ARG_TYPE_Result")] string elements,
                                          [UpnpArgument ("ObjectID"), UpnpRelatedStateVariable ("A_ARG_TYPE_Result")] out string objectId,
                                          [UpnpArgument ("Result"), UpnpRelatedStateVariable ("A_ARG_TYPE_Result")] out string result)
        {
            objectId = CreateObject (containerId, elements, out result);
        }

        protected virtual string CreateObject (string containerId, string elements, out string result)
        {
            throw new NotImplementedException ();
        }
        
        public virtual bool CanDestroyObject {
            get { return false; }
        }
        
        [UpnpAction (OmitUnless = "CanDestroyObject")]
        public virtual void DestroyObject ([UpnpArgument ("ObjectID")] string objectId)
        {
            throw new NotImplementedException ();
        }
        
        public virtual bool CanUpdateObject {
            get { return false; }
        }
        
        [UpnpAction (OmitUnless = "CanUpdateObject")]
        public virtual void UpdateObject ([UpnpArgument ("ObjectID")] string objectId,
                                          [UpnpArgument ("CurrentTagValue"), UpnpRelatedStateVariable ("A_ARG_TYPE_TagValueList")] string currentTagValue,
                                          [UpnpArgument ("NewTagValue"), UpnpRelatedStateVariable ("A_ARG_TYPE_TagValueList")] string newTagValue)
        {
            throw new NotImplementedException ();
        }
        
        public virtual bool CanImportResource {
            get { return false; }
        }
        
        [UpnpAction (OmitUnless = "CanImportResource")]
        public virtual void ImportResource ([UpnpArgument ("SourceURI"), UpnpRelatedStateVariable ("A_ARG_TYPE_URI")] Uri sourceUri,
                                            [UpnpArgument ("DestinationURI"), UpnpRelatedStateVariable ("A_ARG_TYPE_URI")] Uri destinationUri,
                                            [UpnpArgument ("TransferID")] out int transferId)
        {
            transferId = ImportResource (sourceUri, destinationUri);
        }

        protected virtual int ImportResource (Uri sourceUri, Uri destinationUri)
        {
            throw new NotImplementedException ();
        }
        
        public virtual bool CanExportResource {
            get { return false; }
        }
        
        [UpnpAction (OmitUnless = "CanExportResource")]
        public virtual void ExportResource ([UpnpArgument ("SourceURI"), UpnpRelatedStateVariable ("A_ARG_TYPE_URI")] Uri sourceUri,
                                            [UpnpArgument ("DestinationURI"), UpnpRelatedStateVariable ("A_ARG_TYPE_URI")] Uri destinationUri,
                                            [UpnpArgument ("TransferID")] out int transferId)
        {
            transferId = ExportResource (sourceUri, destinationUri);
        }

        protected virtual int ExportResource (Uri sourceUri, Uri destinationUri)
        {
            throw new NotImplementedException ();
        }
        
        public virtual bool CanStopTransferResource {
            get { return false; }
        }
        
        [UpnpAction (OmitUnless = "CanStopTransferResource")]
        public virtual void StopTransferResource ([UpnpArgument ("TransferId")] int transferId)
        {
            throw new NotImplementedException ();
        }
        
        public virtual bool CanGetTransferProgress {
            get { return false; }
        }
        
        [UpnpAction (OmitUnless = "CanGetTransferProgress")]
        public virtual void GetTransferProgress ([UpnpArgument ("TransferId")] int transferId,
                                                 [UpnpArgument ("TransferStatus")] out string transferStatus,
                                                 [UpnpArgument ("TransferLength")] out string transferLength,
                                                 [UpnpArgument ("TransferTotal")] out string transferTotal)
        {
            transferStatus = GetTransferProgress (transferId, out transferLength, out transferTotal);
        }

        protected virtual string GetTransferProgress (int transferId,
                                                      out string transferLength,
                                                      out string transferTotal)
        {
            throw new NotImplementedException ();
        }
        
        public virtual bool CanDeleteResource {
            get { return false; }
        }
        
        [UpnpAction (OmitUnless = "CanDeleteResource")]
        public virtual void DeleteResource ([UpnpArgument ("ResourceURI"), UpnpRelatedStateVariable ("A_ARG_TYPE_URI")] Uri resourceUri)
        {
            throw new NotImplementedException ();
        }
        
        public virtual bool CanCreateReference {
            get { return false; }
        }
        
        [UpnpAction (OmitUnless = "CanCreateReference")]
        public virtual void CreateReference ([UpnpArgument ("ContainerId"), UpnpRelatedStateVariable ("A_ARG_TYPE_ObjectID")] string containerId,
                                             [UpnpArgument ("ObjectID")] string objectId,
                                             [UpnpArgument ("NewID"), UpnpRelatedStateVariable ("A_ARG_TYPE_ObjectID")] out string newId)
        {
            newId = CreateReference (containerId, objectId);
        }

        protected virtual string CreateReference (string containerId, string objectId)
        {
            throw new NotImplementedException ();
        }
        
        [UpnpStateVariable ("SystemUpdateID")]
        public virtual event EventHandler<StateVariableChangedArgs<string>> SystemUpdateIdChanged;
        
        public virtual bool HasTransferIds {
            get { return false; }
        }
        
        [UpnpStateVariable ("TransferIDs", OmitUnless = "HasTransferIds")]
        public virtual event EventHandler<StateVariableChangedArgs<string>> TransferIdsChanged;
        
        public virtual bool HasContainerUpdateIds {
            get { return false; }
        }
        
        [UpnpStateVariable ("ContainerUpdateIDs", OmitUnless = "HasContainerUpdateIds")]
        public virtual event EventHandler<StateVariableChangedArgs<string>> ContainerUpdateIdsChanged;
        
        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }
        
        protected virtual void Dispose (bool disposing)
        {
        }
    }
}
