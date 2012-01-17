// 
// Movie.cs
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
using System.Collections.ObjectModel;

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV
{
    public class Movie : VideoItem
    {
        protected Movie ()
        {
            ScheduledStartTimes = new List<DateTime> ();
            ScheduledEndTimes = new List<DateTime> ();
        }
        
        public Movie (string id, string parentId, MovieOptions options)
            : base (id, parentId, options)
        {
            StorageMedium = options.StorageMedium;
            DvdRegionCode = options.DvdRegionCode;
            ChannelName = options.ChannelName;
            ScheduledStartTimes = Helper.MakeReadOnlyCopy (options.ScheduledStartTimes);
            ScheduledEndTimes = Helper.MakeReadOnlyCopy (options.ScheduledEndTimes);
        }

        protected void CopyToOptions (MovieOptions options)
        {
            base.CopyToOptions (options);

            options.StorageMedium = StorageMedium;
            options.DvdRegionCode = DvdRegionCode;
            options.ChannelName = ChannelName;
            options.ScheduledStartTimes = new List<DateTime> (ScheduledStartTimes);
            options.ScheduledEndTimes = new List<DateTime> (ScheduledEndTimes);
        }

        public new MovieOptions GetOptions ()
        {
            var options = new MovieOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string StorageMedium { get; protected set; }
        
        [XmlElement ("DVDRegionCode", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual int? DvdRegionCode { get; protected set; }
        
        [XmlElement ("channelName", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string ChannelName { get; protected set; }
        
        [XmlArrayItem ("scheduledStartTime", Schemas.UpnpSchema)]
        public virtual IList<DateTime> ScheduledStartTimes { get; private set; }
        
        [XmlArrayItem ("scheduledEndTime", Schemas.UpnpSchema)]
        public virtual IList<DateTime> ScheduledEndTimes { get; private set; }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            base.Deserialize (context);

            ScheduledStartTimes = new ReadOnlyCollection<DateTime> (ScheduledStartTimes);
            ScheduledEndTimes = new ReadOnlyCollection<DateTime> (ScheduledEndTimes);
        }
    
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
