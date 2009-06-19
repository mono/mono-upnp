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
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public class ContentDirectory
    {
        readonly XmlSerializer serializer = new XmlSerializer ();
        
        [UpnpAction]
        [return: UpnpReturnValue (false)]
        [return: UpnpArgument ("SearchCaps")]
        [return: UpnpRelatedStateVariable ("SearchCapabilities")]
        public virtual string GetSearchCapabilities ()
        {
            return GetSearchCapabilitiesCore ();
        }
        
        [UpnpAction]
        [return: UpnpReturnValue (false)]
        [return: UpnpArgument ("SortCaps")]
        [return: UpnpRelatedStateVariable ("SortCapabilities")]
        public virtual string GetSortCapabilities ()
        {
            return GetSortCapabilitiesCore ();
        }
        
        [UpnpAction]
        [return: UpnpReturnValue (false)]
        [return: UpnpArgument ("Id")]
        [return: UpnpRelatedStateVariable ("SystemUpdateID")]
        public virtual string GetSystemUpdateID ()
        {
            return GetSystemUpdateIDCore ();
        }
        
        [UpnpAction]
        [return: UpnpReturnValue (false)]
        [return: UpnpArgument ("Result")]
        public virtual string Browse ([UpnpArgument ("ObjectID")] string objectId,
                                      [UpnpArgument ("BrowseFlag")] BrowseFlag browseFlag,
                                      [UpnpArgument ("StartingIndex")] int startingIndex,
                                      [UpnpArgument ("RequestCount")] int requestCount,
                                      [UpnpArgument ("SortCriteria")] string sortCriteria,
                                      [UpnpArgument ("NumberReturned")] out int numberReturned,
                                      [UpnpArgument ("TotalMatches")] out int totalMatches,
                                      [UpnpArgument ("UpdateID")] out string updateId)
        {
            return BrowseCore (objectId, browseFlag, startingIndex, requestCount, sortCriteria, out numberReturned, out totalMatches);
        }
    }
}
