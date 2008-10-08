// ConnectionManager1.cs auto-generated at 10/9/2008 2:49:26 AM by Sharpener

using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Mono.Upnp.Dcp.MediaServer1
{
    public class ConnectionManager1Factory : ServiceFactory
    {
        private static readonly ServiceType type = new ServiceType ("urn:schemas-upnp-org:service:ConnectionManager:1");
        internal static ServiceType ServiceType {
            get { return type; }
        }

        public override ServiceType Type {
            get { return ServiceType; }
        }

        protected override Service CreateServiceCore (Client client, string deviceId, IEnumerable<string> locations)
        {
            return new ConnectionManager1 (client, deviceId, locations);
        }

        protected override Service CreateServiceCore (Device device, XmlReader reader, WebHeaderCollection headers)
        {
            return new ConnectionManager1 (device, reader, headers);
        }
    }
}