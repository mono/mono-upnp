//
// Argument.cs
//
// Author:
//   Scott Peterson <lunchtimemama@gmail.com>
//
// Copyright (C) 2008 S&S Black Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp.Control
{
	public class Argument
    {
        readonly Action action;
        string name;
        ArgumentDirection? direction;
        string related_state_variable_name;
        StateVariable related_state_variable;
        bool is_return_value;
        bool loaded;

        protected internal Argument (Action action)
        {
            if (action == null) throw new ArgumentNullException ("action");

            this.action = action;
        }

        public Action Action {
            get { return action; }
        }

        public string Name {
            get { return name; }
            protected set { SetField (ref name, value);  }
        }

        public ArgumentDirection Direction {
            get { return direction.Value; }
            protected set {
                CheckLoaded ();
                direction = value;
            }
        }

        public string RelatedStateVariableName {
            get { return related_state_variable_name; }
            set { SetField (ref related_state_variable_name, value);  }
        }

        public StateVariable RelatedStateVariable {
            get {
                if (related_state_variable == null) {
                    action.Controller.StateVariables.TryGetValue (related_state_variable_name, out related_state_variable);
                }
                return related_state_variable;
            }
        }

        public bool IsReturnValue {
            get { return is_return_value; }
            protected set { SetField (ref is_return_value, value); }
        }

        void CheckLoaded ()
        {
            if (loaded) {
                throw new InvalidOperationException ("The argument has already been deserialized.");
            }
        }

        void SetField<T> (ref T field, T value)
        {
            CheckLoaded ();
            field = value;
        }

        public void Deserialize (XmlReader reader)
        {
            DeserializeCore (reader);
            VerifyDeserialization ();
            loaded = true;
        }

        protected virtual void DeserializeCore (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                reader.ReadToFollowing ("argument");
                while (Helper.ReadToNextElement (reader)) {
                    try {
                        DeserializeCore (reader.ReadSubtree (), reader.Name);
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing one of the argument description elements.", e);
                    }
                }
                reader.Close ();
            } catch (Exception e) {
                throw new UpnpDeserializationException (string.Format ("There was a problem deserializing {0}.", ToString ()), e);
            }
        }

        protected virtual void DeserializeCore (XmlReader reader, string element)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            reader.Read ();
            switch (element.ToLower ()) {
            case "name":
                Name = reader.ReadString ().Trim ();
                break;
            case "direction":
                Direction = reader.ReadString ().Trim () == "in" ? ArgumentDirection.In : ArgumentDirection.Out;
                break;
            case "retval":
                IsReturnValue = true;
                break;
            case "relatedstatevariable":
                RelatedStateVariableName = reader.ReadString ().Trim ();
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        void VerifyDeserialization ()
        {
            if (name == null) {
                throw new UpnpDeserializationException (string.Format (
                    "An argument on {0} has no name.", action));
            }
            if (name.Length == 0) {
                Log.Exception (new UpnpDeserializationException (string.Format (
                    "An argument on {0} has an empty name.", action)));
            }
            if (related_state_variable_name == null) {
                throw new UpnpDeserializationException (string.Format (
                    "{0} has no related state variable.", ToString ()));
            }
            if (direction == null) {
                Log.Exception (new UpnpDeserializationException (string.Format (
                    "{0} has no direction, defaulting to 'in'.", ToString ())));
                direction = ArgumentDirection.In;
            }
        }

        public override string ToString ()
        {
            return string.Format (@"Argument {{ {0}, {1} }}", action, name);
        }
    }
}
