// 
// XmlAutomatable.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Thomas
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

namespace Mono.Upnp.Xml
{
    public abstract class XmlAutomatable : XmlDeserializable, IXmlSerializable
    {
        void IXmlSerializable.Serialize (XmlSerializationContext context)
        {
            Serialize (context);
        }
        
        void IXmlSerializable.SerializeMembers (XmlSerializationContext context)
        {
            SerializeMembers (context);
        }
        
        protected abstract void Serialize (XmlSerializationContext context);
        
        protected abstract void SerializeMembers (XmlSerializationContext context);

        protected void AutoSerialize<T> (T @this, XmlSerializationContext context)
        {
            if (context == null) {
                throw new ArgumentNullException ("context");
            }

            context.AutoSerializeObjectStart (@this);
            SerializeMembers (context);
            context.AutoSerializeObjectEnd (@this);
        }

        protected static void AutoSerializeMembers<T> (T @this, XmlSerializationContext context)
        {
            if (context == null) {
                throw new ArgumentNullException ("context");
            }

            context.AutoSerializeMembers (@this);
        }
    }
    
    public abstract class XmlAutomatable<TContext> : XmlDeserializable, IXmlSerializable<TContext>
    {
        void IXmlSerializable<TContext>.Serialize (XmlSerializationContext<TContext> context)
        {
            Serialize (context);
        }
        
        void IXmlSerializable<TContext>.SerializeMembers (XmlSerializationContext<TContext> context)
        {
            SerializeMembers (context);
        }
        
        protected abstract void Serialize (XmlSerializationContext<TContext> context);
        
        protected abstract void SerializeMembers (XmlSerializationContext<TContext> context);

        internal void AutoSerialize<T> (T @this, XmlSerializationContext<TContext> context)
        {
            if (context == null) {
                throw new ArgumentNullException ("context");
            }

            context.AutoSerializeObjectStart (@this);
            SerializeMembers (context);
            context.AutoSerializeObjectEnd (@this);
        }

        internal static void AutoSerializeMembersOnly<T> (T @this, XmlSerializationContext<TContext> context)
        {
            if (context == null) {
                throw new ArgumentNullException ("context");
            }

            context.AutoSerializeMembers (@this);
        }
    }
}
