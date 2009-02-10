// 
// AudioBook.cs
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

namespace Mono.Upnp.DidlLite.Av
{
	public class AudioBook : AudioItem
	{
		readonly List<string> producer_list = new List<string> ();
		readonly ReadOnlyCollection<string> producers;
		readonly List<string> contributor_list = new List<string> ();
		readonly ReadOnlyCollection<string> contributors;
		
		protected AudioBook ()
		{
			producers = producer_list.AsReadOnly ();
			contributors = contributor_list.AsReadOnly ();
		}
		
        public string StorageMedium { get; private set; }
        public ReadOnlyCollection<string> Producers { get { return producers; } }
        public ReadOnlyCollection<string> Contributors { get { return contributors; } }
        public string Date { get; private set; }
		
		protected override void DeserializePropertyElement (XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException ("reader");
			protected
			if (reader.NamespaceURI == Protocol.UpnpSchema) {
				switch (reader.Name) {
				case "producer":
					producer_list.Add (reader.ReadString ());
					break;
				default:
					base.DeserializePropertyElement (reader);
					break;
				}
			} else if (reader.NamespaceURI == Protocol.DublinCoreSchema) {
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
