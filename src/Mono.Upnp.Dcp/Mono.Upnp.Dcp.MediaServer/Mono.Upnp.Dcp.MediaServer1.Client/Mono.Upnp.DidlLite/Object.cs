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
using System.Xml;

namespace Mono.Upnp.DidlLite
{
	public abstract class Object
	{
		readonly List<Uri> res = new List<Uri> ();
        bool has_restricted;

        public string Id { get; private set; }
        public string ParentId { get; private set; }
        public string Title { get; private set; }
        public string Creator { get; private set; }
        public IEnumerable<Uri> Res { get { return res; } }
        public string Class { get; private set; }
        public bool Restricted { get; private set; }
        public WriteStatus? WriteStatus { get; private set; }

        public override bool Equals (object obj)
        {
            Object @object = obj as Object;
            return @object != null && @object.Id == Id;
        }

        public override int GetHashCode ()
        {
            return ~Id.GetHashCode ();
        }

        internal void Deserialize (XmlReader reader)
        {
            DeserializeCore (reader);
            Verify ();
        }

        protected virtual void DeserializeCore (XmlReader reader)
        {
			using (reader) {
	            reader.Read ();
	            Id = reader["id"];
	            ParentId = reader["parentID"];
	            bool restricted;
	            if (bool.TryParse (reader["restricted"], out restricted)) {
	                Restricted = restricted;
					has_restricted = true;
	            }
			}
        }

        void Verify ()
        {
            if (Id == null) throw new DeserializationException ("The object does not have an ID.");
            if (ParentId == null) throw new DeserializationException (string.Format ("The object {0} does not have a parent ID.", Id));
            if (Title == null) throw new DeserializationException (string.Format ("The object {0} does not have a title.", Id));
            if (Class == null) throw new DeserializationException (string.Format ("The object {0} does not have a class.", Id));
            if (!has_restricted) throw new DeserializationException (string.Format ("The object {0} does not have a restricted value.", Id));
			VerifyCore ();
        }
		
		protected virtual void VerifyCore ()
		{
		}
	}
}