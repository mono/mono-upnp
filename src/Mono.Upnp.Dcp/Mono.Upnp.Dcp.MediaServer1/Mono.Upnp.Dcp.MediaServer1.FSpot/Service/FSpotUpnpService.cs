// 
// MyClass.cs
//  
// Author:
//       Yavor Georgiev <fealebenpae@gmail.com>
// 
// Copyright (c) 2010 Yavor Georgiev
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
using FSpot.Extensions;
using Mono.Upnp.Dcp.MSMediaServerRegistrar1;

namespace Mono.Upnp.Dcp.MediaServer1.FSpot
{
    public class FSpotUpnpService : IService
    {
        internal static Guid ServiceGuid = new Guid ("c0a64f08-c1e6-4c42-82e7-d6870e1c3c67");
        MediaServer media_server;

        #region IService implementation
        public bool Start ()
        {
            var udn = "uuid:" + ServiceGuid.ToString ();

            var friendly_name = string.Format ("{0} | F-Spot Photo Sharing", Environment.UserName);
            var manufacturer = "Mono Project";
            var model_name = "Mono.Upnp.Dcp.MediaServer1.FSpot";
            var manufacturer_url = new Uri ("http://www.mono-project.org/");
            var model_description = "A F-Spot add-in for sharing photos over the MediaServer1 spec.";
            var model_number = "1";
            var model_url = new Uri ("http://www.mono-project.org/Mono.Upnp");
            var serial_number = "MONO-UPNP-MEDIA-SERVER-1";
            string upc = null;

            try {
                var ms_media_server_registrar = new Service<MSMediaServerRegistrar> (
                MSMediaServerRegistrar.ServiceType, "urn:microsoft.com:serviceId:X_MS_MediaReceiverRegistrar", new DummyMSMediaServerRegistrar ());
                
                media_server = new MediaServer (
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
                    new DummyConnectionManager (),
                    new FSpotContentDirectory ()
                );
                
                media_server.Start ();
                return true;
            } catch (Exception ex) {
                Console.WriteLine ("Error loading F-Spot UPnP add-in: {0}", ex.Message);
                return false;
            }
        }
        
        
        public bool Stop ()
        {
            media_server.Stop ();
            media_server.Dispose ();
            return true;
        }
        
        #endregion
    }
}

