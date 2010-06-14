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
            return (obj, context) => {
                var other_obj = context.Context.OtherValue;
                if (other_obj != null) {
                    if (!property.GetValue (obj, empty_args).Equals (property.GetValue (other_obj, empty_args))) {
                        context.Writer.Flush ();
                        context.Context.DelineateUpdate ();
                        serializerDelegate (obj, CreateContext (context.Writer, new UpdateContext ()));
                    }
                } else {
                    serializerDelegate (obj, context);
                }
            };
        }
    }
}
