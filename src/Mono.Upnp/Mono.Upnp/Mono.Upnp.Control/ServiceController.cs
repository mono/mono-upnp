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
    [XmlType ("scdp", Protocol.ServiceUrn)]
    public class ServiceController : Description, IXmlDeserializable
    {
        readonly Map<string, ServiceAction> actions;
        readonly Map<string, StateVariable> state_variables;
        readonly Service service;
        readonly object service_object;
        DataServer scpd_server;
        ControlServer control_server;
        ControlClient control_client;
        EventServer event_server;
        EventClient event_client;
        int events_ref_count;

        protected internal ServiceController (Deserializer deserializer, Service service)
            : base (deserializer)
        {
            actions = new Map<string, ServiceAction> (ServiceActionMapper);
            state_variables = new Map<string, StateVariable> (StateVariableMapper);
        }
        
        protected internal ServiceController (object service, IEnumerable<ServiceAction> actions, IEnumerable<StateVariable> stateVariables)
        {
            if (service == null) throw new ArgumentNullException ("service");
            
            this.actions = Helper.MakeReadOnlyCopy<string, ServiceAction> (actions, ServiceActionMapper);
            this.state_variables = Helper.MakeReadOnlyCopy<string, StateVariable> (state_variables, StateVariableMapper);
            SpecVersion = new SpecVersion (1, 1);
        }
        
        static string ServiceActionMapper (ServiceAction serviceAction)
        {
            return serviceAction.Name;
        }
        
        static string StateVariableMapper (StateVariable stateVariable)
        {
            return stateVariable.Name;
        }
        
        internal Service Service {
            get { return service; }
        }
        
        [XmlAttribute ("configId")]
        protected internal virtual string ConfigurationId { get; set; }
        
        // FIXME this doesn't get deserialize because the namespaces differ. Yeah UPnP spec!
        [XmlElement ("specVersion", Protocol.ServiceUrn)]
        public virtual SpecVersion SpecVersion { get; protected set; }
        
        [XmlArray ("actionList", Protocol.ServiceUrn)]
        protected virtual ICollection<ServiceAction> ActionCollection {
            get { return actions; }
        }
        
        public IMap<string, ServiceAction> Actions {
            get { return actions; }
        }
        
        [XmlArray ("serviceStateTable", Protocol.ServiceUrn)]
        protected ICollection<StateVariable> StateVariablesCollection {
            get { return state_variables; }
        }
        
        public IMap<string, StateVariable> StateVariables {
            get { return state_variables; }
        }
        
        protected internal virtual void Initialize (Server server, Service service)
        {
            if (service == null) throw new ArgumentNullException ("service");
            
            control_server = new ControlServer (this, service.ControlUrl);
            event_server = new EventServer (this, service.EventUrl);
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
    }
}
