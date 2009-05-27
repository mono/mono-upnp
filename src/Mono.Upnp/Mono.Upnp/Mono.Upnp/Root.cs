// 
// Root.cs
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

using Mono.Upnp.Control;
using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp
{
    [XmlType ("root", Protocol.DeviceUrn)]
    public class Root : DescriptionBase
    {
        internal protected Root (Deserializer deserializer, Uri url)
            : base (deserializer)
        {
            if (url == null) throw new ArgumentNullException ("url");
            
            UrlBase = url;
        }
        
        internal protected Root (XmlSerializer serializer)
        {
            if (serializer == null) throw new ArgumentNullException ("serializer");
            
            //this.serializer = serializer;
        }
        
        public Version SpecVersion { get; protected set; }
        
        [XmlAttribute ("configId")]
        public virtual string ConfigurationId { get; protected set; }
        
        [XmlElement ("URLBase", Protocol.DeviceUrn, OmitIfNull = true)]
        public virtual Uri UrlBase { get; protected set; }
        
        [XmlElement ("device", Protocol.DeviceUrn)]
        public virtual Device RootDevice { get; protected set; }
		
		[XmlTypeDeserializer]
		Uri DeserializeUri (XmlDeserializationContext context)
		{
			return new Uri (context.Reader.ReadString ());
		}
        
        [XmlTypeDeserializer]
        protected virtual Device DeserializeDevice (XmlDeserializationContext context)
        {
            return Deserializer != null ? Deserializer.DeserializeDevice (context) : null;
        }
        
        protected override void DeserializeAttribute (XmlDeserializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoDeserializeAttribute (this);
        }
        
        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            if (context.Reader.LocalName == "specVersion" && context.Reader.NamespaceURI == Protocol.DeviceUrn) {
                context.Reader.Read ();
                var major = context.Reader.ReadElementContentAsInt ("major", Protocol.DeviceUrn);
                var minor = context.Reader.ReadElementContentAsInt ("minor", Protocol.DeviceUrn);
                SpecVersion = new Version (major, minor);
            } else {
                context.AutoDeserializeElement (this);
            }
        }
        
        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoSerializeObjectAndMembers (this);
        }
        
        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.Writer.WriteStartElement ("specVersion", Protocol.DeviceUrn);
            context.Writer.WriteStartElement ("major", Protocol.DeviceUrn);
            context.Writer.WriteValue (SpecVersion.Major);
            context.Writer.WriteEndElement ();
            context.Writer.WriteStartElement ("minor", Protocol.DeviceUrn);
            context.Writer.WriteValue (SpecVersion.Minor);
            context.Writer.WriteEndElement ();
            context.Writer.WriteEndElement ();
            
            context.AutoSerializeMembersOnly (this);
        }
    }
}
