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

using SsdpServer = Mono.Ssdp.Server;
using Mono.Upnp.Server.Internal;

namespace Mono.Upnp.Server
{
	public class Server : IDisposable
	{
        private readonly object mutex = new object ();
        private Device root_device;
        private DescriptionServer description_server;
        private SsdpServer ssdp_server;
        private bool started;

        public Server (Device rootDevice)
            : this (rootDevice, null)
        {
        }

        public Server (Device rootDevice, IEnumerable<Device> embeddedDevices)
        {
            if (rootDevice == null) {
                throw new ArgumentNullException ("rootDevice");
            }
            root_device = rootDevice;
            rootDevice.Devices = embeddedDevices;
        }

        public virtual void Start ()
        {
            lock (mutex) {
                if (started) {
                    throw new InvalidOperationException ("The server is already started.");
                }
                if (root_device == null) {
                    throw new ObjectDisposedException (ToString ());
                }

                if (description_server == null) {
                    Initialize ();
                }
                root_device.Start ();
                description_server.Start ();
                ssdp_server.Start ();
                started = true;
            }
        }

        public virtual void Stop ()
        {
            lock (mutex) {
                if (!started) {
                    return;
                }
                ssdp_server.Stop ();
                root_device.Stop ();
                description_server.Stop ();
                started = false;
            }
        }

        protected virtual void Initialize ()
        {
            Uri url = MakeUrl ();
            root_device.Initialize (url);
            description_server = new DescriptionServer (Serialize, new Uri (url, String.Format ("{0}/{1}/", root_device.Type.ToUrlString (), root_device.Id)));
            Announce ();
        }

        private void Announce ()
        {
            ssdp_server = new SsdpServer (description_server.Url.ToString ());
            ssdp_server.Announce ("upnp:rootdevice", root_device.Udn + "::upnp:rootdevice", false);
            AnnounceDevice (root_device);
        }

        private void AnnounceDevice (Device device)
        {
            ssdp_server.Announce (device.Udn, device.Udn, false);
            ssdp_server.Announce (device.Type.ToString (), String.Format ("{0}::{1}", device.Udn, device.Type), false);

            if (device.Devices != null) {
                foreach (Device d in device.Devices) {
                    AnnounceDevice (d);
                }
            }

            if (device.Services != null) {
                foreach (Service service in device.Services) {
                    AnnounceService (device, service);
                }
            }
        }

        private void AnnounceService (Device device, Service service)
        {
            ssdp_server.Announce (service.Type.ToString (), String.Format ("{0}::{1}", device.Udn,service.Type), false);
        }

        protected virtual void Serialize (XmlWriter writer)
        {
            writer.WriteStartElement ("root", Protocol.DeviceUrn);
            Helper.WriteSpecVersion (writer);
            root_device.Serialize (writer);
            writer.WriteEndElement ();
        }

        private static readonly Random random = new Random ();

        private readonly int port = random.Next (1024, 5000);

        private static IPAddress host;
        private static IPAddress Host {
            get {
                if (host == null) {
                    foreach (IPAddress address in Dns.GetHostAddresses (Dns.GetHostName ())) {
                        if (address.AddressFamily == AddressFamily.InterNetwork) {
                            host = address;
                            break;
                        }
                    }
                }
                return host;
            }
        }

        private Uri MakeUrl ()
        {
            foreach (IPAddress address in Dns.GetHostAddresses (Dns.GetHostName ())) {
                if (address.AddressFamily == AddressFamily.InterNetwork) {
                    return new Uri (String.Format ("http://{0}:{1}/upnp/", Host, port));
                }
            }
            return null;
        }

        public void Dispose ()
        {
            lock (mutex) {
                if (root_device != null) {
                    Dispose (true);
                    GC.SuppressFinalize (this);
                }
            }
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposing) {
                Stop ();
                root_device.Dispose ();
                root_device = null;
                if (description_server != null) {
                    description_server.Dispose ();
                    ssdp_server.Dispose ();
                }
            }
        }
    }
}
