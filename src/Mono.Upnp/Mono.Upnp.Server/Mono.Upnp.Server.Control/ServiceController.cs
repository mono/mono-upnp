// 
// ServiceController.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Peterson
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Reflection;

using Mono.Upnp.Xml;
using Mono.Upnp.Server.Internal;

namespace Mono.Upnp.Server.Control
{
    [XmlType ("scdp", Protocol.ServiceUdn)]
    public abstract class ServiceController : XmlSerializable
    {
        static readonly MethodInfo on_event = typeof (ServiceController).
            GetMethod ("OnEvent", BindingFlags.Instance | BindingFlags.NonPublic);
        
        readonly XmlSerializer serializer;
        
        protected ServiceController (Server server)
        {
            if (server == null) throw new ArgumentNullException ("server");
            
            serializer = server.Serializer;
        }
        
        protected abstract IDictionary<string, ServiceAction> ServiceActions { get; }
        protected abstract IDictionary<string, RelatedStateVariable> RelatedStateVariables { get; }
        protected abstract IDictionary<string, EventedStateVariable> EventedStateVariables { get; }
        
        [XmlArray ("actionList")]
        public virtual IEnumerable<ServiceAction> Actions {
            get { return ServiceActions.Values; }
        }
        
        [XmlArray ("serviceStateTable")]
        public virtual IEnumerable<StateVariable> StateVariables {
            get {
                foreach (var state_variable in RelatedStateVariables.Values) {
                    yield return state_variable;
                }
                foreach (var state_variable in EventedStateVariables.Values) {
                    yield return state_variable;
                }
            }
        }
        
        protected virtual void ProcessActions (Type type)
        {
            if (type == null) throw new ArgumentNullException ("type");
            
            foreach (MethodInfo method in type.GetMethods ()) {
                ProcessAction (method);
            }
        }

        protected virtual void ProcessAction (MethodInfo methodInfo)
        {
            if (method == null) throw new ArgumentNullException ("methodInfo");
            
            string name = null;
            foreach (var custom_attribute in methodInfo.GetCustomAttributes (true)) {
                var action_attribute = custom_attribute as UpnpActionAttribute;
                if (action_attribute != null) {
                    name = string.IsNullOrEmpty (action_attribute.Name) ? method.Name : action_attribute.Name;
                    break;
                }
            }
            if (name == null) {
                return;
            }

            AddServiceAction (CreateServiceAction (name, methodInfo));
        }
        
        protected virtual ServiceAction CreateAction (string name, MethodInfo methodInfo)
        {
            var action = new ReflectedServiceAction (name);
            action.Initialize (this, methodInfo);
            return action;
        }
        
        protected virtual void AddServiceAction (ServiceAction serviceAction)
        {
            if (serviceAction != null) {
                if (ServiceActions.ContainsKey (serviceAction.Name))
                    throw new UpnpServerException (string.Format ("The service already contains an action named '{0}'.", action.Name));
                
                ServiceActions.Add (serviceAction.Name, serviceAction);
            }
        }

        protected virtual void ProcessEventedStateVariables (Type type)
        {
            if (type == null) throw new ArgumentNullException ("type");
            
            foreach (var @event in type.GetEvents ()) {
                ProcessEventedStateVariable (@event);
            }
            
            foreach (var @event in GetType ().GetEvents (BindingFlags.Instance | BindingFlags.NonPublic)) {
                foreach (var custom_attribute in @event.GetCustomAttributes (true)) {
                    if (custom_attribute is UpnpStateVariableAttribute) {
                        throw new UpnpServerException ("State variable events must be public.");
                    }
                }
            }
        }

        protected virtual void ProcessEventedStateVariable (EventInfo eventInfo)
        {
            if (eventInfo == null) throw new ArgumentNullException ("eventInfo");
            
            string name = null;
            foreach (var custom_attribute in eventInfo.GetCustomAttributes (true)) {
                var state_variable_attribute = custom_attribute as UpnpStateVariableAttribute;
                if (state_variable_attribute != null) {
                    name = state_variable_attribute.Name;
                }
            }
            
            if (name == null) {
                return;
            }

            AddEventedStateVariable (CreateEventedStateVariable (name, eventInfo));
        }
        
        protected virtual EventedStateVariable CreateEventedStateVariable (string name, EventInfo eventInfo)
        {
            return new EventedStateVariable (name, eventInfo);
        }
        
        protected virtual void AddEventedStateVariable (EventedStateVariable eventedStateVariable)
        {
            if (eventedStateVariable != null) {
                if (EventedStateVariables.ContainsKey (eventedStateVariable.Name))
                    throw new UpnpServerException (string.Format ("The service already contains an state variable named '{0}'.", stateVariable.Name));
                
                EventedStateVariables.Add (eventedStateVariable.Name, eventedStateVariable);
            }
        }
        
        protected internal virtual StateVariable GetRelatedStateVariable (string argumentName,
                                                                          Type dataType, object defaultValue,
                                                                          AllowedValueRange allowedValueRange)
        {
            var name = "A_ARG_TYPE_" + argumentName;
            if (related_state_variables.ContainsKey (name)) {
                var count = 1;
                while (StateVariableNameConflict (name, dataType)) {
                    name = string.Format ("A_ARG_TYPE_{0}_{1}", argumentName, count);
                    count++;
                }
                if (related_state_variables.ContainsKey (name)) {
                    return related_state_variables[name];
                }
            }
            
            var state_variable = CreateStateVariable (name, dataType, defaultValue, allowedValueRange);
            AddRelatedStateVariable (state_variable);
            return state_variable;
        }
        
        protected virtual StateVariable CreateRelatedStateVariable (string name, Type dataType, object defaultValue, AllowedValueRange allowedValueRange)
        {
            return new RelatedStateVariable (name, dataType, defaultValue, allowedValueRange);
        }
        
        protected virtual void AddRelatedStateVariable (RelatedStateVariable relatedStateVariable)
        {
            if (relatedStateVariable != null) {
                if (RelatedStateVariables.ContainsKey (relatedStateVariable.Name))
                    throw new UpnpServerException (string.Format ("The service already contains an state variable named '{0}'.", stateVariable.Name));
                
                RelatedStateVariables.Add (relatedStateVariable.Name, relatedStateVariable);
            }
        }
        
        bool StateVariableNameConflict (string name, Type dataType)
        {
            if (!RelatedStateVariables.ContainsKey (name)) {
                return false;
            }
            var variable = RelatedStateVariables[name];
            return variable.DataType != dataType || variable.SendEvents == true;
        }
        
        protected virtual void RegisterEvents (object service)
        {
            foreach (var service_variable in EventedStateVariables.Values) {
                var del = Delegate.CreateDelegate (
                    service_variable.EventInfo.EventHandlerType, this, on_event.MakeGenericMethod (
                    service_variable.Type));
                service_variable.EventInfo.AddEventHandler (service, del);
            }
        }
        
        void OnEvent<T> (object sender, StateVariableChangedArgs<T> args)
        {
            //value = args.NewValue;
            //service_controller.PublishStateVariableChange ();
        }
        
        internal byte[] Serialize ()
        {
            return Serialize (serializer);
        }
        
        protected virtual byte[] Serialize (XmlSerializer serializer)
        {
            return serializer.GetBytes (this);
        }
        
        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            context.AutoSerializeObjectAndMembers (this);
        }
        
        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            context.AutoSerializeMembersOnly (this);
        }
        
        internal void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposing) {
                Stop ();
                if (action_server != null) {
                    action_server.Dispose ();
                    event_server.Dispose ();
                    description_server.Dispose ();
                }
            }
        }
    }
    
    public class ServiceController<T> : ServiceController
    {
        static readonly Dictionary<string, ServiceAction> actions;
        static readonly Dictionary<string, RelatedStateVariable> related_state_variables;
        static readonly Dictionary<string, EventedStateVariable> evented_state_variables;        
        
        readonly T service;
        
        protected internal ServiceController (Server server, T service)
             : base (server)
        {
            if (service == null) throw new ArgumentNullException ("service");
            
            this.service = service;
        }
        
        protected override IDictionary<string, ServiceAction> ServiceActions {
            get { return actions; }
        }
        
        protected override IDictionary<string, RelatedStateVariable> RelatedStateVariables {
            get { return related_state_variables; }
        }
        
        protected override IDictionary<string, EventedStateVariable> EventedStateVariables {
            get { return evented_state_variables; }
        }

        internal void Initialize ()
        {
            if (actions == null) {
                actions = new Dictionary<string, ServiceAction> ();
                related_state_variables = new Dictionary<string, RelatedStateVariable> ();
                evented_state_variables = new Dictionary<string, EventedStateVariable> ();
                ProcessActions (typeof (T));
                ProcessEventedStateVariables (typeof (T));
            }
            RegisterEvents (service);
        }
    }
}
