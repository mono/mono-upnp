// 
// FilteringXmlSerializerTests.cs
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

using NUnit.Framework;

using Mono.Upnp.Dcp.MediaServer1.Xml;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.Tests
{
    [TestFixture]
    public class FilteringXmlSerializerTests
    {
        readonly XmlSerializer<FilteringContext> xml_serializer =
            new XmlSerializer<FilteringContext> (
                (serializer, type) => new FilteringDelegateSerializationCompiler (serializer, type));


        const string bar = "www.bar.org";       const string foo = "www.foo.org";
        const string bat = "www.bat.org";

        [XmlType ("data", foo)]
        [XmlNamespace (bat, "bat")]
        [XmlNamespace (bar, "bar")]
        class Data
        {
            [XmlElement ("manditoryFooElement", foo)] public string ManditoryFooElement { get; set; }
            [XmlElement ("manditoryBarElement", bar)] public string ManditoryBarElement { get; set; }
            [XmlElement ("manditoryBatElement", bat)] public string ManditoryBatElement { get; set; }
            [XmlAttribute ("manditoryAttribute")] public string ManditoryAttribute { get; set; }
            [XmlAttribute ("manditoryBarAttribute", bar)] public string ManditoryBarAttribute { get; set; }
            [XmlAttribute ("manditoryBatAttribute", bat)] public string ManditoryBatAttribute { get; set; }
            [XmlElement ("optionalFooElement", foo, OmitIfNull = true)] public string OptionalFooElement { get; set; }
            [XmlElement ("optionalBarElement", bar, OmitIfNull = true)] public string OptionalBarElement { get; set; }
            [XmlElement ("optionalBatElement", bat, OmitIfNull = true)] public string OptionalBatElement { get; set; }
            [XmlAttribute ("optionalAttribute", OmitIfNull = true)] public string OptionalAttribute { get; set; }
            [XmlAttribute ("optionalBarAttribute", bar, OmitIfNull = true)] public string OptionalBarAttribute { get; set; }
            [XmlAttribute ("optionalBatAttribute", bat, OmitIfNull = true)] public string OptionalBatAttribute { get; set; }
        }

        static XmlSerializationOptions<FilteringContext> Options<T> (params string[] filters)
        {
            return new XmlSerializationOptions<FilteringContext> {
                XmlDeclarationType = XmlDeclarationType.None,
                Context = new FilteringContext (typeof (T), filters)
            };
        }

        [Test]
        public void NoFilterWithNoData ()
        {
            Assert.AreEqual (
                @"<data xmlns:bar=""www.bar.org"" xmlns:bat=""www.bat.org"" " +
                @"manditoryAttribute="""" " +
                @"bar:manditoryBarAttribute="""" " +
                @"bat:manditoryBatAttribute="""" xmlns=""www.foo.org"">" +
                    "<manditoryFooElement />" +
                    "<bar:manditoryBarElement />" +
                    "<bat:manditoryBatElement />" +
                "</data>",
                xml_serializer.GetString (new Data (), Options<Data> ()));
        }

        static Data FullData {
            get {
                return new Data {
                    ManditoryFooElement = "Manditory Foo Element",
                    ManditoryBarElement = "Manditory Bar Element",
                    ManditoryBatElement = "Manditory Bat Element",
                    ManditoryAttribute = "Manditory Attribute",
                    ManditoryBarAttribute = "Manditory Bar Attribute",
                    ManditoryBatAttribute = "Manditory Bat Attribute",
                    OptionalFooElement = "Optional Foo Element",
                    OptionalBarElement = "Optional Bar Element",
                    OptionalBatElement = "Optional Bat Element",
                    OptionalAttribute = "Optional Attribute",
                    OptionalBarAttribute = "Optional Bar Attribute",
                    OptionalBatAttribute = "Optional Bat Attribute"
                };
            }
        }

        [Test]
        public void NoFilterWithAllData ()
        {
            Assert.AreEqual (
                @"<data xmlns:bar=""www.bar.org"" xmlns:bat=""www.bat.org"" " +
                @"manditoryAttribute=""Manditory Attribute"" " +
                @"bar:manditoryBarAttribute=""Manditory Bar Attribute"" " +
                @"bat:manditoryBatAttribute=""Manditory Bat Attribute"" xmlns=""www.foo.org"">" +
                    "<manditoryFooElement>Manditory Foo Element</manditoryFooElement>" +
                    "<bar:manditoryBarElement>Manditory Bar Element</bar:manditoryBarElement>" +
                    "<bat:manditoryBatElement>Manditory Bat Element</bat:manditoryBatElement>" +
                "</data>",
                xml_serializer.GetString (FullData, Options<Data> ()));
        }

        [Test]
        public void WildCardWithNoData ()
        {
            Assert.AreEqual (
                @"<data xmlns:bar=""www.bar.org"" xmlns:bat=""www.bat.org"" " +
                @"manditoryAttribute="""" " +
                @"bar:manditoryBarAttribute="""" " +
                @"bat:manditoryBatAttribute="""" xmlns=""www.foo.org"">" +
                    "<manditoryFooElement />" +
                    "<bar:manditoryBarElement />" +
                    "<bat:manditoryBatElement />" +
                "</data>",
                xml_serializer.GetString (new Data (), Options<Data> ("*")));
        }

        [Test]
        public void WildCardWithAllData ()
        {
            Assert.AreEqual (
                @"<data xmlns:bar=""www.bar.org"" xmlns:bat=""www.bat.org"" " +
                @"manditoryAttribute=""Manditory Attribute"" " +
                @"bar:manditoryBarAttribute=""Manditory Bar Attribute"" " +
                @"bat:manditoryBatAttribute=""Manditory Bat Attribute"" " +
                @"optionalAttribute=""Optional Attribute"" " +
                @"bar:optionalBarAttribute=""Optional Bar Attribute"" " +
                @"bat:optionalBatAttribute=""Optional Bat Attribute"" xmlns=""www.foo.org"">" +
                    "<manditoryFooElement>Manditory Foo Element</manditoryFooElement>" +
                    "<bar:manditoryBarElement>Manditory Bar Element</bar:manditoryBarElement>" +
                    "<bat:manditoryBatElement>Manditory Bat Element</bat:manditoryBatElement>" +
                    "<optionalFooElement>Optional Foo Element</optionalFooElement>" +
                    "<bar:optionalBarElement>Optional Bar Element</bar:optionalBarElement>" +
                    "<bat:optionalBatElement>Optional Bat Element</bat:optionalBatElement>" +
                "</data>",
                xml_serializer.GetString (FullData, Options<Data> ("*")));
        }

        [Test]
        public void UnprefixedFilterWithAllData ()
        {
            Assert.AreEqual (
                @"<data xmlns:bar=""www.bar.org"" xmlns:bat=""www.bat.org"" " +
                @"manditoryAttribute=""Manditory Attribute"" " +
                @"bar:manditoryBarAttribute=""Manditory Bar Attribute"" " +
                @"bat:manditoryBatAttribute=""Manditory Bat Attribute"" xmlns=""www.foo.org"">" +
                    "<manditoryFooElement>Manditory Foo Element</manditoryFooElement>" +
                    "<bar:manditoryBarElement>Manditory Bar Element</bar:manditoryBarElement>" +
                    "<bat:manditoryBatElement>Manditory Bat Element</bat:manditoryBatElement>" +
                    "<optionalFooElement>Optional Foo Element</optionalFooElement>" +
                "</data>",
                xml_serializer.GetString (FullData, Options<Data> ("optionalFooElement")));
        }

        [Test]
        public void PrefixedFilterWithAllData ()
        {
            Assert.AreEqual (
                @"<data xmlns:bar=""www.bar.org"" xmlns:bat=""www.bat.org"" " +
                @"manditoryAttribute=""Manditory Attribute"" " +
                @"bar:manditoryBarAttribute=""Manditory Bar Attribute"" " +
                @"bat:manditoryBatAttribute=""Manditory Bat Attribute"" xmlns=""www.foo.org"">" +
                    "<manditoryFooElement>Manditory Foo Element</manditoryFooElement>" +
                    "<bar:manditoryBarElement>Manditory Bar Element</bar:manditoryBarElement>" +
                    "<bat:manditoryBatElement>Manditory Bat Element</bat:manditoryBatElement>" +
                    "<bar:optionalBarElement>Optional Bar Element</bar:optionalBarElement>" +
                "</data>",
                xml_serializer.GetString (FullData, Options<Data> ("bar:optionalBarElement")));
        }

        [Test]
        public void TwoPrefixedFiltersWithAllData ()
        {
            Assert.AreEqual (
                @"<data xmlns:bar=""www.bar.org"" xmlns:bat=""www.bat.org"" " +
                @"manditoryAttribute=""Manditory Attribute"" " +
                @"bar:manditoryBarAttribute=""Manditory Bar Attribute"" " +
                @"bat:manditoryBatAttribute=""Manditory Bat Attribute"" xmlns=""www.foo.org"">" +
                    "<manditoryFooElement>Manditory Foo Element</manditoryFooElement>" +
                    "<bar:manditoryBarElement>Manditory Bar Element</bar:manditoryBarElement>" +
                    "<bat:manditoryBatElement>Manditory Bat Element</bat:manditoryBatElement>" +
                    "<bar:optionalBarElement>Optional Bar Element</bar:optionalBarElement>" +
                    "<bat:optionalBatElement>Optional Bat Element</bat:optionalBatElement>" +
                "</data>",
                xml_serializer.GetString (FullData, Options<Data> ("bar:optionalBarElement", "bat:optionalBatElement")));
        }

        [Test]
        public void UnprefixedAndPrefixedFiltersWithAllData ()
        {
            Assert.AreEqual (
                @"<data xmlns:bar=""www.bar.org"" xmlns:bat=""www.bat.org"" " +
                @"manditoryAttribute=""Manditory Attribute"" " +
                @"bar:manditoryBarAttribute=""Manditory Bar Attribute"" " +
                @"bat:manditoryBatAttribute=""Manditory Bat Attribute"" xmlns=""www.foo.org"">" +
                    "<manditoryFooElement>Manditory Foo Element</manditoryFooElement>" +
                    "<bar:manditoryBarElement>Manditory Bar Element</bar:manditoryBarElement>" +
                    "<bat:manditoryBatElement>Manditory Bat Element</bat:manditoryBatElement>" +
                    "<optionalFooElement>Optional Foo Element</optionalFooElement>" +
                    "<bar:optionalBarElement>Optional Bar Element</bar:optionalBarElement>" +
                "</data>",
                xml_serializer.GetString (FullData, Options<Data> ("optionalFooElement", "bar:optionalBarElement")));
        }

        [Test]
        public void FilterAndWildCardWithAllData ()
        {
            Assert.AreEqual (
                @"<data xmlns:bar=""www.bar.org"" xmlns:bat=""www.bat.org"" " +
                @"manditoryAttribute=""Manditory Attribute"" " +
                @"bar:manditoryBarAttribute=""Manditory Bar Attribute"" " +
                @"bat:manditoryBatAttribute=""Manditory Bat Attribute"" " +
                @"optionalAttribute=""Optional Attribute"" " +
                @"bar:optionalBarAttribute=""Optional Bar Attribute"" " +
                @"bat:optionalBatAttribute=""Optional Bat Attribute"" xmlns=""www.foo.org"">" +
                    "<manditoryFooElement>Manditory Foo Element</manditoryFooElement>" +
                    "<bar:manditoryBarElement>Manditory Bar Element</bar:manditoryBarElement>" +
                    "<bat:manditoryBatElement>Manditory Bat Element</bat:manditoryBatElement>" +
                    "<optionalFooElement>Optional Foo Element</optionalFooElement>" +
                    "<bar:optionalBarElement>Optional Bar Element</bar:optionalBarElement>" +
                    "<bat:optionalBatElement>Optional Bat Element</bat:optionalBatElement>" +
                "</data>",
                xml_serializer.GetString (FullData, Options<Data> ("optionalFooElement", "*")));
        }

        [Test]
        public void IncludeOptionalAttribute ()
        {
            Assert.AreEqual (
                @"<data xmlns:bar=""www.bar.org"" xmlns:bat=""www.bat.org"" " +
                @"manditoryAttribute=""Manditory Attribute"" " +
                @"bar:manditoryBarAttribute=""Manditory Bar Attribute"" " +
                @"bat:manditoryBatAttribute=""Manditory Bat Attribute"" " +
                @"optionalAttribute=""Optional Attribute"" xmlns=""www.foo.org"">" +
                    "<manditoryFooElement>Manditory Foo Element</manditoryFooElement>" +
                    "<bar:manditoryBarElement>Manditory Bar Element</bar:manditoryBarElement>" +
                    "<bat:manditoryBatElement>Manditory Bat Element</bat:manditoryBatElement>" +
                "</data>",
                xml_serializer.GetString (FullData, Options<Data> ("@optionalAttribute")));
        }

        [Test]
        public void IncludeOptionalNamespacedAttribute ()
        {
            Assert.AreEqual (
                @"<data xmlns:bar=""www.bar.org"" xmlns:bat=""www.bat.org"" " +
                @"manditoryAttribute=""Manditory Attribute"" " +
                @"bar:manditoryBarAttribute=""Manditory Bar Attribute"" " +
                @"bat:manditoryBatAttribute=""Manditory Bat Attribute"" " +
                @"bar:optionalBarAttribute=""Optional Bar Attribute"" xmlns=""www.foo.org"">" +
                    "<manditoryFooElement>Manditory Foo Element</manditoryFooElement>" +
                    "<bar:manditoryBarElement>Manditory Bar Element</bar:manditoryBarElement>" +
                    "<bat:manditoryBatElement>Manditory Bat Element</bat:manditoryBatElement>" +
                "</data>",
                xml_serializer.GetString (FullData, Options<Data> ("@bar:optionalBarAttribute")));
        }

        [XmlType ("nestedData", foo)]
        [XmlNamespace (bar, "bar")]
        class NestedData
        {
            [XmlAttribute ("attribute", OmitIfNull = true)] public string Attribute { get; set; }
            [XmlAttribute ("barAttribute", bar, OmitIfNull = true)] public string BarAttribute { get; set; }
            [XmlElement ("nested", OmitIfNull = true)] public NestedData Nested { get; set; }
            [XmlElement ("barNested", bar, OmitIfNull = true)] public NestedData BarNested { get; set; }
        }

        static NestedData FullNestedData {
            get {
                return new NestedData {
                    Attribute = "Attribute",
                    BarAttribute = "Bar Attribute",
                    Nested = new NestedData {
                        Attribute = "Nested Attribute",
                        BarAttribute = "Nested Bar Attribute"
                    },
                    BarNested = new NestedData {
                        Attribute = "Bar Nested Attribute",
                        BarAttribute = "Bar Nested Bar Attribute"
                    }
                };
            }
        }

        [Test]
        public void NestedDataWithAnAttribute ()
        {
            Assert.AreEqual (
                @"<nestedData xmlns:bar=""www.bar.org"" " +
                @"attribute=""Attribute"" xmlns=""www.foo.org"" />",
                xml_serializer.GetString (FullNestedData, Options<NestedData> ("@attribute")));
        }

        [Test]
        public void NestedDataWithAttributeAndNamespacedAttribute ()
        {
            Assert.AreEqual (
                @"<nestedData xmlns:bar=""www.bar.org"" " +
                @"attribute=""Attribute"" " +
                @"bar:barAttribute=""Bar Attribute"" xmlns=""www.foo.org"" />",
                xml_serializer.GetString (FullNestedData, Options<NestedData> ("@attribute", "@bar:barAttribute")));
        }

        [Test]
        public void NestedDataWithNoAttributes ()
        {
            Assert.AreEqual (
                @"<nestedData xmlns:bar=""www.bar.org"" xmlns=""www.foo.org""><nested /></nestedData>",
                xml_serializer.GetString (FullNestedData, Options<NestedData> ("nested")));
        }

        [Test]
        public void NamespacedNestedDataWithNoAttributes ()
        {
            Assert.AreEqual (
                @"<nestedData xmlns:bar=""www.bar.org"" xmlns=""www.foo.org""><bar:barNested /></nestedData>",
                xml_serializer.GetString (FullNestedData, Options<NestedData> ("bar:barNested")));
        }

        [Test]
        public void NestedDataWithAttribute ()
        {
            Assert.AreEqual (
                @"<nestedData xmlns:bar=""www.bar.org"" xmlns=""www.foo.org"">" +
                    @"<nested attribute=""Nested Attribute"" />"+
                "</nestedData>",
                xml_serializer.GetString (FullNestedData, Options<NestedData> ("nested", "nested@attribute")));
        }

        [Test]
        public void NestedDataWithNamespacedAttribute ()
        {
            Assert.AreEqual (
                @"<nestedData xmlns:bar=""www.bar.org"" xmlns=""www.foo.org"">" +
                    @"<nested bar:barAttribute=""Nested Bar Attribute"" />"+
                "</nestedData>",
                xml_serializer.GetString (FullNestedData, Options<NestedData> ("nested", "nested@bar:barAttribute")));
        }

        [Test]
        public void NamespacedNestedDataWithAttribute ()
        {
            Assert.AreEqual (
                @"<nestedData xmlns:bar=""www.bar.org"" xmlns=""www.foo.org"">" +
                    @"<bar:barNested attribute=""Bar Nested Attribute"" />"+
                "</nestedData>",
                xml_serializer.GetString (FullNestedData, Options<NestedData> ("bar:barNested", "bar:barNested@attribute")));
        }

        [Test]
        public void NamespacedNestedDataWithNamespacedAttribute ()
        {
            Assert.AreEqual (
                @"<nestedData xmlns:bar=""www.bar.org"" xmlns=""www.foo.org"">" +
                    @"<bar:barNested bar:barAttribute=""Bar Nested Bar Attribute"" />"+
                "</nestedData>",
                xml_serializer.GetString (FullNestedData, Options<NestedData> ("bar:barNested", "bar:barNested@bar:barAttribute")));
        }

        [Test]
        public void NestedAttributeButNoNestedData ()
        {
            Assert.AreEqual (
                @"<nestedData xmlns:bar=""www.bar.org"" xmlns=""www.foo.org"" />",
                xml_serializer.GetString (FullNestedData, Options<NestedData> ("nested@attribute")));
        }

        [Test]
        public void EmptyNestedDataAndAnAttribute ()
        {
            Assert.AreEqual (
                @"<nestedData xmlns:bar=""www.bar.org"" " +
                @"attribute=""Attribute"" xmlns=""www.foo.org"">" +
                    "<nested />" +
                "</nestedData>",
                xml_serializer.GetString (FullNestedData, Options<NestedData> ("@attribute", "nested")));
        }

        [XmlType ("container", foo)]
        [XmlNamespace (bar, "bar")]
        class Container<T>
        {
            [XmlElement ("content")] public T Content { get; set; }
        }

        [Test]
        public void ContainedNestedData ()
        {
            Assert.AreEqual (
                @"<container xmlns:bar=""www.bar.org"" xmlns=""www.foo.org"">" +
                    @"<content attribute=""Attribute"">" +
                        "<nested />" +
                    "</content>" +
                "</container>",
                xml_serializer.GetString (
                    new Container<NestedData> { Content = FullNestedData },
                    Options<NestedData> ("@attribute", "nested")));
        }

        [Test]
        public void ContainedWildCardNestedData ()
        {
            Assert.AreEqual (
                @"<container xmlns:bar=""www.bar.org"" xmlns=""www.foo.org"">" +
                    @"<content attribute=""Attribute"" bar:barAttribute=""Bar Attribute"">" +
                        @"<nested attribute=""Nested Attribute"" bar:barAttribute=""Nested Bar Attribute"" />" +
                        @"<bar:barNested attribute=""Bar Nested Attribute"" bar:barAttribute=""Bar Nested Bar Attribute"" />" +
                    "</content>" +
                "</container>",
                xml_serializer.GetString (
                    new Container<NestedData> { Content = FullNestedData },
                    Options<NestedData> ("*")));
        }
    }
}

