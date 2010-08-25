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
using System.Collections.ObjectModel;

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV
{
    public class MusicTrack : AudioItem
    {
        protected MusicTrack ()
        {
            Artists = new List<PersonWithRole> ();
            Albums = new List<string> ();
            Playlists = new List<string> ();
            Contributors = new List<string> ();
        }
        
        public MusicTrack (string id, string parentId, MusicTrackOptions options)
            : base (id, parentId, options)
        {
            AlbumArtUri = options.AlbumArtUri;
            Date = options.Date;
            OriginalTrackNumber = options.OriginalTrackNumber;
            StorageMedium = options.StorageMedium;
            Artists = Helper.MakeReadOnlyCopy (options.Artists);
            Albums = Helper.MakeReadOnlyCopy (options.Albums);
            Playlists = Helper.MakeReadOnlyCopy (options.Playlists);
            Contributors = Helper.MakeReadOnlyCopy (options.Contributors);
        }

        protected void CopyToOptions (MusicTrackOptions options)
        {
            base.CopyToOptions (options);

            options.AlbumArtUri = AlbumArtUri;
            options.Date = Date;
            options.OriginalTrackNumber = OriginalTrackNumber;
            options.StorageMedium = StorageMedium;
            options.Artists = new List<PersonWithRole> (Artists);
            options.Albums = new List<string> (Albums);
            options.Playlists = new List<string> (Playlists);
            options.Contributors = new List<string> (Contributors);
        }

        public new MusicTrackOptions GetOptions ()
        {
            var options = new MusicTrackOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlElement ("albumArtURI", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual Uri AlbumArtUri { get; protected set; }
        
        [XmlArrayItem ("artist", Schemas.UpnpSchema)]
        public virtual IList<PersonWithRole> Artists { get; private set; }
        
        [XmlArrayItem ("album", Schemas.UpnpSchema)]
        public virtual IList<string> Albums { get; private set; }
        
        [XmlElement ("orginalTrackElement", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual int? OriginalTrackNumber { get; protected set; }
        
        [XmlArrayItem ("playlist", Schemas.UpnpSchema)]
        public virtual IList<string> Playlists { get; private set; }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual string StorageMedium { get; protected set; }
        
        [XmlArrayItem ("contributor", Schemas.DublinCoreSchema)]
        public virtual IList<string> Contributors { get; private set; }
        
        [XmlElement ("date", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Date { get; protected set; }
        
        // TODO what is the deal with this?!?!?!
        //public Uri LyricsUri { get; private set; }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            base.Deserialize (context);

            Artists = new ReadOnlyCollection<PersonWithRole> (Artists);
            Albums = new ReadOnlyCollection<string> (Albums);
            Playlists = new ReadOnlyCollection<string> (Playlists);
            Contributors = new ReadOnlyCollection<string> (Contributors);
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
