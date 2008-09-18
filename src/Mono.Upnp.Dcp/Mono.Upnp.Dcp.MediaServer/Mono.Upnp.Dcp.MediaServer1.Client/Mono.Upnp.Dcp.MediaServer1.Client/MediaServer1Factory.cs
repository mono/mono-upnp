using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Mono.Upnp.Dcp.MediaServer1
{
	public class MediaServer1Factory : IDeviceFactory
	{
        private static readonly DeviceType type = new DeviceType ("urn:schemas-upnp-org:device:MediaServer:1");
        internal static DeviceType Type {
            get { return type; }
        }

        DeviceType IDeviceFactory.Type{
            get { return Type; }
        }

        public Device CreateDevice (Client client, string udn, IEnumerable<string> locations)
        {
            return new MediaServer1 (client, udn, locations);
        }

        public Device CreateDevice (Client client, Root root, WebHeaderCollection headers, XmlReader reader)
        {
            return new MediaServer1 (client, root, headers, reader);
        }

        public IEnumerable<IServiceFactory> Services {
            get {
                yield return new ContentDirectory1Factory ();
                yield return new ConnectionManager1Factory ();
            }
        }

        public IEnumerable<IDeviceFactory> Devices {
            get { yield break; }
        }
    }
}
