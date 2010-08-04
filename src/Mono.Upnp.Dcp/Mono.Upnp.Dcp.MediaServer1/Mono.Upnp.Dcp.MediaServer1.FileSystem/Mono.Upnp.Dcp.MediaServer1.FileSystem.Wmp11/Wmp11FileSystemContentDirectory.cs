// 
// Wmp11FileSystemContentDirectory.cs
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

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV;

using Object = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem.Wmp11
{
    public class Wmp11FileSystemContentDirectory : ObjectBasedContentDirectory
    {
        Dictionary<string, Object> objects = new Dictionary<string, Object> ();
        Dictionary<string, ContainerInfo> containers = new Dictionary<string, ContainerInfo> ();

        public Wmp11FileSystemContentDirectory ()
        {
        }

        protected override Object GetObject (string objectId)
        {
            return objects[objectId];
        }

        protected override IEnumerable<Object> GetChildren (string objectId,
                                                            int startIndex,
                                                            int requestCount,
                                                            string sortCriteria,
                                                            out int totalMatches)
        {
            var container = containers[objectId];
            totalMatches = container.Children.Count;
            return GetResults (container.Children, startIndex, requestCount);
        }

        static IEnumerable<T> GetResults<T> (IList<T> objects, int startIndex, int requestCount)
        {
            var endIndex = System.Math.Min (startIndex + requestCount, objects.Count);
            for (var i = startIndex; i < endIndex; i++) {
                yield return objects[i];
            }
        }

        protected override IEnumerable<Object> Search (string containerId,
                                                       Query query,
                                                       int startingIndex,
                                                       int requestCount,
                                                       string sortCriteria,
                                                       out int totalMatches)
        {
            var visitor = new Wmp11QueryVisitor (this);
            if (visitor.Results != null) {
                totalMatches = visitor.Results.Count;
                return GetResults (visitor.Results, startingIndex, requestCount);
            } else {
                return base.Search (containerId, query, startingIndex, requestCount, sortCriteria, out totalMatches);
            }
        }

        protected override string SearchCapabilities {
            get { return string.Empty; }
        }

        protected override string SortCapabilities {
            get { return string.Empty; }
        }

        class Wmp11QueryVisitor : QueryVisitor
        {
            readonly Wmp11FileSystemContentDirectory content_directory;

            public Wmp11QueryVisitor (Wmp11FileSystemContentDirectory contentDirectory)
            {
                this.content_directory = contentDirectory;
            }

            public override void VisitDerivedFrom (string property, string value)
            {
                if (property != "upnp:class") {
                    return;
                }
                switch (value) {
                case "object.item.audioItem":
                    Results = content_directory.containers[Wmp11Ids.AllMusic].Children;
                    break;
                }
            }

            public IList<Object> Results { get; private set; }
        }
    }
}
