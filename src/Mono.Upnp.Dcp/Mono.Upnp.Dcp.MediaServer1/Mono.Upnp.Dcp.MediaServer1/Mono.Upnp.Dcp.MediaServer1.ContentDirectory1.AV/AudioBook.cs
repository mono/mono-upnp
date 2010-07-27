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
using System.Collections.ObjectModel;

using Mono.Upnp.Dcp.MediaServer1.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV
{
    public class AudioBook : AudioItem
    {
        protected AudioBook ()
        {
            Producers = new List<string> ();
            Contributors = new List<string> ();
        }
        
        public AudioBook (string id, AudioBookOptions options)
            : base (id, options)
        {
            StorageMedium = options.StorageMedium;
            Date = options.Date;
            Producers = Helper.MakeReadOnlyCopy (options.Producers);
            Contributors = Helper.MakeReadOnlyCopy (options.Contributors);
        }

        protected void CopyToOptions (AudioBookOptions options)
        {
            base.CopyToOptions (options);

            options.StorageMedium = StorageMedium;
            options.Date = Date;
            options.Producers = new List<string> (Producers);
            options.Contributors = new List<string> (Contributors);
        }

        public new AudioBookOptions GetOptions ()
        {
            var options = new AudioBookOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string StorageMedium { get; protected set; }
        
        [XmlArrayItem ("producer", Schemas.UpnpSchema)]
        public virtual IList<string> Producers { get; private set; }
        
        [XmlArrayItem ("contributor", Schemas.DublinCoreSchema)]
        public virtual IList<string> Contributors { get; private set; }
        
        [XmlElement ("date", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Date { get; protected set; }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            base.Deserialize (context);

            Producers = new ReadOnlyCollection<string> (Producers);
            Contributors = new ReadOnlyCollection<string> (Contributors);
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
