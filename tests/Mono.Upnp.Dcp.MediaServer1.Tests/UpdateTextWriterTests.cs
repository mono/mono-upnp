// 
// UpdateTextWriterTests.cs
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
using System.IO;
using System.Text;
using System.Xml;

using NUnit.Framework;

using Mono.Upnp.Dcp.MediaServer1.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.Tests
{
    [TestFixture]
    public class UpdateTextWriterTests
    {
        delegate void Test (UpdateTextWriter writer);
        
        [Test]
        public void NoComma ()
        {
            AssertStringsAreEqual ("foo", "foo");
        }
        
        [Test]
        public void EscapeComma ()
        {
            AssertStringsAreEqual (@"\,", ",");
        }
        
        [Test]
        public void EscapeTwoCommas ()
        {
            AssertStringsAreEqual (@"\,\,", ",,");
        }
        
        [Test]
        public void EscapeThreeCommas ()
        {
            AssertStringsAreEqual (@"\,\,\,", ",,,");
        }
        
        [Test]
        public void EscapeLeadingComma ()
        {
            AssertStringsAreEqual (@"\,foo", ",foo");
        }
        
        [Test]
        public void EscapeTwoLeadingCommas ()
        {
            AssertStringsAreEqual (@"\,\,foo", ",,foo");
        }
        
        [Test]
        public void EscapeTrailingComma ()
        {
            AssertStringsAreEqual (@"foo\,", "foo,");
        }
        
        [Test]
        public void EscapeTwoTrailingCommas ()
        {
            AssertStringsAreEqual (@"foo\,\,", "foo,,");
        }
        
        [Test]
        public void EscapeInSituComma ()
        {
            AssertStringsAreEqual (@"foo\,bar", "foo,bar");
        }
        
        [Test]
        public void EscapeTwoInSituCommas ()
        {
            AssertStringsAreEqual (@"foo\,bar\,bat", "foo,bar,bat");
        }
        
        [Test]
        public void EscapeLeadingAndInSituCommas ()
        {
            AssertStringsAreEqual (@"\,foo\,bar", ",foo,bar");
        }
        
        [Test]
        public void EscapeLeadingAndTrailingCommas ()
        {
            AssertStringsAreEqual (@"\,foo\,", ",foo,");
        }
        
        [Test]
        public void EscapeInSituAndTrailingCommas ()
        {
            AssertStringsAreEqual (@"foo\,bar\,", "foo,bar,");
        }
        
        [Test]
        public void EscapeLeadingInSituAndTrailingCommas ()
        {
            AssertStringsAreEqual (@"\,foo\,bar\,", ",foo,bar,");
        }
        
        [Test]
        public void EscapeThreeInSituCommas ()
        {
            AssertStringsAreEqual (@"foo\,bar\,bat\,baz", "foo,bar,bat,baz");
        }
        
        [Test]
        public void EscapeThreeEachOfLeadingInSituAndTrailingCommas ()
        {
            AssertStringsAreEqual (@"\,\,\,foo\,bar\,bat\,baz\,\,\,", ",,,foo,bar,bat,baz,,,");
        }
        
        const int test_array_padding = 5;
        
        void AssertStringsAreEqual (string expected, string test)
        {
            var test_array = test.ToCharArray ();
            AssertAreEqual (expected, writer => writer.Write (test));
            AssertAreEqual (expected, writer => writer.Write (test_array));
            AssertAreEqual (expected, writer => {
                var length = test_array.Length;
                var array = new char[length + test_array_padding];
                Array.Copy (test_array, array, length);
                writer.Write (array, 0, length);
            });
            AssertAreEqual (expected, writer => {
                var length = test_array.Length;
                var array = new char[length + test_array_padding * 2];
                Array.Copy (test_array, 0, array, test_array_padding, length);
                writer.Write (array, test_array_padding, length);
            });
            AssertAreEqual (expected, writer => {
                var length = test_array.Length;
                var array = new char[length + test_array_padding];
                Array.Copy (test_array, 0, array, test_array_padding, length);
                writer.Write (array, test_array_padding, length);
            });
            AssertAreEqual (expected, writer => {
                foreach (var character in test) {
                    writer.Write (character);
                }
            });
        }
        
        void AssertAreEqual (string expected, Test test)
        {
            var builder = new StringBuilder ();
            using (var string_writer = new StringWriter (builder)) {
                using (var update_writer = new UpdateTextWriter (string_writer)) {
                    test (update_writer);
                }
            }
            
            Assert.AreEqual (expected, builder.ToString ());
        }
    }
}
