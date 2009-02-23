// 
// VideoItemBuilder.cs
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
using System.Xml.Serialization;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Av
{
	[ClassName ("videoItem")]
	public class VideoItemBuilder : ItemBuilder
	{
		readonly List<string> genres = new List<string> ();
		readonly List<string> producers = new List<string> ();
		readonly List<PersonWithRole> actors = new List<PersonWithRole> ();
		readonly List<string> directors = new List<string> ();
		readonly List<string> publishers = new List<string> ();
		readonly List<Uri> relations = new List<Uri> ();
		
		[XmlElement ("genre", Namespace = Schemas.UpnpSchema)]
		public ICollection<string> Genres { get { return genres; } }
		
		[XmlElement ("longDescription", Namespace = Schemas.UpnpSchema)]
        public string LongDescription { get; set; }
		
		[XmlElement ("producer", Namespace = Schemas.UpnpSchema)]
        public ICollection<string> Producers { get { return producers; } }
		
		[XmlElement ("rating", Namespace = Schemas.UpnpSchema)]
        public string Rating { get; set; }
		
		[XmlElement ("actor", Namespace = Schemas.UpnpSchema)]
        public ICollection<PersonWithRole> Actors { get { return actors; } }
		
		[XmlElement ("director", Namespace = Schemas.UpnpSchema)]
        public ICollection<string> Directors { get { return directors; } }
		
		[XmlElement ("description", Namespace = Schemas.DublinCoreSchema)]
        public string Description { get; set; }
		
		[XmlElement ("publisher", Namespace = Schemas.DublinCoreSchema)]
        public ICollection<string> Publishers { get {return publishers; } }
		
		[XmlElement ("language", Namespace = Schemas.DublinCoreSchema)]
        public string Language { get; set; }
		
		[XmlElement ("relation", Namespace = Schemas.DublinCoreSchema)]
        public ICollection<Uri> Relations { get { return relations; } }
	}
}
