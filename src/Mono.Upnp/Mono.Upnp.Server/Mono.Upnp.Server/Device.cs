//
// Device.cs
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
using System.Xml;

using Mono.Upnp.Server.Internal;

namespace Mono.Upnp.Server
{
	public class Device
	{
        private IconServer icon_server;
        private bool initialized;

        public Device (DeviceType type, IEnumerable<Service> services, string id, string friendlyName, string manufacturer, string modelName)
        {
            if (type == null) {
                throw new ArgumentNullException ("type");
            }
            if (String.IsNullOrEmpty (id)) {
                throw new ArgumentNullException ("id");
            }
            if (friendlyName == null) {
                throw new ArgumentNullException ("friendlyName");
            }
            if (manufacturer == null) {
                throw new ArgumentNullException ("manufacturer");
            }
            if (modelName == null) {
                throw new ArgumentNullException ("modelName");
            }

            if (id.StartsWith ("uuid:")) {
                id = id.Substring (5);
            }

            this.type = type;
            this.services = services;
            this.id = id;
            this.udn = "uuid:" + id;
            this.friendly_name = friendlyName;
            this.manufacturer = manufacturer;
            this.model_name = modelName;
        }

        private readonly DeviceType type;
        public DeviceType Type {
            get { return type; }
        }

        private readonly string id;
        public string Id {
            get { return id; }
        }

        private readonly string udn;
        public string Udn {
            get { return udn; }
        }

        private readonly IEnumerable<Service> services;
        public IEnumerable<Service> Services {
            get { return services; }
        }

        private IEnumerable<Device> devices;
        public IEnumerable<Device> Devices {
            get { return devices; }
            internal set { devices = value; }
        }

        private readonly string friendly_name;
        public string FriendlyName {
            get { return friendly_name; }
        }

        private readonly string manufacturer;
        public string Manufacturer {
            get { return manufacturer; }
        }

        private Uri manufacturer_url;
        public Uri ManufacturerUrl {
            get { return manufacturer_url; }
            set {
                CheckInitialized ();
                manufacturer_url = value;
            }
        }

        private string model_description;
        public string ModelDescription {
            get { return model_description; }
            set {
                CheckInitialized ();
                model_description = value;
            }
        }

        private readonly string model_name;
        public string ModelName {
            get {
                CheckInitialized ();
                return model_name;
            }
        }

        private string model_number;
        public string ModelNumber {
            get { return model_number; }
            set {
                CheckInitialized ();
                model_number = value;
            }
        }

        private Uri model_uri;
        public Uri ModelUrl {
            get { return model_uri; }
            set {
                CheckInitialized ();
                model_uri = value;
            }
        }

        private string serial_number;
        public string SerialNumber {
            get { return serial_number; }
            set {
                CheckInitialized ();
                serial_number = value;
            }
        }

        private string upc;
        public string Upc {
            get { return upc; }
            set {
                CheckInitialized ();
                upc = value;
            }
        }

        private IList<Icon> icons;
        public IList<Icon> Icons {
            get { return icons; }
            set {
                CheckInitialized ();
                icons = value;
            }
        }

        protected internal virtual void Initialize (Uri baseUrl)
        {
            Uri url = new Uri (baseUrl, String.Format ("{0}/{1}/", type.ToUrlString (), id));
            if (services != null) {
                foreach (Service service in services) {
                    service.Initialize (url);
                }
            }
            if (devices != null) {
                foreach (Device device in devices) {
                    device.Initialize (url);
                }
            }
            if (icons != null && icons.Count > 0) {
                url = new Uri (url, "icons/");
                icon_server = new IconServer (this, url);
                for (int i = 0; i < icons.Count; i++) {
                    icons[i].Initialize (new Uri (url, String.Format ("{0}/", i)));
                }
            }
            initialized = true;
        }

        private void CheckInitialized ()
        {
            if (initialized) {
                throw new InvalidOperationException ("You may not alter the device after it has been initialized.");
            }
        }

        protected internal virtual void Start ()
        {
            if (services != null) {
                foreach (Service service in services) {
                    service.Start ();
                }
            }
            if (devices != null) {
                foreach (Device device in devices) {
                    device.Start ();
                }
            }
            if (icon_server != null) {
                icon_server.Start ();
            }
        }

        protected internal virtual void Stop ()
        {
            if (services != null) {
                foreach (Service service in services) {
                    service.Stop ();
                }
            }
            if (devices != null) {
                foreach (Device device in devices) {
                    device.Stop ();
                }
            }
            if (icon_server != null) {
                icon_server.Stop ();
            }
        }

        protected internal void Serialize (XmlWriter writer)
        {
            writer.WriteStartElement ("device");
            WriteElement (writer, "deviceType", Type);
            WriteElement (writer, "friendlyName", friendly_name);
            WriteElement (writer, "manufacturer", manufacturer);
            WriteElement (writer, "manufacturerURL", manufacturer_url);
            WriteElement (writer, "modelDescription", model_description);
            WriteElement (writer, "modelName", model_name);
            WriteElement (writer, "modelNumber", model_number);
            WriteElement (writer, "modelURL", model_uri);
            WriteElement (writer, "serialNumber", serial_number);
            WriteElement (writer, "UDN", udn);
            WriteElement (writer, "UPC", upc);
            if (icons != null) {
                writer.WriteStartElement ("iconList");
                foreach (Icon icon in icons) {
                    icon.Serialize (writer);
                }
                writer.WriteEndElement ();
            }
            if (services != null) {
                writer.WriteStartElement ("serviceList");
                foreach (Service service in services) {
                    service.SerializeForDeviceDescription (writer);
                }
                writer.WriteEndElement ();
            }
            if (devices != null) {
                writer.WriteStartElement ("deviceList");
                foreach (Device device in devices) {
                    device.Serialize (writer);
                }
                writer.WriteEndElement ();
            }
            // TODO presentation URL
            writer.WriteEndElement ();
        }

        private static void WriteElement (XmlWriter writer, string name, object value)
        {
            if (value != null) {
                writer.WriteStartElement (name);
                writer.WriteValue (value.ToString ());
                writer.WriteEndElement ();
            }
        }

        internal void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (!disposing) {
                return;
            }

            Stop ();

            if (devices != null) {
                foreach (Device device in devices) {
                    device.Dispose ();
                }
            }
            if (services != null) {
                foreach (Service service in services) {
                    service.Dispose ();
                }
            }
        }
    }
}
