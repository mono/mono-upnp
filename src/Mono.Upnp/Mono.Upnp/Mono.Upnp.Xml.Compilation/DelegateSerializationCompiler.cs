// 
// DelegateSerializationCompiler.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Mono.Upnp.Xml.Compilation
{
    public class DelegateSerializationCompiler<TContext> : SerializationCompiler<TContext>
    {
        public DelegateSerializationCompiler (XmlSerializer<TContext> xmlSerializer, Type type)
            : base (xmlSerializer, type)
        {
        }
        
        protected override Serializer<TContext> CreateTypeStartAutoSerializer ()
        {
            var name = Type.Name;
            string @namespace = null;
            string prefix = null;
            
            XmlTypeAttribute type_attribute = null;
            var namespaces = new List<XmlNamespaceAttribute> ();
            
            foreach (var custom_attribute in Type.GetCustomAttributes (false)) {
                var xml_type = custom_attribute as XmlTypeAttribute;
                if (xml_type != null) {
                    type_attribute = xml_type;
                    continue;
                } else {
                    var xml_namespace = custom_attribute as XmlNamespaceAttribute;
                    if (xml_namespace != null) {
                        namespaces.Add (xml_namespace);
                    }
                }
            }
            
            if (type_attribute != null) {
                if (!string.IsNullOrEmpty (type_attribute.Name)) {
                    name = type_attribute.Name;
                }
                @namespace = type_attribute.Namespace;
                prefix = type_attribute.Prefix;
            }
            
            return CreateTypeStartAutoSerializer (
                name, @namespace, prefix, namespaces.Count == 0 ? null : namespaces.ToArray ());
        }
        
        protected virtual Serializer<TContext> CreateTypeStartAutoSerializer (string name,
                                                                              string @namespace,
                                                                              string prefix,
                                                                              IEnumerable<XmlNamespaceAttribute> namespaces)
        {
            if (namespaces != null) {
                return (obj, context) => {
                    context.Writer.WriteStartElement (prefix, name, @namespace);
                    foreach (var ns in namespaces) {
                        context.Writer.WriteAttributeString ("xmlns", ns.Prefix, null, ns.Namespace);
                    }
                };
            } else {
                return (obj, context) => {
                    context.Writer.WriteStartElement (prefix, name, @namespace);
                };
            }
        }

        protected override Serializer<TContext> CreateTypeEndAutoSerializer ()
        {
            return (obj, context) => context.Writer.WriteEndElement ();
        }
        
        protected override Serializer<TContext> CreateMemberAutoSerializer ()
        {
            var attribute_serializers = new List<Serializer<TContext>> ();
            var element_serializers = new List<Serializer<TContext>> ();
            
            foreach (var property in Properties) {
                ProcessProperty (property, attribute_serializers, element_serializers);
            }
            
            attribute_serializers.AddRange (element_serializers);
            
            if (attribute_serializers.Count == 0) {
                return (obj, context) => {
                    if (obj != null) {
                        context.Writer.WriteValue (obj.ToString ());
                    }
                };
            } else {
                var serializers = attribute_serializers.ToArray ();
                return (obj, context) => {
                    foreach (var serializer in serializers) {
                        serializer (obj, context);
                    }
                };
            }
        }
        
        protected virtual void ProcessProperty (PropertyInfo property,
                                                ICollection<Serializer<TContext>> attributeSerializers,
                                                ICollection<Serializer<TContext>> elementSerializers)
        {
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
                attributeSerializers.Add (
                    CreateSerializer (property, CreateAttributeSerializer (property, attribute_attribute)));
            } else if (element_attribute != null) {
                elementSerializers.Add (
                    CreateSerializer (property, CreateElementSerializer (property, element_attribute)));
            } else if (flag_attribute != null) {
                elementSerializers.Add (
                    CreateSerializer (property, CreateFlagSerializer (property, flag_attribute)));
            } else if (array_attribute != null) {
                elementSerializers.Add (
                    CreateSerializer (property,
                        CreateArraySerializer (property, array_attribute, array_item_attribute)));
            } else if (array_item_attribute != null) {
                elementSerializers.Add (
                    CreateSerializer (property, CreateArrayItemSerializer (property, array_item_attribute)));
            } else if (value_attribute != null) {
                elementSerializers.Add (
                    CreateSerializer (property, CreateValueSerializer (property)));
            }
        }
        
        protected virtual Serializer<TContext> CreateSerializer (PropertyInfo property,
                                                                 Serializer<TContext> serializer)
        {
            return (obj, context) => serializer (property.GetValue (obj, null), context);
        }
        
        static Serializer<TContext> CreateSerializer (Serializer<TContext> serializer, bool omitIfNull)
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
        
        protected virtual Serializer<TContext> CreateAttributeSerializer (PropertyInfo property,
                                                                          XmlAttributeAttribute attributeAttribute)
        {
            return CreateSerializer (
                CreateAttributeSerializer (
                    property,
                    string.IsNullOrEmpty (attributeAttribute.Name) ? property.Name : attributeAttribute.Name,
                    attributeAttribute.Namespace,
                    attributeAttribute.Prefix),
                attributeAttribute.OmitIfNull);
        }
        
        protected virtual Serializer<TContext> CreateAttributeSerializer (PropertyInfo property,
                                                                          string name,
                                                                          string @namespace,
                                                                          string prefix)
        {
            if (!property.CanRead) {
                // TODO throw
            }
            
            if (property.PropertyType.IsEnum) {
                var map = GetEnumMap (property.PropertyType);
                return (obj, context) => {
                    context.Writer.WriteAttributeString (prefix, name, @namespace, map[obj]);
                };
            } else {
                return (obj, context) => {
                    context.Writer.WriteAttributeString (
                        prefix, name, @namespace, obj != null ? obj.ToString () : string.Empty);
                };
            }
        }
        
        protected virtual Serializer<TContext> CreateElementSerializer (PropertyInfo property,
                                                                        XmlElementAttribute elementAttribute)
        {
            return CreateSerializer (
                CreateElementSerializer (
                    property,
                    string.IsNullOrEmpty (elementAttribute.Name) ? property.Name : elementAttribute.Name,
                    elementAttribute.Namespace,
                    elementAttribute.Prefix),
                elementAttribute.OmitIfNull);
        }
        
        protected virtual Serializer<TContext> CreateElementSerializer (PropertyInfo property,
                                                                        string name,
                                                                        string @namespace,
                                                                        string prefix)
        {
            if (!property.CanRead) {
                // TODO throw
            }
            
            var next = GetCompilerForType (property.PropertyType).MemberSerializer;
            return (obj, context) => {
                context.Writer.WriteStartElement (prefix, name, @namespace);
                next (obj, context);
                context.Writer.WriteEndElement ();
            };
        }
        
        protected virtual Serializer<TContext> CreateArraySerializer (PropertyInfo property,
                                                                      XmlArrayAttribute arrayAttribute,
                                                                      XmlArrayItemAttribute arrayItemAttribute)
        {
            return CreateSerializer (
                CreateArraySerializer (
                    property,
                    string.IsNullOrEmpty (arrayAttribute.Name) ? property.Name : arrayAttribute.Name,
                    arrayAttribute.Namespace,
                    arrayAttribute.Prefix,
                    arrayAttribute.OmitIfEmpty,
                    arrayItemAttribute),
                arrayAttribute.OmitIfNull);
        }
        
        protected virtual Serializer<TContext> CreateArraySerializer (PropertyInfo property,
                                                                      string name,
                                                                      string @namespace,
                                                                      string prefix,
                                                                      bool omitIfEmpty,
                                                                      XmlArrayItemAttribute arrayItemAttribute)
        {
            if (!property.CanRead) {
                // TODO throw
            }
            
            var item_type = GetIEnumerable (property.PropertyType).GetGenericArguments ()[0];
            var next = CreateArrayItemSerializer (item_type, arrayItemAttribute);
            if (omitIfEmpty) {
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
        
        protected virtual Serializer<TContext> CreateArrayItemSerializer (Type type,
                                                                          XmlArrayItemAttribute arrayItemAttribute)
        {
            if (arrayItemAttribute == null || string.IsNullOrEmpty (arrayItemAttribute.Name)) {
                return GetCompilerForType (type).TypeSerializer;
            } else {
                var name = arrayItemAttribute.Name;
                var @namespace = arrayItemAttribute.Namespace;
                var prefix = arrayItemAttribute.Prefix;
                var next = GetCompilerForType (type).MemberSerializer;
                return (obj, context) => {
                    context.Writer.WriteStartElement (prefix, name, @namespace);
                    next (obj, context);
                    context.Writer.WriteEndElement ();
                };
            }
        }
        
        protected virtual Serializer<TContext> CreateArrayItemSerializer (PropertyInfo property,
                                                                          XmlArrayItemAttribute arrayItemAttribute)
        {
            if (!property.CanRead) {
                // TODO throw
            }
            
            if (string.IsNullOrEmpty (arrayItemAttribute.Name)) {
                return CreateArrayItemSerializer (property);
            } else {
                return CreateArrayItemSerializer (
                    property, arrayItemAttribute.Name, arrayItemAttribute.Namespace, arrayItemAttribute.Prefix);
            }
        }
        
        protected virtual Serializer<TContext> CreateArrayItemSerializer (PropertyInfo property)
        {
            var item_type = GetIEnumerable (property.PropertyType).GetGenericArguments ()[0];
            var serializer = GetCompilerForType (item_type).TypeSerializer;
            return (obj, context) => {
                if (obj != null) {
                    foreach (var item in (IEnumerable)obj)  {
                        serializer (item, context);
                    }
                }
            };
        }
        
        protected virtual Serializer<TContext> CreateArrayItemSerializer (PropertyInfo property,
                                                                          string name,
                                                                          string @namespace,
                                                                          string prefix)
        {
            var item_type = GetIEnumerable (property.PropertyType).GetGenericArguments ()[0];
            var serializer = GetCompilerForType (item_type).MemberSerializer;
            return (obj, context) => {
                if (obj != null) {
                    foreach (var item in (IEnumerable)obj) {
                        context.Writer.WriteStartElement (prefix, name, @namespace);
                        serializer (item, context);
                        context.Writer.WriteEndElement ();
                    }
                }
            };
        }
        
        protected virtual Serializer<TContext> CreateFlagSerializer (PropertyInfo property,
                                                                     XmlFlagAttribute flagAttribute)
        {
            return CreateFlagSerializer (
                property,
                string.IsNullOrEmpty (flagAttribute.Name) ? property.Name : flagAttribute.Name,
                flagAttribute.Namespace,
                flagAttribute.Prefix);
        }
        
        protected virtual Serializer<TContext> CreateFlagSerializer (PropertyInfo property,
                                                                     string name,
                                                                     string @namespace,
                                                                     string prefix)
        {
            if (property.PropertyType != typeof (bool)) {
                // TODO throw
            }
            
            return (obj, context) => {
                if ((bool)obj) {
                    context.Writer.WriteStartElement (prefix, name, @namespace);
                    context.Writer.WriteEndElement ();
                }
            };
        }
        
        protected static Type GetIEnumerable (Type type)
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
                    context.Writer.WriteString (obj.ToString ());
                }
            };
        }
    }
}
