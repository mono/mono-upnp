// 
// FileSystemContentDirectory.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Thomas
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
using System.Net;

using Mono.Upnp.Control;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV;
using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

using Object = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object;
using Mono.Upnp.Dcp.MediaServer1.ConnectionManager1;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem
{
    public class FileSystemContentDirectory : ObjectBasedContentDirectory
    {
        const int file_buffer_size = 8192;

        readonly Uri url;
        HttpListener listener;
        IDictionary<string, ObjectInfo> objects;
        IDictionary<string, ContainerInfo> containers;

        public FileSystemContentDirectory (Uri url,
                                           IDictionary<string, ObjectInfo> objects,
                                           IDictionary<string, ContainerInfo> containers)
        {
            if (url == null) {
                throw new ArgumentNullException ("url");
            } else if (objects == null) {
                throw new ArgumentNullException ("objects");
            } else if (containers == null) {
                throw new ArgumentNullException ("containers");
            }

            this.url = url;
            this.objects = objects;
            this.containers = containers;
            this.listener = new HttpListener ();
            listener.Prefixes.Add (url.ToString ());

            Log.Information (string.Format ("FileSystemContentDirectory created at {0}.", url));
        }

        public override void Start ()
        {
            CheckDisposed ();

            if (IsStarted) {
                return;
            }

            listener.Start ();
            listener.BeginGetContext (OnGetContext, null);

            base.Start ();
        }

        public override void Stop ()
        {
            CheckDisposed ();

            if (!IsStarted) {
                return;
            }
            
            base.Stop ();
            
            lock (listener) {
                listener.Stop ();
            }
        }
        
        protected override void Dispose (bool disposing)
        {
            if (IsDisposed) {
                return;
            }
            
            base.Dispose (disposing);
            
            if (disposing) {
                Stop ();
                listener.Close ();
            }
            
            listener = null;
        }
        
        public bool IsDisposed {
            get { return listener == null; }
        }
        
        void CheckDisposed ()
        {
            if (IsDisposed) {
                throw new ObjectDisposedException (ToString ());
            }
        }

        protected override Object GetObject (string objectId)
        {
            ObjectInfo @object;
            if (!objects.TryGetValue (objectId, out @object)) {
                throw new UpnpControlException (
                    Error.NoSuchObject (), string.Format (@"The object ""{0}"" does not exist.", objectId));
            }
            return @object.Object;
        }

        protected override int VisitChildren (Action<Object> consumer,
                                              string objectId,
                                              int startIndex,
                                              int requestCount,
                                              string sortCriteria,
                                              out int totalMatches)
        {
            var children = GetChildren (objectId);
            totalMatches = children.Count;
            return VisitResults (consumer, children, startIndex, requestCount);
        }

        protected static int VisitResults<T> (Action<T> consumer, IList<T> objects, int startIndex, int requestCount)
        {
            if (consumer == null) {
                throw new ArgumentNullException ("consumer");
            } else if (objects == null) {
                throw new ArgumentNullException ("objects");
            }

            var endIndex = requestCount > 0 ? System.Math.Min (startIndex + requestCount, objects.Count) : objects.Count;
            for (var i = startIndex; i < endIndex; i++) {
                consumer (objects[i]);
            }
            return endIndex - startIndex;
        }

        protected IList<Object> GetChildren (string containerId)
        {
            ContainerInfo container;
            if (!containers.TryGetValue (containerId, out container)) {
                throw new UpnpControlException (
                    Error.NoSuchContainer (), string.Format (@"The container ""{0}"" does not exist.", containerId));
            }
            return container.Children;
        }
        
        void OnGetContext (IAsyncResult result)
        {
            lock (listener) {
                if (!listener.IsListening) {
                    return;
                }
                
                var context = listener.EndGetContext (result);

                Log.Information (string.Format ("Got request from {0} for {1}.",
                    context.Request.RemoteEndPoint, context.Request.Url));

                var url = this.url.MakeRelativeUri (context.Request.Url);

                GetFile (context.Response, url.ToString ());
                
                /*if (query.StartsWith ("?id=") && query.Length > 4) {
                    GetFile (context.Response, query.Substring (4));
                } else if (query.StartsWith ("?art=")) {
                    //GetArtwork (context.Response, query);
                } else {
                    context.Response.StatusCode = 404;
                }*/
                
                listener.BeginGetContext (OnGetContext, null);
            }
        }
        
        void GetFile (HttpListenerResponse response, string id)
        {
            using (response) {
                ObjectInfo object_info;
                if (!objects.TryGetValue (id, out object_info) || object_info.Path == null) {
                    Log.Error (string.Format ("The requested object {0} does not exist.", id));

                    response.StatusCode = 404;
                    return;
                }

                Log.Information (string.Format ("Serving file {0}.", object_info.Path));

                using (var reader = System.IO.File.OpenRead (object_info.Path)) {
                    response.ContentType = object_info.Object.Resources[0].ProtocolInfo.ContentFormat;
                    response.ContentLength64 = reader.Length;
                    try {
                        using (var writer = new BinaryWriter (response.OutputStream)) {
                            var buffer = new byte[file_buffer_size];
                            int read;
                            do {
                                read = reader.Read (buffer, 0, buffer.Length);
                                writer.Write (buffer, 0, read);
                            } while (IsStarted && read > 0);
                        }
                    } catch (Exception e) {
                        Log.Exception (string.Format ("Failed while serving file {0}.", object_info.Path), e);
                    }
                }
            }
        }
        
        /*void GetArtwork (HttpListenerResponse response, string query)
        {
            using (response) {
                if (query.Length < 5) {
                    response.StatusCode = 404;
                    return;
                }
                
                int id;
                
                if (!int.TryParse (query.Substring (4), out id)) {
                    response.StatusCode = 404;
                    return;
                }
                
                Console.WriteLine ("Serving artwork for: {0}", object_cache[id].Path);
                
                if (id >= object_cache.Count) {
                    response.StatusCode = 404;
                    return;
                }
                
                var picture = GetAlbumArt (object_cache[id].Path);
                if (picture == null)
                {
                    response.StatusCode = 404;
                    return;
                }
        
                try {
                    using (var reader = new BinaryReader (new MemoryStream (picture.Data.Data))) {
                        response.ContentType = picture.MimeType;
                        response.ContentLength64 = picture.Data.Data.Length;
                        using (var stream = response.OutputStream) {
                            using (var writer = new BinaryWriter (stream)) {
                                var buffer = new byte[8192];
                                int read;
                                do {
                                    read = reader.Read (buffer, 0, buffer.Length);
                                    writer.Write (buffer, 0, read);
                                } while (started && read > 0);
                            }
                        }
                    }
                } catch {
                }
            }
        }*/
        
        protected override string SearchCapabilities {
            get { return string.Empty; }
        }

        protected override string SortCapabilities {
            get { return string.Empty; }
        }
    }
}
