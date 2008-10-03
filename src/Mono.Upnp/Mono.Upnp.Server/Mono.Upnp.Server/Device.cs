using System;
using System.Collections.Generic;
using System.Xml;

namespace Mono.Upnp.Server
{
	public class Device : IDisposable
	{
        private readonly object mutex = new object ();

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

            if (!id.StartsWith ("uuid:")) {
                id = "uuid:" + id;
            }

            this.type = type;
            this.services = services;
            this.id = id;
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
            set { manufacturer_url = value; }
        }

        private string model_description;
        public string ModelDescription {
            get { return model_description; }
            set { model_description = value; }
        }

        private readonly string model_name;
        public string ModelName {
            get { return model_name; }
        }

        private string model_number;
        public string ModelNumber {
            get { return model_number; }
            set { model_number = value; }
        }

        private Uri model_uri;
        public Uri ModelUrl {
            get { return model_uri; }
            set { model_uri = value; }
        }

        private string serial_number;
        public string SerialNumber {
            get { return serial_number; }
            set { serial_number = value; }
        }

        private string upc;
        public string Upc {
            get { return upc; }
            set { upc = value; }
        }

        private IEnumerable<Icon> icons;
        public IEnumerable<Icon> Icons {
            get { return icons; }
            set { icons = value; }
        }

        public virtual void Start ()
        {
            lock (mutex) {
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
            }
        }

        public virtual void Stop ()
        {
            lock (mutex) {
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
            WriteElement (writer, "UDN", id);
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

        public void Dispose ()
        {
            lock (mutex) {
                Dispose (true);
                GC.SuppressFinalize (this);
            }
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposing) {
                Stop ();
            }
        }
    }
}
