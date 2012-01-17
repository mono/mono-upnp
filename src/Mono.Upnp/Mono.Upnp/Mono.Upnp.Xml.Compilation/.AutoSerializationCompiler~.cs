// 
// SerializationCompiler.cs
//  
// Author:
//       scott <${AuthorEmail}>
// 
// Copyright (c) 2009 scott
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

namespace Mono.Upnp.Xml.Compilation
{
    public abstract class SerializationCompiler<TContext> : Compiler
    {
        protected SerializationCompiler (Type type)
            : base (type)
        {
        }
        
        public virtual Serializer<TContext> CreateTypeSerializer ()
        {
            if (typeof (IXmlSerializable<TContext>).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlSerializable<TContext>)obj).SerializeSelfAndMembers (context);
            } else if (typeof (IXmlSerializable).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlSerializable)obj).SerializeSelfAndMembers (context);
            } else {
                return info.TypeAutoSerializer;
            }
        }
        
        public abstract Serializer<TContext> CreateTypeAutoSerializer ();
        
        public virtual Serializer<TContext> CreateMemberSerializer ()
        {
            if (typeof (IXmlSerializable<TContext>).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlSerializable<TContext>)obj).SerializeMembersOnly (context);
            } else if (typeof (IXmlSerializable).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlSerializable)obj).SerializeMembersOnly (context);
            } else if (Type.IsEnum) {
                var map = GetEnumMap (Type);
                return (obj, context) => context.Writer.WriteValue (map [obj]);
            } else {
                return info.MemberAutoSerializer;
            }
        }
        
        public abstract Serializer<TContext> CreateMemberAutoSerializer ();
    }
}
