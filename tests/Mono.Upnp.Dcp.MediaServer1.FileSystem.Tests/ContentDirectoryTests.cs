// 
// Test.cs
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
using System.IO;

using NUnit.Framework;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem.Tests
{
    [TestFixture]
    public class Test
    {
        MediaServer media_server;
        
        [SetUp]
        public void SetUp ()
        {
            var path = Path.Combine (Directory.GetCurrentDirectory (), "ContentDirectory");
            var connection_manager = new DummyConnectionManager ();
            var content_directory = new FileSystemContentDirectory (path);
            
            media_server = new MediaServer (
                new MediaServerSettings (
                    "uuid:mono-upnp-dcp-mediaserver1-filesystem-test",
                    "Mono.Upnp Test FileSystem MediaServer",
                    "Mono Project",
                    "FileSystem MediaServer"
                ),
                connection_manager,
                content_directory
            );
            
            media_server.Start ();
        }
        
        [TearDown]
        public void TearDown ()
        {
            media_server.Dispose ();
        }
        
        [Test]
        public void TestCase ()
        {
        }
    }
}
