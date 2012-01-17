// 
// XmlShell.cs
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

using Mono.Upnp.Xml;

namespace Mono.Upnp.Internal
{
    class XmlShell<T> : XmlAutomatable
    {
        static readonly string local_name;
        static readonly string @namespace;
        static readonly string prefix;
        
        static XmlShell ()
        {
            var xml_types = typeof (T).GetCustomAttributes (typeof (XmlTypeAttribute), true);
            if (xml_types.Length == 0) {
                throw new TypeLoadException ("XmlShell may only be closed with a type annotated with XmlType");
            }
            var xml_type = (XmlTypeAttribute)xml_types[0];
            local_name = xml_type.Name;
            @namespace = xml_type.Namespace;
            prefix = xml_type.Prefix;
        }
        
        public XmlShell ()
        {
        }
        
        public XmlShell (T value)
        {
            Value = value;
        }
        
        public T Value { get; set; }
        
        protected override void Deserialize (XmlDeserializationContext context)
        {
            if (context.Reader.ReadToDescendant (local_name, @namespace)) {
                using (var reader = context.Reader.ReadSubtree ()) {
                    reader.Read ();
                    Value = context.Deserialize<T> ();
                }
            }
        }
        
        protected override void SerializeMembers (XmlSerializationContext context)
        {
            context.Writer.WriteStartElement (prefix, local_name, @namespace);
            context.Serialize (Value);
            context.Writer.WriteEndElement ();
        }
        
        protected override void Serialize (XmlSerializationContext context)
        {
            throw new InvalidOperationException ("An XmlShell cannot be the root of a serialization operation.");
        }
    }
}
