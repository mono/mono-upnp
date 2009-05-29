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
    [XmlType ("stateVariable", Protocol.ServiceUrn)]
    public class StateVariable : XmlAutomatable
    {
        readonly ServiceController controller;
        IList<string> allowed_values;
        //event EventHandler<StateVariableChangedArgs<string>> changed;

        protected internal StateVariable (ServiceController service)
        {
            if (service == null) throw new ArgumentNullException ("service");

            this.controller = service;
        }

        public ServiceController Controller {
            get { return controller; }
        }
        
        [XmlElement ("name", Protocol.ServiceUrn)]
        public virtual string Name { get; protected set; }

        [XmlElement ("dataType", Protocol.ServiceUrn)]
        public virtual string DataType { get; protected set; }

        public Type Type { get; private set; }

        [XmlAttribute ("sendEvents")]
        protected virtual string SendsEventsString {
            get { return SendsEvents ? "yes" : "no"; }
            set { SendsEvents = value == "yes"; }
        }
        
        public bool SendsEvents { get; protected set; }
        
        [XmlAttribute ("multicast", Protocol.ServiceUrn)]
        public virtual bool IsMulticast { get; protected set; }

        [XmlElement ("defaultValue", Protocol.ServiceUrn)]
        public virtual string DefaultValue { get; protected set; }

        [XmlArray ("allowedValueList", Protocol.ServiceUrn)]
        [XmlArrayItem ("allowedValue", Protocol.ServiceUrn)]
        protected virtual ICollection<string> AllowedValueCollection {
            get { return allowed_values; }
        }
        
        public IEnumerable<string> AllowedValues {
            get { return allowed_values; }
        }

        [XmlElement ("allowedValueRange", Protocol.ServiceUrn)]
        public virtual AllowedValueRange AllowedValueRange { get; protected set; }
        
        protected override void DeserializeAttribute (XmlDeserializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoDeserializeAttribute (this);
        }
        
        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            if (context.Reader.LocalName == "allowedValueList" && context.Reader.NamespaceURI == Protocol.ServiceUrn) {
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
    }
}
