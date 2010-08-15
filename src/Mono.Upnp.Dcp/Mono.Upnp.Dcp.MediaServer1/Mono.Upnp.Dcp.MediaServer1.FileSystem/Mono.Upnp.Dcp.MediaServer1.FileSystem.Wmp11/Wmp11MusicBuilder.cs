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

using Object = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem.Wmp11
{
    public class Wmp11MusicBuilder : Wmp11ContainerBuilder
    {
        int ids;
        List<Object> audio_items = new List<Object> ();
        ContainerBuilder<GenreOptions> genre_builder;
        ContainerBuilder<BuildableMusicArtistOptions> artist_builder;
        ContainerBuilder<MusicAlbumOptions> album_builder;
        ContainerBuilder<PlaylistContainerOptions> playlists_builder;
        ContainerBuilder<StorageFolderOptions> folders_builder;
        ContainerBuilder<MusicArtistOptions> contributing_artists_builder;
        ContainerBuilder<BuildableMusicArtistOptions> album_artist_builder;
        ContainerBuilder<BuildableMusicArtistOptions> composer_builder;
        ContainerBuilder<ContainerOptions> one_star_builder;
        ContainerBuilder<ContainerOptions> two_star_builder;
        ContainerBuilder<ContainerOptions> three_star_builder;
        ContainerBuilder<ContainerOptions> four_star_builder;
        ContainerBuilder<ContainerOptions> five_star_builder;

        public Wmp11MusicBuilder (Uri url)
            : base (url, Wmp11Ids.Music, "Music")
        {
            genre_builder = new ContainerBuilder<GenreOptions> (
                Wmp11Ids.MusicGenre, "Genre", GetId);
            artist_builder = new ContainerBuilder<BuildableMusicArtistOptions> (
                Wmp11Ids.MusicArtist, "Artist", GetId);
            album_builder = new ContainerBuilder<MusicAlbumOptions> (
                Wmp11Ids.MusicAlbum, "Album", GetId);
            playlists_builder = new ContainerBuilder<PlaylistContainerOptions> (
                Wmp11Ids.Playlists, "Playlists", GetId);
            folders_builder = new ContainerBuilder<StorageFolderOptions> (
                Wmp11Ids.MusicFolders, "Folders", GetId);
            contributing_artists_builder = new ContainerBuilder<MusicArtistOptions> (
                Wmp11Ids.MusicContributingArtists, "Contributing Artists", GetId);
            album_artist_builder = new ContainerBuilder<BuildableMusicArtistOptions> (
                Wmp11Ids.MusicAlbumArtist, "Album Artist", GetId);
            composer_builder = new ContainerBuilder<BuildableMusicArtistOptions> (
                Wmp11Ids.MusicComposer, "Composer", GetId);
            one_star_builder = new ContainerBuilder<ContainerOptions> (
                Wmp11Ids.MusicRating1Star, "1+ stars", GetId);
            two_star_builder = new ContainerBuilder<ContainerOptions> (
                Wmp11Ids.MusicRating2Star, "2+ stars", GetId);
            three_star_builder = new ContainerBuilder<ContainerOptions> (
                Wmp11Ids.MusicRating3Star, "3+ stars", GetId);
            four_star_builder = new ContainerBuilder<ContainerOptions> (
                Wmp11Ids.MusicRating4Star, "4+ stars", GetId);
            five_star_builder = new ContainerBuilder<ContainerOptions> (
                Wmp11Ids.MusicRating5Star, "5+ stars", GetId);
        }

        public void OnTag (Tag tag, Action<Object> consumer)
        {
            var genres = tag.Genres;
            var artists = tag.Performers;
            var album_artists = tag.AlbumArtists;
            var composers = tag.Composers;
            var id = GetId ();

            var music_track = new MusicTrack (id, Wmp11Ids.AllMusic, new MusicTrackOptions {
                Title = tag.Title,
                OriginalTrackNumber = (int)tag.Track,
                Genres = genres,
                Artists = GetArtists (artists),
                Resources = new[] { new Resource (new Uri (Url, "?id=" + id), new ResourceOptions ()) }
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

        public Container Build (Action<ContainerInfo> consumer)
        {
            return Build (consumer, new Object[] {
                Build (consumer, "All Music", Wmp11Ids.AllMusic, audio_items),
                Build (consumer, genre_builder, (id, options) => new MusicGenre (GetId (), id, options)),
                Build (consumer, artist_builder, ArtistProvider),
                Build (consumer, album_builder, (id, options) => new MusicAlbum (GetId (), id, options)),
                Build (consumer, playlists_builder, (id, options) => new PlaylistContainer (GetId (), id, options)),
                Build (consumer, folders_builder, (id, options) => new StorageFolder (GetId (), id, options)),
                Build (consumer, contributing_artists_builder, ArtistProvider),
                Build (consumer, album_artist_builder, ArtistProvider),
                Build (consumer, composer_builder, ArtistProvider),
                Build (consumer, "Rating", Wmp11Ids.MusicRating, new Object[] {
                    Build (consumer, one_star_builder, RatingProvider),
                    Build (consumer, two_star_builder, RatingProvider),
                    Build (consumer, three_star_builder, RatingProvider),
                    Build (consumer, four_star_builder, RatingProvider),
                    Build (consumer, five_star_builder, RatingProvider),
                })
            });
        }

        MusicArtist ArtistProvider (string parentId, MusicArtistOptions options)
        {
            return new MusicArtist (GetId (), parentId, options);
        }

        Container RatingProvider (string parentId, ContainerOptions options)
        {
            return new Container (GetId (), parentId, options);
        }

        string GetId ()
        {
            return string.Concat ("music", (ids++).ToString());
        }

        static IEnumerable<PersonWithRole> GetArtists (IEnumerable<string> artists)
        {
            foreach (var artist in artists) {
                yield return new PersonWithRole (artist, "performer");
            }
        }
    }
}
