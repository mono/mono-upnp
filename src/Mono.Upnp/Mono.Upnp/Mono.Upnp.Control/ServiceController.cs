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

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Control
{
    [XmlType ("scpd", Protocol.ServiceSchema)]
    public class ServiceController : Description, IXmlDeserializable
    {
        readonly CollectionMap<string, ServiceAction> actions;
        readonly CollectionMap<string, StateVariable> state_variables;
        DataServer scpd_server;
        ControlServer control_server;
        ControlClient control_client;
        EventServer event_server;
        EventClient event_client;

        protected internal ServiceController (Deserializer deserializer, Service service)
            : base (deserializer)
        {
            if (service == null) throw new ArgumentNullException ("service");
            if (service.ControlUrl == null) throw new ArgumentException ("The service has no ControlUrl.", "service");
            if (service.EventUrl == null) throw new ArgumentException ("The service has no EventUrl.", "service");
            
            actions = new CollectionMap<string, ServiceAction> ();
            state_variables = new CollectionMap<string, StateVariable> ();
            control_client = new ControlClient (service.Type.ToString (), service.ControlUrl, deserializer.XmlDeserializer);
            event_client = new EventClient (state_variables, service.EventUrl);
        }
        
        public ServiceController (IEnumerable<ServiceAction> actions, IEnumerable<StateVariable> stateVariables)
        {
            this.actions = Helper.MakeReadOnlyCopy<string, ServiceAction> (actions);
            this.state_variables = Helper.MakeReadOnlyCopy<string, StateVariable> (stateVariables);
            SpecVersion = new SpecVersion (1, 1);
        }
        
        [XmlAttribute ("configId")]
        protected internal virtual string ConfigurationId { get; set; }
        
        [XmlElement ("specVersion")]
        public virtual SpecVersion SpecVersion { get; protected set; }
        
        [XmlArray ("actionList")]
        protected virtual ICollection<ServiceAction> ActionCollection {
            get { return actions; }
        }
        
        public IMap<string, ServiceAction> Actions {
            get { return actions; }
        }
        
        [XmlArray ("serviceStateTable")]
        protected ICollection<StateVariable> StateVariablesCollection {
            get { return state_variables; }
        }
        
        public IMap<string, StateVariable> StateVariables {
            get { return state_variables; }
        }
        
        protected internal virtual void Initialize (XmlSerializer serializer, Service service)
        {
            if (serializer == null) throw new ArgumentNullException ("serializer");
            if (service == null) throw new ArgumentNullException ("service");
            if (service.ScpdUrl == null) throw new ArgumentException ("The service has no ScpdUrl.", "service");
            if (service.ControlUrl == null) throw new ArgumentException ("The service has no ControlUrl.", "service");
            if (service.EventUrl == null) throw new ArgumentException ("The service has no EventUrl.", "service");
            
            scpd_server = new DataServer (serializer.GetBytes (this), service.ScpdUrl);
            control_server = new ControlServer (Actions, service.Type.ToString (), service.ControlUrl, serializer);
            event_server = new EventServer (StateVariables.Values, service.EventUrl);
        }
        
        protected internal virtual void Start ()
        {
            if (scpd_server == null) throw new InvalidOperationException ("The service controller has not been initialized.");
            
            scpd_server.Start ();
            control_server.Start ();
            event_server.Start ();
        }
        
        protected internal virtual void Stop ()
        {
            if (scpd_server == null) throw new InvalidOperationException ("The service controller has not been initialized.");
            
            scpd_server.Stop ();
            control_server.Stop ();
            event_server.Stop ();
        }
        
        protected internal virtual IMap<string, string> Invoke (ServiceAction action, IDictionary<string, string> arguments, int retryAttempts)
        {
            // TODO try dispose on timeout
            // TODO retry attempts
            if (control_client == null) throw new InvalidOperationException ("The service controller was created to describe a local service and cannot be invoked across the network. Use the constructor which takes a Deserializer.");
            
            return control_client.Invoke (action.Name, arguments);
        }
        
        internal void RefEvents ()
        {
            event_client.Ref ();
        }
        
        internal void UnrefEvents ()
        {
            event_client.Unref ();
        }
        
        protected internal virtual void UpdateStateVariable (StateVariable stateVariable)
        {
            if (stateVariable == null) throw new ArgumentNullException ("stateVariable");
            if (!state_variables.ContainsKey (stateVariable.Name)) throw new ArgumentException ("The state variable does not belong to this service controller.", "stateVariable");
            
            if (event_server != null) {
                event_server.QueueUpdate (stateVariable);
            }
        }
        
        [XmlTypeDeserializer]
        protected virtual ServiceAction DeserializeAction (XmlDeserializationContext context)
        {
            return Deserializer != null ? Deserializer.DeserializeAction (this, context) : null;
        }
        
        [XmlTypeDeserializer]
        protected virtual StateVariable DeserializeStateVariable (XmlDeserializationContext context)
        {
            return Deserializer != null ? Deserializer.DeserializeStateVariable (this, context) : null;
        }
        
        void IXmlDeserializable.Deserialize (XmlDeserializationContext context)
        {
            Deserialize (context);
            actions.MakeReadOnly ();
            state_variables.MakeReadOnly ();
        }
        
        void IXmlDeserializable.DeserializeAttribute (XmlDeserializationContext context)
        {
            DeserializeAttribute (context);
        }
        
        void IXmlDeserializable.DeserializeElement (XmlDeserializationContext context)
        {
            DeserializeElement (context);
        }
        
        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoDeserializeElement (this);
        }
        
        protected override void SerializeSelfAndMembers (Mono.Upnp.Xml.XmlSerializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoSerializeObjectAndMembers (this);
        }
        
        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoSerializeMembersOnly (this);
        }
    }
}
