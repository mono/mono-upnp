using System;
using System.Xml;

namespace Mono.Upnp.Dcp.Sharpener
{
	internal class Sharpener
	{
        public static int Main (string[] args)
        {
            var context = new RunnerContext (
                "urn:schemas-upnp-org:service:ContentDirectory:1",
                "ContentDirectory1",
                "Mono.Upnp.Dcp.MediaServer1",
                @"C:\Users\Scott\Desktop\ContentDirectory1.xml");
            var runner = new ServerRunner (context);
            runner.Run ();
            return 0;
        }
	}
}
