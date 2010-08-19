// 
// RemoteContentDirectory.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Thomas
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
using System.IO;
using System.Text;
using System.Xml;

using Mono.Upnp.Dcp.MediaServer1.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public class RemoteContentDirectory
    {
        ContentDirectoryController controller;
        XmlDeserializer deserializer;

        public RemoteContentDirectory (ContentDirectoryController controller)
            : this (controller, null)
        {
        }

        public RemoteContentDirectory (ContentDirectoryController controller, XmlDeserializer deserializer)
        {
             if (controller == null) {
                throw new ArgumentNullException ("controller");
            }

            this.controller = controller;
            this.deserializer = deserializer ?? new XmlDeserializer ();
        }

        public Container GetRootObject ()
        {
            return GetObject<Container> ("0");
        }

        public T GetObject<T> (string id)
        {
            uint returned, total, update_id;
            var xml = controller.Browse (
                id, BrowseFlag.BrowseMetadata, "*", 0, 1, "", out returned, out total, out update_id);
            foreach (var result in Deserialize<T> (xml)) {
                return result;
            }
            return default (T);
        }

        public Results<T> GetChildren<T> (Container container, ResultsSettings settings)
        {
            if (container == null) {
                throw new ArgumentNullException ("container");
            }

            uint returned, total, update_id;
            var xml = controller.Browse (container.Id, BrowseFlag.BrowseDirectChildren, settings.Filter,
                settings.Offset, settings.RequestCount, settings.SortCriteria, out returned, out total, out update_id);

            var results = new List<T> ((int)returned);
            foreach (var result in Deserialize<T> (xml)) {
                results.Add (result);
            }

            return new BrowseResults<T> (container, settings.SortCriteria, settings.Filter,
                settings.RequestCount, settings.Offset, total, results);
        }

        public Results<T> Search<T> (Container container, Action<QueryVisitor> query, ResultsSettings settings)
        {
            if (container == null) {
                throw new ArgumentNullException ("container");
            } else if (query == null) {
                throw new ArgumentNullException ("query");
            }

            var builder = new StringBuilder ();
            query (new QueryStringifier (builder));
            var search_criteria = builder.ToString ();

            uint returned, total, update_id;
            var xml = controller.Search (container.Id, search_criteria, settings.Filter, settings.Offset,
                settings.RequestCount, settings.SortCriteria, out returned, out total, out update_id);

            var results = new List<T> ((int)returned);
            foreach (var result in Deserialize<T> (xml)) {
                results.Add (result);
            }

            return new SearchResults<T> (container, query, settings.SortCriteria,
                settings.Filter, settings.RequestCount, settings.Offset, total, results);
        }

        protected virtual IEnumerable<T> Deserialize<T> (string xml)
        {
            using (var reader = XmlReader.Create (new StringReader (xml))) {
                if (reader.MoveToContent () != XmlNodeType.Element) {
                }
                if (!reader.ReadToFollowing ("DIDL-Lite")) {
                }
                if (!reader.Read ()) {
                }
                while (reader.NodeType != XmlNodeType.Element) {
                    if (!reader.Read ()) {
                        // TODO throw
                    }
                }
                while (reader.NodeType == XmlNodeType.Element) {
                    using (var subtree = reader.ReadSubtree ()) {
                        subtree.Read ();
                        yield return Deserialize<T> (subtree);
                    }
                    if (!reader.ReadToNextSibling (reader.LocalName)) {
                    }
                }
            }
        }

        protected virtual T Deserialize<T> (XmlReader reader)
        {
            return deserializer.Deserialize<T> (reader);
        }
    }
}
