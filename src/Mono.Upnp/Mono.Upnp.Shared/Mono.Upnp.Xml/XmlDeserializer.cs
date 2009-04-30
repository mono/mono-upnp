// 
// XmlDeserializer.cs
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
    public sealed class XmlDeserializer
    {
        delegate void Action ();
        delegate object Factory ();
        delegate object Deserializer (XmlDeserializationContext context);
        delegate void ObjectDeserializer (object obj, XmlDeserializationContext context);
        
        struct TypeDeserializer
        {
            public bool DeserializeSubclasses;
            public MethodInfo Method;
        }
        
        class Deserializers
        {
            public Deserializer Deserializer;
            public ObjectDeserializer AutoDeserializer;
            public ObjectDeserializer AttributeDeserializer;
            public ObjectDeserializer AttributeAutoDeserializer;
            public ObjectDeserializer ElementDeserializer;
            public ObjectDeserializer ElementAutoDeserializer;
            public List<TypeDeserializer> TypeDeserializers;
        }
        
        readonly Dictionary<Type, Factory> factories = new Dictionary<Type, Factory> ();
        readonly Dictionary<Type, Deserializers> deserializers = new Dictionary<Type, Deserializers> ();
        
        public void RegisterFactory<T> (IFactory<T> factory)
        {
            if (factory == null) throw new ArgumentNullException ("factory");

            var type = typeof (IFactory<T>);
            if (!factories.ContainsKey (type)) {
                // Where is generic variance when you need it?
                var create = type.GetMethod ("Create");
                factories[type] = () => create.Invoke (factory, null);
            }
        }
        
        public T Deserialize<T> (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");
            
            return DeserializeCore<T> (reader);
        }
        
        T DeserializeCore<T> (XmlReader reader)
        {
            var deserializer = GetDeserializer (typeof (T));
            return (T) deserializer (new XmlDeserializationContext (this, reader));
        }
        
        internal void AutoDeserialize<T> (T obj, XmlDeserializationContext context)
        {
            var deserializer = GetAutoDeserializer (typeof (T));
            deserializer (obj, context);
        }
        
        internal void AutoDeserializeAttribute<T> (T obj, XmlDeserializationContext context)
        {
            var deserializer = GetAttributeAutoDeserializer (typeof (T));
            deserializer (obj, context);
        }
        
        internal void AutoDeserializeElement<T> (T obj, XmlDeserializationContext context)
        {
            var deserializer = GetElementAutoDeserializer (typeof (T));
            deserializer (obj, context);
        }
        
        Deserializers GetDeserializers (Type type)
        {
            if (!deserializers.ContainsKey (type)) {
                return deserializers[type] = new Deserializers ();
            }
            return deserializers[type];
        }
        
        Deserializer GetDeserializer (Type type)
        {
            var deserializers = GetDeserializers (type);
            if (deserializers.Deserializer == null) {
                Deserializer deserializer = null;
                deserializers.Deserializer = context => deserializer (context);
                deserializer = CreateDeserializer (type, deserializers);
                return deserializers.Deserializer = deserializer;
            }
            return deserializers.Deserializer;
        }
        
        Deserializer CreateDeserializer (Type type, Deserializers deserializers)
        {
            if (type == typeof (string)) {
                return context => context.Reader.ReadElementContentAsString ();
            } else if (type == typeof (int)) {
                return context => context.Reader.ReadElementContentAsInt ();
            } else if (type == typeof (double)) {
                return context => context.Reader.ReadElementContentAsDouble ();
            } else if (type == typeof (bool)) {
                return context => context.Reader.ReadElementContentAsBoolean ();
            } else if (type == typeof (long)) {
                return context => context.Reader.ReadElementContentAsLong ();
            } else if (type == typeof (float)) {
                return context => context.Reader.ReadElementContentAsFloat ();
            } else if (type == typeof (decimal)) {
                return context => context.Reader.ReadElementContentAsDecimal ();
            } else if (type == typeof (DateTime)) {
                return context => context.Reader.ReadElementContentAsDateTime ();
            } else {
                var factory = GetFactory (type);
                if (typeof (IXmlDeserializable).IsAssignableFrom (type)) {
                    return context => {
                        var obj = factory ();
                        ((IXmlDeserializable)obj).Deserialize (context);
                        return obj;
                    };
                } else {
                    var deserializer = GetAutoDeserializer (type, deserializers);
                    return context => {
                        var obj = factory ();
                        deserializer (obj, context);
                        return obj;
                    };
                }
            }
        }
        
        ObjectDeserializer GetAutoDeserializer (Type type)
        {
            return GetAutoDeserializer (type, GetDeserializers (type));
        }
        
        ObjectDeserializer GetAutoDeserializer (Type type, Deserializers deserializers)
        {
            if (deserializers.AutoDeserializer == null) {
                return deserializers.AutoDeserializer = CreateAutoDeserializer (type, deserializers);
            }
            return deserializers.AutoDeserializer;
        }
        
        Factory GetFactory (Type type)
        {
            if (factories.ContainsKey (type)) {
                return factories[type];
            } else {
                // TODO check for public parameterless constructor
                return () => Activator.CreateInstance (type);
            }
        }
        
        ObjectDeserializer CreateAutoDeserializer (Type type, Deserializers deserializers)
        {
            deserializers.TypeDeserializers = GetTypeDeserializers (type);
            var attribute_deserializer = GetAttributeDeserializer (type, deserializers);
            var element_deserializer = GetElementDeserializer (type, deserializers);
            return (obj, context) => {
                try {
                    while (context.Reader.MoveToNextAttribute ()) {
                        try {
                            attribute_deserializer (obj, context);
                        } catch {
                        }
                    }
                    var depth = context.Reader.Depth;
                    while (context.Reader.Read () && context.Reader.NodeType == XmlNodeType.Element && context.Reader.Depth > depth) {
                        var element_reader = context.Reader.ReadSubtree ();
                        element_reader.Read ();
                        try {
                            element_deserializer (obj, new XmlDeserializationContext (this, element_reader));
                        } catch {
                        } finally {
                            element_reader.Close ();
                        }
                    }
                } catch {
                }
            };
        }
        
        List<TypeDeserializer> GetTypeDeserializers (Type type)
        {
            List<TypeDeserializer> type_deserializers = null;
            foreach (var method in type.GetMethods ()) {
                var attributes =  method.GetCustomAttributes (typeof (XmlTypeDeserializerAttribute), false);
                if (attributes.Length != 0) {
                    if (method.ReturnType == typeof (void)) {
                        // TODO throw
                        continue;
                    }
                    var parameters = method.GetParameters ();
                    if (parameters.Length != 1 || parameters[0].ParameterType != typeof (XmlDeserializationContext)) {
                        // TODO throw
                        continue;
                    }
                    if (type_deserializers == null) {
                        type_deserializers = new List<TypeDeserializer> ();
                    }
                    var type_deserializer_attribute = (XmlTypeDeserializerAttribute)attributes[0];
                    type_deserializers.Add (new TypeDeserializer {
                        DeserializeSubclasses = type_deserializer_attribute.DeserializeSubclasses,
                        Method = method
                    }); 
                }
            }
            return type_deserializers;
        }
        
        ObjectDeserializer GetAttributeDeserializer (Type type, Deserializers deserializers)
        {
            if (deserializers.AttributeDeserializer == null) {
                if (typeof (IXmlDeserializable).IsAssignableFrom (type)) {
                    return deserializers.AttributeDeserializer = (obj, context) => ((IXmlDeserializable)obj).DeserializeAttribute (context);
                } else {
                    return deserializers.AttributeDeserializer = GetAttributeAutoDeserializer (type, deserializers);
                }
            }
            return deserializers.AttributeDeserializer;
        }
        
        ObjectDeserializer GetAttributeAutoDeserializer (Type type)
        {
            return GetAttributeAutoDeserializer (type, GetDeserializers (type));
        }
        
        ObjectDeserializer GetAttributeAutoDeserializer (Type type, Deserializers deserializers)
        {
            if (deserializers.AttributeAutoDeserializer == null) {
                return deserializers.AttributeAutoDeserializer = CreateAutoDeserializer (
                    CreateAttributeAutoDeserializers (type, deserializers));
            }
            return deserializers.AttributeAutoDeserializer;
        }
        
        ObjectDeserializer CreateAutoDeserializer (Dictionary<string, ObjectDeserializer> deserializers)
        {
            if (deserializers.Count == 0) {
                return (obj, context) => {};
            } else {
                return (obj, context) => {
                    var name = CreateName (context.Reader.LocalName, context.Reader.NamespaceURI);
                    if (deserializers.ContainsKey (name)) {
                        deserializers[name] (obj, context);
                    }
                };
            }
        }
        
        Dictionary<string, ObjectDeserializer> CreateAttributeAutoDeserializers (Type type, Deserializers deserializers)
        {
            var object_deserializers = new Dictionary<string, ObjectDeserializer> ();
            foreach (var property in type.GetProperties ()) {
                var attributes = property.GetCustomAttributes (typeof (XmlAttributeAttribute), false);
                if (attributes.Length != 0) {
                    var attribute_attribute = (XmlAttributeAttribute)attributes[0];
                    var deserialization_type = GetType (attribute_attribute.Type, property.PropertyType);
                    var deserializer =
                        CreateCustomDeserializer (property, deserialization_type, deserializers) ??
                        CreateAttributeDeserializer (property, deserialization_type);
                    AddDeserializer (object_deserializers,
                        CreateName (property.Name, attribute_attribute.Name, attribute_attribute.Namespace),
                        deserializer);
                }
            }
            return object_deserializers;
        }
        
        ObjectDeserializer CreateAttributeDeserializer (PropertyInfo property, Type type)
        {
            if (!property.CanWrite) {
                // TODO throw
            }
            var deserializer = CreateAttributeDeserializer (type);
            return (obj, context) => property.SetValue (obj, deserializer (context), null);
        }
                    
        ObjectDeserializer CreateCustomDeserializer (PropertyInfo property, Type type, Deserializers deserializers)
        {
            if (!property.CanWrite) {
                // TODO throw
            }
            if (deserializers.TypeDeserializers != null) {
                foreach (var type_deserializer in deserializers.TypeDeserializers) {
                    var method = type_deserializer.Method;
                    if (type == method.ReturnType || (type_deserializer.DeserializeSubclasses && type.IsAssignableFrom (method.ReturnType))) {
                        return (obj, context) => property.SetValue (obj, method.Invoke (obj, new object [] { context }), null);
                    }
                }
            }
            return null;
        }
        
        Deserializer CreateAttributeDeserializer (Type type)
        {
            if (type == typeof (string)) {
                return context => context.Reader.ReadContentAsString ();
            } else if (type == typeof (int)) {
                return context => context.Reader.ReadContentAsInt ();
            } else if (type == typeof (double)) {
                return context => context.Reader.ReadContentAsDouble ();
            } else if (type == typeof (bool)) {
                return context => context.Reader.ReadContentAsBoolean ();
            } else if (type == typeof (long)) {
                return context => context.Reader.ReadContentAsLong ();
            } else if (type == typeof (float)) {
                return context => context.Reader.ReadContentAsFloat ();
            } else if (type == typeof (decimal)) {
                return context => context.Reader.ReadContentAsDecimal ();
            } else if (type == typeof (DateTime)) {
                return context => context.Reader.ReadContentAsDateTime ();
            } else {
                return context => context.Reader.ReadContentAs (type, null);
            }
        }
        
        ObjectDeserializer GetElementDeserializer (Type type, Deserializers deserializers)
        {
            if (deserializers.ElementDeserializer == null) {
                if (typeof (IXmlDeserializable).IsAssignableFrom (type)) {
                    return deserializers.ElementDeserializer = (obj, context) => ((IXmlDeserializable)obj).DeserializeElement (context);
                } else {
                    return deserializers.ElementDeserializer = GetElementAutoDeserializer (type, deserializers);
                }
            }
            return deserializers.ElementDeserializer;
        }
        
        ObjectDeserializer GetElementAutoDeserializer (Type type)
        {
            return GetElementAutoDeserializer (type, GetDeserializers (type));
        }
        
        ObjectDeserializer GetElementAutoDeserializer (Type type, Deserializers deserializers)
        {
            if (deserializers.ElementAutoDeserializer == null) {
                return deserializers.ElementAutoDeserializer = CreateAutoDeserializer (CreateElementAutoDeserializers (type, deserializers));
            }
            return deserializers.ElementAutoDeserializer;
        }
        
        Dictionary<string, ObjectDeserializer> CreateElementAutoDeserializers (Type type, Deserializers deserializers)
        {
            var object_deserializers = new Dictionary<string, ObjectDeserializer> ();
            
            foreach (var property in type.GetProperties ()) {
                XmlElementAttribute element_attribute = null;
                XmlFlagAttribute flag_attribute = null;
                XmlArrayAttribute array_attribute = null;
                XmlArrayItemAttribute array_item_attribute = null;
                
                foreach (var custom_attribute in property.GetCustomAttributes (false)) {
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
                
                if (element_attribute != null) {
                    var deserialization_type = GetType (element_attribute.Type, property.PropertyType);
                    var deserializer =
                        CreateCustomDeserializer (property, deserialization_type, deserializers) ??
                        CreateElementDeserializer (property, deserialization_type);
                    AddDeserializer (object_deserializers,
                        CreateName (property.Name, element_attribute.Name, element_attribute.Namespace),
                        deserializer);
                    continue;
                }
                
                if (flag_attribute != null) {
                    AddDeserializer (object_deserializers,
                        CreateName (property.Name, flag_attribute.Name, flag_attribute.Namespace),
                        CreateElementDeserializer (property));
                    continue;
                }
                
                if (array_attribute != null) {
                    AddDeserializer (object_deserializers,
                        CreateName (property.Name, array_attribute.Name, array_attribute.Namespace),
                        CreateElementDeserializer (property, array_item_attribute));
                    continue;
                }
            }
            
            return object_deserializers;
        }
        
        ObjectDeserializer CreateElementDeserializer (PropertyInfo property, Type type)
        {
            if (!property.CanWrite) {
                // TODO throw
            }
            var next = GetDeserializer (type);
            return (obj, context) => property.SetValue (obj, next (context), null);
        }
        
        ObjectDeserializer CreateElementDeserializer (PropertyInfo property)
        {
            if (!property.CanWrite) {
                // TODO throw
            }
            return (obj, context) => property.SetValue (obj, true, null);
        }
        
        ObjectDeserializer CreateElementDeserializer (PropertyInfo property, Type type, XmlArrayItemAttribute arrayItemAttribute)
        {
            if (!property.CanWrite) {
                // TODO throw
            }
            
            var icollection = type.GetInterface ("ICollection`1");
            if (icollection == null) {
                // TODO throw
            }
            
            var add = icollection.GetMethod ("Add");
            var item_type = GetType (arrayItemAttribute.Type, icollection.GetGenericArguments ()[0]);
            var item_deserializer = GetDeserializer (item_type);
            return (obj, context) => {
                var collection = Activator.CreateInstance (type);
                var depth = context.Reader.Depth;
                while (context.Reader.Read () && context.Reader.NodeType == XmlNodeType.Element && context.Reader.Depth > depth) {
                    var item_reader = context.Reader.ReadSubtree ();
                    item_reader.Read ();
                    try {
                        add.Invoke (collection, new object [] { item_deserializer (new XmlDeserializationContext (this, item_reader)) }); 
                    } catch {
                    } finally {
                        item_reader.Close ();
                    }
                }
            };
        }
        
        static Type GetType (Type specifiedType, Type propertyType)
        {
            if (specifiedType != null) {
                if (!propertyType.IsAssignableFrom (specifiedType)) {
                    // TODO throw
                }
                return specifiedType;
            } else {
                return propertyType;
            }
        }
        
        static string CreateName (string name, string @namespace)
        {
            return CreateName (null, name, @namespace);
        }
        
        static string CreateName (string backupName, string name, string @namespace)
        {
            return string.Format ("{0}/{1}", name ?? backupName, @namespace);
        }
        
        static void AddDeserializer (Dictionary<string, ObjectDeserializer> deserializers, string name, ObjectDeserializer deserializer)
        {
            if (deserializers.ContainsKey (name)) {
                // TODO throw
            }
            deserializers[name] = deserializer;
        }
    }
}
