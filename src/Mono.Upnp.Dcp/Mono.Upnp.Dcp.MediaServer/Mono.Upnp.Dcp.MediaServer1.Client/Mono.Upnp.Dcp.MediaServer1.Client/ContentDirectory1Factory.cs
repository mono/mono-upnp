using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Mono.Upnp.Dcp.MediaServer1
{
	public class ContentDirectory1Factory : IServiceFactory
	{
        private static readonly ServiceType type = new ServiceType ("urn:schemas-upnp-org:service:ContentDirectory:1");
        internal static ServiceType Type {
            get { return type; }
        }

        ServiceType IServiceFactory.Type
        {
            get { return Type; }
        }

        public Service CreateService (Client client, IEnumerable<string> locations)
        {
            return new ContentDirectory1 (client, locations);
        }

        public Service CreateService (Device device, WebHeaderCollection headers, XmlReader reader)
        {
            return new ContentDirectory1 (device, headers, reader);
        }
    }
}
