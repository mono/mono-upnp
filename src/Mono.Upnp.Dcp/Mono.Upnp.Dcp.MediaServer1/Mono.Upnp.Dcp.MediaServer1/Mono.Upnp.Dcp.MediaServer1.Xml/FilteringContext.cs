// 
// FilteringContext.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Peterson
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

namespace Mono.Upnp.Dcp.MediaServer1.Xml
{
    public class FilteringContext
    {
        readonly Type type;
        readonly ICollection<string> elements;
        readonly ICollection<string> attributes;
        readonly string nested_property_name;

        public FilteringContext (Type type, ICollection<string> properties)
        {
            if (type == null) {
                throw new ArgumentNullException ("type");
            }

            this.type = type;

            if (properties == null) {
                this.elements = new string[0];
                this.attributes = new string[0];
            } else if (!properties.Contains ("*")) {
                elements = new List<string> ();
                attributes = new List<string> ();
                foreach (var property in properties) {
                    var aeropostal = property.IndexOf ('@');
                    if (aeropostal == -1) {
                        elements.Add (property);
                    } else {
                        // TODO include the element if it's not present?
                        attributes.Add (property);
                    }
                }
            }
        }

        FilteringContext (Type type,
                          ICollection<string> elements,
                          ICollection<string> attributes,
                          string nestedPropertyName)
        {
            this.type = type;
            this.elements = elements;
            this.attributes = attributes;
            this.nested_property_name = nestedPropertyName;
        }

        public bool IncludesElement (string element)
        {
            return IncludesProperty (elements, element);
        }

        public bool IncludesAttribute (string attribute)
        {
            return IncludesProperty (attributes, attribute);
        }

        static bool IncludesProperty (ICollection<string> collection, string property)
        {
            if (collection == null) {
                return true;
            } else {
                return collection.Contains (property);
            }
        }

        internal Type Type {
            get { return type; }
        }

        internal bool IsNested {
            get { return nested_property_name != null; }
        }

        internal string NestedPropertyName {
            get { return nested_property_name; }
        }

        internal FilteringContext GetNestedContext (string nestedPropertyname)
        {
            return new FilteringContext (type, elements, attributes, nestedPropertyname);
        }
    }
}

