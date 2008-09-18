//
// StateVariable.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp.Control
{
	public class StateVariable
	{
        protected internal StateVariable (Service service, WebHeaderCollection headers, XmlReader reader)
        {
            this.service = service;
            Deserialize (headers, reader);
        }

        private Service service;

        private bool send_events;
        public bool SendEvents {
            get { return send_events; }
        }

        private string name;
        public string Name {
            get { return name; }
        }

        private string data_type;

        private Type type;
        public Type Type {
            get { return type; }
        }

        // FIXME is string the proper type for this?
        private string default_value;
        public string DefaultValue {
            get { return default_value; }
        }

        private ReadOnlyCollection<string> allowed_values;
        public ReadOnlyCollection<string> AllowedValues {
            get { return allowed_values; }
        }

        private AllowedValueRange allowed_value_range;
        public AllowedValueRange AllowedValueRange {
            get { return allowed_value_range; }
        }

        private event EventHandler<StateVariableChangedArgs<string>> changed;
        public event EventHandler<StateVariableChangedArgs<string>> Changed {
            add {
                if (!send_events) {
                    throw new InvalidOperationException ("This state variable does not send events.");
                } else if (value == null) {
                    return;
                }
                service.RefEvents ();
                changed += value;
            }
            remove {
                if (!send_events) {
                    throw new InvalidOperationException ("This state variable does not send events.");
                } else if (value == null) {
                    return;
                }
                service.UnrefEvents ();
                changed -= value;
            }
        }

        protected internal virtual void OnChanged (string newValue)
        {
            EventHandler<StateVariableChangedArgs<string>> changed = this.changed;
            if (changed != null) {
                changed (this, new StateVariableChangedArgs<string> (newValue));
            }
        }

        private int retry = 1;
        public int Retry {
            get { return retry; }
        }

        #region Deserialization

        private void Deserialize (WebHeaderCollection headers, XmlReader reader)
        {
            Deserialize (headers);
            Deserialize (reader);
            Verify ();
        }

        protected virtual void Deserialize (WebHeaderCollection headers)
        {
        }

        protected virtual void Deserialize (XmlReader reader)
        {
            try {
                reader.Read ();
                send_events = reader["sendEvents"] != "no";
                while (Helper.ReadToNextElement (reader)) {
                    Deserialize (reader.ReadSubtree (), reader.Name);
                }
                reader.Close ();
            } catch (Exception e) {
                string message = String.IsNullOrEmpty (name)
                    ? "There was a problem deserializing a state variable."
                    : String.Format ("There was a problem deserializing the state variable {0}.", name);
                throw new UpnpDeserializationException (message, e);
            }
        }

        protected virtual void Deserialize (XmlReader reader, string element)
        {
            reader.Read ();
            switch (element) {
            case "name":
                name = reader.ReadString ();
                break;
            case "dataType":
                data_type = reader.ReadString ();
                switch (data_type) {
                case "ui1":
                    type = typeof (byte);
                    break;
                case "ui2":
                    type = typeof (ushort);
                    break;
                case "ui4":
                    type = typeof (uint);
                    break;
                case "i1":
                    type = typeof (sbyte);
                    break;
                case "i2":
                    type = typeof (short);
                    break;
                case "i4":
                    type = typeof (int);
                    break;
                case "int":
                    type = typeof (long); // Is this right? The UPnP docs are vague
                    break;
                case "r4":
                    type = typeof (float);
                    break;
                case "r8":
                case "number":
                case "fixed.14.4":
                case "float":
                    type = typeof (double);
                    break;
                case "char":
                    type = typeof (char);
                    break;
                case "string":
                    type = typeof (string);
                    break;
                // TODO handle these better
                case "date":
                case "dateTime":
                case "dateTime.tz":
                case "time":
                case "time.tz":
                    type = typeof (DateTime);
                    break;
                case "boolean":
                    type = typeof (bool);
                    break;
                case "bin.base64":
                case "bin.hex":
                    type = typeof (byte[]);
                    break;
                // TODO handle uuids with care
                case "uri":
                case "uuid":
                    type = typeof (Uri);
                    break;
                }
                break;
            case "defaultValue":
                default_value = reader.ReadString ();
                break;
            case "allowedValueList":
                DeserializeAllowedValues (reader.ReadSubtree ());
                break;
            case "allowedValueRange":
                allowed_value_range = new AllowedValueRange (reader.ReadSubtree ());
                break;
            case "sendEventsAttribute":
                send_events = reader.ReadString () != "no";
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        private void DeserializeAllowedValues (XmlReader reader)
        {
            List<string> allowed_value_list = new List<string> ();
            while (reader.ReadToFollowing ("allowedValue") && reader.NodeType == XmlNodeType.Element) {
                allowed_value_list.Add (reader.ReadString ());
            }
            allowed_values = allowed_value_list.AsReadOnly ();
            reader.Close ();
        }

        protected virtual void Verify ()
        {
            if (String.IsNullOrEmpty (name)) {
                throw new UpnpDeserializationException ("The state variable has no name.");
            }
            if (type == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The state variable {0} has no type.", name));
            }
            if (allowed_values != null && Type != typeof (string)) {
                throw new UpnpDeserializationException (String.Format (
                    "The state variable {0} has allowedValues, but is of type {1}.", name, type));
            }
            // TODO something here
            //if (allowed_value_range != null && !typeof (double).IsAssignableFrom (type)) {
            //    throw new UpnpDeserializationException (String.Format (
            //        "The state variable {0} has allowedValueRange, but is of type {2}.", name, type));
            //}
        }

        #endregion

    }
}
