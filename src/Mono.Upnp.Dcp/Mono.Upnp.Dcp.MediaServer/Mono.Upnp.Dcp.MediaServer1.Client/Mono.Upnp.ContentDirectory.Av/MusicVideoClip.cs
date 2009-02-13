// 
// MusicVideoClip.cs
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
	public class MusicVideoClip : VideoItem
	{
		readonly List<PersonWithRole> artist_list = new List<PersonWithRole> ();
		readonly ReadOnlyCollection<PersonWithRole> artists;
		readonly List<string> director_list = new List<string> ();
		readonly ReadOnlyCollection<string> directors;
		readonly List<string> contributor_list = new List<string> ();
		readonly ReadOnlyCollection<string> contributors;
		readonly List<string> album_list = new List<string>();
		readonly ReadOnlyCollection<string> albums;
		readonly List<DateTime> scheduled_start_time_list = new List<DateTime>();
		readonly ReadOnlyCollection<DateTime> scheduled_start_times;
		readonly List<DateTime> scheduled_end_time_list = new List<DateTime>();
		readonly ReadOnlyCollection<DateTime> scheduled_end_times;
		
		protected MusicVideoClip ()
		{
			artists = artist_list.AsReadOnly ();
			directors = director_list.AsReadOnly ();
			contributors = contributor_list.AsReadOnly ();
			albums = album_list.AsReadOnly ();
			scheduled_start_times = scheduled_start_time_list.AsReadOnly ();
			scheduled_end_times = scheduled_end_time_list.AsReadOnly ();
		}
		
        public ReadOnlyCollection<PersonWithRole> Artists { get { return artists; } }
		public string StorageMedium { get; private set; }
		public ReadOnlyCollection<string> Albums { get { return albums; } }
		public ReadOnlyCollection<DateTime> ScheduledStartTimes { get { return scheduled_start_times; } }
        public ReadOnlyCollection<DateTime> ScheduledEndTimes { get { return scheduled_end_times; } }
		public ReadOnlyCollection<string> Directors { get { return directors; } }
		public ReadOnlyCollection<string> Contributors { get { return contributors; } }
		public string Date { get; private set; }
		
		protected override void DeserializePropertyElement (XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException ("reader");
			
			if (reader.NamespaceURI == Schemas.UpnpSchema) {
				switch (reader.Name) {
				case "artist":
					artist_list.Add (new PersonWithRole (reader));
					break;
				case "director":
					director_list.Add (reader.ReadString ());
					break;
				case "album":
					album_list.Add (reader.ReadString ());
					break;
				case "scheduledStartTime":
					scheduled_start_time_list.Add (reader.ReadElementContentAsDateTime ()); // TODO is this going to work?
					break;
				case "scheduledEndTime":
					scheduled_end_time_list.Add (reader.ReadElementContentAsDateTime ());
					break;
				default:
					base.DeserializePropertyElement (reader);
					break;
				}
			} else if (reader.NamespaceURI == Schemas.DublinCoreSchema) {
				switch (reader.Name) {
				case "contributor":
					contributor_list.Add (reader.ReadString ());
					break;
				case "date":
					Date = reader.ReadString ();
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
