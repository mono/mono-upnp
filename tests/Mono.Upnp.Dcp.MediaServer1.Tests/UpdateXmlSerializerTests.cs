// 
// UpdateXmlSerializerTests.cs
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
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

using Mono.Upnp.Xml;
using Mono.Upnp.Dcp.MediaServer1.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.Tests
{
    [TestFixture]
    public class UpdateXmlSerializerTests
    {
        readonly UpdateXmlSerializer serializer = new UpdateXmlSerializer ();
        
        class Data
        {
            [XmlElement] public string Foo { get; set; }
            [XmlElement] public int Bar { get; set; }
            [XmlElement] public bool Bat { get; set; }
        }
        
        [Test]
        public void NoUpdate ()
        {
            AssertAreEqual (null,
                new Data { Foo = "foo", Bar = 42, Bat = true },
                new Data { Foo = "foo", Bar = 42, Bat = true });
        }
        
        [Test]
        public void OneUpdate ()
        {
            AssertAreEqual ("<Foo>foo</Foo>",
                new Data { Foo = "foo", Bar = 42, Bat = true },
                new Data { Foo = "bar", Bar = 42, Bat = true });
        }
        
        [Test]
        public void OneMadeNotNull ()
        {
            AssertAreEqual ("<Foo />",
                new Data { Foo = null, Bar = 42, Bat = true },
                new Data { Foo = "foo", Bar = 42, Bat = true });
        }
        
        [Test]
        public void OneMadeNull ()
        {
            AssertAreEqual ("<Foo>foo</Foo>",
                new Data { Foo = "foo", Bar = 42, Bat = true },
                new Data { Foo = null, Bar = 42, Bat = true });
        }
        
        [Test]
        public void OneUpdateWithCommaEscapes ()
        {
            AssertAreEqual (@"<Foo>the truth\, the whole truth\, and nothing but the truth</Foo>",
                new Data { Foo = "the truth, the whole truth, and nothing but the truth", Bar = 42, Bat = true },
                new Data { Foo = "bar", Bar = 42, Bat = true });
        }
        
        [Test]
        public void TwoUpdates ()
        {
            AssertAreEqual ("<Foo>foo</Foo>,<Bar>42</Bar>",
                new Data { Foo = "foo", Bar = 42, Bat = true },
                new Data { Foo = "bar", Bar = 13, Bat = true });
        }
        
        [Test]
        public void ThreeUpdates ()
        {
            AssertAreEqual ("<Foo>foo</Foo>,<Bar>42</Bar>,<Bat>True</Bat>",
                new Data { Foo = "foo", Bar = 42, Bat = true },
                new Data { Foo = "bar", Bar = 13, Bat = false });
        }
        
        class NamedData
        {
            [XmlElement ("foo")] public string Foo { get; set; }
        }
        
        [Test]
        public void NamedElementUpdate ()
        {
            AssertAreEqual ("<foo>foo</foo>",
                new NamedData { Foo = "foo" },
                new NamedData { Foo = "bar" });
        }
        
        class OmitIfNullData
        {
            [XmlElement (OmitIfNull = true)] public string Foo { get; set; }
            [XmlElement (OmitIfNull = true)] public string Bar { get; set; }
        }
        
        [Test]
        public void OneOmitIfNullMadeNotNull ()
        {
            AssertAreEqual ("",
                new OmitIfNullData (),
                new OmitIfNullData { Foo = "foo" });
        }
        
        [Test]
        public void OneOmitIfNullMadeNull ()
        {
            AssertAreEqual ("<Foo>foo</Foo>",
                new OmitIfNullData { Foo = "foo" },
                new OmitIfNullData ());
        }
        
        [Test]
        public void TwoOmitIfNullMadeNotNull ()
        {
            AssertAreEqual (",",
                new OmitIfNullData (),
                new OmitIfNullData { Foo = "foo", Bar = "bar" });
        }
        
        [Test]
        public void TwoOmitIfNullMadeNull ()
        {
            AssertAreEqual ("<Foo>foo</Foo>,<Bar>bar</Bar>",
                new OmitIfNullData { Foo = "foo", Bar = "bar" },
                new OmitIfNullData ());
        }
        
        class DataWithFlag
        {
            [XmlElement] public string Foo { get; set; }
            [XmlFlag] public bool Bar { get; set; }
        }
        
        [Test]
        public void FlagAdded ()
        {
            AssertAreEqual ("",
                new DataWithFlag { Foo = "foo", Bar = false },
                new DataWithFlag { Foo = "foo", Bar = true });
        }
        
        [Test]
        public void FlagAddedAndStringChanged ()
        {
            AssertAreEqual ("<Foo>foo</Foo>,",
                new DataWithFlag { Foo = "foo", Bar = false },
                new DataWithFlag { Foo = "bar", Bar = true });
        }
        
        [Test]
        public void FlagRemoved ()
        {
            AssertAreEqual ("<Bar />",
                new DataWithFlag { Foo = "foo", Bar = true },
                new DataWithFlag { Foo = "foo", Bar = false });
        }
        
        [Test]
        public void FlagRemovedAndStringChanged ()
        {
            AssertAreEqual ("<Foo>foo</Foo>,<Bar />",
                new DataWithFlag { Foo = "foo", Bar = true },
                new DataWithFlag { Foo = "bar", Bar = false });
        }
        
        class NestedData
        {
            [XmlElement] public string Foo { get; set; }
            [XmlElement (OmitIfNull = true)] public NestedData Child { get; set; }
            
            public override bool Equals (object obj)
            {
                var data = (NestedData)obj;
                return data.Foo == Foo && data.Child.Equals (Child);
            }
        }
        
        [Test]
        public void NestedObjectChanged ()
        {
            AssertAreEqual ("<Child><Foo>foo</Foo></Child>",
                new NestedData { Child = new NestedData { Foo = "foo" } },
                new NestedData { Child = new NestedData { Foo = "bar" } });
        }
        
        class Item
        {
            readonly string foo;
            
            public Item (string foo)
            {
                this.foo = foo;
            }
            
            public override bool Equals (object obj)
            {
                return foo == ((Item)obj).foo;
            }
            
            public override string ToString ()
            {
                return foo;
            }
        }
        
        class ArrayItemData
        {
            [XmlArrayItem] public IEnumerable<Item> Data { get; set; }
        }
        
        [Test]
        public void NullArrayItemUnchanged ()
        {
            AssertAreEqual (null,
                new ArrayItemData (),
                new ArrayItemData ());
        }
        
        [Test]
        public void EmptyArrayItemUnchaged ()
        {
            AssertAreEqual (null,
                new ArrayItemData { Data = new Item[0] },
                new ArrayItemData { Data = new Item[0] });
        }
        
        [Test]
        public void SingleArrayItemUnchanged ()
        {
            AssertAreEqual (null,
                new ArrayItemData { Data = new [] { new Item ("foo") } },
                new ArrayItemData { Data = new [] { new Item ("foo") } });
        }
        
        [Test]
        public void TwoArrayItemsUnchanged ()
        {
            AssertAreEqual (null,
                new ArrayItemData { Data = new [] { new Item ("foo"), new Item ("bar") } },
                new ArrayItemData { Data = new [] { new Item ("foo"), new Item ("bar") } });
        }
        
        [Test]
        public void NullArrayItemMadeEmpty ()
        {
            AssertAreEqual (null,
                new ArrayItemData (),
                new ArrayItemData { Data = new Item[0] });
        }
        
        [Test]
        public void EmptyArrayItemMadeNull ()
        {
            AssertAreEqual (null,
                new ArrayItemData { Data = new Item[0] },
                new ArrayItemData ());
        }
        
        [Test]
        public void NullArrayItemMadeSingle ()
        {
            AssertAreEqual ("",
                new ArrayItemData (),
                new ArrayItemData { Data = new [] { new Item ("foo") } });
        }
        
        [Test]
        public void SingleArrayItemMadeNull ()
        {
            AssertAreEqual ("<Item>foo</Item>",
                new ArrayItemData { Data = new [] { new Item ("foo") } },
                new ArrayItemData ());
        }
        
        [Test]
        public void NullArrayItemMadeDouble ()
        {
            AssertAreEqual (",",
                new ArrayItemData (),
                new ArrayItemData { Data = new [] { new Item ("foo"), new Item ("bar") } });
        }
        
        [Test]
        public void DoubleArrayItemMadeNull ()
        {
            AssertAreEqual ("<Item>foo</Item>,<Item>bar</Item>",
                new ArrayItemData { Data = new [] { new Item ("foo"), new Item ("bar") } },
                new ArrayItemData ());
        }
        
        [Test]
        public void EmptyArrayItemMadeSingle ()
        {
            AssertAreEqual ("",
                new ArrayItemData { Data = new Item[0] },
                new ArrayItemData { Data = new [] { new Item ("foo") } });
        }
        
        [Test]
        public void SingleArrayItemMadeEmpty ()
        {
            AssertAreEqual ("<Item>foo</Item>",
                new ArrayItemData { Data = new [] { new Item ("foo") } },
                new ArrayItemData { Data = new Item[0] });
        }
        
        [Test]
        public void EmptyArrayItemMadeDouble ()
        {
            AssertAreEqual (",",
                new ArrayItemData { Data = new Item[0] },
                new ArrayItemData { Data = new [] { new Item ("foo"), new Item ("bar") } });
        }
        
        [Test]
        public void DoubleArrayItemMadeEmpty ()
        {
            AssertAreEqual ("<Item>foo</Item>,<Item>bar</Item>",
                new ArrayItemData { Data = new [] { new Item ("foo"), new Item ("bar") } },
                new ArrayItemData { Data = new Item[0] });
        }
        
        [Test]
        public void SingleArrayItemMadeDouble ()
        {
            AssertAreEqual ("",
                new ArrayItemData { Data = new [] { new Item ("foo") } },
                new ArrayItemData { Data = new [] { new Item ("foo"), new Item ("bar") } });
        }
        
        [Test]
        public void DoubleArrayItemMadeSingle ()
        {
            AssertAreEqual ("<Item>bar</Item>",
                new ArrayItemData { Data = new [] { new Item ("foo"), new Item ("bar") } },
                new ArrayItemData { Data = new [] { new Item ("foo") } });
        }
        
        [Test]
        public void ArrayItemsChanged ()
        {
            var data1 = new ArrayItemData { Data = new [] { new Item ("1"), new Item ("2"), new Item ("3") } };
            var data2 = new ArrayItemData { Data = new [] { new Item ("a"), new Item ("b"), new Item ("c") } };
            AssertAreEqual ("<Item>1</Item>,<Item>2</Item>,<Item>3</Item>", data1, data2);
            AssertAreEqual ("<Item>a</Item>,<Item>b</Item>,<Item>c</Item>", data2, data1);
        }
        
        [Test]
        public void ArrayItemRemovedWithNull ()
        {
            var data1 = new ArrayItemData { Data = new [] { new Item ("foo") } };
            var data2 = new ArrayItemData { Data = new Item[] { null } };
            AssertAreEqual ("<Item>foo</Item>", data1, data2);
            AssertAreEqual ("", data2, data1);
        }
        
        [Test]
        public void ArrayItemsChangedAndAddedAndRemovedWithNull ()
        {
            var data1 = new ArrayItemData { Data = new [] { new Item ("1"), new Item ("2"), new Item ("3") } };
            var data2 = new ArrayItemData { Data = new [] { null, new Item ("2"), new Item ("three"), new Item ("4") } };
            AssertAreEqual ("<Item>1</Item>,<Item>3</Item>,", data1, data2);
            AssertAreEqual (",<Item>three</Item>,<Item>4</Item>", data2, data1);
        }
        
        void AssertAreEqual<T> (string expected, T obj1, T obj2)
        {
            using (var stream = new MemoryStream ()) {
                var updated = serializer.Serialize (obj1, obj2, stream);
                if (expected == null) {
                    Assert.IsFalse (updated);
                    return;
                }
                stream.Seek (0, SeekOrigin.Begin);
                var buffer = new byte[stream.Length];
                stream.Read (buffer, 0, (int)stream.Length);
                var actual = Encoding.UTF8.GetString (buffer);
                Assert.AreEqual (expected, actual);
            }
        }
    }
}
