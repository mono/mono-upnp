// 
// Serializer.cs
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
using System.Xml;

namespace Mono.Upnp.Server.Serialization
{
    // XmlSerializer does not meet our needs because it really sucks teh big one.
    public static class Xml<T>
    {
        delegate void Action<T1, T2> (T1 t1, T2 t2);
        
        static Action<object, XmlWriter> type_serializer;
        static Action<object, XmlWriter> member_serializer;
        
        public static void Serialize (T obj, XmlWriter writer)
        {
            TypeSerializer (obj, writer);
        }
        
        static Action<object, XmlWriter> TypeSerializer {
            get {
                if (type_serializer == null) {
                    string name = null, @namespace = null;
                    foreach (var custom_attribute in typeof (T).GetCustomAttributes (false)) {
                        var serialize_type = custom_attribute as SerializeTypeAttribute;
                        if (serialize_type != null) {
                            name = serialize_type.Name;
                            @namespace = serialize_type.Namespace;
                            break;
                        }
                    }
                    
                    if (name == null) {
                        name = typeof (T).Name;
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
        
        static Action<object, XmlWriter> MemberSerializer {
            get {
                if (member_serializer == null) {
                    var attribute_serializers = new List<Action<object, XmlWriter>>();
                    var element_serializers = new List<Action<object, XmlWriter>> ();
                    
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
                                   List<Action<object, XmlWriter>> attributeSerializers,
                                   List<Action<object, XmlWriter>> elementSerializers)
        {
            if (property.GetGetMethod () == null) {
                return;
            }
            
            foreach (var custom_attribute in property.GetCustomAttributes (false)) {
                var element = custom_attribute as SerializeElementAttribute;
                if (element != null) {
                    elementSerializers.Add (ProcessElement (property, element.Name, element.Namespace));
                }
                
                var attribute = custom_attribute as SerializeAttributeAttribute;
                if(attribute != null) {
                    attributeSerializers.Add (ProcessAttribute (property, attribute.Name));
                }
                
                var enumerable = custom_attribute as SerializeEnumerableAttribute;
                if (enumerable != null) {
                    elementSerializers.Add (ProcessEnumerable (property, enumerable.Name, enumerable.Namespace));
                }
            }
        }
        
        static Action<object, XmlWriter> ProcessElement (PropertyInfo property, string name, string @namespace)
        {
            var next = (Action<object, XmlWriter>)typeof (Xml<>).MakeGenericType (property.PropertyType).
                GetProperty ("MemberSerializer").GetGetMethod ().Invoke (null, null);
            return (obj, writer) => {
                writer.WriteStartElement (name, @namespace);
                next (property.GetValue (obj, null), writer);
                writer.WriteEndElement ();
            };
        }
        
        static Action<object, XmlWriter> ProcessEnumerable (PropertyInfo property, string name, string @namespace)
        {
            var ienumerable = property.PropertyType.GetInterface ("IEnumerable`1");
            if (ienumerable != null) {
                var next = (Action<object, XmlWriter>)typeof (Xml<>).MakeGenericType (ienumerable.GetGenericArguments ()[0]).
                    GetProperty ("TypeSerializer").GetGetMethod ().Invoke (null, null);
                return (obj, writer) => {
                    writer.WriteStartElement (name, @namespace);
                    foreach (var item in (IEnumerable)obj) {
                        next (item, writer);
                    }
                    writer.WriteEndElement ();
                };
            }  else {
                // TODO throw
                return null;
            }
        }
        
        static Action<object, XmlWriter> ProcessAttribute (PropertyInfo property, string name)
        {
            return (obj, writer) =>
                writer.WriteAttributeString (name, property.GetValue (obj, null).ToString ());
        }
    }
}
