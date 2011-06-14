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
using Mono.Upnp.Dcp.MediaServer1.ConnectionManager1;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    [XmlType ("res", Schemas.DidlLiteSchema)]
    public class Resource : XmlAutomatable, IXmlDeserializer<ProtocolInfo>
    {
        protected Resource ()
        {
        }
        
        public Resource (Uri uri, ResourceOptions options)
        {
            if (uri == null) {
                throw new ArgumentNullException ("uri");
            } else if (options == null) {
                throw new ArgumentNullException ("options");
            }
            
            Uri = uri;
            Size = options.Size;
            Duration = options.Duration;
            BitRate = options.BitRate;
            SampleFrequency = options.SampleFrequency;
            BitsPerSample = options.BitsPerSample;
            NrAudioChannels = options.NrAudioChannels;
            Resolution = options.Resolution;
            ColorDepth = options.ColorDepth;
            ProtocolInfo = options.ProtocolInfo;
            Protection = options.Protection;
            ImportUri = options.ImportUri;
        }

        protected void CopyToOptions (ResourceOptions options)
        {
            if (options == null) {
                throw new ArgumentNullException ("options");
            }

            options.Size = Size;
            options.Duration = Duration;
            options.BitRate = BitRate;
            options.SampleFrequency = SampleFrequency;
            options.BitsPerSample = BitsPerSample;
            options.NrAudioChannels = NrAudioChannels;
            options.Resolution = Resolution;
            options.ColorDepth = ColorDepth;
            options.ProtocolInfo = ProtocolInfo;
            options.Protection = Protection;
            options.ImportUri = ImportUri;
        }

        public ResourceOptions GetOptions ()
        {
            var options = new ResourceOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlValue]
        public virtual Uri Uri { get; protected set; }
        
        [XmlAttribute ("size", OmitIfNull = true)]
        public virtual ulong? Size { get; protected set; }
        
        [XmlAttribute ("duration", OmitIfNull = true)]
        public virtual TimeSpan? Duration { get; protected set; }

        [XmlAttribute ("bitrate", OmitIfNull = true)]
        public virtual uint? BitRate { get; protected set; }

        [XmlAttribute ("sampleFrequency", OmitIfNull = true)]
        public virtual uint? SampleFrequency { get; protected set; }

        [XmlAttribute ("bitsPerSample", OmitIfNull = true)]
        public virtual uint? BitsPerSample { get; protected set; }

        [XmlAttribute ("nrAudioChannels", OmitIfNull = true)]
        public virtual uint? NrAudioChannels { get; protected set; }

        [XmlAttribute ("resolution", OmitIfNull = true)]
        public virtual Resolution? Resolution { get; protected set; }

        [XmlAttribute ("colorDepth", OmitIfNull = true)]
        public virtual uint? ColorDepth { get; protected set; }

        [XmlAttribute ("protocolInfo", OmitIfNull = true)]
        public virtual ProtocolInfo ProtocolInfo { get; protected set; }

        [XmlAttribute ("protection", OmitIfNull = true)]
        public virtual string Protection { get; protected set; }

        [XmlAttribute ("importUri", OmitIfNull = true)]
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

        protected override void Serialize (XmlSerializationContext context)
        {
            AutoSerialize (this, context);
        }
        
        protected override void SerializeMembers (XmlSerializationContext context)
        {
            AutoSerializeMembers (this, context);
        }

        ProtocolInfo IXmlDeserializer<ProtocolInfo>.Deserialize (XmlDeserializationContext context)
        {
            // FIXME I'd like to do a better job with this. Maybe make ProtocolInfo an IXmlDeserializable and
            // support IXmlDeserializable attributes (which would require using the Activator).
            return ProtocolInfo.Parse (context.Reader["protocolInfo"]);
        }
    }
}
