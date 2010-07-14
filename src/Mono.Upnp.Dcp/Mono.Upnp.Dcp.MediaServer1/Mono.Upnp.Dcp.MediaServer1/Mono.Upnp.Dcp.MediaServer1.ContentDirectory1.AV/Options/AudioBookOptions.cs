// 
// AudioBookOptions.cs
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
    public class AudioBookOptions : ObjectOptions
    {
        public AudioBookOptions ()
        {
            ProducerCollection = new List<string> ();
            ContributorCollection = new List<string> ();
        }

        public AudioBookOptions (AudioBook audioBook)
        {
            GetOptionsFrom (audioBook);
        }

        protected override void GetOptionsFrom (Object obj)
        {
            var audio_book = obj as AudioBook;
            if (audio_book != null)
            {
                this.StorageMedium = audio_book.StorageMedium;
                this.Date = audio_book.Date;

                ProducerCollection = new List<string> (audio_book.Producers);
                ContributorCollection = new List<string> (audio_book.Contributors);
            }

            base.GetOptionsFrom (obj);
        }

        public virtual string StorageMedium { get; set; }

        public virtual List<string> ProducerCollection { get; set; }

        public virtual List<string> ContributorCollection { get; set; }

        public virtual string Date { get; set; }
    }
}

