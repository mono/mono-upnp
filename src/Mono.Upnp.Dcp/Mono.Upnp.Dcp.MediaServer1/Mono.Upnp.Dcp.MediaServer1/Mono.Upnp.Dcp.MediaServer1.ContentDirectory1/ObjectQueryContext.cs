// 
// ObjectQueryProvider.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Thomas
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

using Mono.Upnp.Dcp.MediaServer1.Xml;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public class ObjectQueryContext
    {
        static readonly object[] empty_args = new object[0];
        readonly ObjectQueryContext parentContext;
        Dictionary<string, PropertyVisitor> properties = new Dictionary<string, PropertyVisitor> ();

        public ObjectQueryContext (Type type)
            : this (type, null)
        {
        }

        public ObjectQueryContext (Type type, ObjectQueryContext parentContext)
        {
            if (type == null) {
                throw new ArgumentNullException ("type");
            }

            this.parentContext = parentContext;

            foreach (var property_info in GetProperties (type, BindingFlags.DeclaredOnly)) {
                foreach (var attribute in property_info.GetCustomAttributes (false)) {
                    if (ProcessElementAttribute (property_info, attribute as XmlElementAttribute)) {
                        break;
                    } else if (ProcessAttributeAttribute (property_info, attribute as XmlAttributeAttribute)) {
                        break;
                    } else if (ProcessArrayItemAttribute (property_info, attribute as XmlArrayItemAttribute)) {
                        break;
                    }
                }
            }
        }

        bool ProcessElementAttribute (PropertyInfo propertyInfo, XmlElementAttribute attribute)
        {
            if (attribute != null) {
                var name = string.IsNullOrEmpty (attribute.Name) ? propertyInfo.Name : attribute.Name;
                var id = PropertyName.CreateForElement (name, attribute.Prefix);
                var property = new ValuePropertyVisitor (attribute.OmitIfNull, propertyInfo);
                ProcessNestedAttributes (id, property);
                properties[id] = property;
                return true;
            } else {
                return false;
            }
        }

        bool ProcessAttributeAttribute (PropertyInfo propertyInfo, XmlAttributeAttribute attribute)
        {
            if (attribute != null) {
                var name = string.IsNullOrEmpty (attribute.Name) ? propertyInfo.Name : attribute.Name;
                var id = PropertyName.CreateForAttribute (name, attribute.Prefix, null);
                properties[id] = new ValuePropertyVisitor (attribute.OmitIfNull, propertyInfo);
                return true;
            } else {
                return false;
            }
        }

        bool ProcessArrayItemAttribute (PropertyInfo propertyInfo, XmlArrayItemAttribute attribute)
        {
            if (attribute != null) {
                var name = string.IsNullOrEmpty (attribute.Name) ? propertyInfo.Name : attribute.Name;
                var id = PropertyName.CreateForElement (name, attribute.Prefix);
                var property = new EnumerationPropertyVisitor (propertyInfo);
                ProcessNestedAttributes (id, property);
                properties[id] = property;
                return true;
            } else {
                return false;
            }
        }

        void ProcessNestedAttributes (string nestedName, PropertyVisitor parentProperty)
        {
            foreach (var propertyInfo in GetProperties (parentProperty.PropertyInfo.PropertyType, 0)) {
                var attributes = propertyInfo.GetCustomAttributes (typeof (XmlAttributeAttribute), false);
                if (attributes.Length != 0) {
                    var attribute_attribute = (XmlAttributeAttribute)attributes[0];
                    var name = string.IsNullOrEmpty (attribute_attribute.Name)
                        ? propertyInfo.Name
                        : attribute_attribute.Name;
                    var id = PropertyName.CreateForAttribute (name, attribute_attribute.Prefix, nestedName);
                    properties[id] = new NestedValuePropertyVisitor (
                        parentProperty, attribute_attribute.OmitIfNull, propertyInfo);
                }
            }
        }

        public void VisitProperty (string property, object @object, Action<object> consumer)
        {
            PropertyVisitor property_visitor;
            if (properties.TryGetValue (property, out property_visitor)) {
                property_visitor.Visit (@object, value => consumer (value));
            } else if (parentContext != null) {
                parentContext.VisitProperty (property, @object, consumer);
            }
        }

        public void CompareProperty (string property, object @object, string value, Action<int> consumer)
        {
            PropertyVisitor property_visitor;
            if (properties.TryGetValue (property, out property_visitor)) {
                property_visitor.Visit (@object, val => {
                    if (property_visitor.ParseMethod != null) {
                        var parsed_value = property_visitor.ParseMethod.Invoke (null, new[] { value });
                        consumer (((IComparable)val).CompareTo (parsed_value));
                    } else {
                        // TODO throw?
                    }
                });
            } else if (parentContext != null) {
                parentContext.CompareProperty (property, @object, value, consumer);
            }
        }

        public bool PropertyExists (string property, object @object)
        {
            PropertyVisitor property_visitor;
            if (properties.TryGetValue (property, out property_visitor)) {
                return property_visitor.Exists (@object);
            } else if (parentContext != null) {
                return parentContext.PropertyExists (property, @object);
            } else {
                return false;
            }
        }

        static IEnumerable<PropertyInfo> GetProperties (Type type, BindingFlags flags)
        {
            foreach (var property in type.GetProperties (flags | BindingFlags.Instance | BindingFlags.Public)) {
                yield return property;
            }

            foreach (var property in type.GetProperties (flags | BindingFlags.Instance | BindingFlags.NonPublic)) {
                yield return property;
            }
        }

        abstract class PropertyVisitor
        {
            public readonly PropertyInfo PropertyInfo;
            public readonly MethodInfo ParseMethod;

            protected PropertyVisitor (PropertyInfo propertyInfo)
            {
                this.PropertyInfo = propertyInfo;
                var property_type = propertyInfo.PropertyType;
                if (property_type.IsGenericType && property_type.GetGenericTypeDefinition () == typeof (Nullable<>)) {
                    property_type = property_type.GetGenericArguments ()[0];
                }
                if (typeof (IComparable).IsAssignableFrom (property_type)) {
                    this.ParseMethod = property_type.GetMethod (
                        "Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof (string) }, null);
                }
            }

            public abstract void Visit (object @object, Action<object> consumer);

            public abstract bool Exists (object @object);
        }

        class ValuePropertyVisitor : PropertyVisitor
        {
            protected readonly bool omit_if_null;

            public ValuePropertyVisitor (bool omitIfNull, PropertyInfo propertyInfo)
                : base (propertyInfo)
            {
                this.omit_if_null = omitIfNull;
            }

            public override void Visit (object @object, Action<object> consumer)
            {
                var value = PropertyInfo.GetValue (@object, empty_args);
                if (value != null || !omit_if_null) {
                    consumer (value);
                }
            }

            public override bool Exists (object @object)
            {
                if (!omit_if_null) {
                    return true;
                } else {
                    return PropertyInfo.GetValue (@object, empty_args) != null;
                }
            }
        }

        class NestedValuePropertyVisitor : ValuePropertyVisitor
        {
            readonly PropertyVisitor parent_property_visitor;

            public NestedValuePropertyVisitor (PropertyVisitor parentPropertyVisitor,
                                               bool omitIfNull,
                                               PropertyInfo propertyInfo)
                : base (omitIfNull, propertyInfo)
            {
                this.parent_property_visitor = parentPropertyVisitor;
            }

            public override void Visit (object @object, Action<object> consumer)
            {
                parent_property_visitor.Visit (@object, value => base.Visit (value, v => consumer (v)));
            }

            public override bool Exists (object @object)
            {
                var exists = false;
                parent_property_visitor.Visit (@object, value => exists = base.Exists (value));
                return exists;
            }
        }

        class EnumerationPropertyVisitor : PropertyVisitor
        {
            public EnumerationPropertyVisitor (PropertyInfo propertyInfo)
                : base (propertyInfo)
            {
            }

            public override void Visit (object @object, Action<object> consumer)
            {
                foreach (var value in (IEnumerable)PropertyInfo.GetValue (@object, empty_args)) {
                    consumer(value);
                }
            }

            public override bool Exists (object @object)
            {
                var enumerable = (IEnumerable)PropertyInfo.GetValue (@object, empty_args);
                return enumerable != null && enumerable.GetEnumerator ().MoveNext ();
            }
        }
    }
}
