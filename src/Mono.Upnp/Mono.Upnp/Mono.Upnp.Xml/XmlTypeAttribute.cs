// 
// SerializeTypeAttribute.cs
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

namespace Mono.Upnp.Xml
{
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class XmlTypeAttribute : Attribute
    {
        readonly string name;
        readonly string @namespace;
        readonly string prefix;
        
        public XmlTypeAttribute (string name)
            : this (name, null)
        {
        }
        
        public XmlTypeAttribute (string name, string @namespace)
            : this (name, @namespace, null)
        {
        }
        
        public XmlTypeAttribute (string name, string @namespace, string prefix)
        {
            this.name = name;
            this.@namespace = @namespace;
            this.prefix = prefix;
        }
        
        public string Name {
            get { return name; }
        }
        
        public string Namespace {
            get { return @namespace; }
        }
        
        public string Prefix {
            get { return prefix; }
        }
    }
}
