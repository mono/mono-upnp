using System;
using System.Xml;

namespace Mono.Upnp.Dcp.Sharpener
{
	internal class Program
	{
        public static int Main (string[] args)
        {
            var context = new RunnerContext {
                ClassName = "ContentDirectory1",
                Namespace = "Mono.Upnp.Dcp.MediaServer1",
                Reader = XmlReader.Create (@"C:\Users\Scott\Desktop\ContentDirectory1.xml")
            };
            ClientRunner.Run (context);
            return 0;
        }
	}
}
