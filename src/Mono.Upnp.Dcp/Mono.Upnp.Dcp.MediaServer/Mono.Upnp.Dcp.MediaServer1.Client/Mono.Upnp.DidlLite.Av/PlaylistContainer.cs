// 
// PlaylistContainer.cs
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

namespace Mono.Upnp.DidlLite.Av
{
	public sealed class PlaylistContainer : Container
	{
		readonly List<PersonWithRole> artist_list = new List<PersonWithRole> ();
		readonly ReadOnlyCollection<PersonWithRole> artists;
		readonly List<string> producer_list = new List<string> ();
		readonly ReadOnlyCollection<string> producers;
		readonly List<string> contributor_list = new List<string> ();
		readonly ReadOnlyCollection<string> contributors;
		
		internal PlaylistContainer ()
		{
			artists = artist_list.AsReadOnly ();
			producers = producer_list.AsReadOnly ();
			contributors = contributor_list.AsReadOnly ();
		}
		
        public ReadOnlyCollection<PersonWithRole> Artists { get { return artists; } }
		public string Genre { get; private set; }
		public string LongDescription { get; private set; }
		public ReadOnlyCollection<string> Producers { get { return producers; } }
		public string StorageMedium { get; private set; }
		public string Description { get; private set; }
		public ReadOnlyCollection<string> Contributors { get { return contributors; } }
		public string Date { get; private set; }
		public string Language { get; private set; }
		public string Rights { get; private set; }
	}
}
