// 
// Root.cs
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

using Mono.Upnp.Control;
using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp
{
    [XmlType ("root", Protocol.DeviceSchema)]
    public class Root : Description, IXmlDeserializer<Device>
    {
        protected Root ()
        {
        }
        
        protected internal Root (Deserializer deserializer, Uri url)
            : base (deserializer)
        {
            if (url == null) {
                throw new ArgumentNullException ("url");
            }
            
            UrlBase = url;
        }
        
        public Root (DeviceType type, string udn, string friendlyName, string manufacturer, string modelName)
            : this (type, udn, friendlyName, manufacturer, modelName, null)
        {
        }
        
        public Root (DeviceType type,
                     string udn,
                     string friendlyName,
                     string manufacturer,
                     string modelName,
                     DeviceOptions options)
        {
            // The device knows how to handle embedded devices itself so don't pass them in
            RootDevice = new Device(type, udn, friendlyName, manufacturer, modelName, options, null);
            SpecVersion = new SpecVersion (1, 1);
        }
        
        [XmlAttribute ("configId")]
        public virtual string ConfigurationId { get; protected set; }
        
        [XmlElement ("specVersion")]
        public virtual SpecVersion SpecVersion { get; protected set; }
        
        [DoNotSerialize]
        [XmlElement ("URLBase")]
        public virtual Uri UrlBase { get; protected set; }
        
        [XmlElement ("device")]
        public virtual Device RootDevice { get; protected set; }
        
        protected internal virtual void Initialize (XmlSerializer serializer, Uri url)
        {
            UrlBase = url;
            if (RootDevice != null) {
                RootDevice.Initialize (serializer, this, "");
            }
        }
        
        protected internal virtual void Start ()
        {
            if (RootDevice != null) {
                RootDevice.Start ();
            }
        }
        
        protected internal virtual void Stop ()
        {
            if (RootDevice != null) {
                RootDevice.Stop ();
            }
        }
        
        Device IXmlDeserializer<Device>.Deserialize (XmlDeserializationContext context)
        {
            return DeserializeDevice (context);
        }
        
        protected virtual Device DeserializeDevice (XmlDeserializationContext context)
        {
            return Deserializer != null ? Deserializer.DeserializeDevice (context) : null;
        }
        
        protected override void DeserializeAttribute (XmlDeserializationContext context)
        {
            AutoDeserializeAttribute (this, context);
        }
        
        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            AutoDeserializeElement (this, context);
        }
        
        protected override void Serialize (XmlSerializationContext context)
        {
            AutoSerialize (this, context);
        }
        
        protected override void SerializeMembers (XmlSerializationContext context)
        {
            AutoSerializeMembers (this, context);
        }
    }
}
