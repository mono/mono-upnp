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
		
		public Results<Object> Browse ()
		{
			return Browse (null);
		}
		
		public Results<Object> Browse (ResultsSettings settings)
		{
			var results = settings != null
				? new BrowseResults (ContentDirectory, Id, settings.SortCriteria,
					settings.Filter, settings.RequestCount, settings.Offset)
				: new BrowseResults (ContentDirectory, Id, null, null, 0, 0);
			results.FetchResults ();
			return results;
		}
		
		protected Results<T> Browse<T> (ResultsSettings settings) where T : Object
		{
			// TODO I don't know about this
			if (!CanSearchForType<T> ()) throw new ArgumentException (string.Format (
				"The container cannot search for the type {0}.", typeof (T)));
			
			return Search<T> (string.Format (
				@"upnp:class derivedFrom ""{0}""", ClassManager.GetClassFromType<T>()), settings);
		}
		
		public Results<Object> Search (string searchCriteria)
		{
			return Search (searchCriteria, null);
		}
		
		public Results<Object> Search (string searchCriteria, ResultsSettings settings)
		{
			return Search<Object> (searchCriteria, settings);
		}
		
		Results<T> Search<T> (string searchCriteria, ResultsSettings settings) where T : Object
		{
			if (searchCriteria == null) throw new ArgumentNullException ("searchCriteria");
			
			var results = settings != null
				? new SearchResults<T> (ContentDirectory, Id, searchCriteria,
					settings.SortCriteria, settings.Filter, settings.RequestCount, settings.Offset)
				: new SearchResults<T> (ContentDirectory, Id, searchCriteria, null, null, 0, 0);
			results.FetchResults ();
			return results;
		}
		
		public bool CanSearchForType<T> () where T : Object
		{
			return Searchable && IsValidType<T> (search_classes);
		}
		
		public bool CanCreateType<T> () where T : Object
		{
			return IsValidType<T> (create_classes);
		}
		
		static bool IsValidType<T> (IList<ClassReference> classes) where T : Object
		{
			if (classes.Count == 0) {
				return true;
			}
			var type = ClassManager.GetClassFromType<T> ();
			foreach (var @class in classes) {
				var class_name = @class.Class.FullClassName;
				var compare = type.CompareTo (class_name);
				if (compare == 0) {
					return true;
				} else if (compare == 1) {
					return false;
				} else if (type.StartsWith (class_name) && @class.IncludeDerived) {
					return true;
				}
			}
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
		
		protected override void VerifyDeserialization ()
		{
			base.VerifyDeserialization ();
			search_class_list.Sort ();
			create_class_list.Sort ();
		}
	}
}
