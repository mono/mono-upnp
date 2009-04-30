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

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Description
{
    [XmlType ("device")]
	public class DeviceDescription : Description
	{
        readonly IDisposer disposer;
        readonly ProtectedCollection<ServiceDescription> services;
        readonly ProtectedCollection<DeviceDescription> devices;
        readonly ProtectedCollection<IconDescription> icons;
        DeviceType type;
        string udn;
        string friendly_name;
        string manufacturer;
        string model_name;
        Uri manufacturer_url;
        string model_description;
        string model_number;
        Uri model_url;
        string serial_number;
        string upc;
        IconServer icon_server;

        protected internal DeviceDescription (Root root, IDisposer disposer)
            : base (root)
        {
            if (disposer == null) throw new ArgumentNullException ("disposer");

            this.disposer = disposer;
            services = new ProtectedCollection<ServiceDescription> ();
            devices = new ProtectedCollection<DeviceDescription> ();
            icons = new ProtectedCollection<IconDescription> ();
        }
        
        protected internal Device (DeviceType type, string udn, string friendlyName, string manufacturer, string modelName)
        {
            Type = type;
            Udn = udn;
            FriendlyName = friendlyName;
            Manufacturer = manufacturer;
            ModelName = modelName;
            services = new ProtectedCollection<ServiceDescription> (true);
            devices = new ProtectedCollection<DeviceDescription> (true);
            icons = new ProtectedCollection<IconDescription> (true);
        }
        
        public event EventHandler<DisposedEventArgs> Disposed;
        
        [XmlArray ("serviceList")]
        public virtual ICollection<ServiceDescription> Services { get { return services; } }
        
        [XmlArray ("deviceList")]
        public virtual ICollection<DeviceDescription> Devices { get { return devices; } }
        
        [XmlArray ("iconList")]
        public virtual ICollection<IconDescription> Icons { get { return icons; } }
        
        [XmlElement ("deviceType")]
        public virtual DeviceType Type {
            get { return type; }
            set {
                CheckReadOnly ();
                if (value == null) throw new ArgumentNullException ("type");
                type = value;
            }
        }
        
        [XmlElement ("UDN")]
        public virtual string Udn {
            get { return udn; }
            set {
                CheckReadOnly ();
                if (value == null) throw new ArgumentNullException ("udn");
                if (!value.StartsWith ("uuid:")) throw new ArgumentNullException (@"The UDN must begin with ""uuid:"".", "udn");
                udn = value;
            }
        }
        
        //[XmlElement ("presentationURL")]
        //public virtual Uri PresentationUrl { get; set; }
        
        [XmlElement ("friendlyName")]
        public virtual string FriendlyName {
            get { return friendly_name; }
            set {
                CheckReadOnly ();
                if (value == null) throw new ArgumentNullException ("friendlyName");
                friendly_name = value;
            }
        }
        
        [XmlElement ("manufacturer")]
        public virtual string Manufacturer {
            get { return manufacturer; }
            set {
                CheckReadOnly ();
                if (value == null) throw new ArgumentNullException ("manufacturer");
                manufacturer = value;
            }
        }
        
        [XmlElement ("manufacturerURL", OmitIfNull = true)]
        public virtual Uri ManufacturerUrl {
            get { return manufacturer_url; }
            set {
                CheckReadOnly ();
                manufacturer_url = value;
            }
        }
        
        [XmlElement ("modelDescription", OmitIfNull = true)]
        public virtual string ModelDescription {
            get { return model_description; }
            set {
                CheckReadOnly ();
                model_description = value;
            }
        }
        
        [XmlElement ("modelName")]
        public virtual string ModelName {
            get { return model_name; }
            set {
                CheckReadOnly ();
                if (value == null) throw new ArgumentNullException ("modelName");
                model_name = value;
            }
        }
        
        [XmlElement ("modelNumber", OmitIfNull = true)]
        public virtual string ModelNumber {
            get { return model_number; }
            set {
                CheckReadOnly ();
                model_number = value;
            }
        }
        
        [XmlElement ("modelURL", OmitIfNull = true)]
        public virtual Uri ModelUrl {
            get { return model_url; }
            set {
                CheckReadOnly ();
                model_url = value;
            }
        }
        
        [XmlElement ("serialNumber", OmitIfNull = true)]
        public virtual string SerialNumber {
            get { return serial_number; }
            set {
                CheckReadOnly ();
                serial_number = value;
            }
        }
        
        [XmlElement ("UPC", OmitIfNull = true)]
        public virtual string Upc {
            get { return upc; }
            set {
                CheckReadOnly ();
                upc = value;
            }
        }
        
        public bool IsDisposed { get; private set; }
        
        protected void AddService (ServiceDescription service)
        {
            CheckReadOnly ();
            services.Add (service);
        }
        
        public virtual void AddService<T> (T service, ServiceType type, string id)
        {
            AddService (CreateService (service, type, id));
        }
        
        protected virtual Service CreateService<T> (T service, ServiceType type, string id)
        {
            var controller = new ServiceController<TService> (service);
            controller.Initialize ();
            return new Service (type, id, controller);
        }
        
        protected void AddDevice (DeviceDescription device)
        {
            CheckReadOnly ();
            devices.Add (device);
        }
        
        protected void AddIcon (IconDescription icon)
        {
            CheckReadOnly ();
            icons.Add (icon);
        }
        
        public virtual void AddIcon (int width, int height, int depth, string format, byte[] data)
        {
        }
        
        public virtual void AddIcon (int width, int height, int depth, string format, string filename)
        {
        }
        
        internal override void Initialize (Uri url)
        {
            InitializeCore (url);
            base.Initialize (url);
        }

        protected virtual void InitializeCore (Uri deviceUrl)
        {
            foreach (var service in services) {
                service.Initialize (new Uri (deviceUrl, string.Format ("{0}/{1}/", service.Type.ToUrlString (), service.Id)));
            }
            
            if (icons.Count > 0) {
                var icons_url = new Uri (deviceUrl, "icons/");
                for (var i = 0; i < icons.Count; i++) {
                    icons[i].Initialize (new Uri (icons_url, string.Format ("{0}/", i)));
                }
                icon_server = new IconServer (this, icons_url);
            }
        }

        void CheckReadOnly ()
        {
            if (IsReadOnly) {
                throw new InvalidOperationException ("The device is read-only and cannot be modified.");
            }
        }

        protected virtual void Start ()
        {
            foreach (var service in Services) {
                service.Start ();
            }
            
            foreach (var device in Devices) {
                device.Start ();
            }
            
            if (icon_server != null) {
                icon_server.Start ();
            }
        }

        protected virtual void Stop ()
        {
            foreach (var service in Services) {
                service.Stop ();
            }
            
            foreach (var device in Devices) {
                device.Stop ();
            }
                
            if (icon_server != null) {
                icon_server.Stop ();
            }
        }

        protected internal void CheckDisposed ()
        {
            if (disposer != null) {
                disposer.TryDispose (this);
                if (IsDisposed) {
                    throw new ObjectDisposedException (ToString (), "The device has gone off the network.");
                }
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
            
            foreach (var device in Devices) {
                device.Dispose ();
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
        
        [XmlTypeDeserializer]
        public virtual DeviceDescription DeserializeDevice (XmlDeserializationContext context)
        {
            if (Root != null) {
                var device = Root.CreateDeviceDescription ();
                device.Deserialize (context);
                return device;
            }
            return null;
        }
        
        [XmlTypeDeserializer]
        public virtual ServiceDescription DeserializeService (XmlDeserializationContext context)
        {
            if (Root != null) {
                var service = Root.CreateServiceDescription ();
                service.Deserialize (context);
                return service;
            }
            return null;
        }
        
        [XmlTypeDeserializer]
        public virtual IconDescription DeserializeIcon (XmlDeserializationContext context)
        {
            if (Root != null) {
                var icon = Root.CreateIconDescription (this);
                icon.Deserialize (context);
                return icon;
            }
            return null;
        }
        
        void IXmlDeserializer.Deserialize (XmlDeserializationContext context)
        {
            Deserialize (context);
            
            services.Lock ();
            devices.Lock ();
            icons.Lock ();
            
            Verify ();
        }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            context.AutoDeserialize (this);
        }

        protected override void DeserializeAttribute (XmlDeserializationContext context)
        {
            context.AutoDeserializeAttribute (this);
        }

        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            context.AutoDeserializeElement (this);
        }
        
        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            context.AutoSerializeObjectAndMembers (this);
        }
        
        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            context.AutoSerializeMembersOnly (this);
        }

        public override string ToString ()
        {
            return string.Format ("DeviceDescription {{ uuid:{0}::{1} }}", Udn, Type);
        }
	}
}
