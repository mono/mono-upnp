// 
// UpdateDelegateSerializationCompiler.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Peterson
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

using Mono.Upnp.Xml;
using Mono.Upnp.Xml.Compilation;

namespace Mono.Upnp.Dcp.MediaServer1.Xml
{
    public class UpdateDelegateSerializationCompiler : DelegateSerializationCompiler<UpdateContext>
    {
        static object[] empty_args = new object[0];
        
        public UpdateDelegateSerializationCompiler (XmlSerializer<UpdateContext> xmlSerializer, Type type)
            : base (xmlSerializer, type)
        {
        }
        
        protected override Serializer<UpdateContext> CreateTypeAutoSerializer (string name, string @namespace, string prefix, IEnumerable<XmlNamespaceAttribute> namespaces)
        {
            var type_auto_serializer = base.CreateTypeAutoSerializer (name, @namespace, prefix, namespaces);
            var member_serializer = MemberSerializer;
            return (obj, context) => {
                var other_obj = context.Context.OtherValue;
                if (other_obj != null) {
                    if (!Type.IsAssignableFrom (other_obj.GetType ())) {
                        throw new InvalidOperationException ("Two object must be serialize as a common compatable type for an update.");
                    }
                    member_serializer (obj, context);
                } else {
                    type_auto_serializer (obj, context);
                }
            };
        }
        
        protected override Serializer<UpdateContext> CreateSerializer (PropertyInfo property, Serializer<UpdateContext> serializer)
        {
            var serializerDelegate = base.CreateSerializer (property, serializer);
            if (property.GetCustomAttributes (typeof (XmlArrayItemAttribute), false).Length != 0) {
                return serializerDelegate;
            }
            return (obj, context) => {
                var other_obj = context.Context.OtherValue;
                if (other_obj != null) {
                    if (!object.Equals (property.GetValue (obj, empty_args), property.GetValue (other_obj, empty_args))) {
                        context.Writer.Flush ();
                        context.Context.DelineateUpdate ();
                        serializerDelegate (obj, CreateContext (context.Writer, new UpdateContext ()));
                    }
                } else {
                    serializerDelegate (obj, context);
                }
            };
        }
        
        protected override Serializer<UpdateContext> CreateArrayItemSerializer (PropertyInfo property)
        {
            var item_type = GetIEnumerable (property.PropertyType).GetGenericArguments ()[0];
            var serializer = GetCompilerForType (item_type).TypeSerializer;
            Serializer<UpdateContext> item_serializer = (obj, context) => serializer (obj, CreateContext (context.Writer, new UpdateContext ()));
            return CreateArrayItemSerializer (property, item_serializer);
        }
        
        protected override Serializer<UpdateContext> CreateArrayItemSerializer (PropertyInfo property, string name, string @namespace, string prefix)
        {
            var item_type = GetIEnumerable (property.PropertyType).GetGenericArguments ()[0];
            var serializer = GetCompilerForType (item_type).MemberSerializer;
            Serializer<UpdateContext> item_serializer = (obj, context) => {
                context.Writer.WriteStartElement (prefix, name, @namespace);
                serializer (obj, CreateContext (context.Writer, new UpdateContext ()));
                context.Writer.WriteEndElement ();
            };
            return CreateArrayItemSerializer (property, item_serializer);
        }
        
        static Serializer<UpdateContext> CreateArrayItemSerializer (PropertyInfo property, Serializer<UpdateContext> serializer)
        {
            return (obj, context) => {
                var other_obj = context.Context.OtherValue;
                if (other_obj != null) {
                    var other_enumerable = property.GetValue (other_obj, empty_args) as IEnumerable;
                    if (other_enumerable == null) {
                        if (obj != null) {
                            foreach (var item in (IEnumerable)obj)  {
                                context.Writer.Flush ();
                                context.Context.DelineateUpdate ();
                                serializer (item, context);
                            }
                        }
                    } else if (obj == null) {
                        context.Writer.Flush ();
                        var other_enumerator = other_enumerable.GetEnumerator ();
                        while (other_enumerator.MoveNext ()) {
                           context.Context.DelineateUpdate ();
                        }
                    } else {
                        var enumerator = ((IEnumerable)obj).GetEnumerator ();
                        var other_enumerator = other_enumerable.GetEnumerator ();
                        while (enumerator.MoveNext ()) {
                            if (other_enumerator.MoveNext ()) {
                                if (!Object.Equals (enumerator.Current, other_enumerator.Current)) {
                                    context.Writer.Flush ();
                                    context.Context.DelineateUpdate ();
                                    if (enumerator.Current != null) {
                                        serializer (enumerator.Current, context);
                                    }
                                }
                            } else {
                                context.Writer.Flush ();
                                context.Context.DelineateUpdate ();
                                serializer (enumerator.Current, context);
                            }
                        }
                        while (other_enumerator.MoveNext ()) {
                            context.Writer.Flush ();
                            context.Context.DelineateUpdate ();
                        }
                    }
                } else if (obj != null) {
                    foreach (var item in (IEnumerable)obj)  {
                        context.Writer.Flush ();
                        context.Context.DelineateUpdate ();
                        serializer (item, context);
                    }
                }
            };
        }
    }
}
