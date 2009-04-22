// 
// XmlSerializer.cs
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Mono.Upnp.Xml
{
    public class XmlSerializer
    {
        delegate void Serializer (object obj, XmlSerializationContext context);
        
        class Serializers
        {
            public Serializer TypeSerializer;
            public Serializer TypeAutoSerializer;
            public Serializer MemberSerializer;
            public Serializer MemberAutoSerializer;
        }
        
        readonly Dictionary<Type, Serializers> serializers = new Dictionary<Type, Serializers> ();
        
        public void Serialize<T> (T obj, XmlWriter writer)
        {
            if (writer == null) throw new ArgumentNullException ("writer");

            SerializeCore (obj, writer);
        }
                
        public byte[] GetBytes<T> (T obj)
        {
            using (var stream = new MemoryStream ()) {
                using (var writer = XmlWriter.Create (stream)) {
                    SerializeCore (obj, writer);
                    return stream.ToArray ();
                }
            }
        }
        
        public string GetString<T> (T obj)
        {
            using (var string_writer = new StringWriter ()) {
                using (var xml_writer = XmlWriter.Create (string_writer)) {
                    SerializeCore (obj, xml_writer);
                    return string_writer.ToString ();
                }
            }
        }
        
        void SerializeCore<T> (T obj, XmlWriter writer)
        {
            if (obj == null) throw new ArgumentNullException ("obj");
            
            var serializer = GetTypeSerializer (typeof (T));
            serializer (obj, new XmlSerializationContext (this, writer));
        }

        internal void AutoSerializeObjectAndMembers<T> (T obj, XmlSerializationContext context)
        {
            var serializer = GetTypeAutoSerializer (typeof (T));
            serializer (obj, context);
        }
        
        internal void AutoSerializeMembersOnly<T> (T obj, XmlSerializationContext context)
        {
            var serialzer = GetMemberAutoSerializer (typeof (T));
            serialzer (obj, context);
        }
        
        Serializers GetSerializers (Type type)
        {
            if (!serializers.ContainsKey (type)) {
                return serializers[type] = new Serializers ();
            }
            return serializers[type];
        }
        
        Serializer GetTypeSerializer (Type type)
        {
            var serializers = GetSerializers (type);
            if (serializers.TypeSerializer == null) {
                Serializer serializer = null;
                serializers.TypeSerializer = (obj, context) => serializer (obj, context);
                serializer = CreateTypeSerializer (type, serializers);
                return serializers.TypeSerializer = serializer;
            }
            return serializers.TypeSerializer;
        }
        
        Serializer CreateTypeSerializer (Type type, Serializers serializers)
        {
            if (typeof (IXmlSerializable).IsAssignableFrom (type)) {
                return (obj, context) => ((IXmlSerializable)obj).SerializeSelfAndMembers (context);
            } else {
                return GetTypeAutoSerializer (type, serializers);
            }
        }
        
        Serializer GetMemberSerializer (Type type)
        {
            var serializers = GetSerializers (type);
            if (serializers.MemberSerializer == null) {
                Serializer serializer = null;
                serializers.MemberSerializer = (obj, context) => serializer (obj, context);
                serializer = CreateMemberSerializer (type, serializers);
                return serializers.MemberSerializer = serializer;
            }
            return serializers.MemberSerializer;
        }
        
        Serializer CreateMemberSerializer (Type type, Serializers serializers)
        {
            if (typeof (IXmlSerializable).IsAssignableFrom (type)) {
                return (obj, context) => ((IXmlSerializable)obj).SerializeMembersOnly (context);
            } else {
                return GetMemberAutoSerializer (type, serializers);
            }
        }
        
        Serializer GetTypeAutoSerializer (Type type)
        {
            return GetTypeAutoSerializer (type, GetSerializers (type));
        }
        
        Serializer GetTypeAutoSerializer (Type type, Serializers serializers)
        {
            if (serializers.TypeAutoSerializer == null) {
                return serializers.TypeAutoSerializer = CreateTypeAutoSerializer (type);
            }
            return serializers.TypeAutoSerializer;
        }
        
        Serializer CreateTypeAutoSerializer (Type type)
        {
            var name = type.Name;
            string @namespace = null;
            
            var type_attributes = type.GetCustomAttributes (typeof (XmlTypeAttribute), false);
            if (type_attributes.Length > 0) {
                var type_attribute = (XmlTypeAttribute)type_attributes[0];
                name = type_attribute.Name;
                @namespace = type_attribute.Namespace;
            }
            
            var next = GetMemberAutoSerializer (type);
            
            return (obj, context) => {
                context.Writer.WriteStartElement (name, @namespace);
                next (obj, context);
                context.Writer.WriteEndElement ();
            };
        }
        
        Serializer GetMemberAutoSerializer (Type type)
        {
            return GetMemberAutoSerializer (type, GetSerializers (type));
        }
        
        Serializer GetMemberAutoSerializer (Type type, Serializers serializers)
        {
            if (serializers.MemberAutoSerializer == null) {
                return serializers.MemberAutoSerializer = CreateMemberAutoSerializer (type);
            }
            return serializers.MemberAutoSerializer;
        }
        
        Serializer CreateMemberAutoSerializer (Type type)
        {
            var attribute_serializers = new List<Serializer>();
            var element_serializers = new List<Serializer> ();
            
            foreach (var property in type.GetProperties ()) {
                ProcessMember (property, attribute_serializers, element_serializers);
            }
            
            attribute_serializers.AddRange (element_serializers);
            var serializers = attribute_serializers.ToArray ();
            if (serializers.Length > 0) {
                return (obj, context) => {
                    foreach (var serializer in serializers) {
                        serializer (obj, context);
                    }
                };
            } else {
                return (obj, context) => context.Writer.WriteValue (obj.ToString ());
            }
        }
        
        void ProcessMember (PropertyInfo property, List<Serializer> attributeSerializers, List<Serializer> elementSerializers)
        {
            XmlAttributeAttribute attribute_attribute = null;
            XmlElementAttribute element_attribute = null;
            XmlFlagAttribute flag_attribute = null;
            XmlArrayAttribute array_attribute = null;
            XmlArrayItemAttribute array_item_attribute = null;
            
            foreach (var custom_attribute in property.GetCustomAttributes (false)) {
                var attribute = custom_attribute as XmlAttributeAttribute;
                if(attribute != null) {
                    attribute_attribute = attribute;
                    continue;
                }
                
                var element = custom_attribute as XmlElementAttribute;
                if (element != null) {
                    element_attribute = element;
                    continue;
                }
                
                var flag = custom_attribute as XmlFlagAttribute;
                if (flag != null) {
                    flag_attribute = flag;
                    continue;
                }
                
                var array = custom_attribute as XmlArrayAttribute;
                if (array != null) {
                    array_attribute = array;
                    continue;
                }
                
                var array_item = custom_attribute as XmlArrayItemAttribute;
                if (array_item != null) {
                    array_item_attribute = array_item;
                    continue;
                }
            }
            
            if (attribute_attribute != null) {
                attributeSerializers.Add (CreateSerializer (property, attribute_attribute));
                return;
            }
            
            if (element_attribute != null) {
                elementSerializers.Add (CreateSerializer (property, element_attribute));
                return;
            }
            
            if (flag_attribute != null) {
                elementSerializers.Add (CreateSerializer (property, flag_attribute));
                return;
            }
            
            if (array_attribute != null) {
                elementSerializers.Add (CreateSerializer (property, array_attribute, array_item_attribute));
                return;
            }
        }
        
        static Serializer CreateSerializer (Serializer serializer, bool omitIfNull)
        {
            if (omitIfNull) {
                return (obj, writer) => {
                    if (obj != null) {
                        serializer (obj, writer);
                    }
                };
            } else {
                return serializer;
            }
        }
        
        static Serializer CreateSerializer (PropertyInfo property, XmlAttributeAttribute attributeAttribute)
        {
            return CreateSerializer (CreateSerializerCore (property, attributeAttribute), attributeAttribute.OmitIfNull);
        }
        
        static Serializer CreateSerializerCore (PropertyInfo property, XmlAttributeAttribute attributeAttribute)
        {
            if (!property.CanRead) {
                // TODO throw
            }
            var name = attributeAttribute.Name ?? property.Name;
            var @namespace = attributeAttribute.Namespace;
            return (obj, context) =>
                context.Writer.WriteAttributeString (name, @namespace, property.GetValue (obj, null).ToString ());
        }
        
        Serializer CreateSerializer (PropertyInfo property, XmlElementAttribute elementAttribute)
        {
            return CreateSerializer (CreateSerializerCore (property, elementAttribute), elementAttribute.OmitIfNull);
        }
        
        Serializer CreateSerializerCore (PropertyInfo property, XmlElementAttribute elementAttribute)
        {
            if (!property.CanRead) {
                // TODO throw
            }
            var next = GetMemberSerializer (property.PropertyType);
            var name = elementAttribute.Name ?? property.Name;
            var @namespace = elementAttribute.Namespace;
            return (obj, context) => {
                context.Writer.WriteStartElement (name, @namespace);
                next (property.GetValue (obj, null), context);
                context.Writer.WriteEndElement ();
            };
        }
        
        Serializer CreateSerializer (PropertyInfo property, XmlArrayAttribute arrayAttribute, XmlArrayItemAttribute arrayItemAttribute)
        {
            return CreateSerializer (CreateSerializerCore (property, arrayAttribute, arrayItemAttribute), arrayAttribute.OmitIfNull);
        }
        
        Serializer CreateSerializerCore (PropertyInfo property, XmlArrayAttribute arrayAttribute, XmlArrayItemAttribute arrayItemAttribute)
        {
            if (!property.CanRead) {
                // TODO throw
            }
            Type ienumerable;
            if (property.PropertyType.IsGenericType &&
                property.PropertyType.GetGenericTypeDefinition () == typeof (IEnumerable<>)) {
                ienumerable = property.PropertyType;
            } else {
                ienumerable = property.PropertyType.GetInterface ("IEnumerable`1");
            }
            if (ienumerable == null) {
                // TODO throw
            }
            
            var name = arrayAttribute.Name ?? property.Name;
            var @namespace = arrayAttribute.Namespace;
            var next = CreateSerializer (ienumerable.GetGenericArguments ()[0], arrayItemAttribute);
            if (arrayAttribute.OmitIfEmpty) {
                return (obj, context) => {
                    if (obj != null) {
                        var first = true;
                        foreach (var item in (IEnumerable)property.GetValue (obj, null)) {
                            if (first) {
                                context.Writer.WriteStartElement (name, @namespace);
                                first = false;
                            }
                            next (item, context);
                        }
                        if (!first) {
                            context.Writer.WriteEndElement ();
                        }
                    }
                };
            } else {
                return (obj, context) => {
                    context.Writer.WriteStartElement (name, @namespace);
                    if (obj != null) {
                        foreach (var item in (IEnumerable)property.GetValue (obj, null)) {
                            next (item, context);
                        }
                    }
                    context.Writer.WriteEndElement ();
                };
            }
        }
        
        Serializer CreateSerializer (Type type, XmlArrayItemAttribute arrayItemAttribute)
        {
            if (arrayItemAttribute == null) {
                return GetTypeSerializer (type);
            } else {
                var name = arrayItemAttribute.Name;
                var @namespace = arrayItemAttribute.Namespace;
                var next = GetMemberSerializer (type);
                return (obj, context) => {
                    context.Writer.WriteStartElement (name, @namespace);
                    next (obj, context);
                    context.Writer.WriteEndElement ();
                };
            }
        }
        
        static Serializer CreateSerializer (PropertyInfo property, XmlFlagAttribute flagAttribute)
        {
            if (property.PropertyType != typeof (bool)) {
                // TODO throw
            }
            var name = flagAttribute.Name ?? property.Name;
            var @namespace = flagAttribute.Namespace;
            return (obj, context) => {
                if ((bool)obj) {
                    context.Writer.WriteStartElement (name, @namespace);
                    context.Writer.WriteEndElement ();
                }
            };
        }
    }
}
