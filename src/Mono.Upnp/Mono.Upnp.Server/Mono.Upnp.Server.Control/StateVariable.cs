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

using Mono.Upnp.Xml;
using Mono.Upnp.Server.Internal;

namespace Mono.Upnp.Server
{
    [XmlType ("stateVariable")]
    public class StateVariable : XmlSerializable
	{
        readonly string name;
        readonly Type data_type;
        readonly bool sends_events;
        
        [XmlAttribute ("sendEvents", OmitIfNull = true)]
        public string SendsEvents {
            get { return sends_events ? null : "no"; }
        }
        
        [XmlElement ("name")]
        public string Name {
            get { return name; }
        }
        
        [XmlElement ("dataType")]
        public string DataType {
            get { return data_type; }
        }
        
        public Type Type {
            get { return data_type; }
        }
        
        protected StateVariable (string name, Type dataType, bool sendsEvents)
        {
            if (name == null) throw new ArgumentNullException ("name");
            if (dataType == null) throw new ArgumentNullException ("dataType");
            
            //this.data_type = dataType.IsByRef ? dataType.GetElementType () : dataType;
            
            this.name = name;
            this.data_type = dataType;
            this.sends_events = sendsEvents;
        }
        
        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            context.AutoSerializeObjectAndMembers (this);
        }

        
        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            context.AutoSerializeMembersOnly (this);
        }
	}
}
