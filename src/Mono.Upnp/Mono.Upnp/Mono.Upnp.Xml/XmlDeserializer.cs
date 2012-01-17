// 
// XmlDeserializer.cs
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
using System.Collections.Generic;
using System.Xml;

using Mono.Upnp.Xml.Compilation;

namespace Mono.Upnp.Xml
{
    public delegate T Deserializer<T> (XmlDeserializationContext context);
    
    public sealed class XmlDeserializer
    {
        readonly DeserializationCompilerProvider compiler_provider;
        readonly Dictionary<Type, DeserializationCompiler> compilers =
            new Dictionary<Type, DeserializationCompiler> ();
        
        public XmlDeserializer ()
            : this (null)
        {
        }
        
        public XmlDeserializer (DeserializationCompilerProvider compilerProvider)
        {
            this.compiler_provider = compilerProvider ?? ((xmlDeserializer, type) =>
                new DelegateDeserializationCompiler (xmlDeserializer, type));
        }
        
        public T Deserialize<T> (XmlReader reader)
        {
            return Deserialize<T> (reader, null);
        }
        
        public T Deserialize<T> (XmlReader reader, Deserializer<T> typeDeserializer)
        {
            if (reader == null) throw new ArgumentNullException ("reader");
            
            return DeserializeCore<T> (reader, typeDeserializer);
        }
        
        T DeserializeCore<T> (XmlReader reader, Deserializer<T> typeDeserializer)
        {
            var context = new XmlDeserializationContext (this, reader);
            if (typeDeserializer != null) {
                return typeDeserializer (context);
            } else {
                var deserializer = GetCompilerForType (typeof (T)).Deserializer;
                return (T) deserializer (context);
            }
        }
        
        internal void AutoDeserialize<T> (T obj, XmlDeserializationContext context)
        {
            var deserializer = GetCompilerForType (typeof (T)).AutoDeserializer;
            deserializer (obj, context);
        }
        
        internal void AutoDeserializeAttribute<T> (T obj, XmlDeserializationContext context)
        {
            var deserializer = GetCompilerForType (typeof (T)).AttributeAutoDeserializer;
            deserializer (obj, context);
        }
        
        internal void AutoDeserializeElement<T> (T obj, XmlDeserializationContext context)
        {
            var deserializer = GetCompilerForType (typeof (T)).ElementAutoDeserializer;
            deserializer (obj, context);
        }
        
        internal DeserializationCompiler GetCompilerForType (Type type)
        {
            DeserializationCompiler compiler;
            if (!compilers.TryGetValue (type, out compiler)) {
                compiler = compiler_provider (this, type);
                compilers[type] = compiler;
            }
            return compiler;
        }
    }
}
