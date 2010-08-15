//
// Wmp11ContentDirectoryBuilder.cs
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

using Mono.Upnp.Dcp.MediaServer1.ConnectionManager1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV;

using Object = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem.Wmp11
{
    public class Wmp11ContentDirectoryBuilder
    {
        readonly Uri url;
        Dictionary<string, ObjectInfo> objects = new Dictionary<string, ObjectInfo> ();
        Dictionary<string, ContainerInfo> containers = new Dictionary<string, ContainerInfo> ();
        Wmp11MusicBuilder music_builder;
        int id;

        public Wmp11ContentDirectoryBuilder (Uri url)
        {
            if (url == null) {
                throw new ArgumentNullException ("url");
            }

            this.url = url;
            this.music_builder = new Wmp11MusicBuilder ();
        }

        public void OnFile (string path)
        {
            switch (Path.GetExtension (path)) {
            case ".wma":
            case ".wav":
            case ".mp3":
                var id = GetId ();
                var resources = new[] {
                    new Resource (new Uri (url, id), new ResourceOptions {
                        ProtocolInfo = new ProtocolInfo ("http-get", "audio/mpeg")
                    })
                };
                music_builder.OnTag (id, resources, TagLib.File.Create (path).Tag,
                    @object => objects.Add (@object.Id, new ObjectInfo (@object, path)));
                break;
            }
        }

        string GetId ()
        {
            return string.Concat ("object", (id++).ToString ());
        }

        void ContainerInfoConsumer (ContainerInfo containerInfo)
        {
            objects.Add (containerInfo.Container.Id, new ObjectInfo (containerInfo.Container));
            containers.Add (containerInfo.Container.Id, containerInfo);
        }

        public Wmp11ContentDirectory Build ()
        {
            var containers = new List<Object> (4);

            containers.Add (music_builder.Build (ContainerInfoConsumer));

            ContainerInfoConsumer (new ContainerInfo (new Container (Wmp11Ids.Root, "-1",
                new ContainerOptions { Title = "Root", ChildCount = 4 }), containers));

            var content_directory = new Wmp11ContentDirectory (url, this.objects, this.containers);
            this.objects = new Dictionary<string, ObjectInfo> ();
            this.containers = new Dictionary<string, ContainerInfo> ();
            return content_directory;
        }
    }
}
