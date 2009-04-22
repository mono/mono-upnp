// 
// SerializationsTests.cs
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
using NUnit.Framework;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Shared.Test
{
    [TestFixture]
    public class SerializationsTests
    {
        readonly XmlSerializer serializer = new XmlSerializer ();
        
        class ElementAttributeTestType
        {
            readonly string value;
            
            public ElementAttributeTestType (string value)
            {
                this.value = value;
            }
            
            [XmlElement ("foo", OmitIfNull = true)]
            public string Foo { get { return value; } }
            
            [XmlElement]
            public string Bar { get { return value; } }
        }
        
        [Test]
        public void ElementAttributeTest ()
        {
            Assert.AreEqual (
                @"<?xml version=""1.0"" encoding=""utf-8""?><ElementAttributeTestType><foo>blah</foo><Bar>blah</Bar></ElementAttributeTestType>",
                serializer.GetString (new ElementAttributeTestType ("blah")));
            
            Assert.AreEqual (
                @"<?xml version=""1.0"" encoding=""utf-8""?><ElementAttributeTestType><Bar/></ElementAttributeTestType>",
                serializer.GetString (new ElementAttributeTestType (null)));
        }
        
        [XmlType ("typeAttributeTestType")]
        class TypeAttributeTestType
        {
        }
        
        [Test]
        public void TypeAttributeTest ()
        {
            Assert.AreEqual (
                @"<?xml version=""1.0"" encoding=""utf-8""?><typeAttributeTestType/>",
                serializer.GetString (new TypeAttributeTestType ()));
        }
        
        class FlagAttributeTestType
        {
            readonly bool value;
            
            public FlagAttributeTestType (bool value)
            {
                this.value = value;
            }
            
            [XmlFlag]
            public bool Foo { get { return value; } }
            
            [XmlFlag ("bar")]
            public bool Bar { get { return !value; } }
        }
        
        [Test]
        public void FlagAttributeTest ()
        {
            Assert.AreEqual (
                @"<?xml version=""1.0"" encoding=""utf-8""?><FlagAttributeTestType><Foo/></FlagAttributeTestType>",
                serializer.GetString (new FlagAttributeTestType (true)));

            Assert.AreEqual (
                @"<?xml version=""1.0"" encoding=""utf-8""?><FlagAttributeTestType><bar/></FlagAttributeTestType>",
                serializer.GetString (new FlagAttributeTestType (false)));
        }
        
        class AttributeAttributeTestType
        {
            readonly string value;
            
            public AttributeAttributeTestType (string value)
            {
                this.value = value;
            }
            
            [XmlAttribute]
            public string Foo { get { return value; } }
            
            [XmlAttribute ("bar", OmitIfNull = true)]
            public string Bar { get { return value; } }
        }
        
        [Test]
        public void AttributeAttributeTest ()
        {
            Assert.AreEqual (
                @"<?xml version=""1.0"" encoding=""utf-8""?><AttributeAttributeTestType Foo=""blah"" bar=""blah""/>",
                serializer.GetString (new AttributeAttributeTestType ("blah")));

            Assert.AreEqual (
                @"<?xml version=""1.0"" encoding=""utf-8""?><AttributeAttributeTestType Foo=""/>",
                serializer.GetString (new AttributeAttributeTestType (null)));
        }
    }
}
