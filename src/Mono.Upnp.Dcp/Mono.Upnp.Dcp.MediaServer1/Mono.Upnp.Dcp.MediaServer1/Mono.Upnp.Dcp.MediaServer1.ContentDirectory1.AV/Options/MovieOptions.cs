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
namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Av
{
    public class MovieOptions : VideoItemOptions
    {
        public MovieOptions ()
        {
            ScheduledStartTimeCollection = new List<DateTime> ();
            ScheduledEndTimeCollection = new List<DateTime> ();
        }

        public MovieOptions (Movie movie)
        {
            GetOptionsFrom (movie);
        }

        protected override void GetOptionsFrom (Object obj)
        {
            var movie = obj as Movie;
            if (movie != null)
            {
                StorageMedium = movie.StorageMedium;
                DvdRegionCode = movie.DvdRegionCode;
                ChannelName = movie.ChannelName;

                ScheduledStartTimeCollection = new List<DateTime> (movie.ScheduledStartTimes);
                ScheduledEndTimeCollection = new List<DateTime> (movie.ScheduledEndTimes);
            }            
            
            base.GetOptionsFrom (obj);
        }

        public virtual string StorageMedium { get; set; }

        public virtual int? DvdRegionCode { get; set; }

        public virtual string ChannelName { get; set; }

        public virtual List<DateTime> ScheduledStartTimeCollection { get; set; }

        public virtual List<DateTime> ScheduledEndTimeCollection { get; set; }
    }
}

