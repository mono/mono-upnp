// 
// MusicAlbum.cs
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
    public class MusicAlbum : Album
    {
        protected MusicAlbum ()
        {
            Artists = new List<PersonWithRole> ();
            Producers = new List<string> ();
            Genres = new List<string> ();
            AlbumArtUris = new List<Uri> ();
        }
        
        public MusicAlbum (string id, string parentId, MusicAlbumOptions options)
            : base (id, parentId, options)
        {
            Toc = options.Toc;
            Artists = Helper.MakeReadOnlyCopy (options.Artists);
            Producers = Helper.MakeReadOnlyCopy (options.Producers);
            Genres = Helper.MakeReadOnlyCopy (options.Genres);
            AlbumArtUris = Helper.MakeReadOnlyCopy (options.AlbumArtUris);
        }

        protected void CopyToOptions (MusicAlbumOptions options)
        {
            base.CopyToOptions (options);

            options.Toc = Toc;
            options.Artists = new List<PersonWithRole> (Artists);
            options.Producers = new List<string> (Producers);
            options.Genres = new List<string> (Genres);
            options.AlbumArtUris = new List<Uri> (AlbumArtUris);
        }

        public new MusicAlbumOptions GetOptions ()
        {
            var options = new MusicAlbumOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlArrayItem ("artist", Schemas.UpnpSchema)]
        public virtual IList<PersonWithRole> Artists { get; private set; }
        
        [XmlArrayItem ("genre", Schemas.UpnpSchema)]
        public virtual IList<string> Genres { get; private set; }
        
        [XmlArrayItem ("producer", Schemas.UpnpSchema)]
        public virtual IList<string> Producers { get; private set; }
        
        [XmlArrayItem ("albumArtURI", Schemas.UpnpSchema)]
        public virtual IList<Uri> AlbumArtUris { get; private set; }
        
        [XmlElement ("toc", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string Toc { get; protected set; }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            base.Deserialize (context);

            Artists = new ReadOnlyCollection<PersonWithRole> (Artists);
            Producers = new ReadOnlyCollection<string> (Producers);
            Genres = new ReadOnlyCollection<string> (Genres);
            AlbumArtUris = new ReadOnlyCollection<Uri> (AlbumArtUris);
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
