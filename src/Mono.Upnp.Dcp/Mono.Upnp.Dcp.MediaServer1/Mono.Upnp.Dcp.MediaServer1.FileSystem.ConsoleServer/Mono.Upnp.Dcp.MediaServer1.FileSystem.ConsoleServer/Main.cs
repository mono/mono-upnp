// 
// Main.cs
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

using NDesk.Options;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem.ConsoleServer
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var path = Directory.GetCurrentDirectory ();
            var udn = "uuid:" + Guid.NewGuid ().ToString ();
            var friendly_name = "Mono.Upnp File System MediaServer";
            var manufacturer = "Mono Project";
            var model_name = "Mono.Upnp.Dcp.MediaServer1.FileSystem.ConsoleClient";
            var manufacturer_url = new Uri ("http://www.mono-project.org/");
            var model_description = "A console client for sharing file system folders over the MediaServer1 spec.";
            string model_number = null;
            Uri model_url = null;
            string serial_number = null;
            string upc = null;
            
            var options = new OptionSet {
                { "p|path", v => path = v },
                { "u|udn", v => udn = v },
                { "f|friendly-name", v => friendly_name = v },
                { "m|manufacturer", v => manufacturer = v },
                { "n|name", v => model_name = v },
                { "manufacturer-url", v => manufacturer_url = new Uri (v) },
                { "d|description", v => model_description = v },
                { "model-number", v => model_number = v },
                { "model_url", v => model_url = new Uri (v) },
                { "serial-number", v => serial_number = v },
                { "upc", v => upc = v }
            };
            
            options.Parse (args);
            
            var connection_manager = new DummyConnectionManager ();
            var content_directory = new FileSystemContentDirectory (path);
            var media_server = new MediaServer (
                new MediaServerSettings (
                    udn,
                    friendly_name,
                    manufacturer,
                    model_name) {
                    ManufacturerUrl = manufacturer_url,
                    ModelDescription = model_description,
                    ModelNumber = model_number,
                    ModelUrl = model_url,
                    SerialNumber = serial_number,
                    Upc = upc
                },
                connection_manager,
                content_directory
            );
            
            media_server.Start ();
        }
    }
}
