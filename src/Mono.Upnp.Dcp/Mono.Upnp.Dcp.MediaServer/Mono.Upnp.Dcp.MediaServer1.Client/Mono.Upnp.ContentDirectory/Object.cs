// 
// Object.cs
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
	public abstract class Object
	{
		readonly ContentDirectory content_directory;
		readonly List<Resource> resource_list = new List<Resource> ();
		readonly ReadOnlyCollection<Resource> resources;
		bool has_class;
        bool has_restricted;
		bool verified;
		
		protected Object (ContentDirectory contentDirectory)
		{
			if (contentDirectory == null) throw new ArgumentNullException ("contentDirectory");
			
			content_directory = contentDirectory;
			resources = resource_list.AsReadOnly ();
		}
		
		protected BrowseResult Browse (uint requestCount, string sortCriteria)
		{
			var browse_results = new BrowseResult (content_directory, Id, requestCount, sortCriteria);
			browse_results.FetchResults ();
			return browse_results;
		}
		
		public Container GetParent ()
		{
			return ParentId == "-1" ? null : content_directory.GetObject<Container> (ParentId);
		}
		
		public bool IsOutOfDate {
			get { return content_directory.CheckIfObjectIsOutOfDate (this); }
		}

        public string Id { get; private set; }
        public string ParentId { get; private set; }
        public string Title { get; private set; }
        public string Creator { get; private set; }
        public ReadOnlyCollection<Resource> Resources { get { return resources; } }
        public Class Class { get; private set; }
        public bool Restricted { get; private set; }
        public WriteStatus? WriteStatus { get; private set; }
		internal uint ParentUpdateId { get; set; }
		
		public override string ToString ()
		{
			return string.Format("{0} ({1})", Id, Class.FullClassName);
		}


        internal void Deserialize (XmlReader reader)
        {
            DeserializeRootElement (reader);
            Verify ();
        }

        protected virtual void DeserializeRootElement (XmlReader reader)
        {
			if (reader == null) throw new ArgumentNullException ("reader");
			
            Id = reader["id", Schemas.DidlLiteSchema];
            ParentId = reader["parentID", Schemas.DidlLiteSchema];
            bool restricted;
            if (bool.TryParse (reader["restricted", Schemas.DidlLiteSchema], out restricted)) {
                Restricted = restricted;
				has_restricted = true;
            }
			
			while (ReadToNextElement (reader)) {
				var property_reader = reader.ReadSubtree ();
				property_reader.Read ();
				try {
					DeserializePropertyElement (property_reader);
				} catch (Exception e) {
					// TODO log?
				} finally {
					property_reader.Close ();
				}
			}
        }
		
		protected virtual void DeserializePropertyElement (XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException ("reader");
			
			if (reader.NamespaceURI == Schemas.DidlLiteSchema) {
				if (reader.Name == "res") {
					resource_list.Add (new Resource (reader));
				} else {
					reader.Skip (); // This is a workaround for Mono bug 334752
				}
			} else if (reader.NamespaceURI == Schemas.UpnpSchema) {
				switch (reader.Name) {
				case "class":
					Class = new Class (reader);
					has_class = true;
					break;
				case "writeStatus":
					// TODO parse here
					break;
				default: // This is a workaround for Mono bug 334752
					reader.Skip ();
					break;
				}
			} else if (reader.NamespaceURI == Schemas.DublinCoreSchema) {
				switch (reader.Name) {
				case "title":
					Title = reader.ReadString ();
					break;
				case "creator":
					Creator = reader.ReadString ();
					break;
				default: // This is a workaround for Mono bug 334752
					reader.Skip ();
					break;
				}
			} else { // This is a workaround for Mono bug 334752
				reader.Skip ();
			}
		}

        void Verify ()
        {
			VerifyDeserialization ();
			if (!verified) {
				throw new DeserializationException (
					"The deserialization has not been fully verified. Be sure to call base.VerifyDeserialization ().");
			}
        }
		
		protected virtual void VerifyDeserialization ()
		{
			if (Id == null)
				throw new DeserializationException ("The object does not have an ID.");
            if (ParentId == null)
				throw new DeserializationException (string.Format ("The object {0} does not have a parent ID.", Id));
            if (Title == null)
				throw new DeserializationException (string.Format ("The object {0} does not have a title.", Id));
            if (!has_class)
				throw new DeserializationException (string.Format ("The object {0} does not have a class.", Id));
            if (!has_restricted)
				throw new DeserializationException (string.Format ("The object {0} does not have a restricted value.", Id));
			verified = true;
		}
		
		static bool ReadToNextElement (XmlReader reader)
        {
            while (reader.Read ()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    return true;
                }
            }
            return false;
        }
	}
}