using System;

using Mono.Upnp;
using Mono.Upnp.Control;
using Mono.Upnp.Dcp.MediaServer1;

namespace Mono.Upnp.ConsoleClient
{
    class Program
    {
        static void Main (string[] args)
        {
            Client client = new Client ();
            client.BrowseAll ();

            client.ServiceAdded += new EventHandler<ServiceArgs> (client_ServiceAdded);
            //client.DeviceAdded += new EventHandler<DeviceArgs> (client_DeviceAdded);
            //client.BrowseAll ();

            while (true) {
                System.Threading.Thread.Sleep (1000);
            }
        }

        static void client_ServiceAdded (object sender, ServiceArgs e)
        {
            Console.WriteLine ("Found {0} {1}", e.Service.Type, e.Service.Id);
            var service = e.Service;
            if (service.Type.DomainName == "mono") {
                service.Actions ["Test3"].InArguments ["i"].Value = "5";
                service.Actions ["Test3"].Execute ();
                Console.Write (service.Actions ["Test3"].OutArguments ["s"].Value);
            }
        }
    }
}
