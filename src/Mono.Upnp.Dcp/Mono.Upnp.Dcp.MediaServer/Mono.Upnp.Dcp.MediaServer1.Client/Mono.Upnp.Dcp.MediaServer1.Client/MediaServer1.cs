using System.Collections.Generic;
using System.Net;
using System.Xml;

using Mono.Upnp;

namespace Mono.Upnp.Dcp.MediaServer1
{
	public class MediaServer1 : Device
	{
        internal MediaServer1 (Client client, string udn, IEnumerable<string> locations)
            : base (client, udn, locations, MediaServer1Factory.Type)
        {
        }

        internal MediaServer1 (Client client, Root root, WebHeaderCollection headers, XmlReader reader)
            : base (client, root, headers, reader)
        {
        }

        private ContentDirectory1 content_directory;
        public ContentDirectory1 ContentDirectory {
            get {
                if (content_directory == null) {
                    content_directory = GetService<ContentDirectory1> ();
                }
                return content_directory;
            }
        }

        private ConnectionManager1 connection_manager;
        public ConnectionManager1 ConnectionManager {
            get {
                if (connection_manager == null) {
                    connection_manager = GetService<ConnectionManager1> ();
                }
                return connection_manager;
            }
        }
    }
}
