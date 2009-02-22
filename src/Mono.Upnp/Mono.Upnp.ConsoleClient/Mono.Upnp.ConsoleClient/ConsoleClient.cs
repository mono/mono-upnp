using System;

using Mono.Upnp;
using Mono.Upnp.Control;
using Mono.Upnp.ContentDirectory;
using Mono.Upnp.Dcp.MediaServer1;

namespace Mono.Upnp.ConsoleClient
{
    class Program
    {
        static void Main (string[] args)
        {
            var client = new ContentDirectoryClient ();
            client.ContentDirectoryAdded += client_ContentDirectoryAdded;
            client.Browse ();

            while (true) {
                System.Threading.Thread.Sleep (1000);
            }
        }

        static void HandleServiceAdded(object sender, ServiceEventArgs e)
        {
        	Console.WriteLine (e.Service);
        }

        static void client_ContentDirectoryAdded (object sender, DiscoveryEventArgs<Mono.Upnp.ContentDirectory.ContentDirectory> e)
        {
			Console.WriteLine ("Found");
            var root = e.Item.GetRootContainer ();
			foreach (var item in root.Browse ()) {
				Console.WriteLine (item);
			}
        }
    }
}
