// 
// XmlDeserializationTests.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Peterson
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
using System.Xml;

using NUnit.Framework;

namespace Mono.Upnp.Xml.Tests
{
    [TestFixture]
    public class XmlDeserializationTests
    {
        readonly XmlDeserializer deserializer = new XmlDeserializer ();
        
        T Deserialize<T> (string xml)
        {
            return Deserialize<T> (xml, null);
        }
        
        T Deserialize<T> (string xml, Deserializer<T> deserializer)
        {
            using (var string_reader = new StringReader (xml)) {
                using (var xml_reader = XmlReader.Create (string_reader)) {
                    xml_reader.Read ();
                    return this.deserializer.Deserialize<T> (xml_reader, deserializer);
                }
            }
        }
        
        class ElementTestClass
        {
            [XmlElement] public string Foo { get; set; }
        }
        
        [Test]
        public void ElementTest ()
        {
            var deserialized_object = Deserialize<ElementTestClass> ("<Test><Foo>Bar</Foo></Test>");
            Assert.AreEqual ("Bar", deserialized_object.Foo);
        }
        
        class ElementNameTestClass
        {
            [XmlElement ("foo")] public string Foo { get; set; }
        }
        
        [Test]
        public void ElementNameTest ()
        {
            var deserialized_object = Deserialize<ElementNameTestClass> ("<Test><foo>Bar</foo></Test>");
            Assert.AreEqual ("Bar", deserialized_object.Foo);
        }
        
        class ElementNamespaceTestClass
        {
            [XmlElement ("foo", "urn:mono-upnp:tests")] public string Foo { get; set; }
        }
        
        [Test]
        public void ElementNamespaceTest ()
        {
            var deserialized_object = Deserialize<ElementNamespaceTestClass> (@"<Test><foo xmlns=""urn:mono-upnp:tests"">Bar</foo></Test>");
            Assert.AreEqual ("Bar", deserialized_object.Foo);
            
            deserialized_object = Deserialize<ElementNamespaceTestClass> (@"<Test><foo>Bar</foo></Test>");
            Assert.IsNull (deserialized_object.Foo);
        }
        
        class ElementTestClass<T>
        {
            [XmlElement] public T Foo { get; set; }
        }
        
        [Test]
        public void IntElementTest ()
        {
            var deserialized_object = Deserialize<ElementTestClass<int>> ("<Test><Foo>42</Foo></Test>");
            Assert.AreEqual (42, deserialized_object.Foo);
        }
        
        [Test]
        public void DoubleElementTest ()
        {
            var deserialized_object = Deserialize<ElementTestClass<double>> ("<Test><Foo>3.14</Foo></Test>");
            Assert.AreEqual (3.14, deserialized_object.Foo);
        }
        
        [Test]
        public void BoolElementTest ()
        {
            var deserialized_object = Deserialize<ElementTestClass<bool>> ("<Test><Foo>true</Foo></Test>");
            Assert.IsTrue (deserialized_object.Foo);
        }
        
        [Test]
        public void LongElementTest ()
        {
            var deserialized_object = Deserialize<ElementTestClass<long>> ("<Test><Foo>300000000000</Foo></Test>");
            Assert.AreEqual (300000000000L, deserialized_object.Foo);
        }
        
        [Test]
        public void FloatElementTest ()
        {
            var deserialized_object = Deserialize<ElementTestClass<float>> ("<Test><Foo>1.5</Foo></Test>");
            Assert.AreEqual (1.5, deserialized_object.Foo);
        }
        
        [Test]
        public void DecimalElementTest ()
        {
            var deserialized_object = Deserialize<ElementTestClass<decimal>> ("<Test><Foo>.00005</Foo></Test>");
            Assert.AreEqual (0.00005, deserialized_object.Foo);
        }
        
        // TODO DateTime test
        
        [Test]
        public void UriElementTest ()
        {
            var deserialized_object = Deserialize<ElementTestClass<Uri>> ("<Test><Foo>http://localhost/</Foo></Test>");
            Assert.AreEqual (new Uri ("http://localhost/"), deserialized_object.Foo);
        }
        
        class NestedElementTestClass
        {
            [XmlElement] public ElementTestClass Child { get; set; }
        }
        
        [Test]
        public void NestedElementTest ()
        {
            var deserialized_object = Deserialize<NestedElementTestClass> ("<Test><Child><Foo>Bar</Foo></Child></Test>");
            Assert.AreEqual ("Bar", deserialized_object.Child.Foo);
        }
        
        class AttributeTestClass
        {
            [XmlAttribute] public string Foo { get; set; }
        }
        
        [Test]
        public void AttributeTest ()
        {
            var deserialized_object = Deserialize<AttributeTestClass> (@"<Test Foo=""Bar"" />");
            Assert.AreEqual ("Bar", deserialized_object.Foo);
        }
        
        class AttributeNameTestClass
        {
            [XmlAttribute ("foo")] public string Foo { get; set; }
        }
        
        [Test]
        public void AttributeNameTest ()
        {
            var deserialized_object = Deserialize<AttributeNameTestClass> (@"<Test foo=""Bar"" />");
            Assert.AreEqual ("Bar", deserialized_object.Foo);
        }
        
        class AttributeNamespaceTestClass
        {
            [XmlAttribute ("foo", "urn:mono-upnp:tests")] public string Foo { get; set; }
        }
        
        [Test]
        public void AttributeNamespaceTest ()
        {
            var deserialized_object = Deserialize<AttributeNamespaceTestClass> (@"<Test test:foo=""Bar"" xmlns:test=""urn:mono-upnp:tests"" />");
            Assert.AreEqual ("Bar", deserialized_object.Foo);
            
            deserialized_object = Deserialize<AttributeNamespaceTestClass> (@"<Test foo=""Bar"" />");
            Assert.IsNull (deserialized_object.Foo);
        }
        
        class AttributeTestClass<T>
        {
            [XmlAttribute] public T Foo { get; set; }
        }
        
        [Test]
        public void IntAttributeTest ()
        {
            var deserialized_object = Deserialize<AttributeTestClass<int>> (@"<Test Foo=""42"" />");
            Assert.AreEqual (42, deserialized_object.Foo);
        }
        
        [Test]
        public void DoubleAttributeTest ()
        {
            var deserialized_object = Deserialize<AttributeTestClass<double>> (@"<Test Foo=""3.14"" />");
            Assert.AreEqual (3.14, deserialized_object.Foo);
        }
        
        [Test]
        public void BoolAttributeTest ()
        {
            var deserialized_object = Deserialize<AttributeTestClass<bool>> (@"<Test Foo=""true"" />");
            Assert.IsTrue (deserialized_object.Foo);
        }
        
        [Test]
        public void LongAttributeTest ()
        {
            var deserialized_object = Deserialize<AttributeTestClass<long>> (@"<Test Foo=""300000000000"" />");
            Assert.AreEqual (300000000000L, deserialized_object.Foo);
        }
        
        [Test]
        public void FloatAttributeTest ()
        {
            var deserialized_object = Deserialize<AttributeTestClass<float>> (@"<Test Foo=""1.5"" />");
            Assert.AreEqual (1.5, deserialized_object.Foo);
        }
        
        [Test]
        public void DecimalAttributeTest ()
        {
            var deserialized_object = Deserialize<AttributeTestClass<decimal>> (@"<Test Foo="".00005"" />");
            Assert.AreEqual (0.00005, deserialized_object.Foo);
        }
        
        // TODO DateTime test
        
        [Test]
        public void UriAttributeTest ()
        {
            var deserialized_object = Deserialize<AttributeTestClass<Uri>> (@"<Test Foo=""http://localhost/"" />");
            Assert.AreEqual (new Uri ("http://localhost/"), deserialized_object.Foo);
        }
        
        class FlagTestClass
        {
            [XmlFlag] public bool Foo { get; set; }
        }
        
        [Test]
        public void FlagTest ()
        {
            var deserialized_object = Deserialize<FlagTestClass> (@"<Test><Foo /></Test>");
            Assert.IsTrue (deserialized_object.Foo);
        }
        
        class FlagNameTestClass
        {
            [XmlFlag ("foo")] public bool Foo { get; set; }
        }
        
        [Test]
        public void FlagNameTest ()
        {
            var deserialized_object = Deserialize<FlagNameTestClass> (@"<Test><foo /></Test>");
            Assert.IsTrue (deserialized_object.Foo);
        }
        
        class FlagNamespaceTestClass
        {
            [XmlFlag ("foo", "urn:mono-upnp:tests")] public bool Foo { get; set; }
        }
        
        [Test]
        public void FlagNamespaceTest ()
        {
            var deserialized_object = Deserialize<FlagNamespaceTestClass> (@"<Test><foo xmlns=""urn:mono-upnp:tests"" /></Test>");
            Assert.IsTrue (deserialized_object.Foo);
            
            deserialized_object = Deserialize<FlagNamespaceTestClass> (@"<Test><foo /></Test>");
            Assert.IsFalse (deserialized_object.Foo);
        }
        
        class ArrayStringTestClass
        {
            readonly List<string> items = new List<string> ();
            [XmlArray] public List<string> Items { get { return items; } }
        }
        
        [Test]
        public void ArrayStringTest ()
        {
            var deserialized_object = Deserialize<ArrayStringTestClass> (@"<Test><Items><Item>Foo</Item><Item>Bar</Item></Items></Test>");
            Assert.AreEqual (2, deserialized_object.Items.Count);
            Assert.Contains ("Foo", deserialized_object.Items);
            Assert.Contains ("Bar", deserialized_object.Items);
        }
        
        class ArrayItemTestClass
        {
            readonly List<AttributeTestClass> items = new List<AttributeTestClass> ();
            [XmlArray] public List<AttributeTestClass> Items { get { return items; } }
        }
        
        [Test]
        public void ArrayItemTest ()
        {
            var deserialized_object = Deserialize<ArrayItemTestClass> (@"<Test><Items><Item Foo=""bat""/><Item Foo=""baz""/></Items></Test>");
            Assert.AreEqual (2, deserialized_object.Items.Count);
            Assert.AreEqual ("bat", deserialized_object.Items[0].Foo);
            Assert.AreEqual ("baz", deserialized_object.Items[1].Foo);
        }
        
        class ArrayNameTestClass
        {
            readonly List<string> items = new List<string> ();
            [XmlArray ("items")] public List<string> Items { get { return items; } }
        }
        
        [Test]
        public void ArrayNameTest ()
        {
            var deserialized_object = Deserialize<ArrayNameTestClass> (@"<Test><items><Item>Foo</Item><Item>Bar</Item></items></Test>");
            Assert.AreEqual (2, deserialized_object.Items.Count);
            Assert.Contains ("Foo", deserialized_object.Items);
            Assert.Contains ("Bar", deserialized_object.Items);
        }
        
        class ArrayNamespaceTestClass
        {
            readonly List<string> items = new List<string> ();
            [XmlArray ("items", "urn:mono-upnp:tests")] public List<string> Items { get { return items; } }
        }
        
        [Test]
        public void ArrayNamespaceTest ()
        {
            var deserialized_object = Deserialize<ArrayNamespaceTestClass> (@"<Test><items xmlns=""urn:mono-upnp:tests""><Item>Foo</Item><Item>Bar</Item></items></Test>");
            Assert.AreEqual (2, deserialized_object.Items.Count);
            Assert.Contains ("Foo", deserialized_object.Items);
            Assert.Contains ("Bar", deserialized_object.Items);
            
            deserialized_object = Deserialize<ArrayNamespaceTestClass> (@"<Test><items><Item>Foo</Item><Item>Bar</Item></items></Test>");
            Assert.AreEqual (0, deserialized_object.Items.Count);
        }
        
        class ArrayICollectionTestClass
        {
            readonly List<string> items = new List<string> ();
            [XmlArray] public ICollection<string> Items { get { return items; } }
        }
        
        [Test]
        public void ArrayICollectionTest ()
        {
            var deserialized_object = Deserialize<ArrayICollectionTestClass> (@"<Test><Items><Item>Foo</Item><Item>Bar</Item></Items></Test>");
            Assert.AreEqual (2, deserialized_object.Items.Count);
            Assert.IsTrue (deserialized_object.Items.Contains("Foo"));
            Assert.IsTrue (deserialized_object.Items.Contains("Bar"));
        }
        
        class RecursiveTestClass
        {
            [XmlElement] public string Foo { get; set; }
            [XmlElement] public RecursiveTestClass Child { get; set; }
        }
        
        [Test]
        public void RecursiveTest ()
        {
            var deserialized_object = Deserialize<RecursiveTestClass> (@"<Test><Foo>bar</Foo><Child><Foo>bat</Foo></Child></Test>");
            Assert.AreEqual ("bar", deserialized_object.Foo);
            Assert.AreEqual ("bat", deserialized_object.Child.Foo);
        }
        
        class PrivateTestClass
        {
            [XmlAttribute] public string Foo { get; private set; }
        }
        
        [Test]
        public void PrivateTest ()
        {
            var deserialized_object = Deserialize<PrivateTestClass> (@"<Test Foo=""bar"" />");
            Assert.AreEqual ("bar", deserialized_object.Foo);
        }
        
        class IXmlDeserializableDeserializeTestClass : IXmlDeserializable
        {
            public string Foo { get; private set; }
            
            public void Deserialize (XmlDeserializationContext context)
            {
                Foo = context.Reader.LocalName;
            }
            
            public void DeserializeAttribute (XmlDeserializationContext context)
            {
            }
            
            public void DeserializeElement (XmlDeserializationContext context)
            {
            }
        }
        
        [Test]
        public void IXmlDeserializableDeserializeTest ()
        {
            var deserialized_object = Deserialize<IXmlDeserializableDeserializeTestClass> (@"<Bar />");
            Assert.AreEqual ("Bar", deserialized_object.Foo);
        }
        
        class IXmlDeserializableDeserializeAttributeTestClass : IXmlDeserializable
        {
            public string Foo { get; private set; }
            
            public void Deserialize (XmlDeserializationContext context)
            {
                context.AutoDeserialize (this);
            }
            
            public void DeserializeAttribute (XmlDeserializationContext context)
            {
                Foo = context.Reader.ReadContentAsString ();
            }
            
            public void DeserializeElement (XmlDeserializationContext context)
            {
            }
        }
        
        [Test]
        public void IXmlDeserializableDeserializeAttributeTest ()
        {
            var deserialized_object = Deserialize<IXmlDeserializableDeserializeAttributeTestClass> (@"<Test Bar=""hello world"" />");
            Assert.AreEqual ("hello world", deserialized_object.Foo);
        }
        
        class IXmlDeserializableDeserializeElementTestClass : IXmlDeserializable
        {
            [XmlElement] public string Foo { get; private set; }
            [XmlElement] public string Bar { get; private set; }
            
            public void Deserialize (XmlDeserializationContext context)
            {
                context.AutoDeserialize (this);
            }
            
            public void DeserializeAttribute (XmlDeserializationContext context)
            {
            }
            
            public void DeserializeElement (XmlDeserializationContext context)
            {
                if (context.Reader.LocalName == "Foo") {
                    Foo = "Foo is " + context.Reader.ReadElementContentAsString ();
                } else {
                    context.AutoDeserializeElement (this);
                }
            }
        }
        
        [Test]
        public void IXmlDeserializableDeserializeElementTest ()
        {
            var deserialized_object = Deserialize<IXmlDeserializableDeserializeElementTestClass> (@"<Test><Foo>bar</Foo><Bar>bat</Bar></Test>");
            Assert.AreEqual ("Foo is bar", deserialized_object.Foo);
            Assert.AreEqual ("bat", deserialized_object.Bar);
        }
        
        class OverrideTestBaseClass
        {
            [XmlElement] public virtual string Foo { get; set; }
        }
        
        class OverrideTestSubClass : OverrideTestBaseClass
        {
            [XmlElement ("foo")] public override string Foo {
                get { return base.Foo; }
                set { base.Foo = value; }
            }
        }
        
        [Test]
        public void OverrideTest ()
        {
            var deserialized_object = Deserialize<OverrideTestSubClass> (@"<Test><foo>bar</foo></Test>");
            Assert.AreEqual ("bar", deserialized_object.Foo);
            
            deserialized_object = Deserialize<OverrideTestSubClass> (@"<Test><Foo>bar</Foo></Test>");
            Assert.IsNull (deserialized_object.Foo);
            
            var base_deserialized_object = Deserialize<OverrideTestBaseClass> (@"<Test><foo>bar</foo></Test>");
            Assert.IsNull (base_deserialized_object.Foo);
        }
        
        class OverrideOmitTestSubClass : OverrideTestBaseClass
        {
            public override string Foo {
                get { return base.Foo; }
                set { base.Foo = value; }
            }
        }
        
        [Test]
        public void OverrideOmitTest ()
        {
            var deserialized_object = Deserialize<OverrideTestSubClass> (@"<Test><Foo>bar</Foo></Test>");
            Assert.IsNull (deserialized_object.Foo);
        }
        
        class DeserializerTestClass
        {
            readonly string foo;
            
            public DeserializerTestClass (string foo)
            {
                this.foo = foo;
            }
            
            public string Foo { get { return foo; } }
            [XmlAttribute] public string Bar { get; set; }
        }
        
        [Test]
        public void DeserializerTest ()
        {
            var deserialized_object = Deserialize (@"<Test Bar=""bat"" />", context => {
                var obj = new DeserializerTestClass ("baz");
                context.AutoDeserialize (obj);
                return obj;
            });
            Assert.AreEqual ("baz", deserialized_object.Foo);
            Assert.AreEqual ("bat", deserialized_object.Bar);
        }
        
        class TypeDeserializerTestClass
        {
            [XmlElement] public DeserializerTestClass Child { get; set; }
            
            [XmlTypeDeserializer]
            protected virtual DeserializerTestClass DeserializeChild (XmlDeserializationContext context)
            {
                var child = new DeserializerTestClass ("hello world");
                context.AutoDeserialize (child);
                return child;
            }
        }
        
        [Test]
        public void TypeDeserializerTest ()
        {
            var deserialized_object = Deserialize<TypeDeserializerTestClass> (@"<Test><Child Bar=""bat""/></Test>");
            Assert.AreEqual ("hello world", deserialized_object.Child.Foo);
            Assert.AreEqual ("bat", deserialized_object.Child.Bar);
        }
        
        class XmlDeserializableTestBaseClass : XmlDeserializable
        {
            [XmlAttribute] public virtual string Foo { get; protected set; }
            
            public string ElementName { get; private set; }
            
            protected override void Deserialize (XmlDeserializationContext context)
            {
                ElementName = context.Reader.LocalName;
                base.Deserialize (context);
            }
            
            protected override void DeserializeAttribute (XmlDeserializationContext context)
            {
                context.AutoDeserializeAttribute (this);
            }
        }
        
        class XmlDeserializableTestSubClass : XmlDeserializableTestBaseClass
        {
            [XmlAttribute ("foo")] public override string Foo {
                get { return base.Foo; }
                protected set { base.Foo = value; }
            }
            
            protected override void DeserializeAttribute (XmlDeserializationContext context)
            {
                context.AutoDeserializeAttribute (this);
            }
        }
        
        class XmlDeserializableContainerTestBaseClass
        {
            [XmlElement] public XmlDeserializableTestBaseClass Child { get; set; }
            
            [XmlTypeDeserializer]
            protected virtual XmlDeserializableTestBaseClass DeserializeChild (XmlDeserializationContext context)
            {
                var child = new XmlDeserializableTestBaseClass ();
                ((IXmlDeserializable)child).Deserialize (context);
                return child;
            }
        }
        
        class XmlDeserializableContainerTestSubClass : XmlDeserializableContainerTestBaseClass
        {
            [XmlTypeDeserializer]
            protected override XmlDeserializableTestBaseClass DeserializeChild (XmlDeserializationContext context)
            {
                var child = new XmlDeserializableTestSubClass ();
                ((IXmlDeserializable)child).Deserialize(context);
                return child;
            }
        }
        
        [Test]
        public void XmlDeserializableTest ()
        {
            var deserialized_object = Deserialize<XmlDeserializableContainerTestSubClass> (
                @"<Test><Child foo=""bar""/></Test>");
            Assert.AreEqual ("bar", deserialized_object.Child.Foo);
            Assert.AreEqual ("Child", deserialized_object.Child.ElementName);
        }
        
        class XmlDeserializableTyeDeserializerTestClass : XmlDeserializable
        {
            [XmlElement] public DeserializerTestClass Child { get; private set; }
            
            [XmlTypeDeserializer]
            protected DeserializerTestClass ChildDeserializer (XmlDeserializationContext context)
            {
                var child = new DeserializerTestClass ("blah");
                context.AutoDeserialize (child);
                return child;
            }
            
            protected override void DeserializeElement (XmlDeserializationContext context)
            {
                context.AutoDeserializeElement (this);
            }
        }
        
        [Test]
        public void XmlDeserializableTyeDeserializerTest ()
        {
            var deserialized_object = Deserialize<XmlDeserializableTyeDeserializerTestClass> (
                @"<Test><Child Bar=""bat""></Test>");
            
            Assert.AreEqual ("blah", deserialized_object.Child.Foo);
            Assert.AreEqual ("bat", deserialized_object.Child.Bar);
        }
        
        class DeserializerArrayTestClass
        {
            readonly List<DeserializerTestClass> list = new List<DeserializerTestClass> ();
            [XmlArray] public List<DeserializerTestClass> Children { get { return list; } }
            
            [XmlTypeDeserializer]
            protected DeserializerTestClass ChildDeserializer (XmlDeserializationContext context)
            {
                var child = new DeserializerTestClass ("blah");
                context.AutoDeserialize (child);
                return child;
            }
        }
        
        [Test]
        public void DeserializerArrayTest ()
        {
            var deserialized_object = Deserialize<DeserializerArrayTestClass> (
                @"<Test><Children><Child Bar=""one"" /><Child Bar=""two"" /><Child Bar=""five"" /></Children></Test>");
            Assert.AreEqual (3, deserialized_object.Children.Count);
            Assert.AreEqual ("one", deserialized_object.Children[0].Bar);
            Assert.AreEqual ("two", deserialized_object.Children[1].Bar);
            Assert.AreEqual ("five", deserialized_object.Children[2].Bar);
        }
        
        class NonPublicConstructorTestClass
        {
            NonPublicConstructorTestClass ()
            {
            }
            
            [XmlAttribute] public string Foo { get; set; }
        }
        
        class NonPublicConstructorContainerClass
        {
            [XmlElement] public NonPublicConstructorTestClass Child { get; set; }
        }
        
        [Test]
        public void NonPublicConstructorTest ()
        {
            var deserialized_object = Deserialize<NonPublicConstructorContainerClass> (
                @"<Test><Child Foo=""bar""/></Test>");
            Assert.AreEqual ("bar", deserialized_object.Child.Foo);
        }
        
        class DoNotDeserializeTestClass
        {
            [DoNotDeserialize, XmlAttribute] public string Foo { get; set; }
            [DoNotDeserialize, XmlElement] public string Bar { get; set; }
            [XmlAttribute] public string Bat { get; set; }
            [XmlElement] public string Baz { get; set; }
        }
        
        [Test]
        public void DoNotDeserializeTest ()
        {
            var deserialized_object = Deserialize<DoNotDeserializeTestClass> (
                @"<Test Foo=""foo"" Bat=""bat""><Bar>bar</Bar><Baz>baz</Baz></Test>");
            Assert.IsNull (deserialized_object.Foo);
            Assert.IsNull (deserialized_object.Bar);
            Assert.AreEqual ("bat", deserialized_object.Bat);
            Assert.AreEqual ("baz", deserialized_object.Baz);
        }
        
        enum EnumTestEnum
        {
            [XmlEnum ("foo")] Foo,
            [XmlEnum ("bar")] Bar,
            [XmlEnum ("bat")] Bat
        }
        
        class EnumTestClass
        {
            [XmlAttribute] public EnumTestEnum Thing { get; set; }
            [XmlElement] public EnumTestEnum Doohicky { get; set; }
        }
        
        [Test]
        public void EnumTest ()
        {
            var deserialized_object = Deserialize<EnumTestClass> (@"<Test Thing=""bar""><Doohicky>bat</Doohicky></Test>");
            Assert.AreEqual (EnumTestEnum.Bar, deserialized_object.Thing);
            Assert.AreEqual (EnumTestEnum.Bat, deserialized_object.Doohicky);
        }
        
        [Test]
        public void NoNamespaceTest ()
        {
            var deserialized_object = Deserialize<ElementTestClass> (@"<Test xmlns=""urn:mono-upnp:tests""><Foo>bar</Foo></Test>");
            Assert.AreEqual ("bar", deserialized_object.Foo);
        }
        
        class ValueTestClass
        {
            [XmlValue] public string Foo { get; set; }
        }
        
        [Test]
        public void ValueTest ()
        {
            var deserialized_object = Deserialize<ValueTestClass> (@"<Test>bar</Test>");
            Assert.AreEqual ("bar", deserialized_object.Foo);
        }
    }
}
