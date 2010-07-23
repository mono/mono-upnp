// 
// AudioItemOptions.cs
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
    public class AudioItemOptions : ObjectOptions
    {
        public AudioItemOptions ()
        {
            GenreCollection = new List<string> ();
            RightsCollection = new List<string> ();
            RelationCollection = new List<Uri> ();
            PublisherCollection = new List<string> ();
        }

        public AudioItemOptions (AudioItem audioItem)
        {
            GetOptionsFrom (audioItem);
        }

        protected override void GetOptionsFrom (Object obj)
        {        
            var audio_item = obj as AudioItem;
            if (audio_item != null)
            {
                Description = audio_item.Description;
                LongDescription = audio_item.LongDescription;

                GenreCollection = new List<string> (audio_item.Genres);
                RightsCollection = new List<string> (audio_item.Rights);
                RelationCollection = new List<Uri> (audio_item.Relations);
                PublisherCollection = new List<string> (audio_item.Publishers);
            }
            
            base.GetOptionsFrom (obj);
        }

        public virtual List<string> GenreCollection { get; set; }

        public virtual string Description { get; set; }

        public virtual string LongDescription { get; set; }

        public virtual List<string> PublisherCollection { get; set; }

        public virtual string Language { get; set; }

        public virtual List<Uri> RelationCollection { get; set; }

        public virtual List<string> RightsCollection { get; set; }
    }
}

