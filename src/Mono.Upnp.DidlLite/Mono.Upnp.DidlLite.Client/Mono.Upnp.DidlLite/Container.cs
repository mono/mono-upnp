using System;

namespace Mono.Upnp.DidlLite
{
	public abstract class Container : Object
	{
        private int child_count;
        private string create_class;
        private string search_class;
        private bool searchable;
	}
}
