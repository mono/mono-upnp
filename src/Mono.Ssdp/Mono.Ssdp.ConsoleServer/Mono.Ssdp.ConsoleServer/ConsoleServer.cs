using System;
using System.Threading;

namespace Mono.Ssdp.ConsoleServer
{
	public class ConsoleServer
	{
        public static void Main ()
        {
            Server server = new Server ();
            server.Announce ("mono-test", "test1", "http://www.google.com/");
            while (true) {
                Thread.Sleep (5000);
            }
        }
	}
}
