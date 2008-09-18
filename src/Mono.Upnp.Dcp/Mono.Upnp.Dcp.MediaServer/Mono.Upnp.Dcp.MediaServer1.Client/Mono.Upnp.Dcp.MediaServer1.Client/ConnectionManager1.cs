using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Mono.Upnp.Dcp.MediaServer1
{
	public class ConnectionManager1 : Service
	{
        internal ConnectionManager1 (Client client, IEnumerable<string> locations)
            : base (client, locations, ConnectionManager1Factory.Type)
        {
        }

        internal ConnectionManager1 (Device device, WebHeaderCollection headers, XmlReader reader)
            : base (device, headers, reader)
        {
        }
	}
}
