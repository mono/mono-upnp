// 
// MusicBuilderTests.cs
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
    public class MusicBuilderTests
    {
        [Test]
        public void BasicTag ()
        {
            var builder = new MusicBuilder ();
            var objects = new List<UpnpObject> ();
            builder.OnTag (new Tag {
                Title = "Foo Bar",
                Track = 42
            }, item => objects.Add (item));
            var music_track = objects[0] as MusicTrack;
            Assert.AreEqual ("Foo Bar", music_track.Title);
            Assert.AreEqual (42, music_track.OriginalTrackNumber);
        }

        [Test]
        public void BasicArtistTag ()
        {
            var builder = new MusicBuilder ();
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
            var artist_enumerator = music_track.Artists.GetEnumerator ();
            Assert.IsTrue (artist_enumerator.MoveNext ());
            Assert.AreEqual ("Boo Far", artist_enumerator.Current.Name);

            var reference = objects[1] as Item;
            Assert.AreEqual (music_track.Id, reference.RefId);

            var music_artist = objects[2] as MusicArtist;
            Assert.AreEqual ("Boo Far", music_artist.Title);
            Assert.AreEqual (1, music_artist.ChildCount);
        }

        [Test]
        public void BasicArtistAndGenreTag ()
        {
            var builder = new MusicBuilder ();
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
            var artist_enumerator = music_track.Artists.GetEnumerator ();
            Assert.IsTrue (artist_enumerator.MoveNext ());
            Assert.AreEqual ("Boo Far", artist_enumerator.Current.Name);
            var genre_enumerator = music_track.Genres.GetEnumerator ();
            Assert.IsTrue (genre_enumerator.MoveNext ());
            Assert.AreEqual ("Bat", genre_enumerator.Current);

            var reference = objects[1] as Item;
            Assert.AreEqual (music_track.Id, reference.RefId);

            reference = objects[2] as Item;
            Assert.AreEqual (music_track.Id, reference.RefId);

            var music_genre = objects[3] as MusicGenre;
            Assert.AreEqual ("Bat", music_genre.Title);
            Assert.AreEqual (1, music_genre.ChildCount);

            var music_artist = objects[4] as MusicArtist;
            Assert.AreEqual ("Boo Far", music_artist.Title);
            Assert.AreEqual (1, music_artist.ChildCount);
            genre_enumerator = music_artist.Genres.GetEnumerator ();
            Assert.IsTrue (genre_enumerator.MoveNext ());
            Assert.AreEqual ("Bat", genre_enumerator.Current);
        }
    }
}
