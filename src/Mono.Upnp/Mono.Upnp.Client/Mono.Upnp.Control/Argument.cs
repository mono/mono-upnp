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
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp.Control
{
	public class Argument
	{
        internal Argument (Action action, XmlReader reader)
        {
            this.action = action;
            Deserialize (reader);
        }

        private Action action;

        private string name;
        public string Name {
            get { return name; }
        }

        private ArgumentDirection? direction;
        public ArgumentDirection Direction {
            get { return direction.Value; }
        }

        private string related_state_variable_name;

        private StateVariable related_state_variable;
        public StateVariable RelatedStateVariable {
            get {
                if (related_state_variable == null) {
                    action.Service.StateVariables.TryGetValue (related_state_variable_name, out related_state_variable);
                }
                return related_state_variable;
            }
        }

        public Type Type {
            get { return RelatedStateVariable.Type; }
        }

        public ReadOnlyCollection<string> AllowedValues {
            get { return RelatedStateVariable.AllowedValues; }
        }

        public AllowedValueRange AllowedValueRange {
            get { return RelatedStateVariable.AllowedValueRange; }
        }

        private bool is_return_value;
        internal bool IsReturnValue {
            get { return is_return_value; }
        }

        private string value;
        public string Value {
            get { return value; }
            set {
                if (value == null) {
                    throw new ArgumentNullException ("value");
                }
                if (AllowedValues != null && !AllowedValues.Contains(value)) {
                    throw new ArgumentException ("The value is not among the allowed values", "value");
                }
                // TODO this
                //if (AllowedValueRange != null) {
                //    double v = (double)value;
                //    if (v < AllowedValueRange.Minimum || v > AllowedValueRange.Maximum) {
                //        throw new ArgumentOutOfRangeException ("value");
                //    }
                //}
                this.value = value;
            }
        }

        public override string ToString ()
        {
            return String.Format ("{0} {{{1}}}", name, value);
        }

        #region Deserialization

        private void Deserialize (XmlReader reader)
        {
            try {
                reader.Read ();
                while (Helper.ReadToNextElement (reader)) {
                    Deserialize (reader.ReadSubtree (), reader.Name);
                }
                reader.Close ();
            } catch (Exception e) {
                string message = String.IsNullOrEmpty (name)
                    ? "There was a problem deserializing an argument."
                    : String.Format ("There was a problem deserializing the argument {0}.", name);
                throw new UpnpDeserializationException (message, e);
            }
            Verify ();
        }

        private void Deserialize (XmlReader reader, string element)
        {
            reader.Read ();
            switch (element) {
            case "name":
                name = reader.ReadString ().Trim ();
                break;
            case "direction":
                direction = reader.ReadString ().Trim () == "in" ? ArgumentDirection.In : ArgumentDirection.Out;
                break;
            case "retval":
                is_return_value = true;
                break;
            case "relatedStateVariable":
                related_state_variable_name = reader.ReadString ().Trim ();
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        private void Verify ()
        {
            if (String.IsNullOrEmpty (name)) {
                throw new UpnpDeserializationException ("The argument has no name.");
            }
            if (direction == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The argument {0} has no direction.", name));
            }
            if (related_state_variable_name == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The argument {0} has no related state variable.", name));
            }
        }

        #endregion

    }
}
