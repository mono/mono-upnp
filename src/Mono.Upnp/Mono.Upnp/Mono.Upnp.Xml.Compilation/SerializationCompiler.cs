// 
// SerializationCompiler.cs
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
using System.Reflection;
using System.Xml;

namespace Mono.Upnp.Xml.Compilation
{
    public abstract class SerializationCompiler<TContext> : Compiler
    {
        readonly XmlSerializer<TContext> xml_serializer;
        
        Serializer<TContext> type_serializer;
        Serializer<TContext> type_start_auto_serializer;
        Serializer<TContext> type_end_auto_serializer;
        Serializer<TContext> member_serializer;
        Serializer<TContext> member_auto_serializer;
        
        protected SerializationCompiler (XmlSerializer<TContext> xmlSerializer, Type type)
            : base (type)
        {
            if (xmlSerializer == null) throw new ArgumentNullException ("xmlSerializer");
            
            this.xml_serializer = xmlSerializer;
        }
        
        public Serializer<TContext> TypeSerializer {
            get {
                if (this.type_serializer == null) {
                    Serializer<TContext> type_serializer = null;
                    this.type_serializer = (obj, context) => type_serializer (obj, context);
                    type_serializer = CreateTypeSerializer ();
                    this.type_serializer = type_serializer;
                }
                return this.type_serializer;
            }
        }
        
        public Serializer<TContext> TypeStartAutoSerializer {
            get {
                if (type_start_auto_serializer == null) {
                    type_start_auto_serializer = CreateTypeStartAutoSerializer ();
                }
                return type_start_auto_serializer;
            }
        }
        
        public Serializer<TContext> TypeEndAutoSerializer {
            get {
                if (type_end_auto_serializer == null) {
                    type_end_auto_serializer = CreateTypeEndAutoSerializer ();
                }
                return type_end_auto_serializer;
            }
        }
        
        public Serializer<TContext> MemberSerializer {
            get {
                if (this.member_serializer == null) {
                    Serializer<TContext> member_serializer = null;
                    this.member_serializer = (obj, context) => member_serializer (obj, context);
                    member_serializer = CreateMemberSerializer ();
                    this.member_serializer = member_serializer;
                }
                return this.member_serializer;
            }
        }
        
        public Serializer<TContext> MemberAutoSerializer {
            get {
                if (member_auto_serializer == null) {
                    member_auto_serializer = CreateMemberAutoSerializer ();
                }
                return member_auto_serializer;
            }
        }
        
        protected virtual Serializer<TContext> CreateTypeSerializer ()
        {
            if (typeof (IXmlSerializable<TContext>).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlSerializable<TContext>)obj).Serialize (context);
            } else if (typeof (IXmlSerializable).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlSerializable)obj).Serialize (context);
            } else {
                return (obj, context) => {
                    TypeStartAutoSerializer (obj, context);
                    MemberSerializer (obj, context);
                    TypeEndAutoSerializer (obj, context);
                };
            }
        }
        
        protected abstract Serializer<TContext> CreateTypeStartAutoSerializer ();
        
        protected abstract Serializer<TContext> CreateTypeEndAutoSerializer ();
        
        protected virtual Serializer<TContext> CreateMemberSerializer ()
        {
            if (typeof (IXmlSerializable<TContext>).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlSerializable<TContext>)obj).SerializeMembers (context);
            } else if (typeof (IXmlSerializable).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlSerializable)obj).SerializeMembers (context);
            } else if (Type.IsEnum) {
                var map = GetEnumMap (Type);
                return (obj, context) => context.Writer.WriteValue (map [obj]);
            } else {
                return MemberAutoSerializer;
            }
        }
        
        protected abstract Serializer<TContext> CreateMemberAutoSerializer ();
        
        protected static Dictionary<object, string> GetEnumMap (Type type)
        {
            var fields = type.GetFields (BindingFlags.Public | BindingFlags.Static);
            var dictionary = new Dictionary<object, string> (fields.Length);
            foreach (var field in fields) {
                var enum_attribute = field.GetCustomAttributes (typeof (XmlEnumAttribute), false);
                string name;
                if (enum_attribute.Length != 0) {
                    name = ((XmlEnumAttribute)enum_attribute[0]).Value;
                } else {
                    name = field.Name;
                }
                dictionary.Add (field.GetValue (null), name);
            }
            return dictionary;
        }
        
        protected SerializationCompiler<TContext> GetCompilerForType (Type type)
        {
            if (type == null) throw new ArgumentNullException ("type");
            
            return xml_serializer.GetCompilerForType (type);
        }
        
        protected XmlSerializationContext<TContext> CreateContext (XmlWriter writer, TContext context)
        {
            if (writer == null) throw new ArgumentNullException ("writer");
            
            return new XmlSerializationContext<TContext> (xml_serializer, writer, context);
        }
    }
}
