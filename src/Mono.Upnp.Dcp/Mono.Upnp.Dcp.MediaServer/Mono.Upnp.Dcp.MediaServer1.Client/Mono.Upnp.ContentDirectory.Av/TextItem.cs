// 
// TextItem.cs
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
	public class TextItem : Item
	{
		readonly List<PersonWithRole> author_list = new List<PersonWithRole> ();
		readonly ReadOnlyCollection<PersonWithRole> authors;
		readonly List<string> publisher_list = new List<string> ();
		readonly ReadOnlyCollection<string> publishers;
		readonly List<string> contributor_list = new List<string> ();
		readonly ReadOnlyCollection<string> contributors;
		readonly List<Uri> relation_list = new List<Uri>();
		readonly ReadOnlyCollection<Uri> relations;
		readonly List<string> right_list = new List<string> ();
		readonly ReadOnlyCollection<string> rights;
		
		protected TextItem ()
		{
			authors = author_list.AsReadOnly ();
			publishers = publisher_list.AsReadOnly ();
			contributors = contributor_list.AsReadOnly ();
			relations = relation_list.AsReadOnly ();
			rights = right_list.AsReadOnly ();
		}
		
		public ReadOnlyCollection<PersonWithRole> Authors { get { return authors; } }
		public string Protection { get; private set; }
		public string LongDescription { get; private set; }
		public string StorageMedium { get; private set; }
		public string Rating { get; private set; }
		public string Description { get; private set; }
		public ReadOnlyCollection<string> Publishers { get { return publishers; } }
		public ReadOnlyCollection<string> Contributors { get { return contributors; } }
		public string Date { get; private set; }
		public ReadOnlyCollection<Uri> Relations { get { return relations; } }
		public string Language { get; private set; }
		public ReadOnlyCollection<string> Rights { get { return rights; } }
		
		protected override void DeserializePropertyElement (XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException ("reader");
			
			if (reader.NamespaceURI == Schemas.UpnpSchema) {
				switch (reader.Name) {
				case "author":
					author_list.Add (new PersonWithRole (reader));
					break;
				case "longDescription":
					LongDescription = reader.ReadString ();
					break;
				case "rating":
					Rating = reader.ReadString ();
					break;
				default:
					base.DeserializePropertyElement (reader);
					break;
				}
			} else if (reader.NamespaceURI == Schemas.DublinCoreSchema) {
				switch (reader.Name) {
				case "publisher":
					publisher_list.Add (reader.ReadString ());
					break;
				case "contributor":
					contributor_list.Add (reader.ReadString ());
					break;
				case "relation":
					relation_list.Add (new Uri (reader.ReadString ()));
					break;
				case "description":
					Description = reader.ReadString ();
					break;
				case "rights":
					right_list.Add (reader.ReadString ());
					break;
				case "date":
					Date = reader.ReadString ();
					break;
				case "language":
					Language = reader.ReadString ();
					break;
				default:
					base.DeserializeRootElement (reader);
					break;
				}
			} else {
				base.DeserializeRootElement (reader);
			}
		}
	}
}
