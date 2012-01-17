// 
// TypeInfoTests.cs
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

namespace Mono.Upnp.Tests
{
    [TestFixture]
    public class TypeInfoTests
    {
        delegate T Constructor<T> (string domainName, string type, Version version);
        delegate T Parser<T> (string typeDescription);

        [Test]
        public void ParseDeviceType ()
        {
            ParseType ((domainName, type, version) => new DeviceType (domainName, type, version), DeviceType.Parse, "device");
        }

        [Test]
        public void ParseServiceType ()
        {
            ParseType ((domainName, type, version) => new ServiceType (domainName, type, version), ServiceType.Parse, "service");
        }

        [Test, ExpectedException (typeof (UpnpDeserializationException))]
        public void ParseTooFewComponents ()
        {
            DeviceType.Parse ("urn:mydomain.com:device:mytype");
        }

        [Test, ExpectedException (typeof (UpnpDeserializationException))]
        public void ParseBadVersion ()
        {
            DeviceType.Parse ("urn:mydomain.com:device:mytype:a");
        }

        [Test, ExpectedException (typeof (UpnpDeserializationException))]
        public void ParseBadMinorVersion ()
        {
            DeviceType.Parse ("urn:mydomain.com:device:mytype:1.a");
        }

        void ParseType<T> (Constructor<T> constructor, Parser<T> parser, string kind)
        {
            Assert.AreEqual (
                constructor ("mydomain.com", "mytype", new Version (1, 0)),
                parser ("urn:mydomain.com:" + kind + ":mytype:1"));

            Assert.AreEqual (
                constructor ("mydomain.com", "mytype", new Version (2, 0)),
                parser ("urn:mydomain.com:" + kind + ":mytype:2"));

            Assert.AreEqual (
                constructor ("mydomain.com", "mytype", new Version (1, 0)),
                parser ("urn:mydomain.com:" + kind + ":mytype:1.0"));

            Assert.AreEqual (
                constructor ("mydomain.com", "mytype", new Version (1, 1)),
                parser ("urn:mydomain.com:" + kind + ":mytype:1.1"));
        }
    }
}

