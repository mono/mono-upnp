// 
// Resource.cs
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

using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    [XmlType ("res", Schemas.DidlLiteSchema)]
    public class Resource : XmlAutomatable
    {
        protected Resource ()
        {
        }
        
        public Resource (ResourceSettings settings)
        {
            if (settings == null) throw new ArgumentNullException ("settings");
            
            this.Uri = settings.Uri;
            this.Size = settings.Size;
            this.Duration = settings.Duration;
            this.Bitrate = settings.Bitrate;
            this.SampleFrequency = settings.SampleFrequency;
            this.BitsPerSample = settings.BitsPerSample;
            this.NrAudioChannels = settings.NrAudioChannels;
            this.Resolution = settings.Resolution;
            this.ColorDepth = settings.ColorDepth;
            this.ProtocolInfo = settings.ProtocolInfo;
            this.Protection = settings.Protection;
            this.ImportUri = settings.ImportUri;
        }
        
        
        [XmlValue]
        public virtual Uri Uri { get; protected set; }
        
        [XmlAttribute ("size", Schemas.DidlLiteSchema, OmitIfNull = true)]
        public virtual ulong? Size { get; protected set; }
        
        [XmlAttribute ("duration", Schemas.DidlLiteSchema, OmitIfNull = true)]
        public virtual TimeSpan? Duration { get; protected set; }

        [XmlAttribute ("bitrate", Schemas.DidlLiteSchema, OmitIfNull = true)]
        public virtual uint? Bitrate { get; protected set; }

        [XmlAttribute ("samplyFrequency", Schemas.DidlLiteSchema, OmitIfNull = true)]
        public virtual uint? SampleFrequency { get; protected set; }

        [XmlAttribute ("bitsPerSample", Schemas.DidlLiteSchema, OmitIfNull = true)]
        public virtual uint? BitsPerSample { get; protected set; }

        [XmlAttribute ("nrAudioChannels", Schemas.DidlLiteSchema, OmitIfNull = true)]
        public virtual uint? NrAudioChannels { get; protected set; }

        [XmlAttribute ("resolution", Schemas.DidlLiteSchema, OmitIfNull = true)]
        public virtual Resolution? Resolution { get; protected set; }

        [XmlAttribute ("colorDepth", Schemas.DidlLiteSchema, OmitIfNull = true)]
        public virtual uint? ColorDepth { get; protected set; }

        [XmlAttribute ("protocolInfo", Schemas.DidlLiteSchema, OmitIfNull = true)]
        public virtual string ProtocolInfo { get; protected set; }

        [XmlAttribute ("protection", Schemas.DidlLiteSchema, OmitIfNull = true)]
        public virtual string Protection { get; protected set; }

        [XmlAttribute ("importUri", Schemas.DidlLiteSchema, OmitIfNull = true)]
        public virtual Uri ImportUri { get; protected set; }
        
        protected override void Deserialize (XmlDeserializationContext context)
        {
            context.AutoDeserialize (this);
        }
        
        protected override void DeserializeAttribute (XmlDeserializationContext context)
        {
            context.AutoDeserializeAttribute (this);
        }

        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            context.AutoDeserializeElement (this);
        }

        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            context.AutoSerializeObjectAndMembers (this);
        }

        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            context.AutoSerializeMembersOnly (this);
        }
    }
}
