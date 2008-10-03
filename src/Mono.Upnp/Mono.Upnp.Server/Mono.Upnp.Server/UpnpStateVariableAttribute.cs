using System;

namespace Mono.Upnp.Server
{
    [AttributeUsage (AttributeTargets.Event)]
	public class UpnpStateVariableAttribute : Attribute
	{
        private readonly string name;

        public UpnpStateVariableAttribute (string name)
        {
            this.name = name;
        }

        public string Name {
            get { return name; }
        }
	}
}
