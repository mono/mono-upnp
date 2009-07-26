// 
// ObjectBasedContentDirectory.cs
//  
// Author:
//       scott <${AuthorEmail}>
// 
// Copyright (c) 2009 scott
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
using System.Collections.Generic;

using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public abstract class ObjectBasedContentDirectory : ContentDirectory
    {
        [XmlType ("DIDL-Lite", Schemas.DidlLiteSchema)]
        [XmlNamespace (Schemas.DublinCoreSchema, "dc")]
        [XmlNamespace (Schemas.UpnpSchema, "upnp")]
        class ResultsWrapper : IXmlSerializable
        {
            readonly IEnumerable<IXmlSerializable> results;
            
            public ResultsWrapper (IXmlSerializable result)
                : this (Single (result))
            {
            }
            
            public ResultsWrapper (IEnumerable<IXmlSerializable> results)
            {
                this.results = results;
            }
            
            public IEnumerable<IXmlSerializable> Results {
                get { return results; }
            }
            
            public int ResultsCount { get; private set; }
            
            static IEnumerable<IXmlSerializable> Single (IXmlSerializable item)
            {
                yield return item;
            }
            
            public void SerializeSelfAndMembers (XmlSerializationContext context)
            {
                context.AutoSerializeObjectAndMembers (this);
            }
            
            public void SerializeMembersOnly (XmlSerializationContext context)
            {
                foreach (var result in Results) {
                    result.SerializeSelfAndMembers (context);
                    ResultsCount++;
                }
            }
        }
        
        readonly XmlSerializer serializer = new XmlSerializer ();
        
        protected override string Browse (string objectId, BrowseFlag browseFlag, string filter, int startIndex,
                                          int requestCount, string sortCriteria, out int numberReturned,
                                          out int totalMatches, out string updateId)
        {
            updateId = null;
            if (browseFlag == BrowseFlag.BrowseDirectChildren) {
                var children = GetChildren (objectId, startIndex, requestCount, sortCriteria, out totalMatches);
                var results = new ResultsWrapper (children);
                var xml = serializer.GetString (results);
                numberReturned = results.ResultsCount;
                return xml;
            } else {
                var @object = GetObject (objectId);
                if (@object == null) {
                    numberReturned = 0;
                    totalMatches = 0;
                } else {
                    numberReturned = 1;
                    totalMatches = 1;
                }
                return serializer.GetString (new ResultsWrapper (@object),
                    new XmlSerializationOptions { XmlDeclarationType = XmlDeclarationType.None });
            }
        }
        
        protected abstract IEnumerable<IXmlSerializable> GetChildren (string objectId, int startIndex, int requestCount,
                                                                        string sortCriteria, out int totalMatches);
        
        protected abstract IXmlSerializable GetObject (string objectId);
    }
}
