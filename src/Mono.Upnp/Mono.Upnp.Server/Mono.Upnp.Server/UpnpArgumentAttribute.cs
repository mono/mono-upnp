using System;

namespace Mono.Upnp.Server
{
    [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class UpnpArgumentAttribute : Attribute
	{
        private string name;
        private object default_value;
        private AllowedValueRange allowed_value_range;

        public UpnpArgumentAttribute ()
        {
        }

        public UpnpArgumentAttribute (string name)
        {
            this.name = name;
        }

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public object DefaultValue {
            get { return default_value; }
            set { default_value = value; }
        }

        public AllowedValueRange AllowedValueRange {
            get { return allowed_value_range; }
            set { allowed_value_range = value; }
        }
	}
}
