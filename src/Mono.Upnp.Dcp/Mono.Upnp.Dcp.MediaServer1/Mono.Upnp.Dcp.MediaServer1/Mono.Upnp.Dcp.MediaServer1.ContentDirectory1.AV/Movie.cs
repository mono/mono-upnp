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

using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Av
{
    public class Movie : VideoItem
    {
        List<DateTime> scheduled_start_times = new List<DateTime> ();
        List<DateTime> scheduled_end_times = new List<DateTime> ();
        
        protected Movie (ContentDirectory contentDirectory, Container parent)
            : base (contentDirectory, parent)
        {
        }
        
        public Movie (MovieOptions options, ContentDirectory contentDirectory, Container parent)
            : this (contentDirectory, parent)
        {
            UpdateFromOptions (options);
        }
        
        public override void UpdateFromOptions (ObjectOptions options)
        {
            var movie_options = options as MovieOptions;
            if (movie_options != null)
            {
                StorageMedium = movie_options.StorageMedium;
                DvdRegionCode = movie_options.DvdRegionCode;
                ChannelName = movie_options.ChannelName;
                
                scheduled_start_times = new List<DateTime> (movie_options.ScheduledStartTimeCollection);
                scheduled_end_times = new List<DateTime> (movie_options.ScheduledEndTimeCollection);
            }            
            
            base.UpdateFromOptions (options);
        }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string StorageMedium { get; protected set; }
        
        [XmlElement ("DVDRegionCode", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual int? DvdRegionCode { get; protected set; }
        
        [XmlElement ("channelName", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string ChannelName { get; protected set; }
        
        [XmlArrayItem ("scheduledStartTime", Schemas.UpnpSchema)]
        protected virtual ICollection<DateTime> ScheduledStartTimeCollection {
            get { return scheduled_start_times; }
        }
        
        public IEnumerable<DateTime> ScheduledStartTimes {
            get { return scheduled_start_times; }
        }
        
        [XmlArrayItem ("scheduledEndTime", Schemas.UpnpSchema)]
        protected virtual ICollection<DateTime> ScheduledEndTimeCollection {
            get { return scheduled_end_times; }
        }
        
        public IEnumerable<DateTime> ScheduledEndTimes {
            get { return scheduled_end_times; }
        }
    
        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            context.AutoDeserializeElement (this);
        }

        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            context.AutoSerializeMembersOnly (this);
        }
    }
}
