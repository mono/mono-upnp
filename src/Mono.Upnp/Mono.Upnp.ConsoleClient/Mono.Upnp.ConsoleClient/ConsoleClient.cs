using System;

using Mono.Upnp;
using Mono.Upnp.Control;
using Mono.Upnp.Dcp.MediaServer1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Av;

namespace Mono.Upnp.ConsoleClient
{
    class Program
    {
        static void Main (string[] args)
        {
            var client = new UpnpClient ();
            client.ServiceAdded += HandleServiceAdded;
            //client.ContentDirectoryAdded += client_ContentDirectoryAdded;
            client.BrowseAll ();

            while (true) {
                System.Threading.Thread.Sleep (1000);
            }
        }

        static void HandleServiceAdded(object sender, ServiceEventArgs e)
        {
        	Console.WriteLine (e.Service);
        }

        static void client_ContentDirectoryAdded (object sender, DiscoveryEventArgs<ContentDirectory> e)
        {
			Console.WriteLine ("Found");
            var root = e.Item.GetRootContainer ();
			foreach (var item in root.SearchForType<MusicTrack> (new ResultsSettings { RequestCount = 10 })) {
                Console.WriteLine (item);
                foreach (var res in item.Resources) {
                    Console.WriteLine ("\t{0}{1}", res.ProtocolInfo, res.Uri);
                }
			}
        }
    }
}
