using System;

namespace Mono.Upnp.Server.ConsoleServer
{
    public class TestService : Service
    {
        public TestService (string id)
            : base (new ServiceType ("mono", "test", new Version (1, 0)), id)
        {
        }

        [UpnpAction]
        public void Test1 ()
        {
            Console.WriteLine ("Test1 executed");
        }

        [UpnpAction]
        public void Test2 (string value)
        {
            Console.WriteLine ("Test2 executed with {0}", value);
        }

        [UpnpAction]
        public bool ReturnsTrue ()
        {
            Console.WriteLine ("ReturnsTrue executed");
            return true;
        }

        [UpnpAction]
        public bool ReturnsValue (bool value)
        {
            Console.WriteLine ("ReturnsValue executed with {0}", value);
            return value;
        }

        [UpnpAction]
        public void Test3 (int i, out string s, out bool b)
        {
            Console.WriteLine ("Test3 executed with {0}", i);
            s = "Hello from Test3!";
            b = true;
        }
    }
}
