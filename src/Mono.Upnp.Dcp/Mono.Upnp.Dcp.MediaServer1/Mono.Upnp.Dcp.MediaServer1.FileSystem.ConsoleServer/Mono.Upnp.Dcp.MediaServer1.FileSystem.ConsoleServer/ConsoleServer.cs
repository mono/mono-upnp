// 
// ConsoleServer.cs
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
using System.Collections.Generic;

using NDesk.Options;

using Mono.Upnp.Control;
using Mono.Upnp.Dcp.MSMediaServerRegistrar1;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem.ConsoleServer
{
    class ConsoleServer
    {
        public static void Main (string[] args)
        {
            string path = null;
            var udn = "uuid:" + Guid.NewGuid ().ToString ();
            var friendly_name = "Mono.Upnp File System MediaServer";
            var manufacturer = "Mono Project";
            var model_name = "Mono.Upnp.Dcp.MediaServer1.FileSystem.ConsoleClient";
            var manufacturer_url = new Uri ("http://www.mono-project.org/");
            var model_description = "A console client for sharing file system folders over the MediaServer1 spec.";
            var model_number = "1";
            var model_url = new Uri ("http://www.mono-project.org/Mono.Upnp");
            var serial_number = "MONO-UPNP-MEDIA-SERVER-1";
            string upc = null;
            var help = false;
            
            var options = new OptionSet {
                { "p|path=", "the path of the directory to share.", v => path = v },
                { "u|udn=", "the Unique Device Name of the MediaServer device.", v => udn = v },
                { "f|friendly-name=", "the friendly name of the MediaServer device.", v => friendly_name = v },
                { "m|manufacturer=", "The name of the MediaServer device manufacturer.", v => manufacturer = v },
                { "n|name=", "the name of the MediaServer device model.", v => model_name = v },
                { "manufacturer-url=", "the URL for the MediaServer device manufacturer.", v => manufacturer_url = new Uri (v) },
                { "d|description=", "a description of the MediaServer device.", v => model_description = v },
                { "model-number=", "the model number of the MediaServer device.", v => model_number = v },
                { "model_url=", "the URL for the MediaServer device.", v => model_url = new Uri (v) },
                { "serial-number=", "the serial number for the MediaServer device.", v => serial_number = v },
                { "upc=", "the Universal Product Code for the MediaServer device.", v => upc = v },
                { "h|?|help", "show this help message and exit.", v => help = v != null }
            };
            
            List<string> extras;
            
            try {
                extras = options.Parse (args);
            } catch (Exception e) {
                Console.WriteLine ("mono-upnp-simple-media-server:");
                Console.WriteLine (e.Message);
                Console.WriteLine ("Try mono-upnp-simple-media-server --help for more info.");
                return;
            }
            
            if (extras.Count == 0) {
                if (path == null) {
                    path = Directory.GetCurrentDirectory ();
                }
            } else if (extras.Count == 1) {
                if (path != null) {
                    Console.WriteLine ("mono-upnp-simple-media-server:");
                    Console.WriteLine ("You can only specify one path. You specified 2:");
                    Console.WriteLine ("\t" + path);
                    Console.WriteLine ("\t" + extras[0]);
                    Console.WriteLine ("Try mono-upnp-simple-media-server --help for more info.");
                    return;
                } else {
                    path = extras[0];
                }
            } else {
                Console.WriteLine ("mono-upnp-simple-media-server:");
                Console.WriteLine (string.Format ("You can only specify one path. You specified {0}:", extras.Count));
                foreach (var extra in extras) {
                    Console.WriteLine ("\t" + extra);
                }
                Console.WriteLine ("Try mono-upnp-simple-media-server --help for more info.");
                return;
            }
            
            if (help) {
                ShowHelp (options);
                return;
            }
            
            Console.WriteLine ("Serving " + path);
            
            var connection_manager = new DummyConnectionManager ();
            var content_directory = new FileSystemContentDirectory (path);
            var ms_media_server_registrar = new Service<MSMediaServerRegistrar> (
                MSMediaServerRegistrar.ServiceType, "urn:microsoft.com:serviceId:X_MS_MediaReceiverRegistrar", new DummyMSMediaServerRegistrar ());
            var media_server = new MediaServer (
                udn,
                friendly_name,
                manufacturer,
                model_name,
                new RootDeviceOptions {
                    Services = new[] { ms_media_server_registrar },
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
            
            using (media_server) {
                media_server.Start ();
                Console.WriteLine ("Press ENTER to exit.");
                Console.ReadLine ();
            }
        }
        
        static void ShowHelp (OptionSet options)
        {
            Console.WriteLine ("Usage: mono-upnp-simple-media-server [OPTIONS] [CONTENT_DIRECTORY_PATH]");
            Console.WriteLine ("Shares the contents of the specified directory (and sub-directories) over UPnP.");
            Console.WriteLine ("If no directory is specified, the current directory is used.");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            options.WriteOptionDescriptions (Console.Out);
        }
    }
}
