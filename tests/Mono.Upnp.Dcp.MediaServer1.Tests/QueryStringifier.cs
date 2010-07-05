// 
// QueryStringifier.cs
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

using System.Text;

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;

namespace Mono.Upnp.Dcp.MediaServer1.Tests
{
    public class QueryStringifier : QueryVisitor
    {
        StringBuilder builder;

        public QueryStringifier (StringBuilder builder)
        {
            this.builder = builder;
        }

        public override void VisitEquals (string property, string value)
        {
            VisitPropertyExpression (property, "==", value);
        }

        public override void VisitDoesNotEqual (string property, string value)
        {
            VisitPropertyExpression (property, "!=", value);
        }

        public override void VisitLessThan (string property, string value)
        {
            VisitPropertyExpression (property, "<", value);
        }

        public override void VisitLessThanOrEqualTo (string property, string value)
        {
            VisitPropertyExpression (property, "<=", value);
        }

        public override void VisitGreaterThan (string property, string value)
        {
            VisitPropertyExpression (property, ">", value);
        }

        public override void VisitGreaterThanOrEqualTo (string property, string value)
        {
            VisitPropertyExpression (property, ">=", value);
        }

        public override void VisitContains (string property, string value)
        {
            VisitPropertyExpression (property, "contains", value);
        }

        public override void VisitDoesNotContain (string property, string value)
        {
            VisitPropertyExpression (property, "doesNotContain", value);
        }

        public override void VisitDerivedFrom (string property, string value)
        {
            VisitPropertyExpression (property, "derivedFrom", value);
        }

        public override void VisitExists (string property, bool value)
        {
            VisitPropertyExpression (property, "exists", value ? "true" : "false");
        }

        void VisitPropertyExpression (string property, string @operator, string value)
        {
            builder.Append (property);
            builder.Append (' ');
            builder.Append (@operator);
            builder.Append (@" """);
            builder.Append (value);
            builder.Append ('"');
        }
    }
}

