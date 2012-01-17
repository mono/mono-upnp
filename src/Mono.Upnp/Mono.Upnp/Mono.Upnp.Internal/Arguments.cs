// 
// Arguments.cs
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
using System.Xml;

using Mono.Upnp.Control;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Internal
{
    sealed class Arguments : XmlAutomatable
    {
        readonly IDictionary<string, string> arguments;
        readonly bool response;
        
        Arguments ()
        {
            arguments = new Dictionary<string, string> ();
        }
        
        public Arguments (string serviceType, string actionName, IDictionary<string, string> arguments)
            : this (serviceType, actionName, arguments, false)
        {
        }
        
        public Arguments (string serviceType,
                          string actionName,
                          IDictionary<string, string> arguments,
                          bool response)
        {
            ServiceType = serviceType;
            ActionName = actionName;
            this.arguments = arguments;
            this.response = response;
        }
        
        public string ServiceType { get; private set; }
        
        public string ActionName { get; private set; }
        
        public IDictionary<string, string> Values {
            get { return arguments; }
        }
        
        protected override void Serialize (XmlSerializationContext context)
        {
            SerializeMembers (context);
        }
        
        protected override void SerializeMembers (XmlSerializationContext context)
        {
            var writer = context.Writer;
            writer.WriteStartElement ("u", response ? ActionName + "Response" : ActionName, ServiceType);
            foreach (var argument in arguments) {
                writer.WriteElementString (argument.Key, argument.Value);
            }
            writer.WriteEndElement ();
        }
        
        protected override void Deserialize (XmlDeserializationContext context)
        {
            // TODO clean this for safety
            var reader = context.Reader;
            Helper.ReadToNextElement (reader);
            ActionName = reader.LocalName;
            ServiceType = reader.NamespaceURI;
            var depth = reader.Depth;
            Helper.ReadToNextElement (reader);
            while (reader.NodeType == XmlNodeType.Element && reader.Depth > depth) {
                arguments.Add (reader.LocalName, reader.ReadElementContentAsString ());
                while (reader.NodeType != XmlNodeType.Element && reader.Depth > depth) {
                    reader.Read ();
                }
            }
        }
    }
}
