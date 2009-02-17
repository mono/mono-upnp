// 
// ObjectBuilder.cs
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

namespace Mono.Upnp.ContentDirectory
{
	public abstract class ObjectBuilder
	{
		static readonly Dictionary<Type, string> class_names = new Dictionary<Type, string> ();
		
		readonly List<ResourceBuilder> resources = new List<ResourceBuilder> ();
		
		[XmlAttribute ("id", Namespace = Schemas.DidlLiteSchema)]
		string id = "";
		
		[XmlAttribute ("restricted", Namespace = Schemas.DidlLiteSchema)]
        bool restricted;
		
		[XmlElement ("title", Namespace = Schemas.DublinCoreSchema)]
		public string Title { get; set; }
		
		[XmlElement ("creator", Namespace = Schemas.DublinCoreSchema)]
        public string Creator { get; set; }
		
		[XmlArrayItem]
		protected ICollection<ResourceBuilder> Resources { get { return resources; } }
		
        public WriteStatus? WriteStatus { get; set; }
		
		[XmlElement ("class", Namespace = Schemas.UpnpSchema)]
		string Class {
			get {
				var type = GetType ();
				string name;
				if (class_names.TryGetValue (type, out name)) {
					return name;
				}
				name = ClassManager.CreateClassName (type, typeof (ObjectBuilder));
				class_names[type] = name;
				return name;
			}
		}
	}
}
