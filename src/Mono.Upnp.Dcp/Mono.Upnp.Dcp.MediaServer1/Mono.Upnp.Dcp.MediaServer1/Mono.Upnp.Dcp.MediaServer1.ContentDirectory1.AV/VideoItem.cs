// 
// VideoItem.cs
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
    public class VideoItem : Item
    {
        protected VideoItem ()
        {
            Genres = new List<string> ();
            Actors = new List<PersonWithRole> ();
            Directors = new List<string> ();
            Producers = new List<string> ();
            Publishers = new List<string> ();
            Relations = new List<Uri> ();
        }
        
        public VideoItem (string id, string parentId, VideoItemOptions options)
            : base (id, parentId, options)
        {
            Description = options.Description;
            LongDescription = options.LongDescription;
            Rating = options.Rating;
            Language = options.Language;
            Genres = Helper.MakeReadOnlyCopy (options.Genres);
            Actors = Helper.MakeReadOnlyCopy (options.Actors);
            Directors = Helper.MakeReadOnlyCopy (options.Directors);
            Producers = Helper.MakeReadOnlyCopy (options.Producers);
            Publishers = Helper.MakeReadOnlyCopy (options.Publishers);
            Relations = Helper.MakeReadOnlyCopy (options.Relations);
        }

        protected void CopyToOptions (VideoItemOptions options)
        {
            base.CopyToOptions (options);

            options.Description = Description;
            options.LongDescription = LongDescription;
            options.Rating = Rating;
            options.Language = Language;
            options.Genres = new List<string> (Genres);
            options.Actors = new List<PersonWithRole> (Actors);
            options.Directors = new List<string> (Directors);
            options.Producers = new List<string> (Producers);
            options.Publishers = new List<string> (Publishers);
            options.Relations = new List<Uri> (Relations);
        }

        public new VideoItemOptions GetOptions ()
        {
            var options = new VideoItemOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlArrayItemAttribute ("genre", Schemas.UpnpSchema)]
        public virtual IList<string> Genres { get; private set; }
        
        [XmlElement ("longDescription", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string LongDescription { get; protected set; }
        
        [XmlArrayItem ("producer", Schemas.UpnpSchema)]
        public virtual IList<string> Producers { get; private set; }
        
        [XmlElement ("rating", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string Rating { get; protected set; }
        
        [XmlArrayItem ("actor", Schemas.UpnpSchema)]
        public virtual IList<PersonWithRole> Actors { get; private set; }
        
        [XmlArrayItem ("director", Schemas.UpnpSchema)]
        public virtual IList<string> Directors { get; private set; }
        
        [XmlElement ("description", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Description { get; protected set; }
        
        [XmlArrayItem ("publisher", Schemas.DublinCoreSchema)]
        public virtual IList<string> Publishers { get; private set; }
        
        [XmlElement ("language", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Language { get; protected set; }
        
        [XmlArrayItem ("relation", Schemas.DublinCoreSchema)]
        public virtual IList<Uri> Relations { get; private set; }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            base.Deserialize (context);

            Genres = new ReadOnlyCollection<string> (Genres);
            Actors = new ReadOnlyCollection<PersonWithRole> (Actors);
            Directors = new ReadOnlyCollection<string> (Directors);
            Producers = new ReadOnlyCollection<string> (Producers);
            Publishers = new ReadOnlyCollection<string> (Publishers);
            Relations = new ReadOnlyCollection<Uri> (Relations);
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
