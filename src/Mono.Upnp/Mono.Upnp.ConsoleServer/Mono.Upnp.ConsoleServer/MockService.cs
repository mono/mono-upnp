using System;

namespace Mono.Upnp.Server.ConsoleServer
{
    public class MockService : ServiceDescription
    {
        public MockService (ServiceType type, string id)
            : base (type, id)
        {
        }
    }
}
