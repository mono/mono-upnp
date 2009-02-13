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
using System.Xml;

namespace Mono.Upnp.ContentDirectory
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
		
        public int? ChildCount { get; private set; }
        public ReadOnlyCollection<ClassReference> CreateClasses { get { return create_classes; } }
		public ReadOnlyCollection<ClassReference> SearchClasses { get { return search_classes; } }
        public bool Searchable { get; private set; }
		
		public BrowserResults GetContents ()
		{
		}
		
		public bool CanSearchFor (Type type)
		{
			return IsValidType (type, search_classes);
		}
		
		public bool CanCreate (Type type)
		{
			return IsValidType (type, create_classes);
		}
		
		static bool IsValidType (Type type, IEnumerable<ClassReference> classes)
		{
			// TODO this
//			foreach (var @class in classes) {
//				if (@class.Class == type || (@class.IncludeDerived && @class.Class.IsSubclassOf (type))) {
//					return true;
//				}
//			}
			return false;
		}
		
		protected override void DeserializeRootElement (XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException ("reader");
			
			Searchable = reader["searchable", Schemas.DidlLiteSchema] == "true";
			int child_count;
			if (int.TryParse (reader["childCount", Schemas.DidlLiteSchema], out child_count)) {
				ChildCount = child_count;
			}
			
			base.DeserializeRootElement (reader);
		}
		
		protected override void DeserializePropertyElement (XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException ("reader");
			
			if (reader.NamespaceURI == Schemas.UpnpSchema) {
				switch (reader.Name) {
				case "searchClass":
					search_class_list.Add (new ClassReference (reader));
					break;
				case "createClass":
					create_class_list.Add (new ClassReference (reader));
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
