// 
// Container.cs
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

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mono.Upnp.DidlLite
{
	public abstract class Container : Object
	{
		readonly List<ClassReference> search_class_list = new List<ClassReference> ();
		readonly List<ClassReference> create_class_list = new List<ClassReference> ();
		readonly ReadOnlyCollection<ClassReference> search_classes;
		readonly ReadOnlyCollection<ClassReference> create_classes;
		
		internal Container ()
		{
			search_classes = search_class_list.AsReadOnly ();
			create_classes = create_class_list.AsReadOnly ();
		}
		
		bool has_child_count;
		bool has_searchable;
		
        public int ChildCount { get; private set; }
        public ReadOnlyCollection<ClassReference> CreateClasses { get { return create_classes; } }
		public ReadOnlyCollection<ClassReference> SearchClasses { get { return search_classes; } }
        public bool Searchable { get; private set; }
	}
}
