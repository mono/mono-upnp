//
// Server.cs
//
// Author:
//   Scott Thomas <lunchtimemama@gmail.com>
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
        static readonly WeakReference static_serializer = new WeakReference (null);
        
        readonly DataServer description_server;
        readonly SsdpServer ssdp_server;
        Root root;
        
        public Server (Root root)
            : this (root, null)
        {
        }
        
        public Server (Root root, Uri url)
        {
            if (root == null) {
                throw new ArgumentNullException ("root");
            }
            
            this.root = root;
            
            if (url == null) {
                url = MakeUrl ();
            }
            
            var serializer = Helper.Get<XmlSerializer> (static_serializer);
            root.Initialize (serializer, url);
            // FIXME this is a test
            description_server = new DataServer (serializer.GetBytes (root), @"text/xml", url);
            ssdp_server = new SsdpServer (url.ToString ());
            ssdp_server.Announce ("upnp:rootdevice", root.RootDevice.Udn + "::upnp:rootdevice", false);
            AnnounceDevice (root.RootDevice);
        }
        
        public bool Started { get; private set; }

        public void Start ()
        {
            CheckDisposed ();
            
            if (Started) {
                return;
            }

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

        void AnnounceDevice (Device device)
        {
            ssdp_server.Announce (device.Udn, device.Udn, false);
            ssdp_server.Announce (device.Type.ToString (), string.Format ("{0}::{1}", device.Udn, device.Type), false);

            foreach (var child_device in device.Devices) {
                AnnounceDevice (child_device);
            }

            foreach (var service in device.Services) {
                AnnounceService (device, service);
            }
        }

        void AnnounceService (Device device, Service service)
        {
            ssdp_server.Announce (service.Type.ToString (), string.Format ("{0}::{1}", device.Udn,service.Type), false);
        }

        static readonly Random random = new Random ();

        readonly int port = random.Next (1024, 5000);

        Uri MakeUrl ()
        {
            // FIXME configure the network interface
            foreach (var address in Dns.GetHostAddresses (Dns.GetHostName ())) {
                if (address.AddressFamily == AddressFamily.InterNetwork) {
                    return new Uri (String.Format ("http://{0}:{1}/upnp/", address, port));
                }
            }
            return null;
        }
        
        public bool IsDisposed {
            get { return root == null; }
        }
        
        void CheckDisposed ()
        {
            if (IsDisposed) {
                throw new ObjectDisposedException (ToString ());
            }
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (IsDisposed) {
                return;
            }
            
            if (disposing) {
                Stop ();
                root.Dispose ();
                description_server.Dispose ();
                ssdp_server.Dispose ();
            }
            
            root = null;
        }
    }
}
