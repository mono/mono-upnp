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

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Control
{
    [XmlType ("argument")]
    public class Argument : XmlAutomatable, IMappable<string>
    {
        protected internal Argument ()
        {
        }
        
        public Argument (string name, string relatedStateVariable, ArgumentDirection direction)
        {
            if (direction < ArgumentDirection.In || direction > ArgumentDirection.Out)
                throw new ArgumentOutOfRangeException ("direction");
            
            Name = name;
            RelatedStateVariableName = relatedStateVariable;
            Direction = direction;
        }
        
        public Argument (string name, string relatedStateVariable, ArgumentDirection direction, bool isReturnValue)
            : this (name, relatedStateVariable, direction)
        {
            if (direction == ArgumentDirection.In && isReturnValue)
                throw new ArgumentException ("The direction is In, but isReturnValue is true. An IsReturnValue argument must have the direction Out.");
            
            IsReturnValue = isReturnValue;
        }

        [XmlElement ("name")]
        public virtual string Name { get; protected set; }
        
        [XmlElement ("direction")]
        public virtual ArgumentDirection Direction { get; protected set; }
        
        [XmlFlag ("retval")]
        public virtual bool IsReturnValue { get; protected set; }
        
        [XmlElement ("relatedStateVariable")]
        public virtual string RelatedStateVariableName { get; protected set; }
        
        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
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
