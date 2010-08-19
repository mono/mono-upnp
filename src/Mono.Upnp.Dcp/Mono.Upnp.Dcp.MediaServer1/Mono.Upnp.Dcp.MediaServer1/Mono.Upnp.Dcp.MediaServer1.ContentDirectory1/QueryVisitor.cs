// 
// QueryVisitor.cs
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

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    using Query = System.Action<QueryVisitor>;
    
    public class QueryVisitor
    {
        public virtual void VisitAllResults ()
        {
        }

        public virtual void VisitEquals (string property, string value)
        {
            VisitPropertyExpression (property, value);
        }

        public virtual void VisitDoesNotEqual (string property, string value)
        {
            VisitPropertyExpression (property, value);
        }

        public virtual void VisitLessThan (string property, string value)
        {
            VisitPropertyExpression (property, value);
        }

        public virtual void VisitLessThanOrEqualTo (string property, string value)
        {
            VisitPropertyExpression (property, value);
        }

        public virtual void VisitGreaterThan (string property, string value)
        {
            VisitPropertyExpression (property, value);
        }

        public virtual void VisitGreaterThanOrEqualTo (string property, string value)
        {
            VisitPropertyExpression (property, value);
        }

        public virtual void VisitContains (string property, string value)
        {
            VisitPropertyExpression (property, value);
        }

        public virtual void VisitDoesNotContain (string property, string value)
        {
            VisitPropertyExpression (property, value);
        }

        public virtual void VisitDerivedFrom (string property, string value)
        {
            VisitPropertyExpression (property, value);
        }

        public virtual void VisitExists (string property, bool value)
        {
            VisitExpression ();
        }

        public virtual void VisitAnd (Query leftOperand, Query rightOperand)
        {
            VisitBinaryExpression (leftOperand, rightOperand);
        }

        public virtual void VisitOr (Query leftOperand, Query rightOperand)
        {
            VisitBinaryExpression (leftOperand, rightOperand);
        }

        protected virtual void VisitPropertyExpression (string property, string value)
        {
            VisitExpression ();
        }

        protected virtual void VisitBinaryExpression (Query leftOperand, Query rightOperand)
        {
            VisitExpression ();
        }

        protected virtual void VisitExpression ()
        {
        }
    }
}
