using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Mono.Upnp.Internal
{
    internal class DefaultServiceFactory : IServiceFactory
    {
        public ServiceType Type {
            get { return null; }
        }

        public Service CreateService (Client client, IEnumerable<string> locations)
        {
            return new Service (client, locations, null);
        }

        public Service CreateService (Device device, WebHeaderCollection headers, XmlReader reader)
        {
            return new Service (device, headers, reader);
        }
    }
}
