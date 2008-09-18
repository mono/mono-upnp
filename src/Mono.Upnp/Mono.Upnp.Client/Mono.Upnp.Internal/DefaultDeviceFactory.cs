using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Mono.Upnp.Internal
{
	internal class DefaultDeviceFactory : IDeviceFactory
	{
        public DeviceType Type {
            get { return null; }
        }

        public Device CreateDevice (Client client, string udn, IEnumerable<string> locations)
        {
            return new Device (client, udn, locations, null);
        }

        public Device CreateDevice (Client client, Root root, WebHeaderCollection headers, XmlReader reader)
        {
            return new Device (client, root, headers, reader);
        }

        public IEnumerable<IServiceFactory> Services {
            get { yield break; }
        }

        public IEnumerable<IDeviceFactory> Devices {
            get { yield break; }
        }
    }
}
