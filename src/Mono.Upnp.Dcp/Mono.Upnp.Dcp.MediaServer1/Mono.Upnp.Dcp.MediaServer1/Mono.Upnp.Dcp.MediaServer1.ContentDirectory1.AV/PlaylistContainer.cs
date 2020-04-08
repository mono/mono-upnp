// 
// PlaylistContainer.cs
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
    public class PlaylistContainer : Container
    {
        protected PlaylistContainer ()
        {
        }

        public PlaylistContainer (string id, string parentId, PlaylistContainerOptions options)
            : base (id, parentId, options)
        {
            LongDescription = options.LongDescription;
            Description = options.Description;
            Date = options.Date;
            StorageMedium = options.StorageMedium;
            Language = options.Language;
            Artists = Helper.MakeReadOnlyCopy (options.Artists);
            Producers = Helper.MakeReadOnlyCopy (options.Producers);
            Contributors = Helper.MakeReadOnlyCopy (options.Contributors);
            Genres = Helper.MakeReadOnlyCopy (options.Genres);
            Rights = Helper.MakeReadOnlyCopy (options.Rights);
        }

        protected void CopyToOptions (PlaylistContainerOptions options)
        {
            base.CopyToOptions (options);

            options.LongDescription = LongDescription;
            options.Description = Description;
            options.Date = Date;
            options.StorageMedium = StorageMedium;
            options.Language = Language;
            options.Artists = new List<PersonWithRole> (Artists);
            options.Producers = new List<string> (Producers);
            options.Contributors = new List<string> (Contributors);
            options.Genres = new List<string> (Genres);
            options.Rights = new List<string> (Rights);
        }

        public new PlaylistContainerOptions GetOptions ()
        {
            var options = new PlaylistContainerOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlArrayItem ("artist", Schemas.UpnpSchema)]
        public virtual IList<PersonWithRole> Artists { get; private set; }
        
        [XmlArrayItem ("genre", Schemas.UpnpSchema)]
        public virtual IList<string> Genres { get; private set; }
        
        [XmlElement ("longDescription", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string LongDescription { get; protected set; }
        
        [XmlArrayItem ("producer", Schemas.UpnpSchema)]
        public virtual IList<string> Producers { get; private set; }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string StorageMedium { get; protected set; }
        
        [XmlElement ("description", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Description { get; protected set; }
        
        [XmlArrayItem ("contributor", Schemas.DublinCoreSchema)]
        public virtual IList<string> Contributors { get; private set; }
        
        [XmlElement ("date", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Date { get; protected set; }
        
        [XmlElement ("language", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Language { get; protected set; }
        
        [XmlArrayItem ("rights", Schemas.DublinCoreSchema)]
        public virtual IList<string> Rights { get; private set; }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            base.Deserialize (context);

            if (Artists == null)
            {
                Artists = new List<PersonWithRole>();
                Producers = new List<string>();
                Contributors = new List<string>();
                Genres = new List<string>();
                Rights = new List<string>();
            }

            Artists = new ReadOnlyCollection<PersonWithRole> (Artists);
            Producers = new ReadOnlyCollection<string> (Producers);
            Contributors = new ReadOnlyCollection<string> (Contributors);
            Genres = new ReadOnlyCollection<string> (Genres);
            Rights = new ReadOnlyCollection<string> (Rights);
        }
    
        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            context.AutoDeserializeElement (this);
        }

        protected override void SerializeMembers (XmlSerializationContext context)
        {
            AutoSerializeMembers (this, context);
        }
    }
}
