// 
// MovieOptions.cs
//  
// Author:
//       Yavor Georgiev <fealebenpae@gmail.com>
// 
// Copyright (c) 2010 Yavor Georgiev
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

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV
{
    public class MovieOptions : VideoItemOptions
    {
        IEnumerable<DateTime> scheduled_start_times;
        IEnumerable<DateTime> scheduled_end_times;

        public IEnumerable<DateTime> ScheduledStartTimes {
            get { return GetEnumerable (scheduled_start_times); }
            set { scheduled_start_times = value; }
        }

        public IEnumerable<DateTime> ScheduledEndTimes {
            get { return GetEnumerable (scheduled_end_times); }
            set { scheduled_end_times = value; }
        }

        public string StorageMedium { get; set; }

        public int? DvdRegionCode { get; set; }

        public string ChannelName { get; set; }
    }
}
