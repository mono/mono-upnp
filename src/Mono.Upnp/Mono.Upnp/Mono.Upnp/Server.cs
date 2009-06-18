//
// Server.cs
//
// Author:
//   Scott Peterson <lunchtimemama@gmail.com>
//
// Copyright (C) 2008 S&S Black Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Xml;

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

using SsdpServer = Mono.Ssdp.Server;

namespace Mono.Upnp
{
    public class Server : IDisposable
    {
        static WeakReference static_serializer = new WeakReference (null);
        
        DataServer description_server;
        SsdpServer ssdp_server;
        Root root;
        
        public Server (Root root)
            : this (root, null)
        {
        }
        
        public Server (Root root, Uri url)
        {
            if (root == null) throw new ArgumentNullException ("root");
            
            this.root = root;
            Initialize (url ?? MakeUrl ());
        }
        
        void Initialize (Uri url)
        {
            var serializer = Helper.Get<XmlSerializer> (static_serializer);
            root.Initialize (serializer, url);
            description_server = new DataServer (serializer.GetBytes (root), url);
            Announce (url);
        }
        
        public bool Started { get; private set; }

        public void Start ()
        {
            CheckDisposed ();
            if (Started) throw new InvalidOperationException ("The server is already started.");

            root.Start ();
            description_server.Start ();
            ssdp_server.Start ();
            Started = true;
        }

        public void Stop ()
        {
            CheckDisposed ();
            
            if (!Started) {
                return;
            }
            
            ssdp_server.Stop ();
            root.Stop ();
            description_server.Stop ();
            Started = false;
        }

        void Announce (Uri url)
        {
            ssdp_server = new SsdpServer (url.ToString ());
            ssdp_server.Announce ("upnp:rootdevice", root.RootDevice.Udn + "::upnp:rootdevice", false);
            AnnounceDevice (root.RootDevice);
        }

        void AnnounceDevice (Device device)
        {
            ssdp_server.Announce (device.Udn, device.Udn, false);
            ssdp_server.Announce (device.Type.ToString (), String.Format ("{0}::{1}", device.Udn, device.Type), false);

            foreach (var child_device in device.Devices) {
                AnnounceDevice (child_device);
            }

            foreach (var service in device.Services) {
                AnnounceService (device, service);
            }
        }

        void AnnounceService (Device device, Service service)
        {
            ssdp_server.Announce (service.Type.ToString (), String.Format ("{0}::{1}", device.Udn,service.Type), false);
        }

        static readonly Random random = new Random ();

        readonly int port = random.Next (1024, 5000);

        static IPAddress host;
        static IPAddress Host {
            get {
                if (host == null) {
                    foreach (var address in Dns.GetHostAddresses (Dns.GetHostName ())) {
                        if (address.AddressFamily == AddressFamily.InterNetwork) {
                            host = address;
                            break;
                        }
                    }
                }
                return host;
            }
        }

        Uri MakeUrl ()
        {
            foreach (IPAddress address in Dns.GetHostAddresses (Dns.GetHostName ())) {
                if (address.AddressFamily == AddressFamily.InterNetwork) {
                    return new Uri (String.Format ("http://{0}:{1}/upnp/", Host, port));
                }
            }
            return null;
        }
        
        void CheckDisposed ()
        {
            if (root == null) throw new ObjectDisposedException (ToString ());
        }

        public void Dispose ()
        {
            if (root != null) {
                Dispose (true);
                GC.SuppressFinalize (this);
            }
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposing) {
                Stop ();
                //root_device.Dispose ();
                if (description_server != null) {
                    description_server.Dispose ();
                    ssdp_server.Dispose ();
                }
            }
            root = null;
        }
    }
}
