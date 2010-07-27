// 
// ObjectTests.cs
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
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV;

namespace Mono.Upnp.Dcp.MediaServer1.Tests
{
    [TestFixture]
    public class ObjectTests
    {
        [Test]
        public void ObjectInstantiation ()
        {
            var options = new ObjectOptions {
                Title = "foo",
                Creator = "bar",
                WriteStatus = WriteStatus.Protected,
                IsRestricted = true
            };
            var @object = new Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object ("-1", options);
            Assert.AreEqual ("-1", @object.Id);
            AssertObject (@object, options);
            AssertObject (@object, @object.GetOptions ());
        }

        static void AssertObject (Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object @object, ObjectOptions options)
        {
            Assert.AreEqual (options.Title, @object.Title);
            Assert.AreEqual (options.Creator, @object.Creator);
            Assert.AreEqual (options.WriteStatus, @object.WriteStatus);
            Assert.AreEqual (options.IsRestricted, @object.IsRestricted);
            Assert.IsNotNull (@object.Resources);
            Assert.IsTrue (@object.Resources.IsReadOnly);
        }

        [Test]
        public void ItemInstantiation ()
        {
            var options = new ItemOptions {
                Title = "foo",
                RefId = "1"
            };
            var item = new Item ("-1", options);
            AssertItem (item, options);
            AssertItem (item, item.GetOptions ());
        }

        static void AssertItem (Item item, ItemOptions options)
        {
            AssertObject (item, options);
            Assert.AreEqual (options.RefId, item.RefId);
        }

        [Test]
        public void ContainerInstantiation ()
        {
            var options = new ContainerOptions {
                Title = "foo",
                ChildCount = 42,
                IsSearchable = true
            };
            var container = new Container ("-1", options);
            AssertContainer (container, options);
            AssertContainer (container, container.GetOptions ());
        }

        static void AssertContainer (Container container, ContainerOptions options)
        {
            AssertObject (container, options);
            Assert.AreEqual (options.ChildCount, container.ChildCount);
            Assert.AreEqual (options.IsSearchable, container.IsSearchable);
            Assert.IsNotNull (container.SearchClasses);
            Assert.IsTrue (container.SearchClasses.IsReadOnly);
            Assert.IsNotNull (container.CreateClasses);
            Assert.IsTrue (container.CreateClasses.IsReadOnly);
        }
    }
}
