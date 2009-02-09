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

namespace Mono.Upnp.DidlLite.Av
{
	public class TextItem : Item
	{
		readonly List<PersonWithRole> author_list = new List<PersonWithRole> ();
		readonly ReadOnlyCollection<PersonWithRole> authors;
		readonly List<string> publisher_list = new List<string> ();
		readonly ReadOnlyCollection<string> publishers;
		readonly List<string> contributor_list = new List<string> ();
		readonly ReadOnlyCollection<string> contributors;
		
		internal TextItem ()
		{
			authors = author_list.AsReadOnly ();
			publishers = publisher_list.AsReadOnly ();
			contributors = contributor_list.AsReadOnly ();
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
		public string Relation { get; private set; }
		public string Language { get; private set; }
		public string Rights { get; private set; }
	}
}
