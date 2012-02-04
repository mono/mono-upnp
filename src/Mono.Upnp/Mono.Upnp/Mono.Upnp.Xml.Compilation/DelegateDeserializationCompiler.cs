// 
// DelegateDeserializationCompiler.cs
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

using Mono.Upnp.Internal;

namespace Mono.Upnp.Xml.Compilation
{
    public class DelegateDeserializationCompiler : DeserializationCompiler
    {
        delegate object ItemDeserializer (object obj, XmlDeserializationContext context);
        delegate void ElementDeserializer (object obj, XmlDeserializationContext context, int depth);
        
        Dictionary<Type, MethodInfo> type_deserializers;
        bool has_processed_type_deserializers;
        
        PropertyInfo value_property;
        Dictionary<string, ObjectDeserializer> attribute_deserializers;
        Dictionary<string, ObjectDeserializer> element_deserializers;
        
        public DelegateDeserializationCompiler (XmlDeserializer xmlDeserializer, Type type)
            : base (xmlDeserializer, type)
        {
        }
        
        Dictionary<Type, MethodInfo> TypeDeserializers {
            get {
                if (!has_processed_type_deserializers) {
                    has_processed_type_deserializers = true;
                    ProcessTypeDeserializers ();
                }
                return type_deserializers;
            }
        }
        
        PropertyInfo ValueProperty {
            get {
                CheckHasProcessedType ();
                return value_property;
            }
        }
        
        Dictionary<string, ObjectDeserializer> AttributeDeserializers {
            get {
                CheckHasProcessedType ();
                return attribute_deserializers;
            }
        }
        
        Dictionary<string, ObjectDeserializer> ElementDeserializers {
            get {
                CheckHasProcessedType ();
                return element_deserializers;
            }
        }
        
        void CheckHasProcessedType ()
        {
            if (attribute_deserializers == null) {
                ProcessType ();
            }
        }
        
        protected override Deserializer CreateDeserializer ()
        {
            if (Type == typeof (string)) {
                return context => context.Reader.ReadElementContentAsString ();
            } else if (Type == typeof (int)) {
                return context => context.Reader.ReadElementContentAsInt ();
            } else if (Type == typeof (uint)) {
                return context => uint.Parse (context.Reader.ReadElementContentAsString ());
            } else if (Type == typeof (double)) {
                return context => context.Reader.ReadElementContentAsDouble ();
            } else if (Type == typeof (bool)) {
                return context => context.Reader.ReadElementContentAsBoolean ();
            } else if (Type == typeof (long)) {
                return context => context.Reader.ReadElementContentAsLong ();
            } else if (Type == typeof (ulong)) {
                return context => ulong.Parse (context.Reader.ReadElementContentAsString ());
            } else if (Type == typeof (float)) {
                return context => context.Reader.ReadElementContentAsFloat ();
            } else if (Type == typeof (decimal)) {
                return context => context.Reader.ReadElementContentAsDecimal ();
            } else if (Type == typeof (DateTime)) {
                return context => context.Reader.ReadElementContentAsDateTime ();
            } else if (Type == typeof (TimeSpan)) {
                return context => TimeSpan.Parse (context.Reader.ReadElementContentAsString ());
            } else if (Type == typeof (Uri)) {
                return context => {
                    var url = context.Reader.ReadElementContentAsString ();
                    if (url.Length != 0) {
                        try {
                            Uri uri = new Uri (url);
                            return uri;
                        } catch {
                        }
                    }
                    return null;
                };
            } else if (Type.IsEnum) {
                var map = GetEnumMap (Type);
                return context => map[context.Reader.ReadElementContentAsString ()];
            } else if (Type.IsGenericType && Type.GetGenericTypeDefinition () == typeof (Nullable<>)) {
                var type = Type.GetGenericArguments ()[0];
                var deserializer = GetDeserializerForType (type);
                var constructor = Type.GetConstructor (new[] { type });
                return context => constructor.Invoke (new[] { deserializer (context) });
            } else {
                // TODO check for default ctor
                if (typeof (IXmlDeserializable).IsAssignableFrom (Type)) {
                    return context => {
                        var obj = Activator.CreateInstance (Type, true);
                        ((IXmlDeserializable)obj).Deserialize (context);
                        return obj;
                    };
                } else {
                    var deserializer = AutoDeserializer;
                    return context => {
                        var obj = Activator.CreateInstance (Type, true);
                        deserializer (obj, context);
                        return obj;
                    };
                }
            }
        }
        
        protected override ObjectDeserializer CreateAutoDeserializer ()
        {
            var attribute_deserializer = CreateAttributeDeserializer ();
            var element_deserializer = CreateElementDeserializer ();
            return (obj, context) => {
                try {
                    var depth = context.Reader.Depth;
                    while (context.Reader.MoveToNextAttribute ()) {
                        try {
                            attribute_deserializer (obj, context);
                        } catch {
                            throw;
                        }
                    }
                    context.Reader.MoveToElement ();
                    element_deserializer (obj, context, depth);
                } catch {
                    throw;
                }
            };
        }
        
        ObjectDeserializer CreateAttributeDeserializer ()
        {
            if (typeof (IXmlDeserializable).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlDeserializable)obj).DeserializeAttribute (context);
            } else {
                return AttributeAutoDeserializer;
            }
        }
        
        protected override ObjectDeserializer CreateAttributeAutoDeserializer ()
        {
            var attribute_deserializers = AttributeDeserializers;
            
            if (attribute_deserializers.Count == 0) {
                return (obj, context) => {};
            } else {
                return (obj, context) => {
                    var name = CreateName (context.Reader.LocalName, context.Reader.NamespaceURI);
                    if (attribute_deserializers.ContainsKey (name)) {
                        attribute_deserializers[name] (obj, context);
                    }
                };
            }
        }
        
        ElementDeserializer CreateElementDeserializer ()
        {
            if (ValueProperty != null && ValueProperty.CanWrite) {
                var property = ValueProperty;
                var deserializer = GetDeserializerForType (property.PropertyType);
                return (obj, context, depth) => property.SetValue (obj, deserializer (context), null);
            }
            
            var element_deserializer = CreateSubElementDeserializer ();
            return (obj, context, depth) => {
                while (Helper.ReadToNextElement (context.Reader) && context.Reader.Depth > depth) {
                    var element_reader = context.Reader.ReadSubtree ();
                    element_reader.Read ();
                    try {
                        element_deserializer (obj, CreateDeserializationContext (element_reader));
                    } catch {
                        throw;
                    } finally {
                        element_reader.Close ();
                    }
                }
            };
        }
        
        ObjectDeserializer CreateSubElementDeserializer ()
        {
            if (typeof (IXmlDeserializable).IsAssignableFrom (Type)) {
                return (obj, context) => ((IXmlDeserializable)obj).DeserializeElement (context);
            } else {
                return ElementAutoDeserializer;
            }
        }
        
        protected override ObjectDeserializer CreateElementAutoDeserializer ()
        {
            var object_deserializers = ElementDeserializers;
            
            if (object_deserializers.Count == 0) {
                return (obj, context) => {};
            } else {
                return (obj, context) => {
                    var name = CreateName (context.Reader.LocalName, context.Reader.NamespaceURI);
                    if (object_deserializers.ContainsKey (name)) {
                        object_deserializers[name] (obj, context);
                    } else if (object_deserializers.ContainsKey (context.Reader.Name)) {
                        object_deserializers[context.Reader.Name] (obj, context);
                    } else {
                        // TODO this is a workaround for mono bug 334752 and another problem
                        context.Reader.Skip ();
                    }
                };
            }
        }
        
        void ProcessTypeDeserializers ()
        {
            foreach (var @interface in Type.GetInterfaces ()) {
                if (@interface.IsGenericType
                    && @interface.GetGenericTypeDefinition () == typeof (IXmlDeserializer<>))
                {
                    var type = @interface.GetGenericArguments()[0];
                    if (type_deserializers == null) {
                        type_deserializers = new Dictionary<Type, MethodInfo> ();
                    } else if (type_deserializers.ContainsKey (type)) {
                        // TODO throw
                    }
                    type_deserializers[type] = Type.GetInterfaceMap (@interface).TargetMethods[0];
                }
            }
        }
        
        void ProcessType ()
        {
            attribute_deserializers = new Dictionary<string, ObjectDeserializer> ();
            element_deserializers = new Dictionary<string, ObjectDeserializer> ();
            
            foreach (var property in Properties) {
                XmlElementAttribute element_attribute = null;
                XmlFlagAttribute flag_attribute = null;
                XmlArrayAttribute array_attribute = null;
                XmlArrayItemAttribute array_item_attribute = null;
                XmlAttributeAttribute attribute_attribute = null;
                
                foreach (var custom_attribute in property.GetCustomAttributes (false)) {
                    if (custom_attribute is DoNotDeserializeAttribute) {
                        element_attribute = null;
                        flag_attribute = null;
                        array_attribute = null;
                        attribute_attribute = null;
                        value_property = null;
                        break;
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
                    
                    var attribute = custom_attribute as XmlAttributeAttribute;
                    if (attribute != null) {
                        attribute_attribute = attribute;
                        continue;
                    }
                    
                    if (custom_attribute is XmlValueAttribute) {
                        // TODO check if this isn't null and throw
                        value_property = property;
                        continue;
                    }
                }
                
                if (element_attribute != null) {
                    var deserializer =
                        CreateCustomDeserializer (property) ??
                        CreateElementDeserializer (property);
                    AddDeserializer (element_deserializers,
                        CreateName (property.Name, element_attribute.Name, element_attribute.Namespace),
                        deserializer);
                    continue;
                }
                
                if (flag_attribute != null) {
                    AddDeserializer (element_deserializers,
                        CreateName (property.Name, flag_attribute.Name, flag_attribute.Namespace),
                        CreateFlagDeserializer (property));
                    continue;
                }
                
                if (array_attribute != null) {
                    AddDeserializer (element_deserializers,
                        CreateName (property.Name, array_attribute.Name, array_attribute.Namespace),
                        CreateArrayElementDeserializer (property));
                    continue;
                } else if (array_item_attribute != null) {
                    var name = array_item_attribute.Name;
                    var @namespace = array_item_attribute.Namespace;
                    if (string.IsNullOrEmpty (name)) {
                        var item_type = GetICollection (property.PropertyType).GetGenericArguments ()[0];
                        var type_attribute = item_type.GetCustomAttributes (typeof (XmlTypeAttribute), false);
                        if (type_attribute.Length == 0) {
                            name = item_type.Name;
                        } else {
                            var xml_type = (XmlTypeAttribute)type_attribute[0];
                            name = string.IsNullOrEmpty (xml_type.Name) ? item_type.Name : xml_type.Name;
                            @namespace = xml_type.Namespace;
                        }
                    }
                    AddDeserializer (element_deserializers,
                        CreateName (name, @namespace),
                        CreateArrayItemElementDeserializer (property));
                }
                
                if (attribute_attribute != null) {
                    var deserializer =
                        CreateCustomDeserializer (property) ??
                        CreateAttributeDeserializer (property);
                    AddDeserializer (attribute_deserializers,
                        CreateName (property.Name, attribute_attribute.Name, attribute_attribute.Namespace),
                        deserializer);
                    continue;
                }
            }
        }
                    
        ObjectDeserializer CreateCustomDeserializer (PropertyInfo property)
        {
            if (!property.CanWrite) {
                // TODO throw
            }
            
            if (TypeDeserializers != null && TypeDeserializers.ContainsKey (property.PropertyType)) {
                var method = TypeDeserializers[property.PropertyType];
                return (obj, context) => property.SetValue (obj, method.Invoke (obj, new[] { context }), null);
            }
            
            return null;
        }
        
        ObjectDeserializer CreateAttributeDeserializer (PropertyInfo property)
        {
            if (!property.CanWrite) {
                // TODO throw
            }
            
            var deserializer = CreateAttributeDeserializer (property.PropertyType);
            return (obj, context) => property.SetValue (obj, deserializer (context), null);
        }
        
        static Deserializer CreateAttributeDeserializer (Type type)
        {
            if (type == typeof (string)) {
                return context => context.Reader.ReadContentAsString ();
            } else if (type == typeof (int)) {
                return context => context.Reader.ReadContentAsInt ();
            } else if (type == typeof (uint)) {
                return context => uint.Parse (context.Reader.ReadContentAsString ());
            } else if (type == typeof (double)) {
                return context => context.Reader.ReadContentAsDouble ();
            } else if (type == typeof (bool)) {
                return context => context.Reader.ReadContentAsBoolean ();
            } else if (type == typeof (long)) {
                return context => context.Reader.ReadContentAsLong ();
            } else if (type == typeof (ulong)) {
                return context => ulong.Parse (context.Reader.ReadContentAsString ());
            } else if (type == typeof (float)) {
                return context => context.Reader.ReadContentAsFloat ();
            } else if (type == typeof (decimal)) {
                return context => context.Reader.ReadContentAsDecimal ();
            } else if (type == typeof (DateTime)) {
                return context => context.Reader.ReadContentAsDateTime ();
            } else if (type == typeof (TimeSpan)) {
                return context => TimeSpan.Parse (context.Reader.ReadContentAsString ());
            } else if (type == typeof (Uri)) {
                return context => {
                    var url = context.Reader.ReadContentAsString ();
                    if (url.Length != 0) {
                        try {
                            Uri uri = new Uri (url);
                            return uri;
                        } catch {
                        }
                    }
                    return null;     
                };
            } else if (type.IsEnum) {
                var map = GetEnumMap (type);
                return context => map[context.Reader.ReadContentAsString ()];
            }  else if (type.IsGenericType && type.GetGenericTypeDefinition () == typeof (Nullable<>)) {
                var nullable_type = type.GetGenericArguments ()[0];
                var deserializer = CreateAttributeDeserializer (nullable_type);
                var constructor = type.GetConstructor (new[] { nullable_type });
                return context => constructor.Invoke (new[] { deserializer (context) });
            } else {
                return context => context.Reader.ReadContentAs (type, null);
            }
        }
        
        // TODO we could use a trie for this and save some memory
        static Dictionary<string, object> GetEnumMap (Type type)
        {
            var fields = type.GetFields (BindingFlags.Public | BindingFlags.Static);
            var dictionary = new Dictionary<string, object> (fields.Length);
            foreach (var field in fields) {
                var enum_attribute = field.GetCustomAttributes (typeof (XmlEnumAttribute), false);
                string name;
                if (enum_attribute.Length!= 0) {
                    name = ((XmlEnumAttribute)enum_attribute[0]).Value;
                } else {
                    name = field.Name;
                }
                dictionary.Add (name, field.GetValue (null));
            }
            return dictionary;
        }
        
        ObjectDeserializer CreateElementDeserializer (PropertyInfo property)
        {
            if (!property.CanWrite) {
                // TODO throw
            }
            var next = GetDeserializerForType (property.PropertyType);
            return (obj, context) => property.SetValue (obj, next (context), null);
        }
        
        ObjectDeserializer CreateFlagDeserializer (PropertyInfo property)
        {
            if (!property.CanWrite) {
                // TODO throw
            }
            return (obj, context) => property.SetValue (obj, true, null);
        }
        
        ItemDeserializer CreateItemDeserializer (Type type)
        {
            if (TypeDeserializers != null && TypeDeserializers.ContainsKey (type)) {
                var method = TypeDeserializers[type];
                return (obj, context) => method.Invoke (obj, new[] { context });
            } else {
                var deserializer = GetDeserializerForType (type);
                return (obj, context) => deserializer (context);
            }
        }
        
        ObjectDeserializer CreateArrayElementDeserializer (PropertyInfo property)
        {
            if (!property.CanRead) {
                // TODO throw
            }
            
            var icollection = GetICollection (property.PropertyType);
            var add = icollection.GetMethod ("Add");
            var item_deserializer = CreateItemDeserializer (icollection.GetGenericArguments ()[0]);
            return (obj, context) => {
                var collection = property.GetValue (obj, null);
                var depth = context.Reader.Depth;
                while (Helper.ReadToNextElement (context.Reader) && context.Reader.Depth > depth) {
                    var item_reader = context.Reader.ReadSubtree ();
                    item_reader.Read ();
                    try {
                        add.Invoke (collection,
                            new[] { item_deserializer (obj, CreateDeserializationContext (item_reader)) });
                    } catch {
                        throw;
                    } finally {
                        item_reader.Close ();
                    }
                }
            };
        }
        
        ObjectDeserializer CreateArrayItemElementDeserializer (PropertyInfo property)
        {
            if (!property.CanRead) {
                // TODO throw
            }
            
            var icollection = GetICollection (property.PropertyType);
            var add = icollection.GetMethod ("Add");
            var item_deserializer = CreateItemDeserializer (icollection.GetGenericArguments ()[0]);
            return (obj, context) => {
                var collection = property.GetValue (obj, null);
                add.Invoke (collection, new[] { item_deserializer (obj, context) });
            };
        }
        
        static string CreateName (string name, string @namespace)
        {
            return CreateName (null, name, @namespace);
        }
        
        static string CreateName (string backupName, string name, string @namespace)
        {
            if (string.IsNullOrEmpty (@namespace)) {
                return name ?? backupName;
            } else {
                return string.Format ("{0}/{1}", name ?? backupName, @namespace);
            }
        }
        
        static void AddDeserializer (Dictionary<string, ObjectDeserializer> deserializers,
                                     string name,
                                     ObjectDeserializer deserializer)
        {
            if (deserializers.ContainsKey (name)) {
                // TODO throw
            }
            deserializers[name] = deserializer;
        }
        
        static Type GetICollection (Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition () == typeof (ICollection<>)) {
                return type;
            } else {
               var icollection = type.GetInterface ("ICollection`1");
                if (icollection != null) {
                    return icollection;
                } else {
                    // TODO throw
                    return null;
                }
            }
        }
    }
}
