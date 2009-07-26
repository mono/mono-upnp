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

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Control
{
    [XmlType ("stateVariable")]
    public class StateVariable : XmlAutomatable, IMappable<string>
    {
        readonly LinkedList<EventHandler<StateVariableChangedArgs<string>>> value_changed = new LinkedList<EventHandler<StateVariableChangedArgs<string>>> ();
        ServiceController controller;
        IList<string> allowed_values;
        string value;
        
        protected StateVariable ()
        {
        }

        protected internal StateVariable (ServiceController serviceController)
        {
            if (serviceController == null) throw new ArgumentNullException ("serviceController");

            this.controller = serviceController;
        }
        
        public StateVariable (string name, string dataType)
        {
            if (name == null) throw new ArgumentNullException ("name");
            if (dataType == null) throw new ArgumentNullException ("dataType");
            
            Name = name;
            DataType = dataType;
        }
        
        public StateVariable (string name, string dataType, StateVariableOptions options)
            : this (name, dataType)
        {
            if (options != null) {
                DefaultValue = options.DefaultValue;
                if (options.Eventer != null) {
                    SetEventer (options.Eventer, options.IsMulticast);
                }
            }
        }
        
        public StateVariable (string name, IEnumerable<string> allowedValues)
            : this (name, allowedValues, null)
        {
        }
        
        public StateVariable (string name, IEnumerable<string> allowedValues, StateVariableOptions options)
            : this (name, "string", options)
        {
            allowed_values = Helper.MakeReadOnlyCopy (allowedValues);
        }
        
        public StateVariable (string name, string dataType, AllowedValueRange allowedValueRange)
            : this (name, dataType, allowedValueRange, null)
        {
        }
        
        public StateVariable (string name, string dataType, AllowedValueRange allowedValueRange, StateVariableOptions options)
            : this (name, dataType, options)
        {
            AllowedValueRange = allowedValueRange;
        }
        
        [XmlElement ("name")]
        public virtual string Name { get; protected set; }

        [XmlElement ("dataType")]
        public virtual string DataType { get; protected set; }

        [XmlAttribute ("sendEvents", OmitIfNull = true)]
        protected virtual string SendsEventsString {
            get { return SendsEvents ? "yes" : null; }
            set { SendsEvents = value == "yes"; }
        }
        
        public bool SendsEvents { get; protected set; }
        
        public virtual bool IsMulticast { get; protected set; }

        [XmlAttribute ("multicast", OmitIfNull = true)]
        protected virtual string IsMulticastString {
            get { return IsMulticast ? "yes" : null; }
            set { IsMulticast = value == "yes"; }
        }
        
        [XmlElement ("defaultValue", OmitIfNull = true)]
        public string DefaultValue { get; protected set; }

        [XmlArray ("allowedValueList", OmitIfNull = true)]
        [XmlArrayItem ("allowedValue")]
        protected virtual ICollection<string> AllowedValueCollection {
            get { return allowed_values; }
        }
        
        public IEnumerable<string> AllowedValues {
            get { return allowed_values; }
        }

        [XmlElement ("allowedValueRange", OmitIfNull = true)]
        public virtual AllowedValueRange AllowedValueRange { get; protected set; }
        
        public event EventHandler<StateVariableChangedArgs<string>> ValueChanged {
            add {
                if (value == null) {
                    return;
                }
                value_changed.AddLast (value);
                controller.RefEvents ();
            }
            remove {
                if (value == null || value_changed.Count == 0) {
                    return;
                }
                var node = value_changed.First;
                do {
                    if (node.Value == value) {
                        value_changed.Remove (node);
                        controller.UnrefEvents ();
                        break;
                    }
                    node = node.Next;
                } while (node != null);
            }
        }
        
        protected internal string Value {
            get { return value; }
            set {
                this.value = value;
                foreach (var handler in value_changed) {
                    handler (this, new StateVariableChangedArgs<string> (value));
                }
            }
        }
        
        internal void SetEventer (StateVariableEventer eventer, bool isMulticast)
        {
            eventer.StateVariableUpdated += OnStateVariableUpdated;
            SendsEvents = true;
            IsMulticast = isMulticast;
        }
        
        protected internal virtual void Initialize (ServiceController serviceController)
        {
            if (serviceController == null) throw new ArgumentNullException ("serviceController");
            
            this.controller = serviceController;
        }
        
        protected virtual void OnStateVariableUpdated (object sender, StateVariableChangedArgs<string> args)
        {
            Value = args.NewValue;
            if (controller != null) {
                controller.UpdateStateVariable (this);
            }
        }
        
        protected override void DeserializeAttribute (XmlDeserializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoDeserializeAttribute (this);
        }
        
        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            if (context.Reader.Name == "allowedValueList") {
                allowed_values = new List<string> ();
            }
            context.AutoDeserializeElement (this);
        }

        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoSerializeObjectAndMembers (this);
        }

        protected override void SerializeMembersOnly (Mono.Upnp.Xml.XmlSerializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoSerializeMembersOnly (this);
        }
        
        string IMappable<string>.Map ()
        {
            return Name;
        }
    }
}
