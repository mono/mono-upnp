// 
// SerializationCompiler.cs
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
using System.Reflection;
using System.Text;

namespace Mono.Upnp.Xml.Internal
{
    class SerializationCompiler<TContext> : Compiler
    {
        readonly XmlSerializer<TContext> xml_serializer;
        readonly SerializationInfo<TContext> info;
        
        public SerializationCompiler (XmlSerializer<TContext> xmlSerializer, SerializationInfo<TContext> info, Type type)
            : base (type)
        {
            this.xml_serializer = xmlSerializer;
            this.info = info;
        }
        
        public Serializer<TContext> CreateTypeSerializer ()
        {
            if (typeof (IXmlSerializable<TContext>).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlSerializable<TContext>)obj).SerializeSelfAndMembers (context);
            } else if (typeof (IXmlSerializable).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlSerializable)obj).SerializeSelfAndMembers (context);
            } else {
                return info.TypeAutoSerializer;
            }
        }
        
        public Serializer<TContext> CreateTypeAutoSerializer ()
        {
            var name = Type.Name;
            string @namespace = null;
            string prefix = null;
            
            XmlTypeAttribute type_attribute = null;
            var namespace_attributes = new List<XmlNamespaceAttribute> ();
            foreach (var custom_attribute in Type.GetCustomAttributes (false)) {
                var xml_type = custom_attribute as XmlTypeAttribute;
                if (xml_type != null) {
                    type_attribute = xml_type;
                    continue;
                }
                namespace_attributes.Add (custom_attribute as XmlNamespaceAttribute);
            }
            
            if (type_attribute != null) {
                if (!string.IsNullOrEmpty (type_attribute.Name)) {
                    name = type_attribute.Name;
                }
                @namespace = type_attribute.Namespace;
                prefix = type_attribute.Prefix;
            }
            
            var next = info.MemberSerializer;
            
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
        
        public Serializer<TContext> CreateMemberSerializer ()
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
        
        public Serializer<TContext> CreateMemberAutoSerializer ()
        {
            var attribute_serializers = new List<Serializer<TContext>>();
            var element_serializers = new List<Serializer<TContext>> ();
            
            foreach (var property in Properties) {
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
                    attribute_serializers.Add (CreateAttributeSerializer (property, attribute_attribute));
                    continue;
                }
                
                if (element_attribute != null) {
                    element_serializers.Add (CreateElementSerializer (property, element_attribute));
                    continue;
                }
                
                if (flag_attribute != null) {
                    element_serializers.Add (CreateFlagSerializer (property, flag_attribute));
                    continue;
                }
                
                if (array_attribute != null) {
                    element_serializers.Add (CreateArraySerializer (property, array_attribute, array_item_attribute));
                } else if (array_item_attribute != null) {
                    element_serializers.Add (CreateArrayItemSerializer (property, array_item_attribute));
                }
                
                if (value_attribute != null) {
                    element_serializers.Add (CreateValueSerializer (property));
                }
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
                return (obj, context) => {
                    if (obj != null) {
                        context.Writer.WriteValue (obj.ToString ());
                    }
                };
            }
        }
        
        static PropertySerializer<TContext> CreatePropertySerializer (MemberSerializer<TContext> serializer)
        {
            return (obj, context, property, name, @namespace, prefix) => serializer (property.GetValue (obj, null), context, name, @namespace, prefix);
        }
        
        static MemberSerializer<TContext> CreateMemberSerializer (MemberSerializer<TContext> serializer, bool omitIfNull)
        {
            if (omitIfNull) {
                return (obj, context, name, @namespace, prefix) => {
                    if (obj != null) {
                        serializer (obj, context, name, @namespace, prefix);
                    }
                };
            } else {
                return serializer;
            }
        }
        
        Serializer<TContext> CreateAttributeSerializer (PropertyInfo property, XmlAttributeAttribute attributeAttribute)
        {
            var name = string.IsNullOrEmpty (attributeAttribute.Name) ? property.Name : attributeAttribute.Name;
            var @namespace = attributeAttribute.Namespace;
            var prefix = attributeAttribute.Prefix;
            return CreateSerializer (property, name, @namespace, prefix,
                CreatePropertySerializer (
                    CreateMemberSerializer (
                        CreateAttributeSerializer (property.PropertyType),
                        attributeAttribute.OmitIfNull)));
            
        }
        
        MemberSerializer<TContext> CreateAttributeSerializer (Type type)
        {
            if (type.IsEnum) {
                var map = GetEnumMap (type);
                return (obj, context, name, @namespace, prefix) => {
                    context.Writer.WriteAttributeString (prefix, name, @namespace, map[obj]);
                };
            } else {
                return (obj, context, n, ns, p) => {
                    context.Writer.WriteAttributeString (p, n, ns, obj != null ? obj.ToString () : string.Empty);
                };
            }
        }
        
        Serializer<TContext> CreateElementSerializer (PropertyInfo property, XmlElementAttribute elementAttribute)
        {
            var name = string.IsNullOrEmpty (elementAttribute.Name) ? property.Name : elementAttribute.Name;
            var @namespace = elementAttribute.Namespace;
            var prefix = elementAttribute.Prefix;
            return CreateSerializer (property, name, @namespace, prefix,
                CreatePropertySerializer (
                    CreateMemberSerializer (
                        CreateElementSerializer (property.PropertyType),
                        elementAttribute.OmitIfNull)));
        }
        
        MemberSerializer<TContext> CreateElementSerializer (Type type)
        {
            var next = xml_serializer.GetInfo (type).MemberSerializer;
            
            return (obj, context, name, @namespace, prefix) => {
                context.Writer.WriteStartElement (prefix, name, @namespace);
                next (obj, context);
                context.Writer.WriteEndElement ();
            };
        }
        
        Serializer<TContext> CreateArraySerializer (PropertyInfo property, XmlArrayAttribute arrayAttribute, XmlArrayItemAttribute arrayItemAttribute)
        {
            var name = string.IsNullOrEmpty (arrayAttribute.Name) ? property.Name : arrayAttribute.Name;
            var @namespace = arrayAttribute.Namespace;
            var prefix = arrayAttribute.Prefix;
            return CreateSerializer (property, name, @namespace, prefix,
                CreatePropertySerializer (
                    CreateMemberSerializer (
                        CreateArraySerializer (property.PropertyType, arrayItemAttribute, arrayAttribute.OmitIfEmpty),
                        arrayAttribute.OmitIfNull)));
        }
        
        MemberSerializer<TContext> CreateArraySerializer (Type type, XmlArrayItemAttribute arrayItemAttribute, bool omitIfEmpty)
        {
            var item_type = GetIEnumerable (type).GetGenericArguments ()[0];
            var next = CreateArrayItemSerializer (item_type, arrayItemAttribute);
            if (omitIfEmpty) {
                return (obj, context, name, @namespace, prefix) => {
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
                return (obj, context, name, @namespace, prefix) => {
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
        
        Serializer<TContext> CreateArrayItemSerializer (Type type, XmlArrayItemAttribute arrayItemAttribute)
        {
            if (arrayItemAttribute == null || string.IsNullOrEmpty (arrayItemAttribute.Name)) {
                return xml_serializer.GetInfo (type).TypeSerializer;
            } else {
                var name = arrayItemAttribute.Name;
                var @namespace = arrayItemAttribute.Namespace;
                var prefix = arrayItemAttribute.Prefix;
                var next = xml_serializer.GetInfo (type).MemberSerializer;
                return (obj, context) => {
                    context.Writer.WriteStartElement (prefix, name, @namespace);
                    next (obj, context);
                    context.Writer.WriteEndElement ();
                };
            }
        }
        
        Serializer<TContext> CreateArrayItemSerializer (PropertyInfo property, XmlArrayItemAttribute arrayItemAttribute)
        {
            var item_type = GetIEnumerable (property.PropertyType).GetGenericArguments ()[0];
            if (string.IsNullOrEmpty (arrayItemAttribute.Name)) {
                if (!property.CanRead) {
                    // TODO throw
                }
                var serializer = xml_serializer.GetInfo (item_type).TypeSerializer;
                return (obj, context) => {
                    if (obj != null) {
                        foreach (var item in (IEnumerable)obj)  {
                            serializer (item, context);
                        }
                    }
                };
            } else {
                var name = arrayItemAttribute.Name;
                var @namespace = arrayItemAttribute.Namespace;
                var prefix = arrayItemAttribute.Prefix;
                var next = xml_serializer.GetInfo (item_type).MemberSerializer;
                return CreateSerializer (property, name, @namespace, prefix,
                    CreatePropertySerializer ((obj, context, n, ns, p) => {
                        if (obj != null) {
                            foreach (var item in (IEnumerable)obj) {
                                context.Writer.WriteStartElement (p, n, ns);
                                next (item, context);
                                context.Writer.WriteEndElement ();
                            }
                        }
                    }));
            }
        }
        
        Serializer<TContext> CreateFlagSerializer (PropertyInfo property, XmlFlagAttribute flagAttribute)
        {
            var name = string.IsNullOrEmpty (flagAttribute.Name) ? property.Name : flagAttribute.Name;
            var @namespace = flagAttribute.Namespace;
            var prefix = flagAttribute.Prefix;
            return CreateSerializer (property, name, @namespace, prefix,
                CreatePropertySerializer ((obj, context, n, ns, p) => {
                    if ((bool)obj) {
                        context.Writer.WriteStartElement (p, n, ns);
                        context.Writer.WriteEndElement ();
                    }
                }));
        }
        
        Serializer<TContext> CreateSerializer (PropertyInfo property, string name, string @namespace, string prefix, PropertySerializer<TContext> serializer)
        {
            if (!property.CanRead) {
                // TODO throw
            }
            
            if (typeof (IXmlSerializable<TContext>).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlSerializable<TContext>)obj).SerializeMember (new XmlMemberSerializationContext<TContext> (obj, context, serializer, name, @namespace, prefix, property));
            } else if (typeof (IXmlSerializable).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlSerializable)obj).SerializeMember (new XmlMemberSerializationContext<TContext> (obj, context, serializer, name, @namespace, prefix, property));
            } else {
                return (obj, context) => serializer (obj, context, property, name, @namespace, prefix);
            }
        }
        
        static Type GetIEnumerable (Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition () == typeof (IEnumerable<>)) {
                return type;
            } else {
                var ienumerable = type.GetInterface ("IEnumerable`1");
                if (ienumerable != null) {
                    return ienumerable;
                } else {
                    // TODO throw
                    return null;
                }
            }
        }
        
        static Serializer<TContext> CreateValueSerializer (PropertyInfo property)
        {
            return (obj, context) => {
                if (obj != null) {
                    context.Writer.WriteString (property.GetValue (obj, null).ToString ());
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
