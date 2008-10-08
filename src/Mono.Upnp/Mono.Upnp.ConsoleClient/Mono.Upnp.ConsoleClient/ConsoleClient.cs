using System;

using Mono.Upnp;
using Mono.Upnp.Control;
//using Mono.Upnp.Dcp.MediaServer1;

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
            if (e.Service.Type.DomainName == "mono") {
                var action = e.Service.Actions["ReturnsValue"];
                action.InArguments["value"].Value = "true";
                action.Invoke ();
                Console.WriteLine (action.ReturnArgument.Value);
                e.Service.StateVariables["Message"].Changed += Program_Changed;
            }
        }

        static void Program_Changed (object sender, StateVariableChangedArgs<string> e)
        {
            Console.WriteLine (e.NewValue);
        }
    }
}
