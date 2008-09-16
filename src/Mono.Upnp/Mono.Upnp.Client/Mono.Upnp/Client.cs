//
// Client.cs
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
using System.Xml;

using Mono.Ssdp;
using Mono.Upnp.Internal;

namespace Mono.Upnp
{
	public class Client
	{
        private Dictionary<string, List<Device>> devices = new Dictionary<string, List<Device>> ();
        internal IDictionary<string, List<Device>> Devices {
            get { return devices; }
        }

        private Dictionary<string, Dictionary<ServiceType, Service>> services = new Dictionary<string, Dictionary<ServiceType, Service>> ();
        internal Dictionary<string, Dictionary<ServiceType, Service>> Services {
            get { return services; }
        }

        private Mono.Ssdp.Client client = new Mono.Ssdp.Client ();

        public event EventHandler<DeviceArgs> DeviceAdded;
        public event EventHandler<DeviceArgs> DeviceRemoved;
        public event EventHandler<ServiceArgs> ServiceAdded;
        public event EventHandler<ServiceArgs> ServiceRemoved;

        public Client ()
        {
            client.ServiceAdded += ClientServiceAdded;
            client.ServiceRemoved += ClientServiceRemoved;
        }

        // Apparently a root device can have an embedded device with the same UDN but a different device type.
        // Since the service announcement datagrams only contain their containing device's UDN, it is impossible
        // to determin which services belong to which devices based on the SSDP information alone. Lame, I know.
        // In addition, apparently things can have multiple locations. All of this conspires to increase the
        // complexity of this code. I'm sorry, but all of this riff raff is nessisary if it's going to work.
        private void ClientServiceAdded (object sender, Mono.Ssdp.ServiceArgs args)
        {
            // All USNs should start with "uuid:"
            if (!args.Usn.StartsWith ("uuid:")) {
                return;
            }

            int colon = args.Usn.IndexOf (':', 5);
            string uuid = colon == -1 ? args.Usn : args.Usn.Substring (0, colon);

            if (devices.ContainsKey (uuid)) {
                List<Device> device_list = devices[uuid];

                if (args.Usn.Contains (":service:")) {
                    ServiceType type = new ServiceType (args.Service.ServiceType);

                    if (!services[uuid].ContainsKey (type)) {
                        Service service = new Service (this, type, args.Service.Locations);
                        services[uuid].Add (type, service);
                        OnServiceAdded (service);
                    }
                } else if (args.Usn.Contains (":device:")) {
                    DeviceType type = new DeviceType (args.Service.ServiceType);
                    Device open_device = null;

                    foreach (Device device in device_list) {
                        if (device.Type == type) {
                            return;
                        } else if (device.Type == null) {
                            open_device = device;
                        }
                    }

                    if (open_device != null) {
                        open_device.Type = type;
                        OnDeviceAdded (open_device);
                    }
                }
            } else {
                List<Device> device_list = new List<Device> ();
                Dictionary<ServiceType, Service> service_types = new Dictionary<ServiceType, Service> ();
                services.Add (uuid, service_types);
                devices.Add (uuid, device_list);
                if (args.Usn.Contains (":service:")) {
                    Device device = new Device (this, uuid);
                    ServiceType type = new ServiceType (args.Service.ServiceType);
                    Service service = new Service (this, type, args.Service.Locations);
                    service_types.Add (type, service);
                    device_list.Add (device);
                    OnServiceAdded (service);
                } else if (args.Usn.Contains (":device:")) {
                    Device device = new Device (this, uuid, args.Service.Locations, args.Service.ServiceType);
                    device_list.Add (device);
                    OnDeviceAdded (device);
                } else {
                    Device device = new Device (this, uuid, args.Service.Locations);
                    device_list.Add (device);
                }
            }
        }

        private void ClientServiceRemoved (object sender, Mono.Ssdp.ServiceArgs args)
        {
        }

        protected virtual void OnDeviceAdded (Device device)
        {
            EventHandler<DeviceArgs> handler = DeviceAdded;
            if (handler != null) {
                handler (this, new DeviceArgs (device, UpnpOperation.Added));
            }
        }

        protected virtual void OnServiceAdded (Service service)
        {
            EventHandler<ServiceArgs> handler = ServiceAdded;
            if (handler != null) {
                handler (this, new ServiceArgs (service, UpnpOperation.Added));
            }
        }

        public void BrowseAll ()
        {
            client.BrowseAll ();
        }

        internal void LoadDeviceDescription (Device device)
        {
            LoadDeviceDescription (device.Locations);
        }

        internal void LoadDeviceDescription (Service service)
        {
            LoadDeviceDescription (GetLocations (service));
        }

        private void LoadDeviceDescription (IEnumerable<Uri> locations)
        {
            Exception exception = null;
            foreach (Uri location in locations) {
                try {
                    LoadDeviceDescription (location);
                    exception = null;
                    break;
                } catch (Exception e) {
                    exception = e;
                }
            }
            if (exception != null) {
                UpnpException e = new UpnpException ("Unable to load device description.", exception);
                Log.Exception (e);
                throw e;
            }
        }

        private void LoadDeviceDescription (Uri location)
        {
            // TODO handle ACCEPT-LANGUAGE
            WebResponse response = Helper.GetResponse (location);
            Root root = new Root (this, location, response.Headers, XmlReader.Create (response.GetResponseStream ()));
            LogDevice (root.Device);
            response.Close ();
        }

        private void LogDevice (Device device)
        {
            if (!devices.ContainsKey (device.Udn)) {
                services.Add (device.Udn, new Dictionary<ServiceType, Service> ());
                List<Device> device_list = new List<Device> ();
                device_list.Add (device);
                devices.Add (device.Udn, device_list);
                OnDeviceAdded (device);
            } else {
                bool found = false;
                foreach (Device d in devices[device.Udn]) {
                    if (d.Type == device.Type) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    devices[device.Udn].Add (device);
                    OnDeviceAdded (device);
                }
            }

            foreach (Service service in device.Services) {
                if (!services[device.Udn].ContainsKey (service.Type)) {
                    services[device.Udn].Add (service.Type, null);
                    OnServiceAdded (service);
                }
            }

            foreach (Device d in device.Devices) {
                LogDevice (d);
            }
        }

        private IEnumerable<Uri> GetLocations (Service service)
        {
            Dictionary<Uri, Uri> locations = new Dictionary<Uri, Uri> ();
            foreach (Uri location in service.Locations) {
                locations.Add (location, location);
                yield return location;
            }
            foreach (Uri location in service.Device.Locations) {
                if (!locations.ContainsKey (location)) {
                    yield return location;
                }
            }
        }
	}
}
