//
// Wmp11RootBuilder.cs
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

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV;

using Object = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem.Wmp11
{
    public class Wmp11RootBuilder : Wmp11ContainerBuilder
    {
        Wmp11MusicBuilder music_builder = new Wmp11MusicBuilder ();

        public void OnFile (string path, Action<Object> consumer)
        {
            switch (Path.GetExtension (path)) {
            case "mp3":
                music_builder.OnTag (TagLib.File.Create (path).Tag, consumer);
                break;
            }
        }

        public void OnDone (Action<ContainerInfo> consumer)
        {
            var containers = new List<Object> (4);

            containers.Add (BuildContainer (consumer, Wmp11Ids.Music, "Music", music_builder.OnDone (consumer)));

            consumer (new ContainerInfo (new Container (Wmp11Ids.Root, "-1",
                new ContainerOptions { Title = "Root", ChildCount = 4 }), containers));
        }

        protected override string ContainerId {
            get { return Wmp11Ids.Root; }
        }
    }
}
