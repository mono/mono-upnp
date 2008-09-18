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

        protected internal Service (Client client, IEnumerable<string> locations, ServiceType type)
        {
            this.type = type;
            this.client = client;
            foreach (string location in locations) {
                if (!this.locations.ContainsKey (location)) {
                    this.locations.Add (location, location);
                }
            }
        }

        protected internal Service (Device device, WebHeaderCollection headers, XmlReader reader)
        {
            this.device = device;
            Deserialize (headers, reader);
        }

        #endregion

        #region Data

        private Client client;
        private bool device_description_loaded;
        private EventSubscriber subscriber;

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

        private SoapAdapter soap_adapter;
        internal SoapAdapter SoapAdapter {
            get {
                if (soap_adapter == null) {
                    soap_adapter = new SoapAdapter (ControlUrl);
                }
                return soap_adapter;
            }
        }

        protected T GetServiceDescriptionField<T> (ref T field) where T : class
        {
            if (field == null) {
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

        private void LoadServiceDescription ()
        {
            WebResponse response = Helper.GetResponse (DescriptionUrl);
            XmlReader reader = XmlReader.Create (response.GetResponseStream ());
            reader.ReadToFollowing ("scpd");
            Deserialize (response.Headers, reader.ReadSubtree ());
            reader.Close ();
            response.Close ();
        }

        private int events_ref_count;

        internal void RefEvents ()
        {
            if (events_ref_count == 0) {
                if (subscriber == null) {
                    subscriber = new EventSubscriber (this);
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

        #endregion

        #region Equality

        public override bool Equals (object obj)
        {
            Service service = obj as Service;
            return service != null && this == service;
        }

        public override int GetHashCode ()
        {
            return device.GetHashCode() ^ id.GetHashCode ();
        }

        public static bool operator == (Service service1, Service service2)
        {
            if (Object.ReferenceEquals (service1, null) && Object.ReferenceEquals (service2, null)) {
                return true;
            } else if (Object.ReferenceEquals (service1, null) || Object.ReferenceEquals (service2, null)) {
                return false;
            }
            return service1.device == service2.device && service1.Id == service2.Id;
        }

        public static bool operator != (Service service1, Service service2)
        {
            if (Object.ReferenceEquals (service1, null) && Object.ReferenceEquals (service2, null)) {
                return false;
            } else if (Object.ReferenceEquals (service1, null) || Object.ReferenceEquals (service2, null)) {
                return true;
            }
            return service1.device != service2.device || service1.Id != service2.Id;
        }

        #endregion

        #region Deserialization

        private WebHeaderCollection headers;

        private void Deserialize (WebHeaderCollection headers, XmlReader reader)
        {
            this.headers = headers;
            action_dict = new Dictionary<string, Action> ();
            actions = new ReadOnlyDictionary<string, Action> (action_dict);
            state_variable_dict = new Dictionary<string, StateVariable> ();
            state_variables = new ReadOnlyDictionary<string, StateVariable> (state_variable_dict);
            
            Deserialize (headers);
            Deserialize (reader);
            Verify ();

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
                id = reader.ReadString ();
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
                DeserializeServiceStateTable (reader.ReadSubtree ());
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        private void DeserializeActions (XmlReader reader)
        {
            while (reader.ReadToFollowing ("action") && reader.NodeType == XmlNodeType.Element) {
                // TODO die under strict conditions
                try {
                    Action action = new Action (this, headers, reader.ReadSubtree ());
                    action_dict.Add (action.Name, action);
                } catch (UpnpDeserializationException e) {
                    string message = String.IsNullOrEmpty (id)
                        ? "There was a problem deserializing one of the actions of a service."
                        : String.Format ("There was a problem deserializing one of the actions of the service {0}.", id);
                    Log.Exception (message, e);
                }
            }
            reader.Close ();
        }

        private void DeserializeServiceStateTable (XmlReader reader)
        {
            while (reader.ReadToFollowing ("stateVariable") && reader.NodeType == XmlNodeType.Element) {
                StateVariable variable = new StateVariable (this, headers, reader.ReadSubtree ());
                state_variable_dict.Add (variable.Name, variable);
            }
            reader.Close ();
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

        protected virtual void Verify ()
        {
            if (String.IsNullOrEmpty (id)) {
                throw new UpnpDeserializationException ("The service has no ID.");
            }
            if (type == null) {
                throw new UpnpDeserializationException (String.Format (
                    "The service {0} has no service type description."));
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

    }
}
