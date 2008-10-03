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
        private SsdpServer server;
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
                    Announce ();
                }
                root_device.Start ();
                description_server.Start ();
                server.Start ();
                started = true;
            }
        }

        public virtual void Stop ()
        {
            lock (mutex) {
                if (!started) {
                    return;
                }
                server.Stop ();
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
        }

        protected virtual void Announce ()
        {
            server = new SsdpServer (description_server.Url.ToString ());
            server.Announce ("upnp:rootdevice", root_device.Id + "::upnp:rootdevice", false);
            AnnounceDevice (root_device);
            if (root_device.Devices != null) {
                foreach (Device device in root_device.Devices) {
                    AnnounceDevice (device);
                }
            }
        }

        private void AnnounceDevice (Device device)
        {
            server.Announce (device.Id, device.Id, false);
            server.Announce (root_device.Type.ToString (), String.Format ("{0}::{1}", root_device.Id, root_device.Type), false);
            Dictionary<ServiceType, ServiceType> service_types = new Dictionary<ServiceType, ServiceType> ();
            foreach (Service service in device.Services) {
                if (!service_types.ContainsKey (service.Type)) {
                    AnnounceService (service, device);
                    service_types.Add (service.Type, service.Type);
                }
            }
        }

        private void AnnounceService (Service service, Device device)
        {
            server.Announce (service.Type.ToString (), String.Format ("{0}::{1}", device.Id, service.Type), false);
        }

        protected virtual void Serialize (XmlWriter writer)
        {
            writer.WriteStartElement ("root", Protocol.DeviceUrn);
            Helper.WriteSpecVersion (writer);
            root_device.Serialize (writer);
            writer.WriteEndElement ();
        }

        private bool conserve_memory;
        public bool ConserveMemory {
            get { return conserve_memory; }
            set { conserve_memory = value; }
        }

        private static readonly Random random = new Random ();
        private static Uri MakeUrl ()
        {
            foreach (IPAddress address in Dns.GetHostAddresses (Dns.GetHostName ())) {
                if (address.AddressFamily == AddressFamily.InterNetwork) {
                    return new Uri (String.Format ("http://{0}:{1}/upnp/", address, random.Next (1024, 5000)));
                }
            }
            return null;
        }

        public void Dispose ()
        {
            lock (mutex) {
                Dispose (true);
                GC.SuppressFinalize (this);
            }
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposing) {
                Stop ();
                root_device = null;
                if (description_server != null) {
                    description_server.Dispose ();
                    description_server = null;
                }
            }
        }
    }
}
