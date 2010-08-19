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
        Action<Object> consumer;

        public ObjectQueryVisitor (ObjectQueryContext context, Object @object, Action<Object> consumer)
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

        void Yield ()
        {
            if (consumer != null) {
                consumer (@object);
                consumer = null;
            }
        }

        public override void VisitAllResults ()
        {
            Yield ();
        }

        public override void VisitAnd (Action<QueryVisitor> leftOperand, Action<QueryVisitor> rightOperand)
        {
            leftOperand (new ObjectQueryVisitor (context, @object, result => rightOperand (this)));
        }

        public override void VisitOr (Action<QueryVisitor> leftOperand, Action<QueryVisitor> rightOperand)
        {
            Object leftResult = null;
            leftOperand (new ObjectQueryVisitor (context, @object, result => leftResult = result));
            if (leftResult == null) {
                rightOperand (this);
            } else {
                Yield ();
            }
        }

        public override void VisitExists (string property, bool value)
        {
            if (context.PropertyExists (property, @object) == value) {
                Yield ();
            }
        }

        public override void VisitEquals (string property, string value)
        {
            context.VisitProperty (property, @object, val => {
                if (val == null) {
                    if (value == null) {
                        Yield ();
                    }
                } else if (val.ToString ().Equals (value)) {
                    Yield ();
                }
            });
        }

        public override void VisitDoesNotEqual (string property, string value)
        {
            context.VisitProperty (property, @object, val => {
                if (val == null) {
                    if (value != null) {
                        Yield ();
                    }
                } else if (!val.ToString ().Equals (value)) {
                    Yield ();
                }
            });
        }

        public override void VisitLessThan (string property, string value)
        {
            context.CompareProperty (property, @object, value, val => {
                if (val < 0) {
                    Yield ();
                }
            });
        }

        public override void VisitLessThanOrEqualTo (string property, string value)
        {
            context.CompareProperty (property, @object, value, val => {
                if (val <= 0) {
                    Yield ();
                }
            });
        }

        public override void VisitGreaterThan (string property, string value)
        {
            context.CompareProperty (property, @object, value, val => {
                if (val > 0) {
                    Yield ();
                }
            });
        }

        public override void VisitGreaterThanOrEqualTo (string property, string value)
        {
            context.CompareProperty (property, @object, value, val => {
                if (val >= 0) {
                    Yield ();
                }
            });
        }

        public override void VisitContains (string property, string value)
        {
            context.VisitProperty (property, @object, val => {
                if (val != null && val.ToString ().Contains (value)) {
                    Yield ();
                }
            });
        }

        public override void VisitDoesNotContain (string property, string value)
        {
            context.VisitProperty (property, @object, val => {
                if (val == null || !val.ToString ().Contains (value)) {
                    Yield ();
                }
            });
        }
    }
}
