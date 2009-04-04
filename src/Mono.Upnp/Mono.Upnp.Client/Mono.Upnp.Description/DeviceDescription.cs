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
		bool verified;

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
        
        public ReadOnlyCollection<Icon> Icons {
            get { return icons; }
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
            DeserializeRootElement (reader);
            Verify ();
            this.deserializer = null;
        }

        protected virtual void DeserializeRootElement (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                while (Helper.ReadToNextElement (reader)) {
					var property_reader = reader.ReadSubtree ();
					property_reader.Read ();
                    try {
                        DeserializePropertyElement (property_reader);
                    } catch (Exception e) {
                        Log.Exception (
                            "There was a problem deserializing one of the device description elements.", e);
                    } finally {
						property_reader.Close ();
					}
                }
            } catch (Exception e) {
                throw new UpnpDeserializationException (
                    string.Format ("There was a problem deserializing {0}.", ToString ()), e);
            }
        }

        protected virtual void DeserializePropertyElement (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            switch (reader.Name) {
            case "deviceType":
                Type = new DeviceType (reader.ReadString ());
                break;
            case "friendlyName":
                FriendlyName = reader.ReadString ();
                break;
            case "manufacturer":
                Manufacturer = reader.ReadString ();
                break;
            case "manufacturerURL":
                ManufacturerUrl = deserializer.DeserializeUrl (reader);
                break;
            case "modelDescription":
                ModelDescription = reader.ReadString ();
                break;
            case "modelName":
                ModelName = reader.ReadString ();
                break;
            case "modelNumber":
                ModelNumber = reader.ReadString ();
                break;
            case "modelURL":
                ModelUrl = deserializer.DeserializeUrl (reader);
                break;
            case "serialNumber":
                SerialNumber = reader.ReadString ();
                break;
            case "UDN":
                Udn = reader.ReadString ().Trim ();
                break;
            case "UPC":
                Upc = reader.ReadString ();
                break;
            case "iconList":
				using (var icons_reader = reader.ReadSubtree ()) {
                	icons_reader.Read ();
					DeserializeIcons (icons_reader);
				}
                break;
            case "serviceList":
				using (var services_reader = reader.ReadSubtree ()) {
                	services_reader.Read ();
					DeserializeServices (services_reader);
				}
                break;
            case "deviceList":
				using (var devices_reader = reader.ReadSubtree ()) {
                	devices_reader.Read ();
					DeserializeDevice (devices_reader);
				}
                break;
            case "presentationURL":
                PresentationUrl = deserializer.DeserializeUrl (reader);
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
        }

        protected virtual void DeserializeIcons (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            while (reader.ReadToFollowing ("icon")) {
				var icon_reader = reader.ReadSubtree ();
				icon_reader.Read ();
                try {
                    DeserializeIcon (icon_reader);
                } catch (Exception e) {
                    Log.Exception ("There was a problem deserializing an icon list element.", e);
                } finally {
					icon_reader.Close ();
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

            while (reader.ReadToFollowing ("service")) {
				var service_reader = reader.ReadSubtree ();
				service_reader.Read ();
                try {
                    DeserializeService (service_reader);
                } catch (Exception e) {
                    Log.Exception ("There was a problem deserializing a service list element.", e);
                } finally {
					service_reader.Close ();
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

            while (reader.ReadToFollowing ("device")) {
				var device_reader = reader.ReadSubtree ();
				device_reader.Read ();
                try {
                    DeserializeDevices (device_reader);
                } catch (Exception e) {
                    Log.Exception ("There was a problem deserializing a device list element.", e);
                } finally {
					device_reader.Close ();
				}
            }
        }

        protected virtual void DeserializeDevice (XmlReader reader)
        {
            AddDevice (deserializer.DeserializeDevice (reader));
        }
		
		void Verify ()
		{
			VerifyDeserialization ();
			if (!verified) {
				throw new UpnpDeserializationException (
					"The deserialization has not been fully verified. Be sure to call base.VerifyDeserialization ().");
			}
		}

        protected virtual void VerifyDeserialization ()
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
			verified = true;
        }

        public override string ToString ()
        {
            return string.Format ("DeviceDescription {{ uuid:{0}::{1} }}", Udn, Type);
        }
	}
}
