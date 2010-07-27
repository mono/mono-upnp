// 
// ImageItem.cs
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
    public class ImageItem : Item
    {
        protected ImageItem ()
        {
            Publishers = new List<string> ();
            Rights = new List<string> ();
        }
        
        public ImageItem (string id, ImageItemOptions options)
            : base (id, options)
        {
            LongDescription = options.LongDescription;
            StorageMedium = options.StorageMedium;
            Rating = options.Rating;
            Description = options.Description;
            Date = options.Date;
            Publishers = Helper.MakeReadOnlyCopy (options.Publishers);
            Rights = Helper.MakeReadOnlyCopy (options.Rights);
        }

        protected void CopyToOptions (ImageItemOptions options)
        {
            base.CopyToOptions (options);

            options.LongDescription = LongDescription;
            options.StorageMedium = StorageMedium;
            options.Rating = Rating;
            options.Description = Description;
            options.Date = Date;
            options.Publishers = new List<string> (Publishers);
            options.Rights = new List<string> (Rights);
        }
        
        [XmlElement ("longDescription", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string LongDescription { get; protected set; }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string StorageMedium { get; protected set; }
        
        [XmlElement ("rating", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string Rating { get; protected set; }
        
        [XmlElement ("description", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Description { get; protected set; }
        
        [XmlArrayItem ("publisher", Schemas.DublinCoreSchema)]
        public virtual IList<string> Publishers { get; private set; }
        
        [XmlElement ("date", Schemas.DublinCoreSchema)]
        public virtual string Date { get; protected set; }
        
        [XmlArrayItem ("rights", Schemas.DublinCoreSchema)]
        public virtual IList<string> Rights { get; private set; }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            base.Deserialize (context);

            Publishers = new ReadOnlyCollection<string> (Publishers);
            Rights = new ReadOnlyCollection<string> (Rights);
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
