using System.Xml;

using Mono.Upnp;

namespace Mono.Upnp.Dcp.Sharpener
{
	internal class OfflineService : Service
	{
        public OfflineService (XmlReader reader)
            : base (null, null, reader)
        {
        }

        protected override void Verify ()
        {
        }
	}
}
