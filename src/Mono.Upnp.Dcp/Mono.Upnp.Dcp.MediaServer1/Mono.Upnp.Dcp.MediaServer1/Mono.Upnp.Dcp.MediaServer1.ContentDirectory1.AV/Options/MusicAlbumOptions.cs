// 
// MusicAlbumOptions.cs
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
    public class MusicAlbumOptions : AlbumOptions
    {
        public MusicAlbumOptions ()
        {
            ArtistCollection = new List<PersonWithRole> ();
            ProducerCollection = new List<string> ();
            GenreCollection = new List<string> ();
            AlbumArtUriCollection = new List<Uri> ();
        }

        public MusicAlbumOptions (MusicAlbum musicAlbum)
        {
            GetOptionsFrom (musicAlbum);
        }

        protected override void GetOptionsFrom (Object obj)
        {
            var music_album = obj as MusicAlbum;
            if (music_album != null)
            {
                Toc = music_album.Toc;

                ArtistCollection = new List<PersonWithRole> (music_album.Artists);
                ProducerCollection = new List<string> (music_album.Producers);
                GenreCollection = new List<string> (music_album.Genres);
                AlbumArtUriCollection = new List<Uri> (music_album.AlbumArtUris);
            }
            
            base.GetOptionsFrom (obj);
        }

        public virtual List<PersonWithRole> ArtistCollection { get; set; }

        public virtual List<string> GenreCollection { get; set; }

        public virtual List<string> ProducerCollection { get; set; }

        public virtual List<Uri> AlbumArtUriCollection { get; set; }

        public virtual string Toc { get; set; }
    }
}

