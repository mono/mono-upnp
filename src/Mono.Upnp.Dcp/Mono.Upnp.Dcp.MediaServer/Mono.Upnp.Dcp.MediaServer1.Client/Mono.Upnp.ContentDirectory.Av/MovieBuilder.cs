// 
// MovieBuilder.cs
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
using System.Xml.Serialization;

namespace Mono.Upnp.ContentDirectory.Av
{
	public class MovieBuilder : VideoItemBuilder
	{
		readonly List<DateTime> scheduled_start_times = new List<DateTime> ();
		readonly List<DateTime> scheduled_end_times = new List<DateTime> ();
		
		[XmlElement ("storageMedium", Namespace = Schemas.UpnpSchema)]
		public string StorageMedium { get; set; }
		
		[XmlElement ("DVDRegionCode", Namespace = Schemas.UpnpSchema)]
        public int? DvdRegionCode { get; set; }
		
		[XmlElement ("channelName", Namespace = Schemas.UpnpSchema)]
        public string ChannelName { get; set; }
		
		[XmlIgnore]
        public ICollection<DateTime> ScheduledStartTimes { get { return scheduled_start_times; } }
		
		[XmlIgnore]
        public ICollection<DateTime> ScheduledEndTimes { get { return scheduled_end_times; } }
		
		[XmlArrayItem ("scheduledStartTime", Namespace = Schemas.UpnpSchema)]
		IEnumerable<string> StartTimes {
			get { return ToFormattedTimes (scheduled_start_times); }
		}
		
		[XmlArrayItem ("scheduledEndTime", Namespace = Schemas.UpnpSchema)]
		IEnumerable<string> EndTimes {
			get { return ToFormattedTimes (scheduled_end_times); }
		}
	}
}
