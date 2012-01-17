// 
// AudioBroadcast.cs
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

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV
{
    public class AudioBroadcast  : AudioItem
    {
        protected AudioBroadcast ()
        {
        }
        
        public AudioBroadcast (string id, string parentId, AudioBroadcastOptions options)
            : base (id, parentId, options)
        {
            Region = options.Region;
            RadioCallSign = options.RadioCallSign;
            RadioStationId = options.RadioStationId;
            RadioBand = options.RadioBand;
            ChannelNr = options.ChannelNr;
        }

        protected void CopyToOptions (AudioBroadcastOptions options)
        {
            base.CopyToOptions (options);

            options.Region = Region;
            options.RadioCallSign = RadioCallSign;
            options.RadioStationId = RadioStationId;
            options.RadioBand = RadioBand;
            options.ChannelNr = ChannelNr;
        }

        public new AudioBroadcastOptions GetOptions ()
        {
            var options = new AudioBroadcastOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlElement ("region", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string Region { get; protected set; }
        
        [XmlElement ("radioCallSign", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string RadioCallSign { get; protected set; }
        
        [XmlElement ("radioStationID", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string RadioStationId { get; protected set; }
        
        [XmlElement ("radioBand", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string RadioBand { get; protected set; }
        
        [XmlElement ("channelNr", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual int? ChannelNr { get; protected set; } // FIXME is this right?
    
        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            context.AutoDeserializeElement (this);
        }

        protected override void SerializeMembers (XmlSerializationContext context)
        {
            AutoSerializeMembers (this, context);
        }
    }
}
