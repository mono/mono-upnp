// 
// Wmp11MusicBuilderTests.cs
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

using NUnit.Framework;
using TagLib.Id3v2;

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV;

using UpnpObject = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem.Tests
{
    [TestFixture]
    public class Wmp11MusicBuilderTests
    {
        [Test]
        public void BasicTag ()
        {
            var builder = new Wmp11MusicBuilder ();
            var objects = new List<UpnpObject> ();
            builder.OnTag (new Tag {
                Title = "Foo Bar",
                Track = 42
            }, item => objects.Add (item));
            builder.OnDone (info => objects.Add (info.Container));

            var music_track = objects[0] as MusicTrack;
            Assert.AreEqual ("Foo Bar", music_track.Title);
            Assert.AreEqual (42, music_track.OriginalTrackNumber);
        }

        [Test]
        public void BasicGenreTag ()
        {
            var builder = new Wmp11MusicBuilder ();
            var objects = new List<UpnpObject> ();

            builder.OnTag (new Tag {
                Title = "Foo Bar",
                Track = 42,
                Genres = new[] { "Bat" },
            }, item => objects.Add (item));

            builder.OnDone (info => objects.Add (info.Container));

            var music_track = objects[0] as MusicTrack;
            Assert.AreEqual ("Foo Bar", music_track.Title);
            Assert.AreEqual (42, music_track.OriginalTrackNumber);
            Assert.AreEqual ("Bat", music_track.Genres[0]);

            var reference = objects[1] as Item;
            Assert.AreEqual (music_track.Id, reference.RefId);

            var music_genre = objects[3] as MusicGenre;
            Assert.AreEqual ("Bat", music_genre.Title);
            Assert.AreEqual (1, music_genre.ChildCount);
        }

        [Test]
        public void MultipleGenresTag ()
        {
            var builder = new Wmp11MusicBuilder ();
            var objects = new List<UpnpObject> ();

            builder.OnTag (new Tag {
                Title = "Foo Bar",
                Track = 42,
                Genres = new[] { "Bat", "Baz" },
            }, item => objects.Add (item));

            builder.OnDone (info => objects.Add (info.Container));

            var music_track = objects[0] as MusicTrack;
            Assert.AreEqual ("Foo Bar", music_track.Title);
            Assert.AreEqual (42, music_track.OriginalTrackNumber);
            Assert.AreEqual ("Bat", music_track.Genres[0]);
            Assert.AreEqual ("Baz", music_track.Genres[1]);

            Assert.AreEqual (music_track.Id, ((Item)objects[1]).RefId);
            Assert.AreEqual (music_track.Id, ((Item)objects[2]).RefId);

            var music_genre = objects[4] as MusicGenre;
            Assert.AreEqual ("Bat", music_genre.Title);
            Assert.AreEqual (1, music_genre.ChildCount);

            music_genre = objects[5] as MusicGenre;
            Assert.AreEqual ("Baz", music_genre.Title);
            Assert.AreEqual (1, music_genre.ChildCount);
        }

        [Test]
        public void BasicArtistTag ()
        {
            var builder = new Wmp11MusicBuilder ();
            var objects = new List<UpnpObject> ();

            builder.OnTag (new Tag {
                Title = "Foo Bar",
                Track = 42,
                Performers = new[] { "Boo Far" }
            }, item => objects.Add (item));

            builder.OnDone (info => objects.Add (info.Container));

            var music_track = objects[0] as MusicTrack;
            Assert.AreEqual ("Foo Bar", music_track.Title);
            Assert.AreEqual (42, music_track.OriginalTrackNumber);
            Assert.AreEqual ("Boo Far", music_track.Artists[0].Name);

            Assert.AreEqual (music_track.Id, ((Item)objects[1]).RefId);

            var music_artist = objects[4] as MusicArtist;
            Assert.AreEqual ("Boo Far", music_artist.Title);
            Assert.AreEqual (1, music_artist.ChildCount);
        }

        [Test]
        public void BasicArtistAndGenreTag ()
        {
            var builder = new Wmp11MusicBuilder ();
            var objects = new List<UpnpObject> ();

            builder.OnTag (new Tag {
                Title = "Foo Bar",
                Track = 42,
                Performers = new[] { "Boo Far" },
                Genres = new[] { "Bat" }
            }, item => objects.Add (item));

            builder.OnDone (info => objects.Add (info.Container));

            var music_track = objects[0] as MusicTrack;
            Assert.AreEqual ("Foo Bar", music_track.Title);
            Assert.AreEqual (42, music_track.OriginalTrackNumber);
            Assert.AreEqual ("Boo Far", music_track.Artists[0].Name);
            Assert.AreEqual ("Bat", music_track.Genres[0]);

            Assert.AreEqual (music_track.Id, ((Item)objects[1]).RefId);
            Assert.AreEqual (music_track.Id, ((Item)objects[2]).RefId);

            var music_genre = objects[4] as MusicGenre;
            Assert.AreEqual ("Bat", music_genre.Title);
            Assert.AreEqual (1, music_genre.ChildCount);

            var music_artist = objects[6] as MusicArtist;
            Assert.AreEqual ("Boo Far", music_artist.Title);
            Assert.AreEqual (1, music_artist.ChildCount);
            Assert.AreEqual ("Bat", music_artist.Genres[0]);
        }

        [Test]
        public void MultipleArtistAndGenreTags ()
        {
            var builder = new Wmp11MusicBuilder ();
            var objects = new List<UpnpObject> ();
            Action<UpnpObject> consumer = @object => objects.Add (@object);

            builder.OnTag (new Tag {
                Title = "Foo Bar",
                Track = 42,
                Performers = new[] { "Boo Far" },
                Genres = new[] { "Bazz" }
            }, consumer);

            builder.OnTag (new Tag {
                Title = "Hurt",
                Track = 1,
                Performers = new[] { "Our Lady J" },
                Genres = new[] { "Bazz", "Electro Gospel" }
            }, consumer);

            builder.OnDone (info => objects.Add (info.Container));

            var music_track = objects[0] as MusicTrack;
            Assert.AreEqual ("Foo Bar", music_track.Title);
            Assert.AreEqual (42, music_track.OriginalTrackNumber);
            Assert.AreEqual ("Boo Far", music_track.Artists[0].Name);
            Assert.AreEqual ("Bazz", music_track.Genres[0]);

            Assert.AreEqual (music_track.Id, ((Item)objects[1]).RefId);
            Assert.AreEqual (music_track.Id, ((Item)objects[2]).RefId);

            music_track = objects[3] as MusicTrack;
            Assert.AreEqual ("Hurt", music_track.Title);
            Assert.AreEqual (1, music_track.OriginalTrackNumber);
            Assert.AreEqual ("Our Lady J", music_track.Artists[0].Name);
            Assert.AreEqual ("Bazz", music_track.Genres[0]);
            Assert.AreEqual ("Electro Gospel", music_track.Genres[1]);

            Assert.AreEqual (music_track.Id, ((Item)objects[4]).RefId);
            Assert.AreEqual (music_track.Id, ((Item)objects[5]).RefId);
            Assert.AreEqual (music_track.Id, ((Item)objects[6]).RefId);

            var music_genre = objects[8] as MusicGenre;
            Assert.AreEqual ("Bazz", music_genre.Title);
            Assert.AreEqual (2, music_genre.ChildCount);

            music_genre = objects[9] as MusicGenre;
            Assert.AreEqual ("Electro Gospel", music_genre.Title);
            Assert.AreEqual (1, music_genre.ChildCount);

            var music_artist = objects[11] as MusicArtist;
            Assert.AreEqual ("Boo Far", music_artist.Title);
            Assert.AreEqual (1, music_artist.ChildCount);
            Assert.AreEqual ("Bazz", music_artist.Genres[0]);

            music_artist = objects[12] as MusicArtist;
            Assert.AreEqual ("Our Lady J", music_artist.Title);
            Assert.AreEqual (1, music_artist.ChildCount);
            Assert.AreEqual ("Bazz", music_artist.Genres[0]);
            Assert.AreEqual ("Electro Gospel", music_artist.Genres[1]);
        }
    }
}
