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
using System.Text;
using System.Xml;

using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public abstract class ObjectBasedContentDirectory : LocalContentDirectory
    {
        XmlSerializer serializer = new XmlSerializer ();
        Dictionary<Type, ObjectQueryContext> queryContexts = new Dictionary<Type, ObjectQueryContext>();

        ObjectQueryContext GetQueryContext (Type type)
        {
            if (type == null) {
                return null;
            }

            ObjectQueryContext context;
            if (!queryContexts.TryGetValue (type, out context)) {
                context = new ObjectQueryContext (type, GetQueryContext (type.BaseType));
                queryContexts[type] = context;
            }

            return context;
        }

        public override bool CanSearch {
            get { return true; }
        }

        protected override string Search (string containerId,
                                          Action<QueryVisitor> query,
                                          string filter,
                                          int startingIndex,
                                          int requestCount,
                                          string sortCriteria,
                                          out int numberReturned,
                                          out int totalMatches,
                                          out string updateId)
        {
            updateId = "0";
            var serializer = new ResultsSerializer (this.serializer);
            Search (result => serializer.Serialize (result), containerId, query, startingIndex,
                requestCount, sortCriteria, out numberReturned, out totalMatches);
            return serializer.ToString ();
        }

        protected virtual void Search (Action<Object> consumer,
                                       string containerId,
                                       Action<QueryVisitor> query,
                                       int startingIndex,
                                       int requestCount,
                                       string sortCriteria,
                                       out int numberReturned,
                                       out int totalMatches)
        {
            if (consumer == null) {
                throw new ArgumentNullException ("consumer");
            } else if (query == null) {
                throw new ArgumentNullException ("query");
            }

            var total = 0;
            var count = 0;

            Action<Object> query_consumer = result => {
                total++;
                if (total > startingIndex && count < requestCount) {
                    consumer (result);
                    count++;
                }
            };

            VisitChildren (child => {
                query (new ObjectQueryVisitor (GetQueryContext (child.GetType ()), child, query_consumer));
            }, containerId, 0, -1, string.Empty, out totalMatches);

            totalMatches = total;
            numberReturned = count;
        }

        protected override string Browse (string objectId,
                                          BrowseFlag browseFlag,
                                          string filter,
                                          int startIndex,
                                          int requestCount,
                                          string sortCriteria,
                                          out int numberReturned,
                                          out int totalMatches,
                                          out string updateId)
        {
            var serializer = new ResultsSerializer (this.serializer);

            var @object = GetObject (objectId);
            if (browseFlag == BrowseFlag.BrowseDirectChildren) {
                numberReturned = VisitChildren (child => serializer.Serialize (child),
                    @object.Id, startIndex, requestCount, sortCriteria, out totalMatches);
            } else {
                serializer.Serialize (@object);
                numberReturned = 1;
                totalMatches = 1;
            }

            updateId = "0";
            return serializer.ToString ();
        }
        
        protected abstract int VisitChildren (Action<Object> consumer,
                                              string objectId,
                                              int startIndex,
                                              int requestCount,
                                              string sortCriteria,
                                              out int totalMatches);
        
        protected abstract Object GetObject (string objectId);

        class ResultsSerializer
        {
            XmlSerializer serializer;
            StringBuilder builder = new StringBuilder ();
            XmlWriter writer;

            public ResultsSerializer (XmlSerializer serializer)
            {
                this.serializer = serializer;
                writer = XmlWriter.Create (builder);
                writer.WriteStartElement ("DIDL-Lite", Schemas.DidlLiteSchema);
                writer.WriteAttributeString ("xmlns", "dc", null, Schemas.DublinCoreSchema);
                writer.WriteAttributeString ("xmlns", "upnp", null, Schemas.UpnpSchema);
            }

            public void Serialize<T> (T item)
            {
                serializer.Serialize (item, writer);
            }

            public override string ToString ()
            {
                writer.WriteEndElement ();
                return builder.ToString ();
            }
        }
    }
}
