// 
// PlaylistItem.cs
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

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV
{
    public class PlaylistItem : Item
    {
        protected PlaylistItem ()
        {
        }
        
        public PlaylistItem (string id, string parentId, PlaylistItemOptions options)
            : base (id, parentId, options)
        {
            LongDescription = options.LongDescription;
            StorageMedium = options.StorageMedium;
            Description = options.Description;
            Date = options.Date;
            Language = options.Language;
            Artists = Helper.MakeReadOnlyCopy (options.Artists);
            Genres = Helper.MakeReadOnlyCopy (options.Genres);
        }

        protected void CopyToOptions (PlaylistItemOptions options)
        {
            base.CopyToOptions (options);

            options.LongDescription = LongDescription;
            options.StorageMedium = StorageMedium;
            options.Description = Description;
            options.Date = Date;
            options.Language = Language;
            options.Artists = new List<PersonWithRole> (Artists);
            options.Genres = new List<string> (Genres);
        }

        public new PlaylistItemOptions GetOptions ()
        {
            var options = new PlaylistItemOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlArrayItem ("artist", Schemas.UpnpSchema)]
        public virtual IList<PersonWithRole> Artists { get; private set; }
        
        [XmlArrayItem ("genre", Schemas.UpnpSchema)]
        public virtual IList<string> Genres { get; private set; }
        
        [XmlElement ("longDescription", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string LongDescription { get; protected set; }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string StorageMedium { get; protected set; }
        
        [XmlElement ("description", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Description { get; protected set; }
        
        [XmlElement ("date", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Date { get; protected set; }
        
        [XmlElement ("language", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Language { get; protected set; }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            base.Deserialize (context);

            Artists = new ReadOnlyCollection<PersonWithRole> (Artists);
            Genres = new ReadOnlyCollection<string> (Genres);
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
