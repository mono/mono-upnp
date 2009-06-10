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
using System.Text;
using System.Xml;

namespace Mono.Upnp.Xml
{
    public sealed class XmlSerializer
    {
        delegate void Serializer (object obj, XmlSerializationContext context);
        
        class Serializers
        {
            public Serializer TypeSerializer;
            public Serializer TypeAutoSerializer;
            public Serializer MemberSerializer;
            public Serializer MemberAutoSerializer;
        }
        
        static UTF8Encoding utf8 = new UTF8Encoding (false);
        
        readonly Dictionary<Type, Serializers> serializers = new Dictionary<Type, Serializers> ();
        
        public void Serialize<T> (T obj, XmlWriter writer)
        {
            if (writer == null) throw new ArgumentNullException ("writer");

            SerializeCore (obj, writer);
        }
        
        public void Serialize<T> (T obj, Stream stream)
        {
            if (stream == null) throw new ArgumentNullException ("stream");
            
            using (var writer = XmlWriter.Create (stream, new XmlWriterSettings { Encoding = utf8 })) {
                SerializeCore (obj, writer);
            }
        }
                
        public byte[] GetBytes<T> (T obj)
        {
            using (var stream = new MemoryStream ()) {
                using (var writer = XmlWriter.Create (stream, new XmlWriterSettings { Encoding = utf8 })) {
                    SerializeCore (obj, writer);
                }
                return stream.ToArray ();
            }
        }
        
        public string GetString<T> (T obj)
        {
            return utf8.GetString (GetBytes (obj));
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
                serializers[type] = new Serializers ();
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
                serializers.TypeSerializer = serializer;
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
                serializers.MemberSerializer = serializer;
            }
            return serializers.MemberSerializer;
        }
        
        Serializer CreateMemberSerializer (Type type, Serializers serializers)
        {
            if (typeof (IXmlSerializable).IsAssignableFrom (type)) {
                return (obj, context) => ((IXmlSerializable)obj).SerializeMembersOnly (context);
            } else if (type.IsEnum) {
                var map = GetEnumMap (type);
                return (obj, context) => context.Writer.WriteValue (map [obj]);
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
                serializers.TypeAutoSerializer = CreateTypeAutoSerializer (type);
            }
            return serializers.TypeAutoSerializer;
        }
        
        Serializer CreateTypeAutoSerializer (Type type)
        {
            var name = type.Name;
            string @namespace = null;
            string prefix = null;
            
            XmlTypeAttribute type_attribute = null;
            var namespace_attributes = new List<XmlNamespaceAttribute> ();
            foreach (var custom_attribute in type.GetCustomAttributes (false)) {
                var xml_type = custom_attribute as XmlTypeAttribute;
                if (xml_type != null) {
                    type_attribute = xml_type;
                    continue;
                }
                namespace_attributes.Add (custom_attribute as XmlNamespaceAttribute);
            }
            
            if (type_attribute != null) {
                name = type_attribute.Name;
                @namespace = type_attribute.Namespace;
                prefix = type_attribute.Prefix;
            }
            
            var next = GetMemberSerializer (type);
            
            if (namespace_attributes.Count != 0) {
                var namespaces = namespace_attributes.ToArray ();
                return (obj, context) => {
                    context.Writer.WriteStartElement (prefix, name, @namespace);
                    foreach (var ns in namespaces) {
                        context.Writer.WriteAttributeString ("xmlns", ns.Prefix, null, ns.Namespace);
                    }
                    next (obj, context);
                    context.Writer.WriteEndElement ();
                };
            } else {
                return (obj, context) => {
                    context.Writer.WriteStartElement (prefix, name, @namespace);
                    next (obj, context);
                    context.Writer.WriteEndElement ();
                };
            }
        }
        
        Serializer GetMemberAutoSerializer (Type type)
        {
            return GetMemberAutoSerializer (type, GetSerializers (type));
        }
        
        Serializer GetMemberAutoSerializer (Type type, Serializers serializers)
        {
            if (serializers.MemberAutoSerializer == null) {
                serializers.MemberAutoSerializer = CreateMemberAutoSerializer (type);
            }
            return serializers.MemberAutoSerializer;
        }
        
        Serializer CreateMemberAutoSerializer (Type type)
        {
            var attribute_serializers = new List<Serializer>();
            var element_serializers = new List<Serializer> ();
            
            ProcessMember (type, BindingFlags.Instance | BindingFlags.Public, attribute_serializers, element_serializers);
            ProcessMember (type, BindingFlags.Instance | BindingFlags.NonPublic, attribute_serializers, element_serializers);

            attribute_serializers.AddRange (element_serializers);
            var serializers = attribute_serializers.ToArray ();
            
            if (serializers.Length > 0) {
                return (obj, context) => {
                    foreach (var serializer in serializers) {
                        serializer (obj, context);
                    }
                };
            } else {
                return (obj, context) => {
                    if (obj != null) {
                        context.Writer.WriteValue (obj.ToString ());
                    }
                };
            }
        }
        
        void ProcessMember (Type type, BindingFlags flags, List<Serializer> attributeSerializers, List<Serializer> elementSerializers)
        {
            foreach (var property in type.GetProperties (flags)) {
                XmlAttributeAttribute attribute_attribute = null;
                XmlElementAttribute element_attribute = null;
                XmlFlagAttribute flag_attribute = null;
                XmlArrayAttribute array_attribute = null;
                XmlArrayItemAttribute array_item_attribute = null;
                XmlValueAttribute value_attribute = null;
                
                foreach (var custom_attribute in property.GetCustomAttributes (false)) {
                    if (custom_attribute is DoNotSerializeAttribute) {
                        attribute_attribute = null;
                        element_attribute = null;
                        flag_attribute = null;
                        array_attribute = null;
                        value_attribute = null;
                        break;
                    }
                    
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
                    
                    var value = custom_attribute as XmlValueAttribute;
                    if (value != null) {
                        value_attribute = value;
                        continue;
                    }
                }
                
                if (attribute_attribute != null) {
                    attributeSerializers.Add (CreateSerializer (property, CreateSerializer (property, attribute_attribute)));
                    continue;
                }
                
                if (element_attribute != null) {
                    elementSerializers.Add (CreateSerializer (property, CreateSerializer (property, element_attribute)));
                    continue;
                }
                
                if (flag_attribute != null) {
                    elementSerializers.Add (CreateSerializer (property, CreateSerializer (property, flag_attribute)));
                    continue;
                }
                
                if (array_attribute != null) {
                    elementSerializers.Add (CreateSerializer (property, CreateSerializer (property, array_attribute, array_item_attribute)));
                }
                
                if (value_attribute != null) {
                    elementSerializers.Add (CreateSerializer (property, CreateSerializer (property)));
                }
            }
        }
        
        static Serializer CreateSerializer (PropertyInfo property, Serializer serializer)
        {
            return (obj, context) => serializer (property.GetValue (obj, null), context);
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
            var prefix = attributeAttribute.Prefix;
            if (property.PropertyType.IsEnum) {
                var map = GetEnumMap (property.PropertyType);
                return (obj, context) => {
                    context.Writer.WriteAttributeString (prefix, name, @namespace, map[obj]);
                };
            } else {
                return (obj, context) => {
                    context.Writer.WriteAttributeString (prefix, name, @namespace, obj != null ? obj.ToString () : string.Empty);
                };
            }
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
            var prefix = elementAttribute.Prefix;
            return (obj, context) => {
                context.Writer.WriteStartElement (prefix, name, @namespace);
                next (obj, context);
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
            var prefix = arrayAttribute.Prefix;
            var next = CreateSerializer (ienumerable.GetGenericArguments ()[0], arrayItemAttribute);
            if (arrayAttribute.OmitIfEmpty) {
                return (obj, context) => {
                    if (obj != null) {
                        var first = true;
                        foreach (var item in (IEnumerable)obj) {
                            if (first) {
                                context.Writer.WriteStartElement (prefix, name, @namespace);
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
                    context.Writer.WriteStartElement (prefix, name, @namespace);
                    if (obj != null) {
                        foreach (var item in (IEnumerable)obj) {
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
                var prefix = arrayItemAttribute.Prefix;
                var next = GetMemberSerializer (type);
                return (obj, context) => {
                    context.Writer.WriteStartElement (prefix, name, @namespace);
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
            var prefix = flagAttribute.Prefix;
            return (obj, context) => {
                if ((bool)obj) {
                    context.Writer.WriteStartElement (prefix, name, @namespace);
                    context.Writer.WriteEndElement ();
                }
            };
        }
        
        static Serializer CreateSerializer (PropertyInfo property)
        {
            return (obj, context) => {
                if (obj != null) {
                    context.Writer.WriteString (obj.ToString ());
                }
            };
        }
        
        static Dictionary<object, string> GetEnumMap (Type type)
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
    }
}
