// 
// Property.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Thomas
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

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;

namespace Mono.Upnp.Dcp.MediaServer1.Tests
{
    #pragma warning disable 0660, 0661
    public class Property
    {
        readonly string name;

        public Property (string name)
        {
            this.name = name;
        }

        public Query Contains (string value)
        {
            return visitor => visitor.VisitContains (name, value);
        }

        public Query DoesNotContain (string value)
        {
            return visitor => visitor.VisitDoesNotContain (name, value);
        }

        public Query DerivedFrom (string value)
        {
            return visitor => visitor.VisitDerivedFrom (name, value);
        }

        public Query Exists (bool value)
        {
            return visitor => visitor.VisitExists (name, value);
        }

        public static Query operator ==(Property property, string value)
        {
            return visitor => visitor.VisitEquals (property.name, value);
        }

        public static Query operator !=(Property property, string value)
        {
            return visitor => visitor.VisitDoesNotEqual (property.name, value);
        }

        public static Query operator <(Property property, string value)
        {
            return visitor => visitor.VisitLessThan (property.name, value);
        }

        public static Query operator <=(Property property, string value)
        {
            return visitor => visitor.VisitLessThanOrEqualTo (property.name, value);
        }

        public static Query operator >(Property property, string value)
        {
            return visitor => visitor.VisitGreaterThan (property.name, value);
        }

        public static Query operator >=(Property property, string value)
        {
            return visitor => visitor.VisitGreaterThanOrEqualTo (property.name, value);
        }
    }
    #pragma warning restore 0661, 0660
}

