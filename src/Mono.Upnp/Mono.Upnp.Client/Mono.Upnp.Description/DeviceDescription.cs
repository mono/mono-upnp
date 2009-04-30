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

using Mono.Upnp.Xml;

namespace Mono.Upnp.Description
{
	public class DeviceDescription : Description, IXmlDeserializable
	{
        readonly IDisposer disposer;

        protected internal DeviceDescription (IDisposer disposer, Root root)
            : base (root)
        {
            if (disposer == null) throw new ArgumentNullException ("disposer");

            this.disposer = disposer;
        }
        
        public event EventHandler<DisposedEventArgs> Disposed;
        
        [XmlArray ("serviceList", Type = typeof (List<ServiceDescription>))] protected ICollection<ServiceDescription> ServiceCollection { get; set; }
        [XmlArray ("deviceList", Type = typeof (List<DeviceDescription>))] protected ICollection<DeviceDescription> DeviceCollection { get; set; }
        [XmlArray ("iconList", Type = typeof (List<IconDescription>))] protected ICollection<IconDescription> IconCollection { get; set; }
        [XmlElement ("deviceType")] public DeviceType Type { get; set; }
        [XmlElement ("UDN")] public string Udn { get; set; }
        [XmlElement ("presentationURL")] public Uri PresentationUrl { get; set; }
        [XmlElement ("friendlyName")] public string FriendlyName { get; set; }
        [XmlElement ("manufacturer")] public string Manufacturer { get; set; }
        [XmlElement ("manufacturerURL")] public Uri ManufacturerUrl { get; set; }
        [XmlElement ("modelDescription")] public string ModelDescription { get; set; }
        [XmlElement ("modelName")] public string ModelName { get; set; }
        [XmlElement ("modelNumber")] public string ModelNumber { get; set; }
        [XmlElement ("modelURL")] public Uri ModelUrl { get; set; }
        [XmlElement ("serialNumber")] public string SerialNumber { get; set; }
        [XmlElement ("UPC")] public string Upc { get; set; }
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
        
        ReadOnlyCollection<T> MakeReadOnly<T> (IList<T> list)
        {
            return new ReadOnlyCollection<T> (list ?? new List<T> ());
        }
        
        void IXmlDeserializer.Deserialize (XmlDeserializationContext context)
        {
            Deserialize (context);
            
            services = MakeReadOnly (ServiceList);
            devices = MakeReadOnly (DeviceList);
            icons = MakeReadOnly (IconList);
            
            Verify ();
        }

        void IXmlDeserializer.DeserializeAttribute (XmlDeserializationContext context)
        {
            DeserializeAttribute (context);
        }

        void IXmlDeserializer.DeserializeElement (Mono.Upnp.Xml.XmlDeserializationContext context)
        {
            DeserializeElement (context);
        }
        
        protected virtual void Deserialize (XmlDeserializationContext context)
        {
            context.AutoDeserialize (this);
        }
        
        protected virtual void DeserializeAttribute (XmlDeserializationContext context)
        {
            context.AutoDeserializeAttribute (this);
        }
        
        protected virtual void DeserializeElement (XmlDeserializationContext context)
        {
            context.AutoDeserializeElement (this);
        }

        public override string ToString ()
        {
            return string.Format ("DeviceDescription {{ uuid:{0}::{1} }}", Udn, Type);
        }
	}
}
