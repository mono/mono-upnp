// 
// MusicAlbum.cs
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
using System.Xml;

namespace Mono.Upnp.ContentDirectory.Av
{
	public class MusicAlbum : Album
	{
		readonly List<PersonWithRole> artist_list = new List<PersonWithRole> ();
		readonly ReadOnlyCollection<PersonWithRole> artists;
		readonly List<string> producer_list = new List<string> ();
		readonly ReadOnlyCollection<string> producers;
		readonly List<string> genre_list = new List<string> ();
		readonly ReadOnlyCollection<string> genres;
		readonly List<Uri> album_art_uri_list = new List<Uri> ();
		readonly ReadOnlyCollection<Uri> album_art_uris;
		
		protected MusicAlbum ()
		{
			artists = artist_list.AsReadOnly ();
			producers = producer_list.AsReadOnly ();
			genres = genre_list.AsReadOnly ();
			album_art_uris = album_art_uri_list.AsReadOnly ();
		}
		
        public ReadOnlyCollection<PersonWithRole> Artists { get { return artists; } }
		public ReadOnlyCollection<string> Genres { get { return genres; } }
		public ReadOnlyCollection<string> Producers { get { return producers; } }
		public ReadOnlyCollection<Uri> AlbumArtUris { get { return album_art_uris; } }
		public string Toc { get; private set; }
		
		protected override void DeserializePropertyElement (XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException ("reader");
			
			if (reader.NamespaceURI == Schemas.UpnpSchema) {
				switch (reader.Name) {
				case "artist":
					artist_list.Add (new PersonWithRole (reader));
					break;
				case "producer":
					producer_list.Add (reader.ReadString ());
					break;
				case "genre":
					genre_list.Add (reader.ReadString ());
					break;
				case "albumArtURI":
					album_art_uri_list.Add (new Uri (reader.ReadString ()));
					break;
				case "toc":
					Toc = reader.ReadString ();
					break;
				default:
					base.DeserializePropertyElement (reader);
					break;
				}
			} else {
				base.DeserializePropertyElement (reader);
			}
		}
	}
}
