// 
// VideoItemOptions.cs
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
    public class VideoItemOptions : ObjectOptions
    {
        public VideoItemOptions ()
        {
            ActorCollection = new List<PersonWithRole> ();
            DirectorCollection = new List<string> ();
            ProducerCollection = new List<string> ();
            PublisherCollection = new List<string> ();
            RelationCollection = new List<Uri> ();
        }

        public VideoItemOptions (VideoItem videoItem)
        {
            GetOptionsFrom (videoItem);
        }

        protected override void GetOptionsFrom (Object obj)
        {
            var video_item = obj as VideoItem;
            if (video_item != null)
            {
                Description = video_item.Description;
                LongDescription = video_item.LongDescription;
                Rating = video_item.Rating;
                Language = video_item.Language;

                ActorCollection = new List<PersonWithRole> (video_item.Actors);
                DirectorCollection = new List<string> (video_item.Directors);
                ProducerCollection = new List<string> (video_item.Producers);
                PublisherCollection = new List<string> (video_item.Publishers);
                RelationCollection = new List<Uri> (video_item.Relations);
            }

            base.GetOptionsFrom (obj);
        }

        public virtual List<string> GenreCollection { get; set; }

        public virtual string LongDescription { get;  set; }

        public virtual List<string> ProducerCollection { get; set; }

        public virtual string Rating { get; set; }

        public virtual List<PersonWithRole> ActorCollection { get; set; }

        public virtual List<string> DirectorCollection { get; set; }

        public virtual string Description { get; set; }

        public virtual List<string> PublisherCollection { get; set; }

        public virtual string Language { get; set; }

        public virtual List<Uri> RelationCollection { get; set; }
    }
}

