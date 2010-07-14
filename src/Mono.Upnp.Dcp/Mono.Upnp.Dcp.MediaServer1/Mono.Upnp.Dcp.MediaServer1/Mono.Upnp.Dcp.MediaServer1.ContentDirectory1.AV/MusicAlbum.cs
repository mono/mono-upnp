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

using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Av
{
    public class MusicAlbum : Album
    {
        List<PersonWithRole> artists = new List<PersonWithRole> ();
        List<string> producers = new List<string> ();
        List<string> genres = new List<string> ();
        List<Uri> album_art_uris = new List<Uri> ();
        
        protected MusicAlbum (ContentDirectory contentDirectory, Container parent)
            : base (contentDirectory, parent)
        {
        }
        
        public MusicAlbum (MusicAlbumOptions options, ContentDirectory contentDirectory, Container parent)
            : this (contentDirectory, parent)
        {
            UpdateFromOptions (options);
        }
        
        public override void UpdateFromOptions (ObjectOptions options)
        {
            var music_album_options = options as MusicAlbumOptions;
            if (music_album_options != null)
            {
                Toc = music_album_options.Toc;
                
                artists = new List<PersonWithRole> (music_album_options.ArtistCollection);
                producers = new List<string> (music_album_options.ProducerCollection);
                genres = new List<string> (music_album_options.GenreCollection);
                album_art_uris = new List<Uri> (music_album_options.AlbumArtUriCollection);
            }
            
            base.UpdateFromOptions (options);
        }
        
        [XmlArrayItem ("artist", Schemas.UpnpSchema)]
        protected virtual ICollection<PersonWithRole> ArtistCollection {
            get { return artists; }
        }
        
        public IEnumerable<PersonWithRole> Artists {
            get { return artists; }
        }
        
        [XmlArrayItem ("genre", Schemas.UpnpSchema)]
        protected virtual ICollection<string> GenreCollection {
            get { return genres; }
        }
        
        public IEnumerable<string> Genres {
            get { return genres; }
        }
        
        [XmlArrayItem ("producer", Schemas.UpnpSchema)]
        protected virtual ICollection<string> ProducerCollection {
            get { return producers; }
        }
        
        public IEnumerable<string> Producers {
            get { return producers; }
        }
        
        [XmlArrayItem ("albumArtURI", Schemas.UpnpSchema)]
        protected virtual ICollection<Uri> AlbumArtUriCollection {
            get { return album_art_uris; }
        }
        
        public IEnumerable<Uri> AlbumArtUris {
            get { return album_art_uris; }
        }
        
        [XmlElement ("toc", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string Toc { get; protected set; }
    
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
