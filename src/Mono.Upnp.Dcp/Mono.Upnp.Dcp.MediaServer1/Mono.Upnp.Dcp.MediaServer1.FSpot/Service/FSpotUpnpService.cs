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
        internal static Guid ServiceGuid = new Guid ();
        MediaServer media_server;
        GConf.Client client;

        internal static FSpotUpnpService Instance { get; private set; }

        internal bool IsRunning { get; private set; }

        public FSpotUpnpService ()
        {
            Instance = this;
            client = new GConf.Client ();
            client.AddNotify (GConfConstants.GCONF_APP_PATH, OnGConfNotify);
        }

        void OnGConfNotify (object sender, GConf.NotifyEventArgs args)
        {
            switch (args.Key) {
                case GConfConstants.SHARED_CATEGORIES_KEY:
                case GConfConstants.SHARE_ALL_CATEGORIES_KEY:
                    if (IsRunning) {
                        Restart ();
                    }
                    break;
                case GConfConstants.SHARE_LIBRARY_KEY:
                    if ((bool)args.Value && !IsRunning) {
                        Start ();
                    } else if (!(bool)args.Value && IsRunning) {
                        Stop ();
                    }
                    break;
            default:
            break;
            }
        }

        #region IService implementation
        public bool Start ()
        {
            try {
                if (!(bool)client.Get (GConfConstants.SHARE_LIBRARY_KEY)) {
                    return false;
                }
            } catch (GConf.NoSuchKeyException) {
                return false;
            }

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

                IsRunning = true;

                return true;
            } catch (Exception ex) {
                Console.WriteLine ("Error loading F-Spot UPnP add-in: {0}", ex.Message);
                return false;
            }
        }

        public void Restart ()
        {
            if (IsRunning) {
                Stop ();
            }

            Start ();
        }
        
        public bool Stop ()
        {
            if (media_server != null) {
                media_server.Stop ();
                media_server.Dispose ();
            }

            IsRunning = false;

            return true;
        }
        
        #endregion
    }
}

