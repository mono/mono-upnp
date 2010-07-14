// 
// ObjectQueryContextTests.cs
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
    public class ObjectQueryContextTests
    {
        const string bar = "www.bar.org";

        class ElementTestClass
        {
            [XmlElement] public string Foo { get; set; }
        }

        [Test]
        public void Element ()
        {
            var context = new ObjectQueryContext (typeof (ElementTestClass));
            Assert.IsTrue (context.PropertyExists ("Foo", new ElementTestClass ()));
            var foo = new ElementTestClass { Foo = "bar" };
            Assert.IsTrue (context.PropertyExists ("Foo", foo));
            Assert.IsFalse (context.PropertyExists ("Bar", foo));
            var equality = false;
            context.VisitProperty<string> ("Foo", foo, value => equality = value == "bar");
            Assert.IsTrue (equality);
            equality = false;
            context.VisitProperty<string> ("Foo", new ElementTestClass (), value => equality = value == null);
            Assert.IsTrue (equality);
        }

        class OmitIfNullElementTestClass
        {
            [XmlElement (OmitIfNull = true)] public string Foo { get; set; }
        }

        [Test]
        public void OmitIfNullElement ()
        {
            var context = new ObjectQueryContext (typeof (OmitIfNullElementTestClass));
            Assert.IsFalse (context.PropertyExists ("Foo", new OmitIfNullElementTestClass ()));
            Assert.IsTrue (context.PropertyExists ("Foo", new OmitIfNullElementTestClass { Foo = "bar" }));
        }

        class NamedElementTestClass
        {
            [XmlElement ("foo")] public string Foo { get; set; }
        }

        [Test]
        public void NamedElement ()
        {
            var context = new ObjectQueryContext (typeof (NamedElementTestClass));
            Assert.IsTrue (context.PropertyExists ("foo", new NamedElementTestClass ()));
        }

        class PrefixedElementTestClass
        {
            [XmlElement ("foo", bar, "bar")] public string Foo { get; set; }
        }

        [Test]
        public void PrefixedElement ()
        {
            var context = new ObjectQueryContext (typeof (PrefixedElementTestClass));
            Assert.IsTrue (context.PropertyExists ("bar:foo", new PrefixedElementTestClass ()));
        }

        class AttributeTestClass
        {
            [XmlAttribute] public string Foo { get; set; }
        }

        [Test]
        public void Attribute ()
        {
            var context = new ObjectQueryContext (typeof (AttributeTestClass));
            Assert.IsTrue (context.PropertyExists ("@Foo", new AttributeTestClass ()));
            var foo = new AttributeTestClass { Foo = "bar" };
            Assert.IsTrue (context.PropertyExists ("@Foo", foo));
            Assert.IsFalse (context.PropertyExists ("@Bar", foo));
            var equality = false;
            context.VisitProperty<string> ("@Foo", foo, value => equality = value == "bar");
            Assert.IsTrue (equality);
            equality = false;
            context.VisitProperty<string> ("@Foo", new AttributeTestClass (), value => equality = value == null);
            Assert.IsTrue (equality);
        }

        class OmitIfNullAttributeTestClass
        {
            [XmlAttribute (OmitIfNull = true)] public string Foo { get; set; }
        }

        [Test]
        public void OmitIfNullAttribute ()
        {
            var context = new ObjectQueryContext (typeof (OmitIfNullAttributeTestClass));
            Assert.IsFalse (context.PropertyExists ("@Foo", new OmitIfNullAttributeTestClass ()));
            Assert.IsTrue (context.PropertyExists ("@Foo", new OmitIfNullAttributeTestClass { Foo = "bar" }));
        }

        class NamedAttributeTestClass
        {
            [XmlAttribute ("foo")] public string Foo { get; set; }
        }

        [Test]
        public void NamedAttribute ()
        {
            var context = new ObjectQueryContext (typeof (NamedAttributeTestClass));
            Assert.IsTrue (context.PropertyExists ("@foo", new NamedAttributeTestClass ()));
        }

        class PrefixedAttributeTestClass
        {
            [XmlAttribute ("foo", bar, "bar")] public string Foo { get; set; }
        }

        [Test]
        public void PrefixedAttribute ()
        {
            var context = new ObjectQueryContext (typeof (PrefixedAttributeTestClass));
            Assert.IsTrue (context.PropertyExists ("@bar:foo", new PrefixedAttributeTestClass ()));
        }

        class NestedTestClass
        {
            [XmlElement] public AttributeTestClass Attribute { get; set; }
            [XmlElement] public OmitIfNullAttributeTestClass OmitIfNullAttribute { get; set; }
            [XmlElement] public NamedAttributeTestClass NamedAttribute { get; set; }
            [XmlElement] public PrefixedAttributeTestClass PrefixedAttribute { get; set; }
            [XmlElement ("prefixedNestedAttribute", bar, "bar")] public AttributeTestClass PrefixedNestedAttribute { get; set; }
            [XmlElement ("prefixedNestedPrefixedAttribute", bar, "bar")] public PrefixedAttributeTestClass PrefixedNestedPrefixedAttribute { get; set; }
        }

        [Test]
        public void NestedElements ()
        {
            var context = new ObjectQueryContext (typeof (NestedTestClass));
            var test = new NestedTestClass {
                Attribute = new AttributeTestClass (),
                OmitIfNullAttribute = new OmitIfNullAttributeTestClass (),
                NamedAttribute = new NamedAttributeTestClass (),
                PrefixedAttribute = new PrefixedAttributeTestClass (),
                PrefixedNestedAttribute = new AttributeTestClass (),
                PrefixedNestedPrefixedAttribute = new PrefixedAttributeTestClass ()
            };

            Assert.IsTrue (context.PropertyExists ("Attribute", test));
            Assert.IsTrue (context.PropertyExists ("Attribute@Foo", test));
            Assert.IsTrue (context.PropertyExists ("OmitIfNullAttribute", test));
            Assert.IsFalse (context.PropertyExists ("OmitIfNullAttribute@Foo", test));
            Assert.IsTrue (context.PropertyExists ("NamedAttribute", test));
            Assert.IsTrue (context.PropertyExists ("NamedAttribute@foo", test));
            Assert.IsTrue (context.PropertyExists ("PrefixedAttribute", test));
            Assert.IsTrue (context.PropertyExists ("PrefixedAttribute@bar:foo", test));
            Assert.IsTrue (context.PropertyExists ("bar:prefixedNestedAttribute", test));
            Assert.IsTrue (context.PropertyExists ("bar:prefixedNestedAttribute@Foo", test));
            Assert.IsTrue (context.PropertyExists ("bar:prefixedNestedPrefixedAttribute", test));
            Assert.IsTrue (context.PropertyExists ("bar:prefixedNestedPrefixedAttribute@bar:foo", test));

            test.OmitIfNullAttribute.Foo = "bar";

            Assert.IsTrue (context.PropertyExists ("OmitIfNullAttribute@Foo", test));
        }
    }
}

