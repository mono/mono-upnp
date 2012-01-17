// 
// DeserializationCompiler.cs
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
using System.Xml;

namespace Mono.Upnp.Xml.Compilation
{
    public abstract class DeserializationCompiler : Compiler
    {
        readonly XmlDeserializer xml_deserializer;
        
        Deserializer deserializer;
        ObjectDeserializer auto_deserializer;
        ObjectDeserializer attribute_auto_deserializer;
        ObjectDeserializer element_auto_deserializer;
        
        public DeserializationCompiler (XmlDeserializer xmlDeserializer, Type type)
            : base (type)
        {
            if (xmlDeserializer == null) throw new ArgumentNullException ("xmlDeserializer");
            
            this.xml_deserializer = xmlDeserializer;
        }
        
        public Deserializer Deserializer {
            get {
                if (this.deserializer == null) {
                    Deserializer deserializer = null;
                    this.deserializer = context => deserializer (context);
                    deserializer = CreateDeserializer ();
                    this.deserializer = deserializer;
                }
                return this.deserializer;
            }
        }
        
        public ObjectDeserializer AutoDeserializer {
            get {
                if (auto_deserializer == null) {
                    auto_deserializer = CreateAutoDeserializer ();
                }
                return auto_deserializer;
            }
        }
        
        public ObjectDeserializer AttributeAutoDeserializer {
            get {
                if (attribute_auto_deserializer == null) {
                    attribute_auto_deserializer = CreateAttributeAutoDeserializer ();
                }
                return attribute_auto_deserializer;
            }
        }
        
        public ObjectDeserializer ElementAutoDeserializer {
            get {
                if (element_auto_deserializer == null) {
                    element_auto_deserializer = CreateElementAutoDeserializer ();
                }
                return element_auto_deserializer;
            }
        }
        
        protected abstract Deserializer CreateDeserializer ();
        
        protected abstract ObjectDeserializer CreateAutoDeserializer ();
        
        protected abstract ObjectDeserializer CreateAttributeAutoDeserializer ();
        
        protected abstract ObjectDeserializer CreateElementAutoDeserializer ();
        
        protected XmlDeserializationContext CreateDeserializationContext (XmlReader reader)
        {
            return new XmlDeserializationContext (xml_deserializer, reader);
        }
        
        protected Deserializer GetDeserializerForType (Type type)
        {
            if (type == null) throw new ArgumentNullException ("type");
            
            return xml_deserializer.GetCompilerForType (type).Deserializer;
        }
    }
}
