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
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp.Description
{
	public class DeviceDescription
	{
        readonly IDisposer disposer;
        readonly List<ServiceDescription> service_list = new List<ServiceDescription> ();
        readonly ReadOnlyCollection<ServiceDescription> services;
        readonly List<DeviceDescription> device_list = new List<DeviceDescription> ();
        readonly ReadOnlyCollection<DeviceDescription> devices;
        readonly List<Icon> icon_list = new List<Icon> ();
        readonly ReadOnlyCollection<Icon> icons;
        
        IDeserializer deserializer;

        protected internal DeviceDescription (IDisposer disposer)
        {
            if (disposer == null) throw new ArgumentNullException ("disposer");

            this.disposer = disposer;
            services = service_list.AsReadOnly ();
            devices = device_list.AsReadOnly ();
            icons = icon_list.AsReadOnly ();
        }
        
        public event EventHandler<DisposedEventArgs> Disposed;
        
        public ReadOnlyCollection<ServiceDescription> Services {
            get { return services; }
        }

        public ReadOnlyCollection<DeviceDescription> Devices {
            get { return devices; }
        }

        public DeviceType Type { get; private set; }
        public string Udn { get; private set; }
        public Uri PresentationUrl { get; private set; }
        public string FriendlyName { get; private set; }
        public string Manufacturer { get; private set; }
        public Uri ManufacturerUrl { get; private set; }
        public string ModelDescription { get; private set; }
        public string ModelName { get; private set; }
        public string ModelNumber { get; private set; }
        public Uri ModelUrl { get; private set; }
        public string SerialNumber { get; private set; }
        public string Upc { get; private set; }
        public bool IsDisposed { get; private set; }

        protected internal void CheckDisposed ()
        {
            disposer.TryDispose (this);
            if (IsDisposed) {
                throw new ObjectDisposedException (ToString (), "The device has gone off the network.");
            }
        }

        protected internal void Dispose ()
        {
            if (IsDisposed) {
                return;
            }

            IsDisposed = true;
            OnDispose (DisposedEventArgs.Empty);

            foreach (var service in services) {
                service.Dispose ();
            }
            foreach (var device in devices) {
                device.Dispose ();
            }
        }

        protected virtual void OnDispose (DisposedEventArgs e)
        {
            var handler = Disposed;
            if (handler != null) {
                handler (this, e);
            }
        }

        protected void AddService (ServiceDescription service)
        {
            service_list.Add (service);
        }

        protected void AddDevice (DeviceDescription device)
        {
            device_list.Add (device);
        }

        protected void AddIcon (Icon icon)
        {
            icon_list.Add (icon);
        }

        public void Deserialize (IDeserializer deserializer, XmlReader reader)
        {
            if (deserializer == null) throw new ArgumentNullException ("deserializer");

            this.deserializer = deserializer;
            DeserializeCore (reader);
            VerifyDeserialization ();
            this.deserializer = null;
        }

        protected virtual void DeserializeCore (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                reader.Read ();
                while (Helper.ReadToNextElement (reader)) {
                    try {
                        DeserializeCore (reader.ReadSubtree (), reader.Name);
                    } catch (Exception e) {
                        Log.Exception (
                            "There was a problem deserializing one of the device description elements.", e);
                    }
                }
            } catch (Exception e) {
                throw new UpnpDeserializationException (
                    string.Format ("There was a problem deserializing {0}.", ToString ()), e);
            } finally {
                reader.Close ();
            }
        }

        protected virtual void DeserializeCore (XmlReader reader, string element)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            using (reader) {
                reader.Read ();
                switch (element.ToLower ()) {
                case "devicetype":
                    Type = new DeviceType (reader.ReadString ());
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
                    Udn = reader.ReadString ().Trim ();
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
            }
        }

        protected virtual void DeserializeIcons (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            using (reader) {
                while (reader.ReadToFollowing ("icon")) {
                    try {
                        DeserializeIcon (reader.ReadSubtree ());
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing an icon list element.", e);
                    }
                }
            }
        }

        protected virtual void DeserializeIcon (XmlReader reader)
        {
            var icon = CreateIcon ();
            if (icon != null) {
                icon.Deserialize (deserializer, reader);
                AddIcon (icon);
            }
        }

        protected virtual Icon CreateIcon ()
        {
            return new Icon (this);
        }

        protected virtual void DeserializeServices (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            using (reader) {
                while (reader.ReadToFollowing ("service")) {
                    try {
                        DeserializeService (reader.ReadSubtree ());
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing a service list element.", e);
                    }
                }
            }
        }

        protected virtual void DeserializeService (XmlReader reader)
        {
            AddService (deserializer.DeserializeService (reader));
        }

        protected virtual void DeserializeDevices (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            using (reader ) {
                while (reader.ReadToFollowing ("device")) {
                    try {
                        DeserializeDevices (reader.ReadSubtree ());
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing a device list element.", e);
                    }
                }
            }
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
            if (Udn == null) {
                var message = Type == null
                    ? "The device has no UDN or type."
                    : string.Format ("The device of type {0} has no UDN.", Type);
                throw new UpnpDeserializationException (message);
            }
            if (Type == null) {
                throw new UpnpDeserializationException (string.Format (
                    "The device {0} has no device type description.", Udn));
            }
            if (Udn.Length == 0) {
                Log.Exception (new UpnpDeserializationException (string.Format (
                    "The device of type {0} has an empty UDN.", Type)));
            }
            if (string.IsNullOrEmpty (FriendlyName)) {
                Log.Exception (new UpnpDeserializationException (string.Format (
                    "{0} has no friendly name.", ToString ())));
            }
            if (string.IsNullOrEmpty (Manufacturer)) {
                Log.Exception (new UpnpDeserializationException (string.Format (
                    "{0} has no manufaturer.", ToString ())));
            }
            if (string.IsNullOrEmpty (ModelName)) {
                Log.Exception (new UpnpDeserializationException (string.Format (
                    "{0} has no model name.", ToString ())));
            }
            if (icon_list.Count > 0) {
                var has_png = false;
                foreach (var icon in icon_list) {
                    if (icon.MimeType == "image/png") {
                        has_png = true;
                        break;
                    }
                }
                if (!has_png) {
                    Log.Exception (new UpnpDeserializationException (string.Format (
                        "{0} does not have a PNG file in its icon list.", ToString ())));
                }
            }
        }

        public override string ToString ()
        {
            return string.Format ("DeviceDescription {{ uuid:{0}::{1} }}", Udn, Type);
        }
	}
}
