// 
// Query.cs
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

using System;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    #pragma warning disable 0660, 0661
    public class Query
    {
        readonly string name;

        public Query (string name)
        {
            this.name = name;
        }

        public Action<QueryVisitor> Contains (string value)
        {
            return visitor => visitor.VisitContains (name, value);
        }

        public Action<QueryVisitor> DoesNotContain (string value)
        {
            return visitor => visitor.VisitDoesNotContain (name, value);
        }

        public Action<QueryVisitor> DerivedFrom (string value)
        {
            return visitor => visitor.VisitDerivedFrom (name, value);
        }

        public Action<QueryVisitor> Exists (bool value)
        {
            return visitor => visitor.VisitExists (name, value);
        }

        public static Action<QueryVisitor> operator == (Query property, string value)
        {
            return visitor => visitor.VisitEquals (property.name, value);
        }

        public static Action<QueryVisitor> operator != (Query property, string value)
        {
            return visitor => visitor.VisitDoesNotEqual (property.name, value);
        }

        public static Action<QueryVisitor> operator < (Query property, string value)
        {
            return visitor => visitor.VisitLessThan (property.name, value);
        }

        public static Action<QueryVisitor> operator <= (Query property, string value)
        {
            return visitor => visitor.VisitLessThanOrEqualTo (property.name, value);
        }

        public static Action<QueryVisitor> operator > (Query property, string value)
        {
            return visitor => visitor.VisitGreaterThan (property.name, value);
        }

        public static Action<QueryVisitor> operator >= (Query property, string value)
        {
            return visitor => visitor.VisitGreaterThanOrEqualTo (property.name, value);
        }
    }
    #pragma warning restore 0661, 0660
}

