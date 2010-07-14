// 
// AudioBook.cs
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
    public class AudioBook : AudioItem
    {
        List<string> producers = new List<string> ();
        List<string> contributors = new List<string> ();
        
        protected AudioBook (ContentDirectory contentDirectory, Container parent)
            : base (contentDirectory, parent)
        {
        }
        
        public AudioBook (AudioBookOptions options, ContentDirectory contentDirectory, Container parent)
            : this (contentDirectory, parent)
        {
            this.UpdateFromOptions(options);
        }
        
        public override void UpdateFromOptions (ObjectOptions options)
        {
            var audio_book_options = options as AudioBookOptions;
            if (audio_book_options != null)
            {
                this.StorageMedium = audio_book_options.StorageMedium;
                this.Date = audio_book_options.Date;
                
                producers = new List<string> (audio_book_options.ProducerCollection);
                contributors = new List<string> (audio_book_options.ContributorCollection);
            }
            
            base.UpdateFromOptions (options);
        }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string StorageMedium { get; protected set; }
        
        [XmlArrayItem ("producer", Schemas.UpnpSchema)]
        protected virtual ICollection<string> ProducerCollection {
            get { return producers; }
        }
        
        public IEnumerable<string> Producers {
            get { return producers; }
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
