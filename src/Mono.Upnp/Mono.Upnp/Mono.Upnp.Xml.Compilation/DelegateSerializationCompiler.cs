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
        protected struct SerializerInfo
        {
            public readonly Serializer<TContext> Serializer;
            public readonly int Order;
            
            public SerializerInfo (Serializer<TContext> serializer, int order)
            {
                Serializer = serializer;
                Order = order;
            }
        }
        
        public DelegateSerializationCompiler (XmlSerializer<TContext> xmlSerializer, Type type)
            : base (xmlSerializer, type)
        {
        }
        
        protected override Serializer<TContext> CreateTypeAutoSerializer ()
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
                }
                namespaces.Add (custom_attribute as XmlNamespaceAttribute);
            }
            
            if (type_attribute != null) {
                if (!string.IsNullOrEmpty (type_attribute.Name)) {
                    name = type_attribute.Name;
                }
                @namespace = type_attribute.Namespace;
                prefix = type_attribute.Prefix;
            }
            
            return CreateTypeAutoSerializer (name, @namespace, prefix, namespaces.Count == 0 ? null : namespaces.ToArray ());
        }
        
        protected virtual Serializer<TContext> CreateTypeAutoSerializer (string name, string @namespace, string prefix, IEnumerable<XmlNamespaceAttribute> namespaces)
        {
            var next = MemberSerializer;
            
            if (namespaces != null) {
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
        
        protected override Serializer<TContext> CreateMemberAutoSerializer ()
        {
            var attribute_serializers = new LinkedList<SerializerInfo> ();
            var element_serializers = new LinkedList<SerializerInfo> ();
            var attribute_serializers_head = attribute_serializers.Last;
            var element_serializers_head = attribute_serializers.Last;
            
            foreach (var property in Properties) {
                ProcessProperty (property,
                                 attribute_serializers,
                                 ref attribute_serializers_head,
                                 element_serializers,
                                 ref element_serializers_head);
            }
            
            var count = attribute_serializers.Count + element_serializers.Count;
            
            if (count == 0) {
                return (obj, context) => {
                    if (obj != null) {
                        context.Writer.WriteValue (obj.ToString ());
                    }
                };
            }
            
            var serializers = new Serializer<TContext>[count];
            var i = 0;
            foreach (var attribute_serializer in attribute_serializers) {
                serializers[i] = attribute_serializer.Serializer;
                i++;
            }
            foreach (var element_serializer in element_serializers) {
                serializers[i] = element_serializer.Serializer;
                i++;
            }
            
            return (obj, context) => {
                foreach (var serializer in serializers) {
                    serializer (obj, context);
                }
            };
        }
        
        protected virtual void ProcessProperty (PropertyInfo property,
                                                LinkedList<SerializerInfo> attributeSerializers,
                                                ref LinkedListNode<SerializerInfo> attributeSerializersHead,
                                                LinkedList<SerializerInfo> elementSerializers,
                                                ref LinkedListNode<SerializerInfo> elementSerializersHead)
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
                Insert (CreateSerializer (property, CreateAttributeSerializer (property, attribute_attribute)),
                        attribute_attribute.Order, attributeSerializers, ref attributeSerializersHead);
            } else if (element_attribute != null) {
                Insert (CreateSerializer (property, CreateElementSerializer (property, element_attribute)),
                        element_attribute.Order, elementSerializers, ref elementSerializersHead);
            } else if (flag_attribute != null) {
                Insert (CreateSerializer (property, CreateFlagSerializer (property, flag_attribute)),
                        flag_attribute.Order, elementSerializers, ref elementSerializersHead);
            } else if (array_attribute != null) {
                Insert (CreateSerializer (property, CreateArraySerializer (property, array_attribute, array_item_attribute)),
                        array_attribute.Order, elementSerializers, ref elementSerializersHead);
            } else if (array_item_attribute != null) {
                Insert (CreateSerializer (property, CreateArrayItemSerializer (property, array_item_attribute)),
                        array_item_attribute.Order, elementSerializers, ref elementSerializersHead);
            } else if (value_attribute != null) {
                elementSerializers.AddFirst (new SerializerInfo (
                    CreateSerializer (property, CreateValueSerializer (property)), 0));
            }
        }
        
        protected static void Insert (Serializer<TContext> serializer, int order, LinkedList<SerializerInfo> serializers, ref LinkedListNode<SerializerInfo> serializersHead)
        {
            if (order < 0) {
                if (serializers.Count == 0) {
                    serializers.AddFirst (new SerializerInfo (serializer, order));
                } else {
                    InsertAfterFirst (serializer, order, serializers);
                }
            } else if (order > 0) {
                if (serializers.Count == 0) {
                    serializers.AddLast (new SerializerInfo (serializer, order));
                } else {
                    InsertBeforeLast (serializer, order, serializers);
                }
            } else {
                if (serializersHead == null) {
                    serializersHead = InsertAfterFirst (serializer, order, serializers);
                } else {
                    serializers.AddAfter (serializersHead, new SerializerInfo (serializer, order));
                }
            }
        }
        
        static LinkedListNode<SerializerInfo> InsertAfterFirst (Serializer<TContext> serializer, int order, LinkedList<SerializerInfo> serializers)
        {
            var node = serializers.First;
            while (node.Value.Order < order && node.Next != null) {
                node = node.Next;
            }
            if (node.Value.Order < order) {
                return serializers.AddAfter (node, new SerializerInfo (serializer, order));
            } else {
                return serializers.AddBefore (node, new SerializerInfo (serializer, order));
            }
        }
        
        static LinkedListNode<SerializerInfo> InsertBeforeLast (Serializer<TContext> serializer, int order, LinkedList<SerializerInfo> serializers)
        {
            var node = serializers.Last;
            while (node.Value.Order > order && node.Previous != null) {
                node = node.Previous;
            }
            if (node.Value.Order > order) {
                return serializers.AddBefore (node, new SerializerInfo (serializer, order));
            } else {
                return serializers.AddAfter (node, new SerializerInfo (serializer, order));
            }
        }
        
        static Serializer<TContext> CreateSerializer (PropertyInfo property, Serializer<TContext> serializer)
        {
            return (obj, context) => { if (obj == null) Console.WriteLine (property); serializer (property.GetValue (obj, null), context); };
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
        
        Serializer<TContext> CreateAttributeSerializer (PropertyInfo property, XmlAttributeAttribute attributeAttribute)
        {
            return CreateSerializer (
                CreateAttributeSerializer (
                    property,
                    string.IsNullOrEmpty (attributeAttribute.Name) ? property.Name : attributeAttribute.Name,
                    attributeAttribute.Namespace,
                    attributeAttribute.Prefix),
                attributeAttribute.OmitIfNull);
        }
        
        protected virtual Serializer<TContext> CreateAttributeSerializer (PropertyInfo property, string name, string @namespace, string prefix)
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
                    context.Writer.WriteAttributeString (prefix, name, @namespace, obj != null ? obj.ToString () : string.Empty);
                };
            }
        }
        
        Serializer<TContext> CreateElementSerializer (PropertyInfo property, XmlElementAttribute elementAttribute)
        {
            return CreateSerializer (
                CreateElementSerializer (
                    property,
                    string.IsNullOrEmpty (elementAttribute.Name) ? property.Name : elementAttribute.Name,
                    elementAttribute.Namespace,
                    elementAttribute.Prefix),
                elementAttribute.OmitIfNull);
        }
        
        protected virtual Serializer<TContext> CreateElementSerializer (PropertyInfo property, string name, string @namespace, string prefix)
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
        
        Serializer<TContext> CreateArraySerializer (PropertyInfo property, XmlArrayAttribute arrayAttribute, XmlArrayItemAttribute arrayItemAttribute)
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
        
        protected virtual Serializer<TContext> CreateArraySerializer (PropertyInfo property, string name, string @namespace, string prefix, bool omitIfEmpty, XmlArrayItemAttribute arrayItemAttribute)
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
        
        Serializer<TContext> CreateArrayItemSerializer (Type type, XmlArrayItemAttribute arrayItemAttribute)
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
        
        Serializer<TContext> CreateArrayItemSerializer (PropertyInfo property, XmlArrayItemAttribute arrayItemAttribute)
        {
            if (!property.CanRead) {
                // TODO throw
            }
            
            if (string.IsNullOrEmpty (arrayItemAttribute.Name)) {
                var item_type = GetIEnumerable (property.PropertyType).GetGenericArguments ()[0];
                var serializer = GetCompilerForType (item_type).TypeSerializer;
                return (obj, context) => {
                    if (obj != null) {
                        foreach (var item in (IEnumerable)obj)  {
                            serializer (item, context);
                        }
                    }
                };
            } else {
                return CreateArrayItemSerializer (property, arrayItemAttribute.Name, arrayItemAttribute.Namespace, arrayItemAttribute.Prefix);
            }
        }
        
        protected virtual Serializer<TContext> CreateArrayItemSerializer (PropertyInfo property, string name, string @namespace, string prefix)
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
        
        Serializer<TContext> CreateFlagSerializer (PropertyInfo property, XmlFlagAttribute flagAttribute)
        {
            return CreateFlagSerializer (
                property,
                string.IsNullOrEmpty (flagAttribute.Name) ? property.Name : flagAttribute.Name,
                flagAttribute.Namespace,
                flagAttribute.Prefix);
        }
        
        protected virtual Serializer<TContext> CreateFlagSerializer (PropertyInfo property, string name, string @namespace, string prefix)
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
                    context.Writer.WriteString (obj.ToString ());
                }
            };
        }
    }
}
