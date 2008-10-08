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
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp
{
    public class Device
    {
        #region Constructors

        private Device (Client client)
        {
            this.client = client;
        }

        protected internal Device (Client client, string udn, IEnumerable<string> locations, DeviceType type)
            : this (client)
        {
            if (udn == null) {
                throw new ArgumentNullException ("udn");
            } else if (type == null) {
                throw new ArgumentNullException ("type");
            }

            this.udn = udn;
            this.type = type;
            SetLocations (locations);
        }

        protected Device (DeviceFactory factory, Client client, Root root, XmlReader reader)
            : this (factory, client, root, reader, null)
        {
        }

        protected internal Device (DeviceFactory factory, Client client, Root root, XmlReader reader, WebHeaderCollection headers)
            : this (client)
        {
            this.factory = factory;
            this.root = root;
            Deserialize (reader, headers);
        }

        #endregion

        #region Data

        private DeviceFactory factory;
        private Client client;
        private bool device_description_loaded;

        private bool disposed;
        public bool Disposed {
            get { return disposed; }
        }

        private Dictionary<string, string> locations = new Dictionary<string, string> ();
        internal IEnumerable<string> Locations {
            get { return locations.Values; }
        }

        internal void SetLocations (IEnumerable<string> locations)
        {
            foreach (string location in locations) {
                if (!this.locations.ContainsKey (location)) {
                    this.locations.Add (location, location);
                }
            }
        }

        private Root root;
        public Root Root {
            get { return GetDescriptionField (ref root);  }
        }

        private Uri presentation_url;
        public Uri PresentationUrl {
            get { return GetDescriptionField (ref presentation_url); }
        }

        private DeviceType type;
        public DeviceType Type {
            get { return type; }
            internal set { type = value; }
        }

        private string friendly_name;
        public string FriendlyName {
            get { return GetDescriptionField (ref friendly_name); }
        }

        private string manufacturer;
        public string Manufacturer {
            get { return GetDescriptionField (ref manufacturer); }
        }

        private Uri manufacturer_url;
        public Uri ManufacturerUrl {
            get { return GetDescriptionField (ref manufacturer_url); }
        }

        private string model_description;
        public string ModelDescription {
            get { return GetDescriptionField (ref model_description); }
        }

        private string model_name;
        public string ModelName {
            get { return GetDescriptionField (ref model_name); }
        }

        private string model_number;
        public string ModelNumber {
            get { return GetDescriptionField (ref model_number); }
        }

        private Uri model_url;
        public Uri ModelUrl {
            get { return GetDescriptionField (ref model_url); }
        }

        private string serial_number;
        public string SerialNumber {
            get { return GetDescriptionField (ref serial_number); }
        }

        private string udn;
        public string Udn {
            get { return GetDescriptionField (ref udn); }
        }

        private string upc;
        public string Upc {
            get { return GetDescriptionField (ref upc); }
        }

        private List<Icon> icon_list;
        private ReadOnlyCollection<Icon> icons;
        public ReadOnlyCollection<Icon> Icons {
            get { return GetDescriptionField (ref icons); }
        }

        private List<Service> service_list;
        private ReadOnlyCollection<Service> services;
        public ReadOnlyCollection<Service> Services {
            get { return GetDescriptionField (ref services); }
        }

        private List<Device> device_list = new List<Device> ();
        private ReadOnlyCollection<Device> devices;
        public ReadOnlyCollection<Device> Devices {
            get { return GetDescriptionField (ref devices); }
        }

        #endregion

        #region Methods

        #region Internal and Protected

        public event EventHandler Disposing;

        internal void Dispose ()
        {
            if (!disposed) {
                foreach (Service service in services) {
                    service.Dispose ();
                }
                foreach (Device device in devices) {
                    device.Dispose ();
                }
                OnDisposing ();
                disposed = true;
            }
        }

        protected virtual void OnDisposing ()
        {
            EventHandler handler = Disposing;
            if (handler != null) {
                handler (this, EventArgs.Empty);
            }
        }

        protected T GetDescriptionField<T> (ref T field) where T : class
        {
            if (field == null && !device_description_loaded) {
                client.LoadDeviceDescription (this);
            }
            return field;
        }

        protected T GetService<T> () where T : Service
        {
            foreach (Service service in Services) {
                T s = service as T;
                if (s != null) {
                    return s;
                }
            }
            return null;
        }

        protected T GetDevice<T> () where T : Device
        {
            foreach (Device device in Devices) {
                T d = device as T;
                if (d != null) {
                    return d;
                }
            }
            return null;
        }

        protected void CheckDisposed ()
        {
            if (disposed) {
                throw new ObjectDisposedException (ToString (),
                    "The device has gone off the network and is no longer available.");
            }
        }

        #endregion

        #region Overrides

        public override string ToString ()
        {
            return String.Format ("Device {{ uuid:{0}::{1} }}", udn, type);
        }

        public override bool Equals (object obj)
        {
            Device device = obj as Device;
            return device != null && device.udn == udn && device.type == type;
        }

        public override int GetHashCode ()
        {
            return (udn == null ? 0 : udn.GetHashCode ()) ^ (type == null ? 0 : type.GetHashCode ());
        }

        #endregion

        #region Deserialization

        private WebHeaderCollection headers;

        private void Deserialize (XmlReader reader, WebHeaderCollection headers)
        {
            this.headers = headers;
            device_list = new List<Device> ();
            devices = device_list.AsReadOnly ();
            service_list = new List<Service> ();
            services = service_list.AsReadOnly ();
            icon_list = new List<Icon> ();
            icons = icon_list.AsReadOnly ();

            Deserialize (headers);
            Deserialize (reader);
            VerifyDeserialization ();

            this.headers = null;
            device_description_loaded = true;
        }

        protected virtual void Deserialize (WebHeaderCollection headers)
        {
        }

        protected virtual void Deserialize (XmlReader reader)
        {
            try {
                reader.Read ();
                while (Helper.ReadToNextElement (reader)) {
                    Deserialize (reader.ReadSubtree (), reader.Name);
                }
                reader.Close ();
            } catch (Exception e) {
                string message = String.IsNullOrEmpty (udn)
                    ? "There was a problem deserializing a device."
                    : String.Format ("There was a problem deserializing the device {0}.", udn);
                throw new UpnpDeserializationException (message, e);
            }
        }

        protected virtual void Deserialize (XmlReader reader, string element)
        {
            reader.Read ();
            switch (element) {
            case "deviceType":
                type = new DeviceType (reader.ReadString ());
                break;
            case "friendlyName":
                friendly_name = reader.ReadString ();
                break;
            case "manufacturer":
                manufacturer = reader.ReadString ();
                break;
            case "manufacturerURL":
                manufacturer_url = root.DeserializeUrl (reader.ReadSubtree ());
                break;
            case "modelDescription":
                model_description = reader.ReadString ();
                break;
            case "modelName":
                model_name = reader.ReadString ();
                break;
            case "modelNumber":
                model_number = reader.ReadString ();
                break;
            case "modelURL":
                model_url = root.DeserializeUrl (reader.ReadSubtree ());
                break;
            case "serialNumber":
                serial_number = reader.ReadString ();
                break;
            case "UDN":
                udn = reader.ReadString ().Trim ();
                break;
            case "UPC":
                upc = reader.ReadString ();
                break;
            case "iconList":
                DeserializeIcons (reader.ReadSubtree ());
                break;
            case "serviceList":
                DeserializeServices (reader.ReadSubtree ());
                break;
            case "deviceList":
                DeserializeDevices (reader.ReadSubtree ());
                break;
            case "presentationURL":
                presentation_url = root.DeserializeUrl (reader.ReadSubtree ());
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        protected virtual void DeserializeIcons (XmlReader reader)
        {
            while (reader.ReadToFollowing ("icon") && reader.NodeType == XmlNodeType.Element) {
                DeserializeIcon (reader.ReadSubtree ());
            }
            reader.Close ();
        }

        protected virtual void DeserializeIcon (XmlReader reader)
        {
            AddIcon (new Icon (this, reader, headers));
        }

        protected void AddIcon (Icon icon)
        {
            icon_list.Add (icon);
        }

        private void DeserializeServices (XmlReader reader)
        {
            while (reader.ReadToFollowing ("service") && reader.NodeType == XmlNodeType.Element) {
                Service service = DeserializeService (reader.ReadSubtree ());
                if (client.Services.ContainsKey (udn) && client.Services[udn].ContainsKey (service.Type)) {
                    client.Services[udn][service.Type].CopyFrom (service);
                    service = client.Services[udn][service.Type];
                    client.Services[udn][service.Type] = null;
                }
                service_list.Add (service);
            }
            reader.Close ();
        }

        private Service DeserializeService (XmlReader reader)
        {
            // Life would be much easier if the service type were an attribute in the service node.
            reader.Read ();
            string xml = reader.ReadOuterXml ();
            reader.Close ();

            reader = XmlReader.Create (new StringReader (xml));
            reader.ReadToFollowing ("serviceType");
            ServiceType type = new ServiceType (reader.ReadString ());
            reader.Close ();

            ServiceFactory factory = null;

            if (this.factory != null) {
                foreach (ServiceFactory service_factory in this.factory.Services) {
                    if (service_factory.Type == type) {
                        factory = service_factory;
                        break;
                    }
                }
            }

            try {
                return factory.CreateService (this, XmlReader.Create (new StringReader (xml)), headers);
            } catch {
                try {
                    return client.ServiceFactories[type].CreateService (this, XmlReader.Create (new StringReader (xml)), headers);
                } catch {
                    return client.DefaultServiceFactory.CreateService (this, XmlReader.Create (new StringReader (xml)), headers);
                }
            }
        }

        private void DeserializeDevices (XmlReader reader)
        {
            while (reader.ReadToFollowing ("device") && reader.NodeType == XmlNodeType.Element) {
                Device device = Helper.DeserializeDevice (factory, client, root, reader, headers);
                if (client.Devices.ContainsKey (device.udn) && client.Devices[device.udn].ContainsKey (device.type)) {
                    client.Devices[device.udn][device.type].CopyFrom (device);
                    device = client.Devices[device.udn][device.type];
                }
                device_list.Add (device);
            }
            reader.Close ();
        }

        protected virtual void VerifyDeserialization ()
        {
            if (string.IsNullOrEmpty (udn)) {
                throw new UpnpDeserializationException ("The device has no UDN.");
            }
            if (type == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The device {0} has no device type description.", udn));
            }
            if (friendly_name == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The device {0} has no friendly name.", udn));
            }
            if (friendly_name.Length == 0) {
                Log.Exception (new UpnpDeserializationException (String.Format (
                    "The device {0} has an empty friendly name.", udn)));
            }
            if (manufacturer == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The device {0} has no manufaturer.", udn));
            }
            if (manufacturer.Length == 0) {
                Log.Exception (new UpnpDeserializationException (String.Format (
                    "The device {0} has an empty manufacturer.", udn)));
            }
            if (model_name == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The device {0} has no model name.", udn));
            }
            if (model_name.Length == 0) {
                Log.Exception (new UpnpDeserializationException (String.Format (
                    "The device {0} has an empty model name.", udn)));
            }
            if (icon_list.Count > 0) {
                bool png = false;
                foreach (Icon icon in icon_list) {
                    if (icon.MimeType == "image/png") {
                        png = true;
                        break;
                    }
                }
                if (!png) {
                    Log.Exception (new UpnpDeserializationException (String.Format (
                        "The device {0} does not have a PNG file in its icon list.", udn)));
                }
            }
        }

        protected internal virtual void VerifyContract ()
        {
            foreach (Device device in devices) {
                device.VerifyContract ();
            }
            foreach (Service service in services) {
                service.VerifyContract ();
            }
            foreach (Icon icon in icons) {
                icon.VerifyContract ();
            }
        }

        protected internal virtual void CopyFrom (Device device)
        {
            if (udn != device.udn) {
                throw new UpnpDeserializationException ("The deserialized device has a different UDN.");
            } else if (type != device.type) {
                throw new UpnpDeserializationException ("The deserialized device is of a different type.");
            }
            factory = device.factory;
            device_description_loaded = device.device_description_loaded;
            client = device.client;
            root = device.root;
            presentation_url = device.presentation_url;
            friendly_name = device.friendly_name;
            manufacturer = device.manufacturer;
            manufacturer_url = device.manufacturer_url;
            model_description = device.model_description;
            model_name = device.model_name;
            model_number = device.model_number;
            model_url = device.model_url;
            serial_number = device.serial_number;
            upc = device.upc;
            icons = device.icons;
            services = device.services;
            devices = device.devices;
        }

        #endregion

        #endregion

    }
}
