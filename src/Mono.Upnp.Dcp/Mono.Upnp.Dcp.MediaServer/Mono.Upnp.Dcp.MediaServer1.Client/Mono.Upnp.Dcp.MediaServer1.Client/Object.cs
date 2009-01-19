using System;
using System.Collections.Generic;
using System.Xml;

namespace Mono.Upnp.Dcp.MediaServer1
{
	public abstract class Object
	{
        string id;
        string parent_id;
        string title;
        string creator;
        List<Uri> res;
        string @class;
        bool? restricted;
        WriteStatus? write_status;

        public string Id {
            get { return id; }
        }

        public string ParentId {
            get { return parent_id; }
        }

        public string Title {
            get { return title; }
        }

        public string Creator {
            get { return creator; }
        }

        public IList<Uri> Res {
            get { return res; }
        }

        public string Class {
            get { return @class; }
        }

        public bool Restricted {
            get { return restricted.Value; }
        }

        public WriteStatus? WriteStatus {
            get { return write_status; }
        }

        public override bool Equals (object obj)
        {
            Object @object = obj as Object;
            return @object != null && @object.id == id;
        }

        public override int GetHashCode ()
        {
            return ~id.GetHashCode ();
        }

        public void Deseriailize (XmlReader reader)
        {
            DeserializeCore (reader);
            Verify ();
        }

        protected virtual void DeserializeCore (XmlReader reader)
        {
            reader.Read ();
            id = reader["id"];
            parent_id = reader["parentID"];
            bool restricted;
            if (bool.TryParse (reader["restricted"], out restricted)) {
                this.restricted = restricted;
            }
        }

        void Verify ()
        {
            if (id == null) throw new DeserializationException ("The object does not have an ID.");
            if (parent_id == null) throw new DeserializationException (string.Format ("The object {0} does not have a parent ID.", id));
            if (title == null) throw new DeserializationException (string.Format ("The object {0} does not have a title.", id));
            if (@class == null) throw new DeserializationException (string.Format ("The object {0} does not have a class.", id));
            if (restricted == null) throw new DeserializationException (string.Format ("The object {0} does not have a restricted value.", id));
        }
	}
}