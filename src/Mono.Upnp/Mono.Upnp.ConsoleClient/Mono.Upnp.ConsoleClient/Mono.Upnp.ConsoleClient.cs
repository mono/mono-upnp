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
            client.Browse<ContentDirectory1> (new ContentDirectory1Factory (), OnContentDirectory);

            //client.ServiceAdded += new EventHandler<ServiceArgs> (client_ServiceAdded);
            //client.DeviceAdded += new EventHandler<DeviceArgs> (client_DeviceAdded);
            //client.BrowseAll ();

            while (true) {
                System.Threading.Thread.Sleep (1000);
            }
        }

        static void OnContentDirectory (object sender, ServiceArgs<ContentDirectory1> service)
        {
            Console.WriteLine ("Found content directory with these searchs caps: {0}", service.Service.GetSortCapabilities ());
        }

        static void client_DeviceAdded (object sender, DeviceArgs e)
        {
            Console.WriteLine ("Found device: {0}", e.Device.Type);
        }

        static void client_ServiceAdded (object sender, ServiceArgs e)
        {
            if (e.Service.Type.ToString () == "urn:schemas-upnp-org:service:ContentDirectory:1") {
                e.Service.StateVariables["SystemUpdateID"].Changed += new EventHandler<StateVariableChangedArgs<string>> (Program_Changed);
                return;
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
                browse.Execute ();
                Console.WriteLine (browse.OutArguments["Result"]);
                Console.WriteLine (e.Service.Actions["Browse"].InArguments.Count);
            }
        }

        static void Program_Changed (object sender, StateVariableChangedArgs<string> e)
        {
            Console.WriteLine ("SystemUpdateID changed: {0}", e.NewValue);
        }
    }
}
