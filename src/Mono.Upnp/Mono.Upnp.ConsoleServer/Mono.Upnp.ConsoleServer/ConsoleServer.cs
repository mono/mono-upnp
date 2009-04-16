using System;
using System.Collections.Generic;
using System.Threading;

namespace Mono.Upnp.Server.ConsoleServer
{
    public class ConsoleServer
    {
        private static void Main ()
        {
            var service = new TestService ("ts");
            var services = new List<ServiceDescription> { 
                new MockService (new ServiceType ("schemas-upnp-org", "ConnectionManager", new Version (1, 0)), "cm"),
                new MockService (new ServiceType ("schemas-upnp-org", "ConnectionDirectory", new Version (1, 0)), "cd"),
                service
            };
            var device = new Device (new DeviceType ("schemas-upnp-org", "MediaServer", new Version (1, 0)), services, "mono-test-device-1", "Mono Test Device", "Mono", "Test1");
            
            var server = new Server (device);
            server.Start ();
            while (true) {
                Thread.Sleep (5000);
                service.Message = "Hello World!";
            }
        }
    }
}
