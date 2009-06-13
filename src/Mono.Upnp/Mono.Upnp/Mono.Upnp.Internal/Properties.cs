// 
// Properties.cs
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
using System.Collections.Generic;

using Mono.Upnp.Control;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Internal
{
    [XmlType ("propertyset", Protocol.EventSchema, "e")]
    sealed class Properties : XmlAutomatable
    {
        readonly IMap<string, StateVariable> map;
        
        public Properties (IMap<string, StateVariable> map)
        {
            this.map = map;
        }
        
        protected override void Deserialize (XmlDeserializationContext context)
        {
            var reader = context.Reader;
            if (reader.ReadToDescendant ("property", Protocol.EventSchema)) {
                do {
                    reader.Read ();
                    map[reader.LocalName].Value = reader.ReadElementContentAsString ();
                } while (reader.ReadToNextSibling ("property", Protocol.EventSchema));
            }
        }

        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            context.AutoSerializeObjectAndMembers (this);
        }

        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            var writer = context.Writer;
            foreach (var pair in map) {
                writer.WriteStartElement ("property", Protocol.EventSchema);
                writer.WriteElementString (pair.Key, pair.Value.Value);
                writer.WriteEndElement ();
            }
        }
    }
}
