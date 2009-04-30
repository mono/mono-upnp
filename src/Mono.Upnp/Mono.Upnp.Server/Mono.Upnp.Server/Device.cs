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

using Mono.Upnp.Xml;
using Mono.Upnp.Server.Internal;

namespace Mono.Upnp.Server
{
    [XmlType ("device")]
	public class Device : XmlSerializable
	{
        readonly DeviceType type;
        readonly string id;
        readonly string udn;
        readonly string friendly_name;
        readonly string manufacturer;
        readonly string model_name;
        readonly List<Service> services = new List<Service> ();
        readonly List<IconDescription> icons = new List<IconDescription> ();
        Uri manufacturer_url;
        string model_description;
        string model_number;
        Uri model_uri;
        string serial_number;
        string upc;
        IconServer icon_server;
        bool initialized;
        
        public Device (DeviceType type, string id, string friendlyName, string manufacturer, string modelName)
        {
            if (type == null) throw new ArgumentNullException ("type");
            if (id == null) throw new ArgumentNullException ("id");
            if (id.Length == 0) throw new ArgumentException ("The id cannot be an empty string.", "id");
            if (friendlyName == null) throw new ArgumentNullException ("friendlyName");
            if (manufacturer == null) throw new ArgumentNullException ("manufacturer");
            if (modelName == null) throw new ArgumentNullException ("modelName");

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

        [XmlElement ("deviceType")]
        public virtual DeviceType Type {
            get { return type; }
        }
        
        [XmlElement ("friendlyName")]
        public virtual string FriendlyName {
            get { return friendly_name; }
        }
        
        [XmlElement ("manufacturer")]
        public virtual string Manufacturer {
            get { return manufacturer; }
        }
        
        [XmlElement ("manufacturerURL", OmitIfNull = true)]
        public virtual Uri ManufacturerUrl {
            get { return manufacturer_url; }
            set {
                CheckInitialized ();
                manufacturer_url = value;
            }
        }
        
        [XmlElement ("modelDescription", OmitIfNull = true)]
        public virtual string ModelDescription {
            get { return model_description; }
            set {
                CheckInitialized ();
                model_description = value;
            }
        }
        
        [XmlElement ("modelName")]
        public virtual string ModelName {
            get {
                CheckInitialized ();
                return model_name;
            }
        }
        
        [XmlElement ("modelNumber", OmitIfNull = true)]
        public virtual string ModelNumber {
            get { return model_number; }
            set {
                CheckInitialized ();
                model_number = value;
            }
        }
        
        [XmlElement ("modelURL", OmitIfNull = true)]
        public virtual Uri ModelUrl {
            get { return model_uri; }
            set {
                CheckInitialized ();
                model_uri = value;
            }
        }
        
        [XmlElement ("serialNumber", OmitIfNull = true)]
        public virtual string SerialNumber {
            get { return serial_number; }
            set {
                CheckInitialized ();
                serial_number = value;
            }
        }
        
        [XmlElement ("UDN")]
        public virtual string Udn {
            get { return udn; }
        }
        
        [XmlElement ("UPC", OmitIfNull = true)]
        public virtual string Upc {
            get { return upc; }
            set {
                CheckInitialized ();
                upc = value;
            }
        }
        
        [XmlArray ("iconList")]
        public virtual IList<IconDescription> Icons {
            get { return icons; }
        }
        
        [XmlArray ("serviceList", OmitIfEmpty = true)]
        public virtual IEnumerable<Service> Services {
            get { return services; }
        }

        public string Id {
            get { return id; }
        }
        
        public virtual void AddService<T> (T service, ServiceType type, string id)
        {
            CheckInitialized ();
            services.Add (CreateService (service, type, id));
        }
        
        protected virtual Service CreateService<T> (T service, ServiceType type, string id)
        {
            var controller = new ServiceController<TService> (service);
            controller.Initialize ();
            return new Service (type, id, controller);
        }
        
        internal void Initialize (Uri baseUrl)
        {
            InitializeCore (baseUrl);
            initialized = true;
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

        protected void CheckInitialized ()
        {
            if (initialized) {
                throw new InvalidOperationException ("You may not modify the device after it has been initialized.");
            }
        }

        protected virtual void Start ()
        {
            foreach (var service in services) {
                service.Start ();
            }
            
            if (icon_server != null) {
                icon_server.Start ();
            }
        }

        protected virtual void Stop ()
        {
            foreach (var service in services) {
                service.Stop ();
            }
                
            if (icon_server != null) {
                icon_server.Stop ();
            }
        }
        
        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            context.AutoSerializeObjectAndMembers (this);
        }
        
        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            context.AutoSerializeMembersOnly (this);
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

            foreach (var service in services) {
                service.Dispose ();
            }
            
            if (icon_server != null) {
                icon_server.Dispose ();
            }
        }
    }
}
