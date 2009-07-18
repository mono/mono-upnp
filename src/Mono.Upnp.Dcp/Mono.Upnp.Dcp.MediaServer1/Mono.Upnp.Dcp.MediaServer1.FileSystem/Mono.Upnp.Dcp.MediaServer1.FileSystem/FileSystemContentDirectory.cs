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

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Av;

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
        
        readonly List<Object> object_cache = new List<Object> ();
        readonly Dictionary<string, Range> object_hierarchy = new Dictionary<string, Range> ();
        readonly Dictionary<string, FolderInfo> folder_cache = new Dictionary<string, FolderInfo> ();
        
        public FileSystemContentDirectory (string path)
        {
            if (path == null) throw new ArgumentNullException ("path");
            
            CreateFolder (path);
        }
        
        protected override string SearchCapabilities {
            get { return string.Empty; }
        }

        protected override string SortCapabilities {
            get { return string.Empty; }
        }
        
        protected override Object GetObject (string objectId)
        {
            return object_cache[int.Parse (objectId)];
        }
        
        protected override IEnumerable<Object> GetChildren (string objectId, int startIndex, int requestCount, string sortCriteria, out int totalMatches)
        {
            var id = int.Parse (objectId);
            var container = (Container)object_cache[id];
            totalMatches = container.ChildCount.Value;
            return GetChildren (objectId, startIndex, requestCount, sortCriteria);
        }
        
        protected virtual IEnumerable<Object> GetChildren (string objectId, int startIndex, int requestCount, string sortCriteria)
        {
            Range range;
            
            if (!object_hierarchy.TryGetValue (objectId, out range)) {
                range = GetChildren (objectId);
                object_hierarchy[objectId] = range;
            }
            
            for (int i = range.Lower + startIndex, numberReturned = 0; i < range.Upper && numberReturned <= requestCount; i++) {
                numberReturned++;
                var obj = object_cache[i];
                if (obj != null) {
                    yield return obj;
                }
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
                object_cache.Add (CreateObject (file, container.Folder));
            }
            
            folder_cache.Remove (id);
            
            return new Range (lower, object_cache.Count);
        }
        
        protected virtual Object CreateObject (string path, Container parent)
        {
            switch (Path.GetExtension (path)) {
            case "mp3":
                return new MusicTrack (this, parent);
            case "avi":
                return new Movie (this, parent);
            default:
                return null;
            }
        }
        
        void CreateFolder (string path)
        {
            var directories = Directory.GetDirectories (path);
            var files = Directory.GetFiles (path);
            var folder = new StorageFolder (this);
            folder.ChildCount = directories.Length + files.Length;
            object_cache.Add (folder);
            folder_cache[folder.Id] = new FolderInfo (folder, directories, files);
        }
    }
}
