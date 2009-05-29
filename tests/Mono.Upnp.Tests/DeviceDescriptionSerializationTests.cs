// 
// DeviceDescriptionSerializationTests.cs
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

using Mono.Upnp.Xml;

using NUnit.Framework;

namespace Mono.Upnp.Tests
{
    [TestFixture]
    public class DeviceDescriptionSerializationTests
    {
        readonly XmlSerializer serializer = new XmlSerializer ();
        
        [Test]
        public void FullDescriptionTest ()
        {
            var root = new DummyRoot (
                new DeviceSettings (
                    new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-full-device:1"),
                    "uuid:fd1",
                    "Mono.Upnp.Tests Full Device",
                    "Mono Project",
                    "Full Device") {
                    ManufacturerUrl = new Uri ("http://www.mono-project.org/"),
                    ModelDescription = "A device description with all optional information.",
                    ModelNumber = "1",
                    ModelUrl = new Uri ("http://www.mono-project.org/Mono.Upnp/"),
                    SerialNumber = "12345",
                    Upc = "67890",
                    Icons = new Icon[] {
                        new DummyIcon (100, 100, 32, "image/png"),
                        new DummyIcon (100, 100, 32, "image/jpeg")
                    },
                    Services = new Service[] {
                        new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"), "urn:upnp-org:serviceId:testService1"),
                        new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"), "urn:upnp-org:serviceId:testService2"),
                    }
                }, 
                new Device[] {
                    new Device (new DeviceSettings (
                        new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-full-embedded-device:1"),
                        "uuid:fed1",
                        "Mono.Upnp.Tests Full Embedded Device",
                        "Mono Project",
                        "Full Embedded Device") {
                        ManufacturerUrl = new Uri ("http://www.mono-project.org/"),
                        ModelDescription = "An embedded device description with all optional information.",
                        ModelNumber = "1",
                        ModelUrl = new Uri ("http://www.mono-project.org/Mono.Upnp/"),
                        SerialNumber = "12345",
                        Upc = "67890",
                        Icons = new Icon[] {
                            new DummyIcon (100, 100, 32, "image/png"),
                            new DummyIcon (100, 100, 32, "image/jpeg")
                        },
                        Services = new Service[] {
                            new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"), "urn:upnp-org:serviceId:testService1"),
                            new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"), "urn:upnp-org:serviceId:testService2"),
                        }
                    })
                }
            );
            root.Initialize ();
            Assert.AreEqual (Xml.FullDeviceDescription, serializer.GetString<Root> (root));
        }
    }
}
