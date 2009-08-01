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
using System.Net.Sockets;

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Av;
using Mono.Upnp.Xml;

using Object = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem
{
    public class FileSystemContentDirectory : ObjectBasedContentDirectory
    {
        struct Range
        {
            public readonly int Lower;
            public readonly int Upper;
            
            public Range (int lower, int upper)
            {
                Lower = lower;
                Upper = upper;
            }
        }
        
        struct ObjectInfo
        {
            public readonly Object Object;
            public readonly string Path;
            
            public ObjectInfo (Object @object, string path)
            {
                Object = @object;
                Path = path;
            }
        }
        
        class FolderInfo
        {
            public readonly StorageFolder Folder;
            public readonly string[] Directories;
            public readonly string[] Files;
            
            public FolderInfo (StorageFolder folder, string[] directories, string[] files)
            {
                Folder = folder;
                Directories = directories;
                Files = files;
            }
        }
        
        readonly List<ObjectInfo> object_cache = new List<ObjectInfo> ();
        readonly Dictionary<string, Range> object_hierarchy = new Dictionary<string, Range> ();
        readonly Dictionary<string, FolderInfo> folder_cache = new Dictionary<string, FolderInfo> ();
        readonly string prefix = GeneratePrefix ();
        volatile bool started;
        HttpListener listener;
        
        public FileSystemContentDirectory (string path)
        {
            if (path == null) throw new ArgumentNullException ("path");
            
            var root = new StorageFolder (this) {
                IsRestricted = true,
                Title = "root",
                ChildCount = 1
            };
            object_cache.Add (new ObjectInfo (root, null));
            object_hierarchy.Add ("0", new Range (1, 2));
            CreateFolder (path, root);
            
            listener = new HttpListener { IgnoreWriteExceptions = true };
            listener.Prefixes.Add (prefix);
        }
        
        public bool IsStarted {
            get { return started; }
        }
        
        public bool IsDisposed {
            get { return listener == null; }
        }
        
        public override void Start ()
        {
            CheckDisposed ();
            
            if (started) {
                return;
            }
            
            base.Start ();
            
            started = true;
            
            lock (listener) {
                listener.Start ();
                listener.BeginGetContext (OnGotContext, null);
            }
        }

        public override void Stop ()
        {
            CheckDisposed ();
            
            if (!started) {
                return;
            }
            
            started = false;
            
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
        
        void OnGotContext (IAsyncResult result)
        {
            lock (listener) {
                if (!listener.IsListening) {
                    return;
                }
                
                var context = listener.EndGetContext (result);
                GetFile (context.Response, context.Request.Url.Query);
                
                listener.BeginGetContext (OnGotContext, null);
            }
        }
        
        void GetFile (HttpListenerResponse response, string query)
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
                
                if (id >= object_cache.Count) {
                    response.StatusCode = 404;
                    return;
                }
                
                try {
                    using (var reader = System.IO.File.OpenRead (object_cache[id].Path)) {
                        response.ContentType = "audio/mpeg";
                        response.ContentLength64 = reader.Length;
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
        }
        
        void CheckDisposed ()
        {
            if (IsDisposed) {
                throw new ObjectDisposedException (ToString ());
            }
        }
        
        protected override string SearchCapabilities {
            get { return string.Empty; }
        }

        protected override string SortCapabilities {
            get { return string.Empty; }
        }
        
        protected override IXmlSerializable GetObject (string objectId)
        {
            return object_cache[int.Parse (objectId)].Object;
        }
        
        protected override IEnumerable<IXmlSerializable> GetChildren (string objectId, int startIndex, int requestCount, string sortCriteria, out int totalMatches)
        {
            var id = int.Parse (objectId);
            var container = (Container)object_cache[id].Object;
            totalMatches = container.ChildCount.Value;
            return GetChildren (objectId, startIndex, requestCount, sortCriteria);
        }
        
        protected virtual IEnumerable<IXmlSerializable> GetChildren (string objectId, int startIndex, int requestCount, string sortCriteria)
        {
            Range range;
            
            if (!object_hierarchy.TryGetValue (objectId, out range)) {
                range = GetChildren (objectId);
                object_hierarchy[objectId] = range;
            }
            
            for (int i = range.Lower + startIndex, numberReturned = 0; i < range.Upper && numberReturned < requestCount; i++) {
                yield return object_cache[i].Object;
                numberReturned++;
            }
        }
        
        Range GetChildren (string id)
        {
            var container = folder_cache[id];
            var lower = object_cache.Count;
            
            foreach (var directory in container.Directories) {
                CreateFolder (directory, container.Folder);
            }
            
            foreach (var file in container.Files) {
                var obj = CreateObject (file, container.Folder);
                if (obj != null) {
                    object_cache.Add (new ObjectInfo (obj, file));
                }
            }
            
            folder_cache.Remove (id);
            
            return new Range (lower, object_cache.Count);
        }
        
        protected virtual Object CreateObject (string path, Container parent)
        {
            switch (Path.GetExtension (path)) {
            case ".mp3":
                var music_track = new MusicTrack (this, parent) {
                    Title = Path.GetFileNameWithoutExtension (path),
                    IsRestricted = true
                };
                music_track.AddResource (new Resource (new ResourceSettings (
                    new Uri (string.Format ("{0}object?id={1}", prefix, music_track.Id))) {
                    ProtocolInfo = "http-get:*:audio/mpeg:*"
                }));
                return music_track;
            case ".avi":
                return new Movie (this, parent) {
                    Title = Path.GetFileNameWithoutExtension (path),
                    IsRestricted = true
                };
            default:
                return new File (this, parent) {
                    Title = Path.GetFileNameWithoutExtension (path),
                    IsRestricted = true
                };
            }
        }
        
        void CreateFolder (string path, StorageFolder parent)
        {
            var directories = Directory.GetDirectories (path);
            var files = Directory.GetFiles (path);
            var name = path;
            var seperator = name.LastIndexOf (Path.DirectorySeparatorChar);
            if (seperator == name.Length) {
                var start = System.Math.Max (0, name.LastIndexOf (Path.DirectorySeparatorChar, seperator - 1));
                name = name.Substring (start, seperator - start);
            } else if (seperator != -1) {
                seperator += 1;
                name = name.Substring (seperator, name.Length - seperator);
            }
            var folder = new StorageFolder (this, parent) {
                Title = name,
                ChildCount = directories.Length + files.Length,
                IsRestricted = true
            };
            object_cache.Add (new ObjectInfo (folder, path));
            folder_cache[folder.Id] = new FolderInfo (folder, directories, files);
        }
        
        readonly static Random random = new Random ();
                
        static string GeneratePrefix ()
        {
            foreach (var address in Dns.GetHostAddresses (Dns.GetHostName ())) {
                if (address.AddressFamily == AddressFamily.InterNetwork) {
                    return string.Format (
                        "http://{0}:{1}/upnp/media-server/", address, random.Next (1024, 5000));
                }
            }
            
            return null;
        }
    }
}
