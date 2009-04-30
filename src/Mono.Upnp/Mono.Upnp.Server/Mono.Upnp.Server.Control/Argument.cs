//
// Argument.cs
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

namespace Mono.Upnp.Server
{
    [XmlType ("argument")]
    public class Argument : XmlSerializable
	{
        readonly string name;
        readonly ArgumentDirection direction;
        readonly bool is_return_value;
        readonly StateVariable related_state_variable;

        protected internal Argument (string name, ArgumentDirection direction, bool isReturnValue, StateVariable relatedStateVariable)
        {
            if (name == null) throw new ArgumentNullException ("name");
            if (isReturnValue && direction == ArgumentDirection.In) throw new ArgumentException ("If the argument is a return value, it must have an 'Out' direction.");
            if (relatedStateVariable == null) throw new ArgumentNullException ("relatedStateVariable");
            
            this.name = name;
            this.direction = direction;
            this.is_return_value = isReturnValue;
            this.related_state_variable = relatedStateVariable;
        }

        [XmlElement ("name")]
        public string Name {
            get { return name; }
        }

        [XmlElement ("direction")]
        public ArgumentDirection Direction {
            get { return direction; }
        }
        
        [XmlFlag ("retval")]
        public bool IsReturnValue {
            get { return is_return_value; }
        }

        [XmlElement ("relatedStateVariable")]
        public string RelatedStateVariableName {
            get { return related_state_variable.Name; }
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
