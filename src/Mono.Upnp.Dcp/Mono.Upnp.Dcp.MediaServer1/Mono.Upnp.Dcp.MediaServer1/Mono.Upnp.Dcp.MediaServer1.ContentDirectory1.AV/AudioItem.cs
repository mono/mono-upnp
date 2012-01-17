// 
// AudioItem.cs
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
    public class AudioItem : Item
    {
        protected AudioItem ()
        {
            Publishers = new List<string> ();
            Genres = new List<string> ();
            Relations = new List<Uri> ();
            Rights = new List<string> ();
        }
        
        public AudioItem (string id, string parentId, AudioItemOptions options)
            : base (id, parentId, options)
        {
            Description = options.Description;
            LongDescription = options.LongDescription;
            Language = options.Language;
            Publishers = Helper.MakeReadOnlyCopy (options.Publishers);
            Genres = Helper.MakeReadOnlyCopy (options.Genres);
            Relations = Helper.MakeReadOnlyCopy (options.Relations);
            Rights = Helper.MakeReadOnlyCopy (options.Rights);
        }

        protected void CopyToOptions (AudioItemOptions options)
        {
            base.CopyToOptions (options);

            options.Description = Description;
            options.LongDescription = LongDescription;
            options.Language = Language;
            options.Publishers = new List<string> (Publishers);
            options.Genres = new List<string> (Genres);
            options.Relations = new List<Uri> (Relations);
            options.Rights = new List<string> (Rights);
        }

        public new AudioItemOptions GetOptions ()
        {
            var options = new AudioItemOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlArrayItem ("genre", Schemas.UpnpSchema)]
        public virtual IList<string> Genres { get; private set; }
        
        [XmlElement ("description", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Description { get; protected set; }
        
        [XmlElement ("longDescription", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string LongDescription { get; protected set; }
        
        [XmlArrayItem ("publisher", Schemas.DublinCoreSchema)]
        public virtual IList<string> Publishers { get; private set; }
        
        [XmlElement ("language", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Language { get; protected set; }
        
        [XmlArrayItem ("relation", Schemas.DublinCoreSchema)]
        public virtual IList<Uri> Relations { get; private set; }
        
        [XmlArrayItem ("rights", Schemas.DublinCoreSchema)]
        public virtual IList<string> Rights { get; private set; }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            base.Deserialize (context);

            Publishers = new ReadOnlyCollection<string> (Publishers);
            Genres = new ReadOnlyCollection<string> (Genres);
            Relations = new ReadOnlyCollection<Uri> (Relations);
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
