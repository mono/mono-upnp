// 
// ObjectQueryTests.cs
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

using NUnit.Framework;

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.Tests
{
    [TestFixture]
    public class ObjectQueryTests
    {
        class Data : DummyObject
        {
            [XmlElement] public string Foo { get; set; }
        }

        [Test]
        public void Equality ()
        {
            var data = new Data { Foo = "bar" };
            var match = false;
            var visitor = new ObjectQueryVisitor (new ObjectQueryContext (typeof (Data)), data, result => match = result);
            visitor.VisitEquals ("Foo", "bar");
            Assert.IsTrue (match);
            match = false;
            visitor.VisitEquals ("Foo", "bat");
            Assert.IsFalse (match);
        }
    }
}

