// 
// Wmp11MusicBuilder.cs
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

using TagLib;

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV;

using UpnpObject = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem
{
    // See NetCompat_WMP11.docx
    public class Wmp11MusicBuilder
    {
        const string music_id = "1";
        const string all_music_id = "4";
        const string genre_id = "5";
        const string artist_id = "6";
        const string album_id = "7";
        const string playlist_id = "F";
        const string folder_id = "14";
        const string contributing_artists_id = "100";
        const string album_artist_id = "107";
        const string composer_id = "108";
        const string rating_id = "101";
        const string rating_1_star_id = "102";
        const string rating_2_star_id = "103";
        const string rating_3_star_id = "104";
        const string rating_4_star_id = "105";
        const string rating_5_star_id = "106";

        List<UpnpObject> audio_items = new List<UpnpObject> ();

        ContainerBuilder<GenreOptions> genre_builder =
            new ContainerBuilder<GenreOptions> ();
        ContainerBuilder<BuildableMusicArtistOptions> artist_builder =
            new ContainerBuilder<BuildableMusicArtistOptions> ();
        ContainerBuilder<MusicAlbumOptions> album_builder =
            new ContainerBuilder<MusicAlbumOptions> ();
        ContainerBuilder<BuildableMusicArtistOptions> album_artist_builder =
            new ContainerBuilder<BuildableMusicArtistOptions> ();
        ContainerBuilder<BuildableMusicArtistOptions> composer_builder =
            new ContainerBuilder<BuildableMusicArtistOptions> ();

        public void OnTag (Tag tag, Action<UpnpObject> consumer)
        {
            var genres = tag.Genres;
            var artists = tag.Performers;
            var album_artists = tag.AlbumArtists;
            var composers = tag.Composers;

            var music_track = new MusicTrack (GetId (), all_music_id, new MusicTrackOptions {
                Title = tag.Title,
                OriginalTrackNumber = (int)tag.Track,
                Genres = genres,
                Artists = GetArtists (artists)
            });

            audio_items.Add (music_track);
            consumer (music_track);

            foreach (var genre in genres) {
                genre_builder.OnItem (genre, music_track, consumer,
                    options => options != null ? options : new GenreOptions { Title = genre });
            }

            foreach (var artist in artists) {
                artist_builder.OnItem (artist, music_track, consumer,
                    options => ArtistWithGenres (artist, genres, options));
            }

            album_builder.OnItem (tag.Album, music_track, consumer,
                options => options != null ? options : new MusicAlbumOptions {
                    Title = tag.Album,
                    Contributors = album_artists
                });

            foreach (var album_artist in album_artists) {
                album_artist_builder.OnItem (album_artist, music_track, consumer,
                    options => ArtistWithGenres (album_artist, genres, options));
            }

            foreach (var composer in composers) {
                composer_builder.OnItem (composer, music_track, consumer,
                    options => ArtistWithGenres (composer, genres, options));
            }
        }

        static BuildableMusicArtistOptions ArtistWithGenres (string artist,
                                                             IEnumerable<string> genres,
                                                             BuildableMusicArtistOptions options)
        {
            if (options == null) {
                options = new BuildableMusicArtistOptions { Title = artist };
            }

            foreach (var genre in genres) {
                options.OnGenre (genre);
            }

            return options;
        }

        public IList<UpnpObject> OnDone (Action<ContainerInfo> consumer)
        {
            var containers = new List<UpnpObject> (11);

            BuildContainer (consumer, containers, all_music_id, "All Music", audio_items);

            BuildContainer (consumer, containers, genre_id, "Genre",
                genre_builder.OnDone (consumer, options => new MusicGenre (GetId (), genre_id, options)));

            BuildContainer (consumer, containers, artist_id, "Artist",
                artist_builder.OnDone (consumer, options => new MusicArtist (GetId (), artist_id, options)));

            BuildContainer (consumer, containers, album_artist_id, "Album Artist",
                album_builder.OnDone (consumer, options => new MusicAlbum (GetId (), album_artist_id, options)));

            BuildContainer (consumer, containers, composer_id, "Composer",
                composer_builder.OnDone (consumer, options => new MusicArtist (GetId (), composer_id, options)));

            return containers;
        }

        static void BuildContainer (Action<ContainerInfo> consumer,
                                    List<UpnpObject> containers,
                                    string id,
                                    string title,
                                    IList<UpnpObject> children)
        {
            var container = new Container (id, music_id, new ContainerOptions {
                Title = title,
                ChildCount = children.Count
            });
            containers.Add (container);
            consumer (new ContainerInfo (container, children));
        }

        string GetId ()
        {
            return string.Empty;
        }

        static IEnumerable<PersonWithRole> GetArtists (IEnumerable<string> artists)
        {
            foreach (var artist in artists) {
                yield return new PersonWithRole (artist, "performer");
            }
        }
    }
}
