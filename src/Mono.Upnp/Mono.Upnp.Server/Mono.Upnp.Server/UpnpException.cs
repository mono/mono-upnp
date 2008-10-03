using System;

namespace Mono.Upnp.Server
{
	public class UpnpException : Exception
	{
        public UpnpException (string message)
            : base (message)
        {
        }

        public UpnpException (string message, Exception innerException)
            : base (message, innerException)
        {
        }
	}
}
