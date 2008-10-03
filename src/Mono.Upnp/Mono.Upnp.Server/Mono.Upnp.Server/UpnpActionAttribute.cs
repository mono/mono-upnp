using System;

namespace Mono.Upnp.Server
{
    [AttributeUsage (AttributeTargets.Method)]
	public class UpnpActionAttribute : Attribute
	{
        private readonly string name;

        public UpnpActionAttribute ()
            : this (null)
        {
        }

        public UpnpActionAttribute (string name)
        {
            this.name = name;
        }

        public string Name {
            get { return name; }
        }
	}
}
