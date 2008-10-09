using System;
using System.Collections.Generic;

namespace Mono.Upnp.Dcp.MediaServer1
{
	public class Object
	{
        private string id;
        private string parent_id;
        private string title;
        private string creator;
        private List<Uri> res;
        private string @class;
        private bool restricted;
        private WriteStatus? write_status;

        public override bool Equals (object obj)
        {
            Object @object = obj as Object;
            return @object != null && @object.id == id;
        }

        public override int GetHashCode ()
        {
            return id.GetHashCode ();
        }
	}
}
