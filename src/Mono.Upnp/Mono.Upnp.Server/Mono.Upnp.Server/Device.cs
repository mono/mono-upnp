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

using Mono.Upnp.Server.Internal;
using Mono.Upnp.Server.Serialization;

namespace Mono.Upnp.Server
{
    [XmlType ("device")]
	public class Device
	{
        readonly DeviceType type;
        readonly string id;
        readonly string udn;
        readonly string friendly_name;
        readonly string manufacturer;
        readonly string model_name;
        readonly IEnumerable<ServiceDescription> services;
        Uri manufacturer_url;
        string model_description;
        string model_number;
        Uri model_uri;
        string serial_number;
        string upc;
        IList<Icon> icons;
        IconServer icon_server;
        bool initialized;
        
        public Device (DeviceType type, IEnumerable<ServiceDescription> services, string id, string friendlyName, string manufacturer, string modelName)
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
        public DeviceType Type {
            get { return type; }
        }
        
        [XmlElement ("friendlyName")]
        public string FriendlyName {
            get { return friendly_name; }
        }
        
        [XmlElement ("manufacturer")]
        public string Manufacturer {
            get { return manufacturer; }
        }
        
        [XmlElement ("manufacturerURL")]
        public Uri ManufacturerUrl {
            get { return manufacturer_url; }
            set {
                CheckInitialized ();
                manufacturer_url = value;
            }
        }
        
        [XmlElement ("modelDescription")]
        public string ModelDescription {
            get { return model_description; }
            set {
                CheckInitialized ();
                model_description = value;
            }
        }
        
        [XmlElement ("modelName")]
        public string ModelName {
            get {
                CheckInitialized ();
                return model_name;
            }
        }
        
        [XmlElement ("modelNumber")]
        public string ModelNumber {
            get { return model_number; }
            set {
                CheckInitialized ();
                model_number = value;
            }
        }
        
        [XmlElement ("modelURL")]
        public Uri ModelUrl {
            get { return model_uri; }
            set {
                CheckInitialized ();
                model_uri = value;
            }
        }
        
        [XmlElement ("serialNumber")]
        public string SerialNumber {
            get { return serial_number; }
            set {
                CheckInitialized ();
                serial_number = value;
            }
        }
        
        [XmlElement ("UDN")]
        public string Udn {
            get { return udn; }
        }
        
        [XmlElement ("UPC")]
        public string Upc {
            get { return upc; }
            set {
                CheckInitialized ();
                upc = value;
            }
        }
        
        [XmlElement ("iconList")]
        public IList<Icon> Icons {
            get { return icons; }
            set {
                CheckInitialized ();
                icons = value;
            }
        }
        
        [XmlArray ("serviceList")]
        public IEnumerable<ServiceDescription> Services {
            get { return services; }
        }
        
        [XmlArray ("deviceList")]
        public IEnumerable<Device> Devices { get; internal set;}

        public string Id {
            get { return id; }
        }
        
        internal void Initialize (Uri baseUrl)
        {
            InitializeCore (baseUrl);
            initialized = true;
        }

        protected virtual void InitializeCore (Uri baseUrl)
        {
            var url = new Uri (baseUrl, string.Format ("{0}/{1}/", type.ToUrlString (), id));
            if (services != null) {
                foreach (var service in services) {
                    service.Initialize (url);
                }
            }
            if (Devices != null) {
                foreach (var device in Devices) {
                    device.Initialize (url);
                }
            }
            if (icons != null && icons.Count > 0) {
                url = new Uri (url, "icons/");
                icon_server = new IconServer (this, url);
                for (var i = 0; i < icons.Count; i++) {
                    icons[i].Initialize (new Uri (url, string.Format ("{0}/", i)));
                }
            }
        }

        void CheckInitialized ()
        {
            if (initialized) {
                throw new InvalidOperationException ("You may not modify the device after it has been initialized.");
            }
        }
        
        internal void Start ()
        {
            StartCore ();
        }

        protected virtual void StartCore ()
        {
            if (services != null) {
                foreach (var service in services) {
                    service.Start ();
                }
            }
            if (Devices != null) {
                foreach (var device in Devices) {
                    device.Start ();
                }
            }
            if (icon_server != null) {
                icon_server.Start ();
            }
        }
        
        internal void Stop ()
        {
            StopCore ();
        }

        protected virtual void StopCore ()
        {
            if (services != null) {
                foreach (var service in services) {
                    service.Stop ();
                }
            }
            if (Devices != null) {
                foreach (var device in Devices) {
                    device.Stop ();
                }
            }
            if (icon_server != null) {
                icon_server.Stop ();
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

            if (Devices != null) {
                foreach (var device in Devices) {
                    device.Dispose ();
                }
            }
            if (services != null) {
                foreach (var service in services) {
                    service.Dispose ();
                }
            }
        }
    }
}
