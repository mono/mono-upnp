// 
// XmlMemberSerializationContext.cs
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

using System.Reflection;
using System.Xml;

using Mono.Upnp.Xml.Internal;

namespace Mono.Upnp.Xml
{
    public abstract class XmlMemberSerializationContext
    {
        readonly string name;
        readonly string @namespace;
        readonly string prefix;
        readonly PropertyInfo property;
        
        internal XmlMemberSerializationContext (string name, string @namespace, string prefix, PropertyInfo property)
        {
            this.name = name;
            this.@namespace = @namespace;
            this.prefix = prefix;
            this.property = property;
        }
        
        public string Name {
            get { return name; }
        }
        
        public string Namespace {
            get { return @namespace; }
        }
        
        public string Prefix {
            get { return prefix; }
        }
        
        public PropertyInfo Property {
            get { return property; }
        }
        
        public abstract XmlWriter Writer { get; }
        
        public abstract void AutoSerializeMember ();
    }
    
    public sealed class XmlMemberSerializationContext<TContext> : XmlMemberSerializationContext
    {
        readonly object obj;
        readonly XmlSerializationContext<TContext> context;
        readonly PropertySerializer<TContext> serializer;
        
        internal XmlMemberSerializationContext (object obj, XmlSerializationContext<TContext> context, PropertySerializer<TContext> serializer, string name, string @namespace, string prefix, PropertyInfo property)
            : base (name, @namespace, prefix, property)
        {
            this.obj = obj;
            this.context = context;
            this.serializer = serializer;
        }
        
        public TContext Context {
            get { return context.Context; }
        }
        
        public override XmlWriter Writer {
            get { return context.Writer; }
        }
        
        public override void AutoSerializeMember ()
        {
            serializer (obj, context, Property, Name, Namespace, Prefix);
        }
    }
}
