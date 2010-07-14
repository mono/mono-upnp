// 
// MusicTrackOptions.cs
//  
// Author:
//       Yavor Georgiev <fealebenpae@gmail.com>
// 
// Copyright (c) 2010 Yavor Georgiev
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
namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Av
{
    public class MusicTrackOptions : AudioItemOptions
    {
        public MusicTrackOptions ()
        {
            ArtistCollection = new List<PersonWithRole> ();
            AlbumCollection = new List<string> ();
            PlaylistCollection = new List<string> ();
            ContributorCollection = new List<string> ();
        }

        public MusicTrackOptions (MusicTrack musicTrack)
        {
            GetOptionsFrom (musicTrack);
        }

        protected override void GetOptionsFrom (Object obj)
        {
            var music_track = obj as MusicTrack;
            if (music_track != null)
            {
                AlbumArtURI = music_track.AlbumArtURI;
                Date = music_track.Date;
                OriginalTrackNumber = music_track.OriginalTrackNumber;
                StorageMedium = music_track.StorageMedium;

                ArtistCollection = new List<PersonWithRole> (music_track.Artists);
                AlbumCollection = new List<string> (music_track.Albums);
                PlaylistCollection = new List<string> (music_track.Playlists);
                ContributorCollection = new List<string> (music_track.Contributors);
            }
            
            base.GetOptionsFrom (obj);
        }

        public virtual Uri AlbumArtURI { get; set; }

        public virtual List<PersonWithRole> ArtistCollection { get; set; }

        public virtual List<string> AlbumCollection { get; set; }

        public virtual int? OriginalTrackNumber { get;  set; }

        public virtual List<string> PlaylistCollection { get; set; }

        public virtual string StorageMedium { get; set; }

        public virtual List<string> ContributorCollection { get; set; }

        public virtual string Date { get; set; }
    }
}

