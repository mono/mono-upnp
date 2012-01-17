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
using System.Reflection;
using System.Text;
using System.Xml;

using Mono.Upnp.Dcp.MediaServer1.Internal;
using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public class RemoteContentDirectory
    {
        ContentDirectoryController controller;
        XmlDeserializer deserializer;
        MethodInfo deserialize_method;

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
            this.deserialize_method = this.deserializer.GetType ().GetMethod (
                "Deserialize", new Type[] { typeof (XmlReader) });
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

        public Results<T> GetChildren<T> (Container container)
        {
            return GetChildren<T> (container, new ResultsSettings ());
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
            var types = new List<Type> ();
            foreach (var @object in Deserialize<Object> (xml, Deserializers ())) {
                types.Add (ClassManager.GetTypeFromClassName (@object.Class.FullClassName));
            }
            return Deserialize<T> (xml, Deserializers<T> (types));
        }

        IEnumerable<Mono.Upnp.Internal.Func<XmlReader, Object>> Deserializers ()
        {
            while (true) {
                yield return reader => deserializer.Deserialize<Object> (reader);
            }
        }

        IEnumerable<Mono.Upnp.Internal.Func<XmlReader, T>> Deserializers<T> (IEnumerable<Type> types)
        {
            foreach (var type in types) {
                var method = deserialize_method.MakeGenericMethod (type);
                yield return reader => (T)method.Invoke (deserializer, new object[] { reader });
            }
        }

        IEnumerable<T> Deserialize<T> (string xml, IEnumerable<Mono.Upnp.Internal.Func<XmlReader, T>> deserializers)
        {
            var enumerator = deserializers.GetEnumerator ();
            using (var reader = XmlReader.Create (new StringReader (xml))) {
                if (reader.MoveToContent () != XmlNodeType.Element) {
                    throw new DeserializationException ("Unable to read XML content");
                }
                if (reader.LocalName != "DIDL-Lite" && !reader.ReadToFollowing ("DIDL-Lite")) {
                        throw new DeserializationException ("There is no DIDL-Lite element.");
                }
                if (!reader.Read ()) {
                    throw new DeserializationException ("The DIDL-Lite element has no proper children.");
                }
                while (reader.NodeType != XmlNodeType.Element) {
                    if (!reader.Read ()) {
                        throw new DeserializationException ("The DIDL-Lite element has no proper children.");
                    }
                }
                while (reader.NodeType == XmlNodeType.Element) {
                    using (var subtree = reader.ReadSubtree ()) {
                        subtree.Read ();
                        if (!enumerator.MoveNext ()) {
                            throw new ArgumentNullException ("types", "The enumeration of deserializers must have" +
                                " as many elements are there are objects in the XML.");
                        }
                        yield return enumerator.Current (subtree);
                    }
                    if (!reader.ReadToNextSibling (reader.LocalName)) {
                    }
                }
            }
        }
    }
}
