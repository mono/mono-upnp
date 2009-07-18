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
        readonly HttpListener listener;
        
        public FileSystemContentDirectory (string path)
        {
            if (path == null) throw new ArgumentNullException ("path");
            
            CreateFolder (path);
            
            listener = new HttpListener { IgnoreWriteExceptions = true };
            listener.Prefixes.Add (prefix);
        }
        
        public override void Start ()
        {
            base.Start ();
            
            lock (listener) {
                listener.Start ();
                listener.BeginGetContext (OnGotContext, null);
            }
        }

        public override void Stop ()
        {
            base.Stop ();
            
            lock (listener) {
                listener.Stop ();
            }
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
                    return;
                }
                
                int id;
                
                if (!int.TryParse (query.Substring (4), out id)) {
                    return;
                }
                
                if (id >= object_cache.Count) {
                    return;
                }
                
                try {
                    using (var reader = File.OpenRead (object_cache[id].Path)) {
                        response.ContentType = "audio/mpeg";
                        response.ContentLength64 = reader.Length;
                        using (var stream = response.OutputStream) {
                            using (var writer = new BinaryWriter (stream)) {
                                var buffer = new byte[8192];
                                var read = reader.Read (buffer, 0, buffer.Length);
                                while (read > 0) {
                                    writer.Write (buffer, 0, read);
                                    read = reader.Read (buffer, 0, buffer.Length);
                                }
                            }
                        }
                    }
                } catch {
                }
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
            
            for (int i = range.Lower + startIndex, numberReturned = 0; i < range.Upper && numberReturned <= requestCount; i++) {
                numberReturned++;
                yield return object_cache[i].Object;
            }
        }
        
        Range GetChildren (string id)
        {
            var container = folder_cache[id];
            var lower = object_cache.Count;
            
            foreach (var directory in container.Directories) {
                CreateFolder (directory);
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
                    Title = Path.GetFileNameWithoutExtension (path)
                };
                music_track.AddResource (new Resource (new ResourceSettings (
                    new Uri (string.Format ("{0}object?id={1}", prefix, music_track.Id)) ) {
                    ProtocolInfo = "http-get:*:audio/mpeg:*"
                }));
                return music_track;
            case ".avi":
                return new Movie (this, parent) {
                    Title = Path.GetFileNameWithoutExtension (path)
                };
            default:
                return null;
            }
        }
        
        void CreateFolder (string path)
        {
            var directories = Directory.GetDirectories (path);
            var files = Directory.GetFiles (path);
            var folder = new StorageFolder (this) {
                Title = Path.GetDirectoryName (path),
                ChildCount = directories.Length + files.Length
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
