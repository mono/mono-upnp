using System.Xml;

namespace Mono.Upnp.Dcp.Sharpener
{
	internal class OfflineDevice : Device
	{
        public OfflineDevice (XmlReader reader)
            : base (null, null, null, reader)
        {
        }

        protected override void Deserialize (XmlReader reader, string element)
        {
            if (element == "deviceType") {
                base.Deserialize (reader, element);
            }
        }
	}
}
