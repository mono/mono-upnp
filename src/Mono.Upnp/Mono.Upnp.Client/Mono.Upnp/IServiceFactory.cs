using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Mono.Upnp
{
    public interface IServiceFactory
    {
        ServiceType Type { get; }
        Service CreateService (Client client, IEnumerable<string> locations);
        Service CreateService (Device device, WebHeaderCollection headers, XmlReader reader);
    }
}
