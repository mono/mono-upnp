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
        bool started;
        ulong system_id;
        ulong ids;
        
        ~ContentDirectory()
        {
            Dispose (false);
        }
        
        internal string GetNewObjectId ()
        {
            var id = ids.ToString ();
            ids++;
            return id;
        }
        
        protected internal void OnSystemUpdate ()
        {
            if (started) {
                system_id++;
            }
        }
        
        public virtual void Start ()
        {
            started = true;
        }
        
        public virtual void Stop ()
        {
            started = false;
        }
        
        public static readonly ServiceType ServiceType = new ServiceType (
            "urn:schemas-upnp-org:service:ContentDirectory:1");
        
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
        
        [UpnpAction]
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
            result = Browse (objectId, browseFlag, filter, startingIndex, requestCount, sortCriteria, out numberReturned, out totalMatches, out updateId);
        }
        
        protected abstract string Browse (string objectId, BrowseFlag browseFlag, string filter, int startIndex,
                                          int requestCount, string sortCriteria, out int numberReturned,
                                          out int totalMatches, out string updateId);
        
        [UpnpStateVariable ("SystemUpdateID")]
        public virtual event EventHandler<StateVariableChangedArgs<string>> SystemUpdateIdChanged;
        
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
