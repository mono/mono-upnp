// 
// FilteringDelegateSerializationCompiler.cs
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
    public class FilteringDelegateSerializationCompiler : DelegateSerializationCompiler<FilteringContext>
    {
        public FilteringDelegateSerializationCompiler (XmlSerializer<FilteringContext> xmlSerializer, Type type)
            : base (xmlSerializer, type)
        {
        }

        protected override Serializer<FilteringContext> CreateAttributeSerializer (PropertyInfo property,
                                                                                   XmlAttributeAttribute attributeAttribute)
        {
            var serializer = base.CreateAttributeSerializer (property, attributeAttribute);
            if (attributeAttribute.OmitIfNull) {
                var name = attributeAttribute.Name;
                var @namespace = attributeAttribute.Namespace;
                return (obj, context) => {
                    string prefix;
                    if (string.IsNullOrEmpty (@namespace)) {
                        prefix = null;
                    } else {
                        prefix = context.Writer.LookupPrefix (@namespace);
                    }
                    var id = PropertyName.CreateForAttribute (name, prefix, context.Context.NestedPropertyName);
                    if (context.Context.IncludesAttribute (id)) {
                        serializer (obj, context);
                    }
                };
            } else {
                return serializer;
            }
        }

        protected override Serializer<FilteringContext> CreateElementSerializer (PropertyInfo property,
                                                                                 XmlElementAttribute elementAttribute)
        {
            var serializer = base.CreateElementSerializer (property, elementAttribute);
            if (elementAttribute.OmitIfNull) {
                var name = elementAttribute.Name;
                var @namespace = elementAttribute.Namespace;
                return (obj, context) => {
                    string prefix;
                    if (string.IsNullOrEmpty (@namespace)) {
                        prefix = null;
                    } else {
                        prefix = context.Writer.LookupPrefix (@namespace);
                    }
                    var id = PropertyName.CreateForElement (name, prefix);
                    if (context.Context.IncludesElement (id)) {
                        if (context.Context.Type == Type) {
                            context = CreateContext (context.Writer, context.Context.GetNestedContext (id));
                        }
                        serializer (obj, context);
                    }
                };
            } else {
                return serializer;
            }
        }
    }
}

