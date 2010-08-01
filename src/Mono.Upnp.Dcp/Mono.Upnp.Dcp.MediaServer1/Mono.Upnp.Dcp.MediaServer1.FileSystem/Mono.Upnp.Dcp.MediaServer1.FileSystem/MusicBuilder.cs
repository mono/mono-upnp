// 
// MusicBuilder.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Thomas
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

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV;

using UpnpObject = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem
{
    public class MusicBuilder
    {
        /*Container all_music;
        Container genre;
        Container artist;
        Container album;
        Container playlists;
        Container folders;
        Container contributing_artists;
        Container album_artist;
        Container composer;
        Container rating;

        ContainerBuilder<GenreOptions> genre_builder = new ContainerBuilder<GenreOptions> ();
        ContainerBuilder<MusicArtistOptions> artist_builder = new ContainerBuilder<MusicArtistOptions> ();*/

        public void OnFile (string path, Action<UpnpObject> consumer)
        {
            var tags  = TagLib.File.Create (path);
            var genres = tags.Tag.Genres;
            var artists = tags.Tag.Performers;
            var options = new MusicTrackOptions {
                Title = tags.Tag.Title,
                OriginalTrackNumber = (int)tags.Tag.Track,
                Genres = genres,
                Artists = GetArtists (artists)
            };
            Console.WriteLine (options);

            /*foreach (var genre in genres) {
                genre_builder.OnItem (genre, audioItem, consumer,
                    options => options != null ? options : new GenreOptions { Title = genre });
            }

            foreach (var artist in artists) {
                artist_builder.OnItem (artist, audioItem, consumer,
                    options => options 
            }*/
        }

        public void OnDone (Action<ContainerInfo> consumer)
        {
            //genre_builder.OnDone (options_info =>
            //    new ContainerInfo (new MusicGenre (GetId (), options_info.Options), options_info.Items), consumer);
        }

        string GetId ()
        {
            return null;
        }

        static IEnumerable<PersonWithRole> GetArtists (IEnumerable<string> artists)
        {
            foreach (var artist in artists) {
                yield return new PersonWithRole (artist, "performer");
            }
        }
    }
}
