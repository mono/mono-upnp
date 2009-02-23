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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
	public class Container : Object
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
		
		public Results<Object> Search (string searchCriteria)
		{
			return Search (searchCriteria, null);
		}
		
		public Results<Object> Search (string searchCriteria, ResultsSettings settings)
		{
			return Search<Object> (searchCriteria, settings);
		}
        
        public Results<T> SearchForType<T> () where T : Object
        {
            return SearchForType<T> (null);
        }
        
        public Results<T> SearchForType<T> (ResultsSettings settings) where T : Object
        {
            var class_name = ClassManager.GetClassFromType<T> ();
            return Search<T> (string.Format (@"upnp:class derivedfrom ""{0}""", class_name), settings);
        }
		
		internal Results<T> Search<T> (string searchCriteria, ResultsSettings settings) where T : Object
		{
			if (searchCriteria == null) throw new ArgumentNullException ("searchCriteria");
			
			var results = settings != null
				? new SearchResults<T> (ContentDirectory, Id, searchCriteria,
					settings.SortCriteria, settings.Filter, settings.RequestCount, settings.Offset)
				: new SearchResults<T> (ContentDirectory, Id, searchCriteria, null, null, 0, 0);
            results.FetchResults ();
			return results;
		}
		
		public T CreateObject<T> (ObjectBuilder builder) where T : Object
		{
			// TODO update resources
			var xml = new StringBuilder ();
			using (var stream = new StringWriter (xml)) {
				using (var writer = XmlWriter.Create (stream)) {
					writer.WriteStartElement ("DIDL-Lite", Schemas.DidlLiteSchema);
					writer.WriteAttributeString ("xmlns", "dc", null, Schemas.DublinCoreSchema);
					writer.WriteAttributeString ("xmlns", "upnp", null, Schemas.UpnpSchema);
					var serializer = new XmlSerializer (builder.GetType ());
					serializer.Serialize (writer, builder);
				}
			}
			var @object = ContentDirectory.Controller.CreateObject (Id, xml.ToString ());
			foreach (var result in ContentDirectory.Deserialize<T> ("*", @object)) {
				return result;
			}
			return null;
		}
		
		public bool CanSearchForType<T> () where T : Object
		{
			return Searchable && IsValidType<T> (search_classes, true);
		}
		
		public bool CanCreateType<T> () where T : Object
		{
			return ContentDirectory.Controller.CanCreateObject && IsValidType<T> (create_classes, false);
		}
		
		public Object CreateReference (Object target)
		{
			// TODO return actual object? That is another network call
			// (but we're doing network calls anyway, so it's not like
			// speed is SOOO important
			if (IsRestricted) throw new InvalidOperationException (
				"A reference cannot be created because the parent object is restricted.");
			
			var id = ContentDirectory.Controller.CreateReference (Id, target.Id);
			return ContentDirectory.GetObject <Object> (id);
		}
		
		static bool IsValidType<T> (IList<ClassReference> classes, bool allowSuperClasses) where T : Object
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
				} else if (allowSuperClasses && class_name.StartsWith (type)) {
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
			
			Searchable = ParseBool (reader["searchable"]);
			int child_count;
			if (int.TryParse (reader["childCount"], out child_count)) {
				ChildCount = child_count;
			}
			
			base.DeserializeRootElement (reader);
		}
		
		protected override void DeserializePropertyElement (XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException ("reader");
			
			if (reader.NamespaceURI == Schemas.UpnpSchema) {
				switch (reader.LocalName) {
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
