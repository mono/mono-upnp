// 
// MusicVideoClipOptions.cs
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
    public class MusicVideoClipOptions : VideoItemOptions
    {
        public MusicVideoClipOptions ()
        {
            ArtistCollection = new List<PersonWithRole> ();
            AlbumCollection = new List<string> ();
            ScheduledStartTimeCollection = new List<DateTime> ();
            ScheduledEndTimeCollection = new List<DateTime> ();
            ContributorCollection = new List<string> ();
        }

        public MusicVideoClipOptions (MusicVideoClip musicVideoClip)
        {
            GetOptionsFrom (musicVideoClip);
        }

        protected override void GetOptionsFrom (Object obj)
        {
            var music_video_clip = obj as MusicVideoClip;
            if (music_video_clip != null)
            {
                Date = music_video_clip.Date;
                StorageMedium = music_video_clip.StorageMedium;
                
                ArtistCollection = new List<PersonWithRole> (music_video_clip.Artists);
                AlbumCollection = new List<string> (music_video_clip.Albums);
                ScheduledStartTimeCollection = new List<DateTime> (music_video_clip.ScheduledStartTimes);
                ScheduledEndTimeCollection = new List<DateTime> (music_video_clip.ScheduledEndTimes);
                ContributorCollection = new List<string> (music_video_clip.Contributors);
            }
            
            base.GetOptionsFrom (obj);
        }

        public virtual List<PersonWithRole> ArtistCollection { get; set; }

        public virtual string StorageMedium { get; set; }

        public virtual List<string> AlbumCollection { get; set; }

        public virtual List<DateTime> ScheduledStartTimeCollection { get; set; }

        public virtual List<DateTime> ScheduledEndTimeCollection { get; set; }

        public virtual List<string> ContributorCollection { get; set; }

        public virtual string Date { get;  set; }
    }
}

