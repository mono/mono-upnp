// 
// MusicVideoClip.cs
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
    public class MusicVideoClip : VideoItem
    {
        readonly List<PersonWithRole> artists = new List<PersonWithRole> ();
        readonly List<string> contributors = new List<string> ();
        readonly List<string> albums = new List<string> ();
        readonly List<DateTime> scheduled_start_times = new List<DateTime> ();
        readonly List<DateTime> scheduled_end_times = new List<DateTime> ();
        
        protected MusicVideoClip (ContentDirectory contentDirectory, Container parent)
            : base (contentDirectory, parent)
        {
        }
        
        [XmlArrayItem ("artist", Schemas.UpnpSchema)]
        protected virtual ICollection<PersonWithRole> ArtistCollection {
            get { return artists; }
        }
        
        public IEnumerable<PersonWithRole> Artists {
            get { return artists; }
        }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string StorageMedium { get; protected set; }
        
        [XmlArrayItem ("album", Schemas.UpnpSchema)]
        protected virtual ICollection<string> AlbumCollection {
            get { return albums; }
        }
        
        public IEnumerable<string> Albums {
            get { return albums; }
        }
        
        [XmlArrayItem ("scheduledStartTime", Schemas.UpnpSchema)]
        protected virtual ICollection<DateTime> ScheduledStartTimeCollection {
            get { return scheduled_start_times; }
        }
        
        public IEnumerable<DateTime> ScheduledStartTimes {
            get { return scheduled_end_times; }
        }
        
        [XmlArrayItem ("scheduledEndTime", Schemas.UpnpSchema)]
        protected virtual ICollection<DateTime> ScheduledEndTimeCollection {
            get { return scheduled_end_times; }
        }
        
        public IEnumerable<DateTime> ScheduledEndTimes {
            get { return scheduled_end_times; }
        }
        
        [XmlArrayItem ("contributor", Schemas.DublinCoreSchema)]
        protected virtual ICollection<string> ContributorCollection {
            get { return contributors; }
        }
        
        public IEnumerable<string> Contributors {
            get { return contributors; }
        }
        
        [XmlElement ("date", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Date { get; protected set; }
    
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
