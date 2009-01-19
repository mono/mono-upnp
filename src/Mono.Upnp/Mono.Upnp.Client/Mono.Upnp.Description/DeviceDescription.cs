//
// DeviceDescription.cs
//
// Author:
//   Scott Peterson <lunchtimemama@gmail.com>
//
// Copyright (C) 2009 S&S Black Ltd.
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
using System.Net;
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp.Description
{
	public class DeviceDescription
	{
        readonly IDisposer disposer;
        DeviceType type;
        string udn;
        Uri presentation_url;
        string friendly_name;
        string manufacturer;
        Uri manufacturer_url;
        string model_description;
        string model_name;
        string model_number;
        Uri model_url;
        string serial_number;
        string upc;
        readonly List<ServiceDescription> service_list = new List<ServiceDescription> ();
        readonly ReadOnlyCollection<ServiceDescription> services;
        readonly List<DeviceDescription> device_list = new List<DeviceDescription> ();
        readonly ReadOnlyCollection<DeviceDescription> devices;
        readonly List<Icon> icon_list = new List<Icon> ();
        readonly ReadOnlyCollection<Icon> icons;
        IDeserializer deserializer;
        bool loaded;
        bool is_disposed;

        public event EventHandler<DisposedEventArgs> Disposed;

        protected internal DeviceDescription (IDisposer disposer)
        {
            if (disposer == null) throw new ArgumentNullException ("disposer");

            this.disposer = disposer;
            services = service_list.AsReadOnly ();
            devices = device_list.AsReadOnly ();
            icons = icon_list.AsReadOnly ();
        }

        public DeviceType Type {
            get { return type; }
            protected set { SetField (ref type, value); }
        }

        public string Udn {
            get { return udn; }
            protected set { SetField (ref udn, value); }
        }
        
        public Uri PresentationUrl {
            get { return presentation_url; }
            protected set { SetField (ref presentation_url, value); }
        }
        
        public string FriendlyName {
            get { return friendly_name; }
            protected set { SetField (ref friendly_name, value); }
        }
        
        public string Manufacturer {
            get { return manufacturer; }
            protected set { SetField (ref manufacturer, value); }
        }
        
        public Uri ManufacturerUrl {
            get { return manufacturer_url; }
            protected set { SetField (ref manufacturer_url, value); }
        }
        
        public string ModelDescription {
            get { return model_description; }
            protected set { SetField (ref model_description, value); }
        }
        
        public string ModelName {
            get { return model_name; }
            protected set { SetField (ref model_name, value); }
        }
        
        public string ModelNumber {
            get { return model_number; }
            protected set { SetField (ref model_number, value); }
        }
        
        public Uri ModelUrl {
            get { return model_url; }
            protected set { SetField (ref model_url, value); }
        }
        
        public string SerialNumber {
            get { return serial_number; }
            protected set { SetField (ref serial_number, value); }
        }
        
        public string Upc {
            get { return upc; }
            protected set { SetField (ref upc, value); }
        }

        public ReadOnlyCollection<ServiceDescription> Services {
            get { return services; }
        }

        public ReadOnlyCollection<DeviceDescription> Devices {
            get { return devices; }
        }

        void SetField<T> (ref T field, T value)
        {
            CheckLoaded ();
            field = value;
        }

        void CheckLoaded ()
        {
            if (loaded) {
                throw new InvalidOperationException ("The device description has already been deserialized.");
            }
        }

        public bool IsDisposed {
            get { return is_disposed; }
        }

        protected internal void CheckDisposed ()
        {
            disposer.TryDispose (this);
            if (is_disposed) {
                throw new ObjectDisposedException (ToString (), "The device has gone off the network.");
            }
        }

        protected internal void Dispose ()
        {
            if (is_disposed) {
                return;
            }

            is_disposed = true;
            OnDispose ();

            foreach (ServiceDescription service in services) {
                service.Dispose ();
            }
            foreach (DeviceDescription device in devices) {
                device.Dispose ();
            }
        }

        protected virtual void OnDispose ()
        {
            EventHandler<DisposedEventArgs> handler = Disposed;
            if (handler != null) {
                handler (this, DisposedEventArgs.Empty);
            }
        }

        protected void AddService (ServiceDescription service)
        {
            CheckLoaded ();
            service_list.Add (service);
        }

        protected void AddDevice (DeviceDescription device)
        {
            CheckLoaded ();
            device_list.Add (device);
        }

        protected void AddIcon (Icon icon)
        {
            CheckLoaded ();
            icon_list.Add (icon);
        }

        public void Deserialize (IDeserializer deserializer, XmlReader reader)
        {
            if (deserializer == null) throw new ArgumentNullException ("deserializer");

            this.deserializer = deserializer;
            DeserializeCore (reader);
            VerifyDeserialization ();
            this.deserializer = null;
            loaded = true;
        }

        protected virtual void DeserializeCore (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                reader.ReadToFollowing ("device");
                while (Helper.ReadToNextElement (reader)) {
                    try {
                        DeserializeCore (reader.ReadSubtree (), reader.Name);
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing one of the device description elements.", e);
                    }
                }
                reader.Close ();
            } catch (Exception e) {
                throw new UpnpDeserializationException (string.Format ("There was a problem deserializing {0}.", ToString ()), e);
            }
        }

        protected virtual void DeserializeCore (XmlReader reader, string element)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            reader.Read ();
            switch (element.ToLower ()) {
            case "devicetype":
                type = new DeviceType (reader.ReadString ());
                break;
            case "friendlyname":
                FriendlyName = reader.ReadString ();
                break;
            case "manufacturer":
                Manufacturer = reader.ReadString ();
                break;
            case "manufacturerurl":
                ManufacturerUrl = deserializer.DeserializeUrl (reader.ReadSubtree ());
                break;
            case "modeldescription":
                ModelDescription = reader.ReadString ();
                break;
            case "modelname":
                ModelName = reader.ReadString ();
                break;
            case "modelnumber":
                ModelNumber = reader.ReadString ();
                break;
            case "modelurl":
                ModelUrl = deserializer.DeserializeUrl (reader.ReadSubtree ());
                break;
            case "serialnumber":
                SerialNumber = reader.ReadString ();
                break;
            case "udn":
                udn = reader.ReadString ().Trim ();
                break;
            case "upc":
                Upc = reader.ReadString ();
                break;
            case "iconlist":
                DeserializeIcons (reader.ReadSubtree ());
                break;
            case "servicelist":
                DeserializeServices (reader.ReadSubtree ());
                break;
            case "devicelist":
                DeserializeDevice (reader.ReadSubtree ());
                break;
            case "presentationurl":
                PresentationUrl = deserializer.DeserializeUrl (reader.ReadSubtree ());
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        protected virtual void DeserializeIcons (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            while (reader.ReadToFollowing ("icon")) {
                try {
                    DeserializeIcon (reader.ReadSubtree ());
                } catch (Exception e) {
                    Log.Exception ("There was a problem deserializing an icon list element.", e);
                }
            }
            reader.Close ();
        }

        protected virtual void DeserializeIcon (XmlReader reader)
        {
            Icon icon = CreateIcon ();
            icon.Deserialize (deserializer, reader);
            AddIcon (icon);
        }

        protected virtual Icon CreateIcon ()
        {
            return new Icon (this);
        }

        protected virtual void DeserializeServices (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            while (reader.ReadToFollowing ("service")) {
                try {
                    DeserializeService (reader.ReadSubtree ());
                } catch (Exception e) {
                    Log.Exception ("There was a problem deserializing a service list element.", e);
                }
            }
            reader.Close ();
        }

        protected virtual void DeserializeService (XmlReader reader)
        {
            AddService (deserializer.DeserializeService (reader));
        }

        protected virtual void DeserializeDevices (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            while (reader.ReadToFollowing ("device")) {
                try {
                    DeserializeDevices (reader.ReadSubtree ());
                } catch (Exception e) {
                    Log.Exception ("There was a problem deserializing a device list element.", e);
                }
            }
            reader.Close ();
        }

        protected virtual void DeserializeDevice (XmlReader reader)
        {
            AddDevice (deserializer.DeserializeDevice (reader));
        }

        void VerifyDeserialization ()
        {
            // We only throw for critical errors
            // TODO The "critical errors" use to mean things that could cause an NPE in Equals and GetHashCode, but
            // those overrides are no more, so what should throw and what should not? Can we have a "strict" mode?
            if (udn == null) {
                string message = type == null
                    ? "The device has no UDN or type."
                    : string.Format ("The device of type {0} has no UDN.", type);
                throw new UpnpDeserializationException (message);
            }
            if (type == null) {
                throw new UpnpDeserializationException (string.Format (
                    "The device {0} has no device type description.", udn));
            }
            if (udn.Length == 0) {
                Log.Exception (new UpnpDeserializationException (string.Format (
                    "The device {0} has an empty UDN.", udn)));
            }
            if (string.IsNullOrEmpty (friendly_name)) {
                Log.Exception (new UpnpDeserializationException (string.Format (
                    "{0} has no friendly name.", ToString ())));
            }
            if (string.IsNullOrEmpty (manufacturer)) {
                Log.Exception (new UpnpDeserializationException (string.Format (
                    "{0} has no manufaturer.", ToString ())));
            }
            if (string.IsNullOrEmpty (model_name)) {
                Log.Exception (new UpnpDeserializationException (string.Format (
                    "{0} has no model name.", ToString ())));
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
                    Log.Exception (new UpnpDeserializationException (string.Format (
                        "{0} does not have a PNG file in its icon list.", ToString ())));
                }
            }
        }

        public override string ToString ()
        {
            return String.Format ("DeviceDescription {{ uuid:{0}::{1} }}", udn, type);
        }
	}
}
