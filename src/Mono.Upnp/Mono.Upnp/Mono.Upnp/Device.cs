//
// Device.cs
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

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp
{
    [XmlType ("device")]
    public class Device :
        Description,
        IXmlDeserializable,
        IXmlDeserializer<DeviceType>,
        IXmlDeserializer<Device>,
        IXmlDeserializer<Service>,
        IXmlDeserializer<Icon>
    {
        IList<Device> devices;
        IList<Service> services;
        IList<Icon> icons;
        
        public Device (DeviceType type, string udn, string friendlyName, string manufacturer, string modelName)
            : this (type, udn, friendlyName, manufacturer, modelName, null)
        {
        }
        
        public Device (DeviceType type,
                       string udn,
                       string friendlyName,
                       string manufacturer,
                       string modelName,
                       DeviceOptions options)
            : this (type, udn, friendlyName, manufacturer, modelName, options, null)
        {
        }
        
        protected internal Device (DeviceType type,
                                   string udn,
                                   string friendlyName,
                                   string manufacturer,
                                   string modelName,
                                   DeviceOptions options,
                                   IEnumerable<Device> devices)
            : this (devices, GetServices (options), GetIcons (options))
        {
            if (type == null) {
                throw new ArgumentNullException ("type");
            } else if (udn == null) {
                throw new ArgumentNullException ("udn");
            } else if (!udn.StartsWith ("uuid:")) {
                throw new ArgumentException (@"The udn must begin with ""uuid:"".", "udn");
            } else if (friendlyName == null) {
                throw new ArgumentNullException ("friendlyName");
            } else if (manufacturer == null) {
                throw new ArgumentException ("manufacturer");
            } else if (modelName == null) {
                throw new ArgumentNullException ("modelName");
            }
            
            Type = type;
            Udn = udn;
            FriendlyName = friendlyName;
            Manufacturer = manufacturer;
            ModelName = modelName;
            
            if (options != null) {
                ManufacturerUrl = options.ManufacturerUrl;
                ModelDescription = options.ModelDescription;
                ModelNumber = options.ModelNumber;
                ModelUrl = options.ModelUrl;
                SerialNumber = options.SerialNumber;
                Upc = options.Upc;
            }
        }
        
        protected internal Device (IEnumerable<Device> devices, IEnumerable<Service> services, IEnumerable<Icon> icons)
        {
            this.devices = Helper.MakeReadOnlyCopy (devices);
            this.services = Helper.MakeReadOnlyCopy (services);
            this.icons = Helper.MakeReadOnlyCopy (icons);
        }
        
        protected internal Device (Deserializer deserializer)
            : base (deserializer)
        {
            devices = new List<Device> ();
            services = new List<Service> ();
            icons = new List<Icon> ();
        }
        
        static IEnumerable<Service> GetServices (DeviceOptions options)
        {
            if (options != null) {
                return options.Services;
            } else {
                return null;
            }
        }
        
        static IEnumerable<Icon> GetIcons (DeviceOptions options)
        {
            if (options != null) {
                return options.Icons;
            } else {
                return null;
            }
        }
        
        [XmlArray ("iconList", OmitIfEmpty = true)]
        protected virtual ICollection<Icon> IconCollection {
            get { return icons; }
        }
        
        public IEnumerable<Icon> Icons {
            get { return icons; }
        }
        
        [XmlArray ("serviceList", OmitIfEmpty = true)]
        protected virtual ICollection<Service> ServiceCollection {
            get { return services; }
        }
        
        public IEnumerable<Service> Services {
            get { return services; }
        }
        
        [XmlArray ("deviceList", OmitIfEmpty = true)]
        protected virtual ICollection<Device> DeviceCollection {
            get { return devices; }
        }
        
        public IEnumerable<Device> Devices {
            get { return devices; }
        }
        
        [XmlElement ("deviceType")]
        public virtual DeviceType Type { get; protected set; }
        
        [XmlElement ("friendlyName")]
        public virtual string FriendlyName { get; protected set; }
        
        [XmlElement ("manufacturer")]
        public virtual string Manufacturer { get; protected set; }
        
        [XmlElement ("manufacturerURL", OmitIfNull = true)]
        public virtual Uri ManufacturerUrl { get; protected set; }
        
        [XmlElement ("modelDescription", OmitIfNull = true)]
        public virtual string ModelDescription { get; protected set; }
        
        [XmlElement ("modelName")]
        public virtual string ModelName { get; protected set; }
        
        [XmlElement ("modelNumber", OmitIfNull = true)]
        public virtual string ModelNumber { get; protected set; }
        
        [XmlElement ("modelURL", OmitIfNull = true)]
        public virtual Uri ModelUrl { get; protected set; }
        
        [XmlElement ("serialNumber", OmitIfNull = true)]
        public virtual string SerialNumber { get; protected set; }
        
        [XmlElement ("UDN")]
        public virtual string Udn { get; protected set; }
        
        [XmlElement ("UPC", OmitIfNull = true)]
        public virtual string Upc { get; protected set; }

        protected internal virtual void Initialize (XmlSerializer serializer, Root root, string deviceUrlFragment)
        {
            if (deviceUrlFragment == null) {
                throw new ArgumentNullException ("deviceUrlFragment");
            } else if (Deserializer != null) {
                throw new InvalidOperationException (
                    "The device was constructed for deserialization and cannot be initalized. Use one of the other constructors.");
            }
            
            for (var i = 0; i < devices.Count; i++) {
                devices[i].Initialize (serializer, root, string.Concat (deviceUrlFragment, "/device/", i.ToString ()));
            }
            
            for (var i = 0; i < services.Count; i++) {
                services[i].Initialize (serializer, root, string.Concat (deviceUrlFragment, "/service/", i.ToString ()));
            }
            
            for (var i = 0; i < icons.Count; i++) {
                icons[i].Initialize (root, string.Concat (deviceUrlFragment, "/icon/", i.ToString (), "/"));
            }
        }

        protected internal virtual void Start ()
        {
            foreach (var device in devices) {
                device.Start ();
            }
            
            foreach (var service in services) {
                service.Start ();
            }
            
            foreach (var icon in icons) {
                icon.Start ();
            }
        }

        protected internal virtual void Stop ()
        {
            foreach (var device in devices) {
                device.Stop ();
            }
            
            foreach (var service in services) {
                service.Stop ();
            }
            
            foreach (var icon in icons) {
                icon.Stop ();
            }
        }
        
        DeviceType IXmlDeserializer<DeviceType>.Deserialize (XmlDeserializationContext context)
        {
            return DeserializeDeviceType (context);
        }
        
        protected virtual DeviceType DeserializeDeviceType (XmlDeserializationContext context)
        {
            if (context == null) {
                throw new ArgumentNullException ("context");
            }
            
            return new DeviceType (context.Reader.ReadElementContentAsString ());
        }
        
        Device IXmlDeserializer<Device>.Deserialize (XmlDeserializationContext context)
        {
            return DeserializeDevice (context);
        }
        
        protected virtual Device DeserializeDevice (XmlDeserializationContext context)
        {
            return Deserializer != null ? Deserializer.DeserializeDevice (context) : null;
        }
        
        Service IXmlDeserializer<Service>.Deserialize (XmlDeserializationContext context)
        {
            return DeserializeService (context);
        }
        
        protected virtual Service DeserializeService (XmlDeserializationContext context)
        {
            return Deserializer != null ? Deserializer.DeserializeService (context) : null;
        }
        
        Icon IXmlDeserializer<Icon>.Deserialize (XmlDeserializationContext context)
        {
            return DeserializeIcon (context);
        }
        
        protected virtual Icon DeserializeIcon (XmlDeserializationContext context)
        {
            return Deserializer != null ? Deserializer.DeserializeIcon (context) : null;
        }
        
        void IXmlDeserializable.Deserialize (XmlDeserializationContext context)
        {
            Deserialize (context);
            devices = new ReadOnlyCollection<Device> (devices);
            services = new ReadOnlyCollection<Service> (services);
            icons = new ReadOnlyCollection<Icon> (icons);
        }
        
        void IXmlDeserializable.DeserializeAttribute (XmlDeserializationContext context)
        {
            DeserializeAttribute (context);
        }
        
        void IXmlDeserializable.DeserializeElement (XmlDeserializationContext context)
        {
            DeserializeElement (context);
        }

        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            AutoDeserializeElement (this, context);
        }
        
        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            AutoSerializeObjectAndMembers (this, context);
        }
        
        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            AutoSerializeMembersOnly (this, context);
        }
    }
}
