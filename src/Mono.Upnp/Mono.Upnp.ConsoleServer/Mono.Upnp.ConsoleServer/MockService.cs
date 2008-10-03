using System;

namespace Mono.Upnp.Server.ConsoleServer
{
    public class MockService : Service
    {
        public MockService (ServiceType type, string id)
            : base (type, id)
        {
        }
    }
}
