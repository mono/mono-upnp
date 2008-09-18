using System.Collections.Generic;
using System.Net;
using System.Xml;

using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.MediaServer1
{
	public class ContentDirectory1 : Service
	{
        internal ContentDirectory1 (Client client, IEnumerable<string> locations)
            : base (client, locations, ContentDirectory1Factory.Type)
        {
        }

        internal ContentDirectory1 (Device device, WebHeaderCollection headers, XmlReader reader)
            : base (device, headers, reader)
        {
        }

        public string GetSearchCapabilities ()
        {
            Action action = Actions["GetSearchCapabilities"];
            action.Execute ();
            return action.OutArguments["SearchCaps"].Value;
        }

        public string GetSortCapabilities ()
        {
            Action action = Actions["GetSortCapabilities"];
            action.Execute ();
            return action.OutArguments["SortCaps"].Value;
        }
	}
}
