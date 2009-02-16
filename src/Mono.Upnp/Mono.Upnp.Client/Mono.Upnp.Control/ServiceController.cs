//
// ServiceController.cs
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
using System.Net;
using System.Xml;

using Mono.Upnp.Description;
using Mono.Upnp.Internal;

namespace Mono.Upnp.Control
{
	public class ServiceController
	{
        readonly ServiceDescription service_description;
        readonly Dictionary<string, ServiceAction> action_dict = new Dictionary<string, ServiceAction> ();
        readonly ReadOnlyDictionary<string, ServiceAction> actions;
        readonly Dictionary<string, StateVariable> state_variable_dict = new Dictionary<string, StateVariable> ();
        readonly ReadOnlyDictionary<string, StateVariable> state_variables;
        SoapInvoker soap_adapter;
        EventSubscriber event_subscriber;
        int events_ref_count;
		bool verified;

        protected internal ServiceController (ServiceDescription service)
        {
            if (service == null) throw new ArgumentNullException ("service");

            actions = new ReadOnlyDictionary<string, ServiceAction> (action_dict);
            state_variables = new ReadOnlyDictionary<string, StateVariable> (state_variable_dict);

            this.service_description = service;
        }
        
        public event EventHandler<DisposedEventArgs> Disposed;
        
        public bool IsDisposed { get; private set; }

        public ServiceDescription Description {
            get { return service_description; }
        }

        public ReadOnlyDictionary<string, ServiceAction> Actions {
            get { return actions; }
        }

        public ReadOnlyDictionary<string, StateVariable> StateVariables {
            get { return state_variables; }
        }

        protected void AddAction (ServiceAction action)
        {
            if (action == null) throw new ArgumentNullException ("action");
            action_dict [action.Name] = action;
        }

        protected void AddStateVariable (StateVariable stateVariable)
        {
            if (stateVariable == null) throw new ArgumentNullException ("stateVariable");
            state_variable_dict [stateVariable.Name] = stateVariable;
        }

        protected internal void Dispose ()
        {
            if (IsDisposed) {
                return;
            }

			event_subscriber.Dispose ();
            IsDisposed = true;
            OnDisposed (DisposedEventArgs.Empty);
        }

        protected virtual void OnDisposed (DisposedEventArgs e)
        {
            var handler = Disposed;
            if (handler != null) {
                handler (this, e);
            }
        }

        internal ActionResult Invoke (ServiceAction action, IDictionary<string, string> arguments, int retry)
        {
            if (soap_adapter == null) {
                if (service_description.ControlUrl == null) {
                    throw new InvalidOperationException (
                        string.Format ("{0} does not have a control URL.", service_description));
                }
                soap_adapter = new SoapInvoker (service_description.ControlUrl);
            }
            try {
                return soap_adapter.Invoke (action, arguments, retry);
            } catch (WebException e) {
                if (e.Status == WebExceptionStatus.Timeout) {
                    service_description.CheckDisposed ();
                }
                throw e;
            }
        }

        internal void RefEvents ()
        {
            if (events_ref_count == 0) {
                if (event_subscriber == null) {
                    event_subscriber = new EventSubscriber (this);
                }
                if (service_description.EventUrl == null) {
                    // We log this because it's a no-no to throw in event registration
                    Log.Exception (new InvalidOperationException (
                        "Attempting to subscribe to events for a service with no event URL."));
                } else {
                    event_subscriber.Start ();
                }
            }
            events_ref_count++;
        }

        internal void UnrefEvents ()
        {
            events_ref_count--;
            if (events_ref_count == 0) {
                event_subscriber.Stop ();
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
                while (Helper.ReadToNextElement (reader)) {
					var property_reader = reader.ReadSubtree ();
					property_reader.Read ();
                    try {
                        DeserializePropertyElement (property_reader);
                    } catch (Exception e) {
                        Log.Exception (
                            "There was a problem deserializing one of the controller description elements.", e);
                    } finally {
						property_reader.Close ();
					}
                }
            } catch (Exception e) {
                throw new UpnpDeserializationException (
                    string.Format ("There was a problem deserializing {0}.", ToString ()), e);
            }
        }

        protected virtual void DeserializePropertyElement (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            switch (reader.Name) {
            case "actionList":
				using (var actions_reader = reader.ReadSubtree ()) {
					actions_reader.Read ();
                	DeserializeActions (actions_reader);
				}
                break;
            case "serviceStateTable":
				using (var state_variables_reader = reader.ReadSubtree ()) {
					state_variables_reader.Read ();
                	DeserializeStateVariables (state_variables_reader);
				}
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
        }

        protected virtual void DeserializeActions (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            while (reader.ReadToFollowing ("action")) {
				var action_reader = reader.ReadSubtree ();
				action_reader.Read ();
                try {
                    DeserializeAction (action_reader);
                } catch (Exception e) {
                    Log.Exception ("There was a problem deserializing an action list element.", e);
                } finally {
					action_reader.Close ();
				}
            }
        }

        protected virtual void DeserializeAction (XmlReader reader)
        {
            var action = CreateAction ();
            if (action != null) {
                action.Deserialize (reader);
                AddAction (action);
            }
        }

        protected virtual ServiceAction CreateAction ()
        {
            return new ServiceAction (this);
        }

        protected virtual void DeserializeStateVariables (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            while (reader.ReadToFollowing ("stateVariable")) {
				var state_variable_reader = reader.ReadSubtree ();
				state_variable_reader.Read ();
                try {
                    DeserializeStateVariable (state_variable_reader);
                } catch (Exception e) {
                    Log.Exception ("There was a problem deserializing a state variable list element.", e);
                } finally {
					state_variable_reader.Close ();
				}
            }
        }

        protected virtual void DeserializeStateVariable (XmlReader reader)
        {
            var state_variable = CreateStateVariable ();
            if (state_variable != null) {
                state_variable.Deserialize (reader);
                AddStateVariable (state_variable);
            }
        }

        protected virtual StateVariable CreateStateVariable ()
        {
            return new StateVariable (this);
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
            foreach (var action in actions.Values) {
                foreach (var argument in action.InArguments.Values) {
                    VerifyRelatedStateVariable (argument);
                }
                foreach (var argument in action.OutArguments.Values) {
                    VerifyRelatedStateVariable (argument);
                }
				if (action.ReturnArgument != null) {
                	VerifyRelatedStateVariable (action.ReturnArgument);
				}
            }
			verified = true;
        }

        void VerifyRelatedStateVariable (Argument argument)
        {
            if (argument.RelatedStateVariable == null) {
                Log.Exception (new UpnpDeserializationException (
                    string.Format ("{0} does not have the related state variable.", argument)));
            }
        }

        internal void DeserializeEvents (HttpListenerRequest response)
        {
            DeserializeEventsCore (response);
        }
        
        protected virtual void DeserializeEventsCore (HttpListenerRequest response)
        {
            DeserializeEventsCore (XmlReader.Create (response.InputStream));
        }

        protected virtual void DeserializeEventsCore (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");
            
            using (reader) {
                while (reader.ReadToFollowing ("property", Protocol.EventUrn)) {
                    if (Helper.ReadToNextElement (reader) && StateVariables.ContainsKey (reader.Name)) {
                        StateVariables[reader.Name].OnChanged (reader.ReadString ());
                    }
                }
            }
        }

        protected internal virtual Type DeserializeDataType (string dataType)
        {
            // I'd love to do a switch here, but some UPnP services define
            // custom data types that begin with a legal data type. That
            // would probably only ever happen with string, but why risk
            // the exception.

            if (dataType.StartsWith ("ui1")) return typeof (byte);
            else if (dataType.StartsWith ("ui2")) return typeof (ushort);
            else if (dataType.StartsWith ("ui4")) return typeof (uint);
            else if (dataType.StartsWith ("i1")) return typeof (sbyte);
            else if (dataType.StartsWith ("i2")) return typeof (short);
            else if (dataType.StartsWith ("i4")) return typeof (int);
            else if (dataType.StartsWith ("int")) return typeof (long); // Is this right? The UPnP docs are vague
            else if (dataType.StartsWith ("r4")) return typeof (float);
            else if (
                dataType.StartsWith ("r8") ||
                dataType.StartsWith ("number") ||
                dataType.StartsWith ("fixed.14.4") ||
                dataType.StartsWith ("float")) return typeof (double);
            else if (dataType.StartsWith ("char")) return typeof (char);
            else if (dataType.StartsWith ("string")) return typeof (string);
            else if ( // TODO handle all of the variations
                dataType.StartsWith ("date") ||
                dataType.StartsWith ("time")) return typeof (DateTime);
            else if (dataType.StartsWith ("boolean")) return typeof (bool);
            else if (dataType.StartsWith ("bin")) return typeof (byte[]);
            else if (
                dataType.StartsWith ("uri") ||
                dataType.StartsWith ("uuid")) return typeof (Uri);
            else return null;
        }

        public override string ToString ()
        {
            return string.Format ("ServiceController {{ {0} }}", service_description);
        }
	}
}
