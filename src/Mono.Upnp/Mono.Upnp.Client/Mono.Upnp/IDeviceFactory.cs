using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Mono.Upnp
{
	public interface IDeviceFactory
	{
        DeviceType Type { get; }
        Device CreateDevice (Client client, string udn, IEnumerable<string> locations);
        Device CreateDevice (Client client, Root root, WebHeaderCollection headers, XmlReader reader);
        IEnumerable<IServiceFactory> Services { get; }
        IEnumerable<IDeviceFactory> Devices { get; }
	}
}
