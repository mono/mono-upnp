// 
// Xml.cs
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

namespace Mono.Upnp.Server.Serialization
{
    // XmlSerializer does not meet our needs because it really sucks teh big one.
    public static class Xml<T>
    {
        delegate void Serializer (object obj, XmlWriter writer);
        
        static Serializer type_serializer;
        static Serializer member_serializer;
        
        public static byte[] GetBytes (T obj)
        {
            using (var stream = new MemoryStream ()) {
                using (var writer = XmlWriter.Create (stream)) {
                    writer.WriteStartDocument ();
                    Serialize (obj, writer);
                    writer.WriteEndDocument ();
                    writer.Flush ();
                    stream.Seek (0, SeekOrigin.Begin);
                    var bytes = new byte[stream.Length];
                    stream.Read (bytes, 0, bytes.Length);
                    return bytes;
                }
            }
        }
        
        public static void Serialize (T obj, XmlWriter writer)
        {
            if (obj == null) throw new ArgumentNullException ("obj");
            if (writer == null) throw new ArgumentNullException ("writer");
            
            TypeSerializer (obj, writer);
        }
        
        static Serializer TypeSerializer {
            get {
                if (type_serializer == null) {
                    var name = typeof (T).Name;
                    string @namespace = null;
                    
                    foreach (var custom_attribute in typeof (T).GetCustomAttributes (false)) {
                        var serialize_type = custom_attribute as XmlTypeAttribute;
                        if (serialize_type != null) {
                            name = serialize_type.Name;
                            @namespace = serialize_type.Namespace;
                            break;
                        }
                    }
                    
                    var next = MemberSerializer;
                    
                    type_serializer = (obj, writer) => {
                        writer.WriteStartElement (name, @namespace);
                        next (obj, writer);
                        writer.WriteEndElement ();
                    };
                }
                return type_serializer;
            }
        }
        
        static Serializer MemberSerializer {
            get {
                if (member_serializer == null) {
                    var attribute_serializers = new List<Serializer>();
                    var element_serializers = new List<Serializer> ();
                    
                    foreach (var property in typeof (T).GetProperties ()) {
                        ProcessMember (property, attribute_serializers, element_serializers);
                    }
                    
                    attribute_serializers.AddRange (element_serializers);
                    var serializers = attribute_serializers.ToArray ();
                    if (serializers.Length > 0) {
                        member_serializer = (obj, writer) => {
                            foreach (var serializer in serializers) {
                                serializer (obj, writer);
                            }
                        };
                    } else {
                        member_serializer = (obj, writer) => writer.WriteValue (obj.ToString ());
                    }
                }
                return member_serializer;
            }
        }
        
        static void ProcessMember (PropertyInfo property,
                                   List<Serializer> attributeSerializers,
                                   List<Serializer> elementSerializers)
        {
            if (property.GetGetMethod () == null) {
                return;
            }
            
            XmlElementAttribute element_attribute = null;
            XmlFlagAttribute flag_attribute = null;
            XmlAttributeAttribute attribute_attribute = null;
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
                attributeSerializers.Add (ProcessAttribute (property, attribute_attribute));
                return;
            }
            
            if (element_attribute != null) {
                elementSerializers.Add (ProcessElement (property, element_attribute));
                return;
            }
            
            if (flag_attribute != null) {
                elementSerializers.Add (ProcessFlag (property, flag_attribute));
                return;
            }
            
            if (array_attribute != null) {
                elementSerializers.Add (ProcessArray (property, array_attribute, array_item_attributes));
                return;
            }
        }
        
        static Serializer ProcessNullable (Serializer serializer, bool omitIfNull)
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
        
        static Serializer ProcessAttribute (PropertyInfo property, XmlAttributeAttribute attributeAttribute)
        {
            return ProcessNullable (ProcessAttributeCore (property, attributeAttribute), attributeAttribute.OmitIfNull);
        }
        
        static Serializer ProcessAttributeCore (PropertyInfo property, XmlAttributeAttribute attributeAttribute)
        {
            var name = attributeAttribute.Name ?? property.Name;
            var @namespace = attributeAttribute.Namespace;
            return (obj, writer) =>
                writer.WriteAttributeString (name, @namespace, property.GetValue (obj, null).ToString ());
        }
        
        static Serializer ProcessElement (PropertyInfo property, XmlElementAttribute elementAttribute)
        {
            return ProcessNullable (ProcessElementCore (property, elementAttribute), elementAttribute.OmitIfNull);
        }
        
        static Serializer ProcessElementCore (PropertyInfo property, XmlElementAttribute elementAttribute)
        {
            var next = (Serializer)typeof (Xml<>).MakeGenericType (property.PropertyType).
                GetProperty ("MemberSerializer").GetGetMethod ().Invoke (null, null);
            var name = elementAttribute.Name ?? property.Name;
            var @namespace = elementAttribute.Namespace;
            return (obj, writer) => {
                writer.WriteStartElement (name, @namespace);
                next (property.GetValue (obj, null), writer);
                writer.WriteEndElement ();
            };
        }
        
        static Serializer ProcessArray (PropertyInfo property, XmlArrayAttribute arrayAttribute, XmlArrayItemAttribute arrayItemAttribute)
        {
            return ProcessNullable (ProcessArrayCore (property, arrayAttribute, arrayItemAttribute), arrayAttribute.OmitIfNull);
        }
        
        static Serializer ProcessArrayCore (PropertyInfo property, XmlArrayAttribute arrayAttribute, XmlArrayItemAttribute arrayItemAttribute)
        {
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
            
            var array_name = arrayAttribute.Name ?? property.Name;
            var array_namespace = arrayAttribute.Namespace;
            var serializer = typeof (Xml<>).MakeGenericType (ienumerable.GetGenericArguments ()[0]);
            if (arrayItemAttribute == null) {
                var next = (Action<object, XmlWriter>)serializer.GetProperty ("TypeSerializer").GetGetMethod ().Invoke (null, null);
                return (obj, writer) => {
                    writer.WriteStartElement (array_name, array_namespace);
                    foreach (var item in (IEnumerable)obj) {
                        next (item, writer);
                    }
                    writer.WriteEndElement ();
                };
            } else {
                var item_name = arrayItemAttribute.Name;
                var item_namespace = arrayItemAttribute.Namespace;
                var next = (Action<object, XmlWriter>)serializer.GetProperty ("MemberSerializer").GetGetMethod ().Invoke (null, null);
                return (obj, writer) => {
                    writer.WriteStartElement (array_name, array_namespace);
                    foreach (var item in (IEnumerable)obj) {
                        writer.WriteStartElement (item_name, item_namespace);
                        next (item, writer);
                        writer.WriteEndElement ();
                    }
                    writer.WriteEndElement ();
                };
            }
        }
        
        static Serializer ProcessFlag (PropertyInfo property, XmlFlagAttribute flagAttribute)
        {
            if (property.PropertyType != typeof (bool)) {
                // TODO throw
            }
            var name = flagAttribute.Name ?? property.Name;
            var @namespace = flagAttribute.Namespace;
            return (obj, writer) => {
                if ((bool)obj) {
                    writer.WriteStartElement (name, @namespace);
                    writer.WriteEndElement ();
                }
            };
        }
    }
}
