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
//            ContentDirectory1Client client = new ContentDirectory1Client ();
//            client.ContentDirectory1Added += client_ContentDirectory1Added;
//            client.Browse ();

            while (true) {
                System.Threading.Thread.Sleep (1000);
            }
        }

//        static void client_ContentDirectory1Added (object sender, DiscoveryEventArgs<ContentDirectory> e)
//        {
//            string results, returned, count, s;
//            //e.Item.Browse ("0", BrowseFlag.BrowseDirectChildren, "*", 0, 10, "", out results, out returned, out count, out s);
//            Console.WriteLine (results);
//        }
    }
}
