// 
// VirtualXmlSerializerTests.cs
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

using Mono.Upnp.Dcp.MediaServer1.Xml;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.Tests
{
    [TestFixture]
    public class VirtualXmlSerializerTests
    {
        XmlSerializer<VirtualContext> xml_serializer = new XmlSerializer<VirtualContext> (
            (serializer, type) => new VirtualDelegateSerializationCompiler (serializer, type));

        [XmlType ("element")]
        class Element<T>
        {
            [XmlElement ("foo")] public T Foo { get; set; }
        }

        [Test]
        public void TestCase ()
        {
            var data = new Element<string> { Foo = "bar" };
            AssertAreEqual ("<element><foo>foo</foo></element>", data, new Override ("foo", "foo"));
        }

        void AssertAreEqual<T> (string xml, T obj, params Override[] overrides)
        {
            Assert.AreEqual (xml, xml_serializer.GetString (obj, new XmlSerializationOptions<VirtualContext> {
                Context = new VirtualContext (overrides),
                XmlDeclarationType = XmlDeclarationType.None
            }));
        }
    }
}
