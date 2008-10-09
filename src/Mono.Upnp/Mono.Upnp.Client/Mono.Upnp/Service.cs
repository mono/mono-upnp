//
// Service.cs
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

using Mono.Upnp.Control;
using Mono.Upnp.Internal;

namespace Mono.Upnp
{
	public class Service
	{
        #region Constructors

        protected internal Service (Client client, string deviceId, IEnumerable<string> locations, ServiceType type)
        {
            this.type = type;
            this.client = client;
            device_id = deviceId;
            foreach (string location in locations) {
                if (!this.locations.ContainsKey (location)) {
                    this.locations.Add (location, location);
                }
            }
        }

        protected Service (Device device, XmlReader reader)
            : this (device, reader, null)
        {
        }

        protected internal Service (Device device, XmlReader reader, WebHeaderCollection headers)
        {
            this.device = device;
            Deserialize (reader, headers);
        }

        #endregion

        #region Data

        private Client client;
        private SoapAdapter soap_adapter;
        private EventSubscriber subscriber;
        private bool device_description_loaded;
        private bool service_description_loaded;

        private readonly string device_id;
        internal string DeviceId {
            get { return device_id; }
        }

        private bool disposed;
        public bool Disposed {
            get { return disposed; }
        }

        private Dictionary<string, string> locations = new Dictionary<string, string> ();
        internal IEnumerable<string> Locations {
            get { return locations.Values; }
        }

        private Device device;
        public Device Device {
            get { return GetDeviceDescriptionField(ref device); }
        }

        private ServiceType type;
        public ServiceType Type {
            get { return type; }
            internal set { type = value; }
        }

        private string id;
        public string Id {
            get { return GetDeviceDescriptionField (ref id); }
        }

        private Dictionary<string, Action> action_dict;
        private ReadOnlyDictionary<string, Action> actions;
        public ReadOnlyDictionary<string, Action> Actions {
            get { return GetServiceDescriptionField (ref actions); }
        }

        private Dictionary<string, StateVariable> state_variable_dict;
        private ReadOnlyDictionary<string, StateVariable> state_variables;
        public ReadOnlyDictionary<string, StateVariable> StateVariables {
            get { return GetServiceDescriptionField (ref state_variables); }
        }

        private Uri description_url;
        private Uri DescriptionUrl {
            get { return GetDeviceDescriptionField (ref description_url); }
        }

        private Uri control_url;
        private Uri ControlUrl {
            get { return GetDeviceDescriptionField (ref control_url); }
        }

        private Uri event_url;
        internal Uri EventUrl {
            get { return GetDeviceDescriptionField (ref event_url); }
        }

        #endregion

        #region Methods

        #region Internal

        internal void Invoke (Action action)
        {
            if (soap_adapter == null) {
                soap_adapter = new SoapAdapter (ControlUrl);
            }
            try {
                soap_adapter.Invoke (action);
            } catch (WebException e) {
                // If we timeout will executing an action, the service may have dropped off the network.
                // We check by loading the service description again.
                if (e.Status == WebExceptionStatus.Timeout) {
                    LoadServiceDescription ();
                }
                throw e;
            }
        }

        private int events_ref_count;

        internal void RefEvents ()
        {
            if (events_ref_count == 0) {
                if (subscriber == null) {
                    subscriber = new EventSubscriber (this);
                    subscriber.Timeout += OnEventSubscriberTimedout;
                }
                subscriber.Start ();
            }
            events_ref_count++;
        }

        internal void UnrefEvents ()
        {
            events_ref_count--;
            if (events_ref_count == 0) {
                subscriber.Stop ();
            }
        }

        public event EventHandler Disposing;

        internal void Dispose ()
        {
            if (!disposed) {
                OnDisposing ();
                disposed = true;
            }
        }

        #endregion

        #region Protected

        protected virtual void OnDisposing ()
        {
            EventHandler handler = Disposing;
            if (handler != null) {
                handler (this, EventArgs.Empty);
            }
        }

        protected T GetServiceDescriptionField<T> (ref T field) where T : class
        {
            if (field == null && !service_description_loaded) {
                LoadServiceDescription ();
            }
            return field;
        }

        protected T GetDeviceDescriptionField<T> (ref T field) where T : class
        {
            if (field == null && !device_description_loaded) {
                client.LoadDeviceDescription (this);
            }
            return field;
        }

        protected void CheckDisposed ()
        {
            if (disposed) {
                throw new ObjectDisposedException (ToString (),
                    "The service has gone off the network and is no longer available.");
            }
        }

        #endregion

        #region Private

        private void InitializeCollections ()
        {
            if (action_dict == null) {
                action_dict = new Dictionary<string, Action> ();
                actions = new ReadOnlyDictionary<string, Action> (action_dict);
                state_variable_dict = new Dictionary<string, StateVariable> ();
                state_variables = new ReadOnlyDictionary<string, StateVariable> (state_variable_dict);
            }
        }

        private void LoadServiceDescription ()
        {
            try {
                InitializeCollections ();
                WebResponse response = Helper.GetResponse (DescriptionUrl);
                try {
                    if (!service_description_loaded) {
                        XmlReader reader = XmlReader.Create (response.GetResponseStream ());
                        reader.ReadToFollowing ("scpd");
                        Deserialize (reader.ReadSubtree (), response.Headers);
                        reader.Close ();
                        service_description_loaded = true;
                    }
                } finally {
                    response.Close ();
                }
            } catch (WebException e) {
                if (e.Status == WebExceptionStatus.Timeout) {
                    Dispose ();
                    CheckDevice ();
                    throw new ObjectDisposedException (
                        "The service has gone off the network and is no longer available.", e);
                } else {
                    throw e;
                }
            }
        }

        private void CheckDevice ()
        {
            if (device == null || device.Disposed || client == null) {
                return;
            }

            client.CheckDeviceDescription (device);
            foreach (Service service in device.Services) {
                if (service == this) {
                    continue;
                }
                try {
                    service.LoadServiceDescription ();
                } catch {
                }
            }
        }

        private void OnEventSubscriberTimedout (object o, EventArgs args)
        {
            // If the event subscriber hits a server timeout, the service may have fallen off the network.
            // We check this by loading the service description again.
            try {
                LoadServiceDescription ();
            } catch {
            }
        }

        #endregion

        #region Overrides

        public override string ToString ()
        {
            return String.Format ("Service {{ {0}, {1} }}", device, id);
        }

        #endregion

        #region Deserialization

        private WebHeaderCollection headers;

        private void Deserialize (XmlReader reader, WebHeaderCollection headers)
        {
            this.headers = headers;

            Deserialize (headers);
            Deserialize (reader);
            VerifyDeserialization ();

            this.headers = null;
            device_description_loaded = true;
        }

        protected virtual void Deserialize (WebHeaderCollection headers)
        {
        }

        protected virtual void Deserialize (XmlReader reader)
        {
            try {
                reader.Read ();
                while (Helper.ReadToNextElement (reader)) {
                    Deserialize (reader.ReadSubtree (), reader.Name);
                }
            } catch (Exception e) {
                string message = String.IsNullOrEmpty (id)
                    ? "There was a problem deserializing a service."
                    : String.Format ("There was a problem deserializing the service {0}.", id);
                throw new UpnpDeserializationException (message, e);
            }
        }

        protected virtual void Deserialize (XmlReader reader, string element)
        {
            reader.Read ();
            switch (element) {
            case "serviceType":
                type = new ServiceType (reader.ReadString ());
                break;
            case "serviceId":
                // TODO better handling of this complex string
                id = reader.ReadString ().Trim ();
                break;
            case "SCPDURL":
                description_url = device.Root.DeserializeUrl (reader.ReadSubtree ());
                break;
            case "controlURL":
                control_url = device.Root.DeserializeUrl (reader.ReadSubtree ());
                break;
            case "eventSubURL":
                event_url = device.Root.DeserializeUrl (reader.ReadSubtree ());
                break;
            case "actionList":
                DeserializeActions (reader.ReadSubtree ());
                break;
            case "serviceStateTable":
                DeserializeStateVariables (reader.ReadSubtree ());
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        protected virtual void DeserializeActions (XmlReader reader)
        {
            while (reader.ReadToFollowing ("action") && reader.NodeType == XmlNodeType.Element) {
                DeserializeAction (reader.ReadSubtree ());
            }
            reader.Close ();
        }

        protected virtual void DeserializeAction (XmlReader reader)
        {
            try {
                AddAction (new Action (this, reader, headers));
            } catch (UpnpDeserializationException e) {
                string message = String.IsNullOrEmpty (id)
                    ? "There was a problem deserializing one of the actions of a service."
                    : String.Format ("There was a problem deserializing one of the actions of the service {0}.", id);
                Log.Exception (message, e);
            }
        }

        protected void AddAction (Action action)
        {
            InitializeCollections ();
            action_dict.Add (action.Name, action);
        }

        protected virtual void DeserializeStateVariables (XmlReader reader)
        {
            InitializeCollections ();
            while (reader.ReadToFollowing ("stateVariable") && reader.NodeType == XmlNodeType.Element) {
                DeserializeStateVariable (reader.ReadSubtree ());
            }
            reader.Close ();
        }

        protected virtual void DeserializeStateVariable (XmlReader reader)
        {
            AddStateVariable (new StateVariable (this, reader, headers));
        }

        protected void AddStateVariable (StateVariable stateVariable)
        {
            state_variable_dict.Add (stateVariable.Name, stateVariable);
        }

        protected internal virtual void DeserializeEvents (HttpListenerRequest response)
        {
            DeserializeEvents (XmlReader.Create (response.InputStream));
        }

        protected virtual void DeserializeEvents (XmlReader reader)
        {
            while (reader.ReadToFollowing ("property", Protocol.EventUrn)) {
                if (Helper.ReadToNextElement (reader)) {
                    StateVariables[reader.Name].OnChanged (reader.ReadString ());
                }
            }
        }

        protected internal virtual Type DeserializeDataType (string dataType)
        {
            // I'd love to do a switch here, but some UPnP services define
            // custom data types that begin with a legal data type. That
            // would probably only ever happen with strings, but why risk
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
            else if (dataType.StartsWith ("boolean"))  return typeof (bool);
            else if (dataType.StartsWith ("bin")) return typeof (byte[]);
            else if (
                dataType.StartsWith ("uri") ||
                dataType.StartsWith ("uuid")) return typeof (Uri);
            else throw new UpnpDeserializationException (String.Format (
                "Could not deserialize data type {0}", dataType));
        }

        protected virtual void VerifyDeserialization ()
        {
            if (String.IsNullOrEmpty (id)) {
                throw new UpnpDeserializationException ("The service has no ID.");
            }
            if (type == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The service {0} has no service type description.", id));
            }
            if (description_url == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The service {0} has no description URL.", id));
            }
            if (control_url == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The service {0} has no control URL.", id));
            }
            if (event_url == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The service {0} has no event URL.", id));
            }
            if (actions != null) {
                foreach (Action action in actions.Values) {
                    foreach (Argument argument in action.InArguments.Values) {
                        VerifyRelatedStateVariable (argument);
                    }
                    foreach (Argument argument in action.OutArguments.Values) {
                        VerifyRelatedStateVariable (argument);
                    }
                    VerifyRelatedStateVariable (action.ReturnArgument);
                }
            }
        }

        private void VerifyRelatedStateVariable (Argument argument)
        {
            if (argument == null) {
                return;
            }
            if (argument.RelatedStateVariable == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The service {0} does not have the related state variable corrisponding to {1}.{2}.",
                    id, argument.Action.Name, argument.Name));
            }
        }

        protected internal virtual void VerifyContract ()
        {
            foreach (Action action in actions.Values) {
                action.VerifyContract ();
            }
            foreach (StateVariable state_variable in state_variables.Values) {
                state_variable.VerifyContract ();
            }
        }

        protected internal virtual void CopyFrom (Service service)
        {
            device_description_loaded = service.device_description_loaded;
            device = service.device;
            type = service.type;
            id = service.id;
            description_url = service.description_url;
            control_url = service.control_url;
            event_url = service.event_url;
        }

        #endregion

        #endregion

    }
}
