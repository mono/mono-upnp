// 
// MusicTrack.cs
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
    public class MusicTrack : AudioItem
    {
        List<PersonWithRole> artists = new List<PersonWithRole> ();
        List<string> contributors = new List<string> ();
        List<string> albums = new List<string> ();
        List<string> playlists = new List<string> ();
        
        protected MusicTrack (ContentDirectory contentDirectory, Container parent)
            : base (contentDirectory, parent)
        {
        }
        
        public MusicTrack (MusicTrackOptions options, ContentDirectory contentDirectory, Container parent)
            : this (contentDirectory, parent)
        {
            UpdateFromOptions (options);
        }
        
        public override void UpdateFromOptions (ObjectOptions options)
        {
            var music_track_options = options as MusicTrackOptions;
            if (music_track_options != null)
            {
                AlbumArtURI = music_track_options.AlbumArtURI;
                Date = music_track_options.Date;
                OriginalTrackNumber = music_track_options.OriginalTrackNumber;
                StorageMedium = music_track_options.StorageMedium;
                
                artists = new List<PersonWithRole> (music_track_options.ArtistCollection);
                albums = new List<string> (music_track_options.AlbumCollection);
                playlists = new List<string> (music_track_options.PlaylistCollection);
                contributors = new List<string> (music_track_options.ContributorCollection);
            }
            
            base.UpdateFromOptions (options);
        }
        
        [XmlElement ("albumArtURI", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual Uri AlbumArtURI { get; protected set; }
        
        [XmlArrayItem ("artist", Schemas.UpnpSchema)]
        protected virtual ICollection<PersonWithRole> ArtistCollection {
            get { return artists; }
        }
        
        public IEnumerable<PersonWithRole> Artists {
            get { return artists; }
        }
        
        [XmlArrayItem ("album", Schemas.UpnpSchema)]
        protected virtual ICollection<string> AlbumCollection {
            get { return albums; }
        }
        
        public IEnumerable<string> Albums {
            get { return albums; }
        }
        
        [XmlElement ("orginalTrackElement", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual int? OriginalTrackNumber { get; protected set; }
        
        [XmlArrayItem ("playlist", Schemas.UpnpSchema)]
        protected virtual ICollection<string> PlaylistCollection {
            get { return playlists; }
        }
        
        public IEnumerable<string> Playlists {
            get { return playlists; }
        }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string StorageMedium { get; protected set; }
        
        [XmlArrayItem ("contributor", Schemas.DublinCoreSchema)]
        protected virtual ICollection<string> ContributorCollection {
            get { return contributors; }
        }
        
        public IEnumerable<string> Contributors {
            get { return contributors; }
        }
        
        [XmlElement ("date", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Date { get; protected set; }
        
        // TODO what is the deal with this?!?!?!
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
