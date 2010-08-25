// 
// VirtualDelegateSerializationCompiler.cs
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
using System.Collections.Generic;
using System.Reflection;

using Mono.Upnp.Xml;
using Mono.Upnp.Xml.Compilation;

namespace Mono.Upnp.Dcp.MediaServer1.Xml
{
    public class VirtualDelegateSerializationCompiler : DelegateSerializationCompiler<VirtualContext>
    {
        public VirtualDelegateSerializationCompiler (XmlSerializer<VirtualContext> xmlSerializer, Type type)
            : base (xmlSerializer, type)
        {
        }

        protected override Serializer<VirtualContext> CreateAttributeSerializer (PropertyInfo property,
                                                                                 string name,
                                                                                 string @namespace,
                                                                                 string prefix)
        {
            var serializer = base.CreateAttributeSerializer (property, name, @namespace, prefix);
            return CreateSerializer (name, @namespace, serializer);
        }

        protected override Serializer<VirtualContext> CreateElementSerializer (PropertyInfo property,
                                                                               string name,
                                                                               string @namespace,
                                                                               string prefix)
        {
            var serializer = base.CreateElementSerializer (property, name, @namespace, prefix);
            return CreateSerializer (name, @namespace, serializer);
        }

        static Serializer<VirtualContext> CreateSerializer (string name,
                                                            string @namespace,
                                                            Serializer<VirtualContext> serializer)
        {
            return (obj, context) => {
                foreach (var @override in context.Context.Overrides) {
                    if (@override.Name == name && @override.Namespace == @namespace) {
                        serializer (@override.Value, context);
                        return;
                    }
                }
                serializer (obj, context);
            };
        }
    }
}
