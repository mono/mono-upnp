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
        readonly ServiceController controller;
        bool send_events;
        string name;
        string data_type;
        Type type;
        string default_value; // FIXME is string the proper type for this?
        ReadOnlyCollection<string> allowed_values;
        AllowedValueRange allowed_value_range;
        bool loaded;
        event EventHandler<StateVariableChangedArgs<string>> changed;

        protected internal StateVariable (ServiceController service)
        {
            if (service == null) throw new ArgumentNullException ("service");

            this.controller = service;
        }

        public ServiceController Controller {
            get { return controller; }
        }

        public bool SendEvents {
            get { return send_events; }
            protected set { SetField (ref send_events, value); }
        }

        public string Name {
            get { return name; }
            protected set { SetField (ref name, value); }
        }

        public string DataType {
            get { return data_type; }
            set {
                CheckLoaded ();
                data_type = value;
                type = controller.DeserializeDataType (value);
            }
        }

        public Type Type {
            get { return type; }
        }

        public string DefaultValue {
            get { return default_value; }
            protected set { SetField (ref default_value, value); }
        }

        public ReadOnlyCollection<string> AllowedValues {
            get { return allowed_values; }
            protected set { SetField (ref allowed_values, value); }
        }

        public AllowedValueRange AllowedValueRange {
            get { return allowed_value_range; }
            protected set { SetField (ref allowed_value_range, value); }
        }

        public bool IsDisposed {
            get { return controller.IsDisposed; }
        }

        public event EventHandler<StateVariableChangedArgs<string>> Changed {
            add {
                CheckDisposed ();
                if (!send_events) {
                    throw new InvalidOperationException ("This state variable does not send events.");
                } else if (value == null) {
                    return;
                }
                controller.RefEvents ();
                changed += value;
            }
            remove {
                CheckDisposed ();
                if (!send_events) {
                    throw new InvalidOperationException ("This state variable does not send events.");
                } else if (value == null) {
                    return;
                }
                controller.UnrefEvents ();
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

        protected void CheckDisposed ()
        {
            if (IsDisposed) {
                throw new ObjectDisposedException (ToString (),
                    "This state variable is no longer available because its service has gone off the network.");
            }
        }

        void CheckLoaded ()
        {
            if (loaded) {
                throw new InvalidOperationException ("The state variable has already been deserialized.");
            }
        }

        private void SetField<T> (ref T field, T value)
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
                reader.ReadToFollowing ("stateVariable");
                send_events = reader["sendEvents"] != "no";
                while (Helper.ReadToNextElement (reader)) {
                    try {
                        DeserializeCore (reader.ReadSubtree (), reader.Name);
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing one of the state variable description elements.", e);
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
            case "datatype":
                DataType = reader.ReadString ().Trim ();
                break;
            case "defaultvalue":
                DefaultValue = reader.ReadString ();
                break;
            case "allowedvaluelist":
                DeserializeAllowedValues (reader.ReadSubtree ());
                break;
            case "allowedvaluerange":
                AllowedValueRange = new AllowedValueRange (Type, reader.ReadSubtree ());
                break;
            case "sendeventsattribute":
                SendEvents = reader.ReadString ().Trim () != "no";
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        void DeserializeAllowedValues (XmlReader reader)
        {
            List<string> allowed_value_list = new List<string> ();
            while (reader.ReadToFollowing ("allowedValue") && reader.NodeType == XmlNodeType.Element) {
                allowed_value_list.Add (reader.ReadString ());
            }
            allowed_values = allowed_value_list.AsReadOnly ();
            reader.Close ();
        }

        void VerifyDeserialization ()
        {
            if (name == null) {
                throw new UpnpDeserializationException (string.Format ("A state variable on {0} has no name.", controller));
            }
            if (name.Length == 0) {
                Log.Exception (new UpnpDeserializationException (string.Format ("A state variable on {0} has an empty name.", controller)));
            }
            if (data_type == null) {
                throw new UpnpDeserializationException (string.Format ("{0} has no type.", ToString ()));
            }
            if (type == null) {
                Log.Exception (new UpnpDeserializationException (string.Format ("Unable to deserialize data type {0}.", data_type)));
            }
            if (allowed_values != null && type != typeof (string)) {
                Log.Exception (new UpnpDeserializationException (string.Format (
                    "{0} has allowedValues, but is of type {1}.", ToString (), type)));
            }
            if (allowed_value_range != null && !(type is IComparable)) {
                Log.Exception (new UpnpDeserializationException (string.Format (
                    "{0} has allowedValueRange, but is of type {1}.", ToString (), type)));
            }
            // TODO something here
            //if (allowed_value_range != null && !typeof (double).IsAssignableFrom (type)) {
            //    throw new UpnpDeserializationException (String.Format (
            //        "The state variable {0} has allowedValueRange, but is of type {2}.", name, type));
            //}
        }

        public override string ToString ()
        {
            return String.Format ("StateVariable {{ {0}, {1} ({2}) }}", controller, name, data_type);
        }
    }
}
