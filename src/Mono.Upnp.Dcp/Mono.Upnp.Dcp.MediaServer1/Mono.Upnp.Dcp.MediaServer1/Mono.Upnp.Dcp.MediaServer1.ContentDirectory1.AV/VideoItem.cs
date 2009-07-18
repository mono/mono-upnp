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

using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Av
{
    public class VideoItem : Item
    {
        readonly List<PersonWithRole> actors = new List<PersonWithRole> ();
        readonly List<string> producers = new List<string> ();
        readonly List<string> directors = new List<string> ();
        readonly List<string> publishers = new List<string> ();
        readonly List<string> genres = new List<string> ();
        readonly List<Uri> relations = new List<Uri>();
        
        protected VideoItem (ContentDirectory contentDirectory, Container parent)
            : base (contentDirectory, parent)
        {
        }
        
        [XmlArrayItemAttribute ("genre", Schemas.UpnpSchema)]
        protected virtual ICollection<string> GenreCollection {
            get { return genres; }
        }
        
        public IEnumerable<string> Genres {
            get { return genres; }
        }
        
        [XmlElement ("longDescription", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string LongDescription { get; protected set; }
        
        [XmlArrayItem ("producer", Schemas.UpnpSchema)]
        protected virtual ICollection<string> ProducerCollection {
            get { return producers; }
        }
        
        public IEnumerable<string> Producers {
            get { return producers; }
        }
        
        [XmlElement ("rating", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string Rating { get; protected set; }
        
        [XmlArrayItem ("actor", Schemas.UpnpSchema)]
        protected virtual ICollection<PersonWithRole> ActorCollection {
            get { return actors; }
        }
        
        public IEnumerable<PersonWithRole> Actors {
            get { return actors; }
        }
        
        [XmlArrayItem ("director", Schemas.UpnpSchema)]
        protected virtual ICollection<string> DirectorCollection {
            get { return directors; }
        }
        
        public IEnumerable<string> Directors {
            get { return directors; }
        }
        
        [XmlElement ("description", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Description { get; protected set; }
        
        [XmlArrayItem ("publisher", Schemas.DublinCoreSchema)]
        protected virtual ICollection<string> PublisherCollection {
            get { return publishers; }
        }
        
        public IEnumerable<string> Publishers {
            get { return publishers; }
        }
        
        [XmlElement ("language", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Language { get; protected set; }
        
        [XmlArrayItem ("relation", Schemas.DublinCoreSchema)]
        protected virtual ICollection<Uri> RelationCollection {
            get { return relations; }
        }
        
        public IEnumerable<Uri> Relations {
            get { return relations; }
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
