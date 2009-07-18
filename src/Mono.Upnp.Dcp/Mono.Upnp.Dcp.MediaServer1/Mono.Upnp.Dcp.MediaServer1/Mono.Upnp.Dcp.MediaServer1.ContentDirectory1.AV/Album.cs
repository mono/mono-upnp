// 
// Album.cs
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
    public class Album : Container
    {
        readonly List<string> publishers = new List<string> ();
        readonly List<string> contributors = new List<string> ();
        readonly List<Uri> relations = new List<Uri> ();
        readonly List<string> rights = new List<string> ();
        
        protected Album (ContentDirectory contentDirectory, Container parent)
            : base (contentDirectory, parent)
        {
        }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string StorageMedium { get; protected set; }
        
        [XmlElement ("longDescription", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string LongDescription { get; protected set; }
        
        [XmlElement ("description", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Description { get; protected set; }
        
        [XmlArrayItem ("publisher", Schemas.DublinCoreSchema)]
        protected virtual ICollection<string> PublisherCollection {
            get { return publishers; }
        }
        
        public IEnumerable<string> Publishers {
            get { return publishers; }
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
        
        [XmlArrayItem ("relation", Schemas.DublinCoreSchema)]
        protected virtual ICollection<Uri> RelationCollection {
            get { return relations; }
        }
        
        public IEnumerable<Uri> Relations {
            get { return relations; }
        }
        
        [XmlArrayItem ("rights", Schemas.DublinCoreSchema)]
        protected virtual ICollection<string> RightsCollections {
            get { return rights; }
        }
        
        public IEnumerable<string> Rights {
            get { return rights; }
        }
        
        // TODO what the hell about this?!?!?!
        //public Uri LyricsUri { get; private set; }
    
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
