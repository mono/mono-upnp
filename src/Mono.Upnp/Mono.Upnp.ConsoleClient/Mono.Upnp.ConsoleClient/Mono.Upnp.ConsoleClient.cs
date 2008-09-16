using System;

using Mono.Upnp;
using Mono.Upnp.Control;

namespace Mono.Upnp.ConsoleClient
{
    class Program
    {
        static void Main (string[] args)
        {
            Client client = new Client ();
            client.ServiceAdded += new EventHandler<ServiceArgs> (client_ServiceAdded);
            client.DeviceAdded += new EventHandler<DeviceArgs> (client_DeviceAdded);
            client.BrowseAll ();
            while (true) {
                System.Threading.Thread.Sleep (1000);
            }
        }

        static void client_DeviceAdded (object sender, DeviceArgs e)
        {
            Console.WriteLine ("Found device: {0}", e.Device.Type);
        }

        static void client_ServiceAdded (object sender, ServiceArgs e)
        {
            if (e.Service.Type.ToString () == "urn:schemas-upnp-org:service:ContentDirectory:1") {
                var browse = e.Service.Actions["Browse"];

                //try {
                //    var s = e.Service.StateVariables["SystemUpdateID"].GetValue ();
                //    Console.WriteLine (s);
                //} catch (UpnpControlException ex) {
                //    Console.WriteLine ("boo");
                //}

                browse.InArguments["ObjectID"].Value = "0";
                browse.InArguments["BrowseFlag"].Value = "BrowseDirectChildren";
                browse.InArguments["Filter"].Value = "*";
                browse.InArguments["StartingIndex"].Value = "0";
                browse.InArguments["RequestedCount"].Value = "10";
                try {
                    browse.Execute ();
                } catch (UpnpControlException ex) {
                    Console.WriteLine (ex.Status);
                }
                Console.WriteLine (browse.OutArguments["Result"]);
                Console.WriteLine (e.Service.Actions["Browse"].InArguments.Count);
            }
        }
    }
}
