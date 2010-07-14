// 
// PlaylistContainerOptions.cs
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
    public class PlaylistContainerOptions : ObjectOptions
    {
        public PlaylistContainerOptions ()
        {
            ArtistCollection = new List<PersonWithRole> ();
            ProducerCollection = new List<string> ();
            ContributorCollection = new List<string> ();
            GenreCollection = new List<string> ();
            RightsCollection = new List<string> ();
        }

        public PlaylistContainerOptions (PlaylistContainer playlistContainer)
        {
            GetOptionsFrom (playlistContainer);
        }

        protected override void GetOptionsFrom (Object obj)
        {
            var playlist_container = obj as PlaylistContainer;
            if (playlist_container != null)
            {
                LongDescription = playlist_container.LongDescription;
                Description = playlist_container.Description;
                Date = playlist_container.Date;
                StorageMedium = playlist_container.StorageMedium;
                Language = playlist_container.Language;
                
                ArtistCollection = new List<PersonWithRole> (playlist_container.Artists);
                ProducerCollection = new List<string> (playlist_container.Producers);
                ContributorCollection = new List<string> (playlist_container.Contributors);
                GenreCollection = new List<string> (playlist_container.Genres);
                RightsCollection = new List<string> (playlist_container.Rights);
            }
            
            base.GetOptionsFrom (obj);
        }

        public virtual List<PersonWithRole> ArtistCollection { get; set; }

        public virtual List<string> GenreCollection { get; set; }

        public virtual string LongDescription { get; set; }

        public virtual List<string> ProducerCollection { get; set; }

        public virtual string StorageMedium { get; set; }

        public virtual string Description { get; set; }

        public virtual List<string> ContributorCollection { get; set; }

        public virtual string Date { get; set; }

        public virtual string Language { get; set; }

        public virtual List<string> RightsCollection { get; set; }
    }
}

