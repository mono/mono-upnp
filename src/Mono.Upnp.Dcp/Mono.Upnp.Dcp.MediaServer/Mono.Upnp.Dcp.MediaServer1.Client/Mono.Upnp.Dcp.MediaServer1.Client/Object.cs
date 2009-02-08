using System;
using System.Collections.Generic;
using System.Xml;

namespace Mono.Upnp.Dcp.MediaServer1
{
	public abstract class Object
	{
        bool has_restricted;

        public string Id { get; private set; }
        public string ParentId { get; private set; }
        public string Title { get; private set; }
        public string Creator { get; private set; }
        public IList<Uri> Res { get; private set; }
        public string Class { get; private set; }
        public bool Restricted { get; private set; }
        public WriteStatus? WriteStatus { get; private set; }

        public override bool Equals (object obj)
        {
            Object @object = obj as Object;
            return @object != null && @object.Id == Id;
        }

        public override int GetHashCode ()
        {
            return ~Id.GetHashCode ();
        }

        public void Deseriailize (XmlReader reader)
        {
            DeserializeCore (reader);
            Verify ();
        }

        protected virtual void DeserializeCore (XmlReader reader)
        {
			using (reader) {
	            reader.Read ();
	            Id = reader["id"];
	            ParentId = reader["parentID"];
	            bool restricted;
	            if (bool.TryParse (reader["restricted"], out restricted)) {
	                Restricted = restricted;
					has_restricted = true;
	            }
			}
        }

        void Verify ()
        {
            if (Id == null) throw new DeserializationException ("The object does not have an ID.");
            if (ParentId == null) throw new DeserializationException (string.Format ("The object {0} does not have a parent ID.", Id));
            if (Title == null) throw new DeserializationException (string.Format ("The object {0} does not have a title.", Id));
            if (Class == null) throw new DeserializationException (string.Format ("The object {0} does not have a class.", Id));
            if (!has_restricted) throw new DeserializationException (string.Format ("The object {0} does not have a restricted value.", Id));
        }
	}
}