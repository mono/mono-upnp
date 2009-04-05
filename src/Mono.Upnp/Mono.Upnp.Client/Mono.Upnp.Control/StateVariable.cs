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
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp.Control
{
	public class StateVariable
    {
        readonly ServiceController controller;
        event EventHandler<StateVariableChangedArgs<string>> changed;
		bool verified;

        protected internal StateVariable (ServiceController service)
        {
            if (service == null) throw new ArgumentNullException ("service");

            this.controller = service;
        }

        public ServiceController Controller {
            get { return controller; }
        }
        
        public string Name { get; private set; }

        public string DataType { get; private set; }

        public Type Type { get; private set; }
        
        public bool SendsEvents { get; private set; }

        public string DefaultValue { get; private set; }

        public ReadOnlyCollection<string> AllowedValues { get; private set; }

        public AllowedValueRange AllowedValueRange { get; private set; }

        public bool IsDisposed {
            get { return controller.IsDisposed; }
        }

        public event EventHandler<StateVariableChangedArgs<string>> Changed {
            add {
                CheckDisposed ();
                if (!SendsEvents) {
                    throw new InvalidOperationException ("This state variable does not send events.");
                } else if (value == null) {
                    return;
                }
                controller.RefEvents ();
                changed += value;
            }
            remove {
                CheckDisposed ();
                if (!SendsEvents) {
                    throw new InvalidOperationException ("This state variable does not send events.");
                } else if (value == null) {
                    return;
                }
                controller.UnrefEvents ();
                changed -= value;
            }
        }
        
        internal void OnChanged (string newValue)
        {
            OnChanged (new StateVariableChangedArgs<string> (newValue));
        }

        protected virtual void OnChanged (StateVariableChangedArgs<string> e)
        {
            var changed = this.changed;
            if (changed != null) {
                changed (this, e);
            }
        }

        protected void CheckDisposed ()
        {
            if (IsDisposed) {
                throw new ObjectDisposedException (ToString (),
                    "This state variable is no longer available because its service has gone off the network.");
            }
        }

        public void Deserialize (XmlReader reader)
        {
            DeserializeRootElement (reader);
            Verify ();
        }

        protected virtual void DeserializeRootElement (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                SendsEvents = reader["sendEvents"] != "no";
                while (Helper.ReadToNextElement (reader)) {
					var property_reader = reader.ReadSubtree ();
					property_reader.Read ();
                    try {
                        DeserializePropertyElement (property_reader);
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing one of the state variable description elements.", e);
                    } finally {
						property_reader.Close ();
					}
                }
            } catch (Exception e) {
                throw new UpnpDeserializationException (string.Format ("There was a problem deserializing {0}.", ToString ()), e);
            }
        }

        protected virtual void DeserializePropertyElement (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            switch (reader.Name) {
            case "name":
                Name = reader.ReadString ().Trim ();
                break;
            case "dataType":
                DataType = reader.ReadString ().Trim ();
                Type = controller.DeserializeDataType (DataType);
                break;
            case "defaultValue":
                DefaultValue = reader.ReadString ();
                break;
            case "allowedValueList":
                DeserializeAllowedValues (reader);
                break;
            case "allowedValueRange":
                AllowedValueRange = new AllowedValueRange (Type, reader.ReadSubtree ());
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
        }

        void DeserializeAllowedValues (XmlReader reader)
        {
            var allowed_value_list = new List<string> ();
            while (reader.ReadToFollowing ("allowedValue") && reader.NodeType == XmlNodeType.Element) {
                allowed_value_list.Add (reader.ReadString ());
            }
            AllowedValues = allowed_value_list.AsReadOnly ();
        }
		
		void Verify ()
		{
			VerifyDeserialization ();
			if (!verified) {
				throw new UpnpDeserializationException (
					"The deserialization has not been fully verified. Be sure to call base.VerifyDeserialization ().");
			}
		}

        protected virtual void VerifyDeserialization ()
        {
            if (Name == null) {
                throw new UpnpDeserializationException (
                    string.Format ("A state variable on {0} has no name.", controller));
            }
            if (Name.Length == 0) {
                Log.Exception (new UpnpDeserializationException (
                    string.Format ("A state variable on {0} has an empty name.", controller)));
            }
            if (DataType == null) {
                throw new UpnpDeserializationException (
                    string.Format ("{0} has no type.", ToString ()));
            }
            if (Type == null) {
                Log.Exception (new UpnpDeserializationException (
                    string.Format ("Unable to deserialize data type {0}.", DataType)));
            }
            if (AllowedValues != null && Type != typeof (string)) {
                Log.Exception (new UpnpDeserializationException (
                    string.Format ("{0} has allowedValues, but is of type {1}.", ToString (), Type)));
            }
            if (AllowedValueRange != null && !(Type is IComparable)) {
                Log.Exception (new UpnpDeserializationException (
                    string.Format ("{0} has allowedValueRange, but is of type {1}.", ToString (), Type)));
            }
            // TODO something here
            //if (allowed_value_range != null && !typeof (double).IsAssignableFrom (type)) {
            //    throw new UpnpDeserializationException (String.Format (
            //        "The state variable {0} has allowedValueRange, but is of type {2}.", name, type));
            //}
			verified = true;
        }

        public override string ToString ()
        {
            return String.Format ("StateVariable {{ {0} ({1}) }}", Name, DataType);
        }
    }
}
