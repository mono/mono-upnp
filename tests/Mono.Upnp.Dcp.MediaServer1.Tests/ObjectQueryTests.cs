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
using System.Collections.Generic;

using NUnit.Framework;

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.Tests
{
    [TestFixture]
    public class ObjectQueryTests : QueryTests
    {
        static readonly Property text = new Property ("Text");
        static readonly Property number = new Property ("Number");
        static readonly Property nullable_number = new Property ("NullableNumber");
        static readonly Property text_attribute = new Property ("@Text");
        static readonly Property number_attribute = new Property ("@Number");
        static readonly Property nullable_number_attribute = new Property ("@NullableNumber");
        static readonly Property nested_text_attribute = new Property ("Data@Text");
        static readonly Property nested_number_attribute = new Property ("Data@Number");
        static readonly Property nested_nullable_number_attribute = new Property ("Data@NullableNumber");

        class Data : DummyObject
        {
            [XmlElement] public string Text { get; set; }
            [XmlElement] public int Number { get; set; }
            [XmlElement] public int? NullableNumber { get; set; }
        }

        Data FullData {
            get { return new Data { Text = "bar", Number = 42, NullableNumber = 13 }; }
        }

        [Test]
        public void Equality ()
        {
            Assert.IsTrue (Matches (FullData, text == "bar"));
            Assert.IsFalse (Matches (FullData, text == null));
            Assert.IsFalse (Matches (FullData, text == "bat"));
            Assert.IsTrue (Matches (new Data (), text == null));
            Assert.IsTrue (Matches (FullData, number == "42"));
            Assert.IsFalse (Matches (FullData, number == "13"));
            Assert.IsTrue (Matches (FullData, nullable_number == "13"));
            Assert.IsTrue (Matches (new Data (), nullable_number == null));
            Assert.IsFalse (Matches (FullData, nullable_number == null));
        }

        [Test]
        public void Inequality ()
        {
            Assert.IsTrue (Matches (FullData, text != "bat"));
            Assert.IsTrue (Matches (FullData, text != null));
            Assert.IsFalse (Matches (FullData, text != "bar"));
            Assert.IsFalse (Matches (new Data (), text != null));
            Assert.IsTrue (Matches (FullData, number != "13"));
            Assert.IsFalse (Matches (FullData, number != "42"));
            Assert.IsFalse (Matches (new Data (), nullable_number != null));
            Assert.IsTrue (Matches (FullData, nullable_number != null));
        }

        [Test]
        public void LessThan ()
        {
            Assert.IsTrue (Matches (FullData, number < "43"));
            Assert.IsFalse (Matches (FullData, number < "42"));
            Assert.IsTrue (Matches (FullData, nullable_number < "14"));
            Assert.IsFalse (Matches (FullData, nullable_number < "13"));
        }

        [Test]
        public void LessThanOrEqualTo ()
        {
            Assert.IsTrue (Matches (FullData, number <= "43"));
            Assert.IsTrue (Matches (FullData, number <= "42"));
            Assert.IsFalse (Matches (FullData, number <= "41"));
            Assert.IsTrue (Matches (FullData, nullable_number <= "14"));
            Assert.IsTrue (Matches (FullData, nullable_number <= "13"));
            Assert.IsFalse (Matches (FullData, nullable_number <= "12"));
        }

        [Test]
        public void GreaterThan ()
        {
            Assert.IsTrue (Matches (FullData, number > "41"));
            Assert.IsFalse (Matches (FullData, number > "42"));
            Assert.IsTrue (Matches (FullData, nullable_number > "12"));
            Assert.IsFalse (Matches (FullData, nullable_number > "13"));
        }

        [Test]
        public void GreaterThanOrEqualTo ()
        {
            Assert.IsTrue (Matches (FullData, number >= "41"));
            Assert.IsTrue (Matches (FullData, number >= "42"));
            Assert.IsFalse (Matches (FullData, number >= "43"));
            Assert.IsTrue (Matches (FullData, nullable_number >= "12"));
            Assert.IsTrue (Matches (FullData, nullable_number >= "13"));
            Assert.IsFalse (Matches (FullData, nullable_number >= "14"));
        }

        [Test]
        public void Contains ()
        {
            Assert.IsTrue (Matches (FullData, text.Contains ("bar")));
            Assert.IsTrue (Matches (FullData, text.Contains ("ba")));
            Assert.IsTrue (Matches (FullData, text.Contains ("ar")));
            Assert.IsFalse (Matches (FullData, text.Contains ("bart")));
            Assert.IsFalse (Matches (FullData, text.Contains ("bb")));
            Assert.IsFalse (Matches (new Data (), text.Contains ("bb")));
        }

        [Test]
        public void DoesNotContain ()
        {
            Assert.IsFalse (Matches (FullData, text.DoesNotContain ("bar")));
            Assert.IsFalse (Matches (FullData, text.DoesNotContain ("ba")));
            Assert.IsFalse (Matches (FullData, text.DoesNotContain ("ar")));
            Assert.IsTrue (Matches (FullData, text.DoesNotContain ("bart")));
            Assert.IsTrue (Matches (FullData, text.DoesNotContain ("bb")));
            Assert.IsTrue (Matches (new Data (), text.DoesNotContain ("bb")));
        }

        [Test]
        public void Conjoin ()
        {
            Assert.IsTrue (Matches (FullData, Conjoin (text == "bar", number == "42")));
            Assert.IsTrue (Matches (
                FullData, Conjoin (Conjoin (text == "bar", number == "42"), nullable_number <= "13")));
            Assert.IsFalse (Matches (FullData, Conjoin (text == "bar", number < "42")));
            Assert.IsFalse (Matches (FullData, Conjoin (text == "bart", number == "42")));
        }

        [Test]
        public void Disjoin ()
        {
            Assert.IsTrue (Matches (FullData, Disjoin (text == "bar", number == "42")));
            Assert.IsTrue (Matches (FullData, Disjoin (text == "bar", number < "42")));
            Assert.IsTrue (Matches (FullData, Disjoin (text == "bart", number == "42")));
            Assert.IsTrue (Matches (
                FullData, Disjoin (Disjoin (text == "bart", number == "42"), nullable_number == "13")));
            Assert.IsTrue (Matches (
                FullData, Disjoin (Disjoin (text == "bart", number != "42"), nullable_number == "13")));
            Assert.IsFalse (Matches (FullData, Disjoin (text == "bart", number < "42")));
        }

        class OmitIfNullData : DummyObject
        {
            [XmlElement (OmitIfNull = true)] public string Text { get; set; }
            [XmlElement (OmitIfNull = true)] public int? Number { get; set; }
        }

        [Test]
        public void Exists ()
        {
            var data = new OmitIfNullData { Text = "foo", Number = 42 };
            Assert.IsTrue (Matches (data, text.Exists (true)));
            Assert.IsFalse (Matches (data, text.Exists (false)));
            Assert.IsTrue (Matches (data, number.Exists (true)));
            Assert.IsFalse (Matches (data, number.Exists (false)));
            Assert.IsFalse (Matches (new OmitIfNullData (), text.Exists (true)));
            Assert.IsTrue (Matches (new OmitIfNullData (), text.Exists (false)));
            Assert.IsFalse (Matches (new OmitIfNullData (), number.Exists (true)));
            Assert.IsTrue (Matches (new OmitIfNullData (), number.Exists (false)));
            Assert.IsFalse (Matches (data, text_attribute.Exists (true)));
            Assert.IsTrue (Matches (data, text_attribute.Exists (false)));
        }

        class ArrayItemData : DummyObject
        {
            [XmlArrayItem] public IEnumerable<string> Text { get; set; }
            [XmlArrayItem] public IEnumerable<int> Number { get; set; }
        }

        [Test]
        public void ArrayItems ()
        {
            var data = new ArrayItemData {
                Text = new[] { "one", "two", "three" },
                Number = new[] { 1, 2, 3 }
            };
            Assert.IsTrue (Matches (data, text == "two"));
            Assert.IsTrue (Matches (data, text.Contains ("ee")));
            Assert.IsTrue (Matches (data, text.Contains ("t")));
            Assert.IsTrue (Matches (data, text.Exists (true)));
            Assert.IsFalse (Matches (data, text.Exists (false)));
            Assert.IsTrue (Matches (new ArrayItemData (), text.Exists (false)));
            Assert.IsFalse (Matches (new ArrayItemData (), text.Exists (true)));
            Assert.IsTrue (Matches (data, number < "4"));
            Assert.IsTrue (Matches (data, number < "3"));
            Assert.IsTrue (Matches (data, number < "2"));
            Assert.IsFalse (Matches (data, number < "1"));
        }

        class ListArrayItemData : DummyObject
        {
            [XmlArrayItem] public List<int> Number { get; set; }
        }

        [Test]
        public void ListArrayItems ()
        {
            var data = new ListArrayItemData { Number = new List<int> { 1, 2, 3 } };
            Assert.IsTrue (Matches (data, number <= "4"));
            Assert.IsTrue (Matches (data, number <= "3"));
            Assert.IsTrue (Matches (data, number <= "2"));
            Assert.IsTrue (Matches (data, number <= "1"));
            Assert.IsFalse (Matches (data, number <= "0"));
        }

        class AttributeData : DummyObject
        {
            [XmlAttribute] public string Text { get; set; }
            [XmlAttribute] public int Number { get; set; }
            [XmlAttribute] public int? NullableNumber { get; set; }
        }

        [Test]
        public void Attributes ()
        {
            var data = new AttributeData { Text = "foo", Number = 42, NullableNumber = 13 };
            Assert.IsTrue (Matches (data, text_attribute == "foo"));
            Assert.IsTrue (Matches (data, number_attribute == "42"));
            Assert.IsTrue (Matches (data, nullable_number_attribute == "13"));
            Assert.IsFalse (Matches (data, text_attribute.Contains ("bar")));
            Assert.IsFalse (Matches (data, number_attribute < "42"));
            Assert.IsFalse (Matches (data, nullable_number_attribute >= "14"));
        }

        class OmitIfNullAttributeData : DummyObject
        {
            [XmlAttribute (OmitIfNull = true)] public string Text { get; set; }
            [XmlAttribute] public int Number { get; set; }
            [XmlAttribute (OmitIfNull = true)] public int? NullableNumber { get; set; }
        }

        [Test]
        public void AttributeExists ()
        {
            var data = new OmitIfNullAttributeData { Text = "foo", Number = 42, NullableNumber = 13 };
            Assert.IsTrue (Matches (data, text_attribute.Exists (true)));
            Assert.IsFalse (Matches (data, text_attribute.Exists (false)));
            Assert.IsTrue (Matches (data, number_attribute.Exists (true)));
            Assert.IsFalse (Matches (data, number_attribute.Exists (false)));
            Assert.IsTrue (Matches (data, nullable_number_attribute.Exists (true)));
            Assert.IsFalse (Matches (data, nullable_number_attribute.Exists (false)));
            data = new OmitIfNullAttributeData ();
            Assert.IsFalse (Matches (data, text_attribute.Exists (true)));
            Assert.IsTrue (Matches (data, text_attribute.Exists (false)));
            Assert.IsTrue (Matches (data, number_attribute.Exists (true)));
            Assert.IsFalse (Matches (data, number_attribute.Exists (false)));
            Assert.IsFalse (Matches (data, nullable_number_attribute.Exists (true)));
            Assert.IsTrue (Matches (data, nullable_number_attribute.Exists (false)));
            Assert.IsFalse (Matches (data, text.Exists (true)));
            Assert.IsTrue (Matches (data, text.Exists (false)));
        }

        class NestedAttributeData : DummyObject
        {
            [XmlElement] public AttributeData Data { get; set; }
        }

        [Test]
        public void NestedAttributes ()
        {
            var data = new NestedAttributeData {
                Data = new AttributeData { Text = "foo", Number = 42, NullableNumber = 13 }
            };
            Assert.IsTrue (Matches (data, nested_text_attribute == "foo"));
            Assert.IsTrue (Matches (data, nested_number_attribute == "42"));
            Assert.IsTrue (Matches (data, nested_nullable_number_attribute == "13"));
            Assert.IsFalse (Matches (data, nested_text_attribute.Exists (false)));
            Assert.IsFalse (Matches (data, nested_number_attribute > "42"));
            Assert.IsFalse (Matches (data, nested_nullable_number_attribute != "13"));
        }

        static bool Matches (Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object @object, Query query)
        {
            var match = false;
            query (new ObjectQueryVisitor (new ObjectQueryContext (@object.GetType ()), @object, result => {
                if (match) {
                    Assert.Fail ("Multiple matches for a single input.");
                }
                match = true;
            }));
            return match;
        }
    }
}

