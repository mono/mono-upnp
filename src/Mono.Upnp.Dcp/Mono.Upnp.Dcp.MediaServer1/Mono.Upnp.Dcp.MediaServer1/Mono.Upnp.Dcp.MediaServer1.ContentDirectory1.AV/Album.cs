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
using System.Collections.ObjectModel;

using Mono.Upnp.Dcp.MediaServer1.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV
{
    public class Album : Container
    {
        protected Album ()
        {
            Publishers = new List<string> ();
            Contributors = new List<string> ();
            Relations = new List<Uri> ();
            Rights = new List<string> ();
        }
        
        public Album (string id, string parentId, AlbumOptions options)
            : base (id, parentId, options)
        {
            StorageMedium = options.StorageMedium;
            LongDescription = options.LongDescription;
            Description = options.Description;
            Date = options.Date;
            Publishers = Helper.MakeReadOnlyCopy (options.Publishers);
            Contributors = Helper.MakeReadOnlyCopy (options.Contributors);
            Relations = Helper.MakeReadOnlyCopy (options.Relations);
            Rights = Helper.MakeReadOnlyCopy (options.Rights);
        }

        protected void CopyToOptions (AlbumOptions options)
        {
            base.CopyToOptions (options);

            options.StorageMedium = StorageMedium;
            options.LongDescription = LongDescription;
            options.Description = Description;
            options.Date = Date;
            options.Publishers = new List<string> (Publishers);
            options.Contributors = new List<string> (Contributors);
            options.Relations = new List<Uri> (Relations);
            options.Rights = new List<string> (Rights);
        }

        public new AlbumOptions GetOptions ()
        {
            var options = new AlbumOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string StorageMedium { get; protected set; }
        
        [XmlElement ("longDescription", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string LongDescription { get; protected set; }
        
        [XmlElement ("description", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Description { get; protected set; }
        
        [XmlArrayItem ("publisher", Schemas.DublinCoreSchema)]
        public virtual IList<string> Publishers { get; private set; }
        
        [XmlArrayItem ("contributor", Schemas.DublinCoreSchema)]
        public virtual IList<string> Contributors { get; private set; }
        
        [XmlElement ("date", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Date { get; protected set; }
        
        [XmlArrayItem ("relation", Schemas.DublinCoreSchema)]
        public virtual IList<Uri> Relations { get; private set; }
        
        [XmlArrayItem ("rights", Schemas.DublinCoreSchema)]
        public virtual IList<string> Rights { get; private set; }
        
        // TODO What about this?
        //public Uri LyricsUri { get; private set; }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            base.Deserialize (context);

            Publishers = new ReadOnlyCollection<string> (Publishers);
            Contributors = new ReadOnlyCollection<string> (Contributors);
            Relations = new ReadOnlyCollection<Uri> (Relations);
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
