// 
// ObjectQueryVisitor.cs
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
using System.Collections.Generic;
using System.Reflection;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public class ObjectQueryVisitor : QueryVisitor
    {
        readonly ObjectQueryContext context;
        readonly Object @object;
        readonly Action<bool> consumer;

        public ObjectQueryVisitor (ObjectQueryContext context, Object @object, Action<bool> consumer)
        {
            if (context == null) {
                throw new ArgumentNullException ("context");
            } else if (@object == null) {
                throw new ArgumentNullException ("object");
            } else if (consumer == null) {
                throw new ArgumentNullException ("consumer");
            }
            this.context = context;
            this.@object = @object;
            this.consumer = consumer;
        }

        public override void VisitAllResults ()
        {
            consumer (true);
        }

        public override void VisitAnd (Query leftOperand, Query rightOperand)
        {
            Visit (leftOperand, leftResult => {
                if (leftResult) {
                    Visit (rightOperand, rightResult => consumer (rightResult));
                } else {
                    consumer (false);
                }
            });
        }

        public override void VisitOr (Query leftOperand, Query rightOperand)
        {
            Visit (leftOperand, leftResult => {
                if (leftResult) {
                    consumer (true);
                } else {
                    Visit (rightOperand, rightResult => consumer (rightResult));
                }
            });
        }

        public override void VisitEquals (string property, string value)
        {
            context.VisitProperty <object> (property, @object, v => {
                if (v == null) {
                    consumer (value == null);
                } else {
                    consumer (v.ToString ().Equals (value));
                }
            });
        }

        public override void VisitDoesNotEqual (string property, string value)
        {
            //VisitPropertyExpression<object> (property, val => consumer (!val.Equals (value)));
        }

        public override void VisitContains (string property, string value)
        {
            //VisitPropertyExpression<string> (property, val => consumer (val.Contains (value)));
        }

        void Visit (Query query, Action<bool> consumer)
        {
            query (new ObjectQueryVisitor (context, @object, consumer));
        }
    }
}

