// 
// DeviceDescriptionTests.cs
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

namespace Mono.Upnp.Tests
{
    [TestFixture]
    public class DeviceDescriptionTests
    {
        readonly XmlSerializer serializer = new XmlSerializer ();
        readonly DummyDeserializer deserializer = new DummyDeserializer ();
        
        static DummyRoot CreateRoot ()
        {
            return new DummyRoot (
                new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-full-device:1"),
                "uuid:fd1",
                "Mono.Upnp.Tests Full Device",
                "Mono Project",
                "Full Device",
                new RootDeviceOptions {
                    ManufacturerUrl = new Uri ("http://www.mono-project.org/"),
                    ModelDescription = "A device description with all optional information.",
                    ModelNumber = "1",
                    ModelUrl = new Uri ("http://www.mono-project.org/Mono.Upnp/"),
                    SerialNumber = "12345",
                    Upc = "67890",
                    Icons = new[] {
                        new DummyIcon (100, 100, 32, "image/png"),
                        new DummyIcon (100, 100, 32, "image/jpeg")
                    },
                    Services = new[] {
                        new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"), "urn:upnp-org:serviceId:testService1"),
                        new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"), "urn:upnp-org:serviceId:testService2"),
                    },
                    EmbeddedDevices = new[] {
                        new Device (
                            new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-full-embedded-device:1"),
                            "uuid:fed1",
                            "Mono.Upnp.Tests Full Embedded Device",
                            "Mono Project",
                            "Full Embedded Device",
                            new DeviceOptions {
                                ManufacturerUrl = new Uri ("http://www.mono-project.org/"),
                                ModelDescription = "An embedded device description with all optional information.",
                                ModelNumber = "1",
                                ModelUrl = new Uri ("http://www.mono-project.org/Mono.Upnp/"),
                                SerialNumber = "12345",
                                Upc = "67890",
                                Icons = new[] {
                                    new DummyIcon (100, 100, 32, "image/png"),
                                    new DummyIcon (100, 100, 32, "image/jpeg")
                                },
                                Services = new[] {
                                    new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"), "urn:upnp-org:serviceId:testService1"),
                                    new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"), "urn:upnp-org:serviceId:testService2"),
                                }
                            }
                        )
                    }
                }
            );
        }
        
        public static void AssertEquality (Root sourceRoot, Root targetRoot)
        {
            Assert.AreEqual (sourceRoot.SpecVersion.Major, targetRoot.SpecVersion.Major);
            Assert.AreEqual (sourceRoot.SpecVersion.Minor, targetRoot.SpecVersion.Minor);
            Assert.AreEqual (sourceRoot.UrlBase, targetRoot.UrlBase);
            AssertEquality (sourceRoot.RootDevice, targetRoot.RootDevice);
        }
        
        public static void AssertEquality (Device sourceDevice, Device targetDevice)
        {
            Assert.AreEqual (sourceDevice.Type, targetDevice.Type);
            Assert.AreEqual (sourceDevice.FriendlyName, targetDevice.FriendlyName);
            Assert.AreEqual (sourceDevice.Manufacturer, targetDevice.Manufacturer);
            Assert.AreEqual (sourceDevice.ManufacturerUrl, targetDevice.ManufacturerUrl);
            Assert.AreEqual (sourceDevice.ModelDescription, targetDevice.ModelDescription);
            Assert.AreEqual (sourceDevice.ModelName, targetDevice.ModelName);
            Assert.AreEqual (sourceDevice.ModelNumber, targetDevice.ModelNumber);
            Assert.AreEqual (sourceDevice.ModelUrl, targetDevice.ModelUrl);
            Assert.AreEqual (sourceDevice.SerialNumber, targetDevice.SerialNumber);
            Assert.AreEqual (sourceDevice.Udn, targetDevice.Udn);
            Assert.AreEqual (sourceDevice.Upc, targetDevice.Upc);
            var source_icons = sourceDevice.Icons.GetEnumerator ();
            var target_icons = targetDevice.Icons.GetEnumerator ();
            while (source_icons.MoveNext ()) {
                Assert.IsTrue (target_icons.MoveNext ());
                AssertEquality (source_icons.Current, target_icons.Current);
            }
            Assert.IsFalse (target_icons.MoveNext ());
            var source_services = sourceDevice.Services.GetEnumerator ();
            var target_services = targetDevice.Services.GetEnumerator ();
            while (source_services.MoveNext ()) {
                Assert.IsTrue (target_services.MoveNext ());
                AssertEquality (source_services.Current, target_services.Current);
            }
            Assert.IsFalse (target_services.MoveNext ());
            var source_devices = sourceDevice.Devices.GetEnumerator ();
            var target_devices = targetDevice.Devices.GetEnumerator ();
            while (source_devices.MoveNext ()) {
                Assert.IsTrue (target_devices.MoveNext ());
                AssertEquality (source_devices.Current, target_devices.Current);
            }
            Assert.IsFalse (target_services.MoveNext ());
        }
        
        public static void AssertEquality (Icon sourceIcon, Icon targetIcon)
        {
            Assert.AreEqual (sourceIcon.MimeType, targetIcon.MimeType);
            Assert.AreEqual (sourceIcon.Width, targetIcon.Width);
            Assert.AreEqual (sourceIcon.Height, targetIcon.Height);
            Assert.AreEqual (sourceIcon.Depth, targetIcon.Depth);
            Assert.AreEqual (sourceIcon.Url, targetIcon.Url);
        }
        
        public static void AssertEquality (Service sourceService, Service targetService)
        {
            Assert.AreEqual (sourceService.Type, targetService.Type);
            Assert.AreEqual (sourceService.Id, targetService.Id);
            Assert.AreEqual (sourceService.ScpdUrl, targetService.ScpdUrl);
            Assert.AreEqual (sourceService.ControlUrl, targetService.ControlUrl);
            Assert.AreEqual (sourceService.EventUrl, targetService.EventUrl);
        }
        
        [Test]
        public void OfflineFullDeviceDescriptionTest ()
        {
            var source_root = CreateRoot ();
            source_root.Initialize ();
            var target_root = deserializer.DeserializeRoot (serializer.GetString<Root> (source_root));
            AssertEquality (source_root, target_root);
        }
        
        [Test]
        public void FullDeviceDescriptionDeserializationTest ()
        {
            var root = deserializer.DeserializeRoot (Xml.FullDeviceDescription);
            Assert.AreEqual ("", root.ConfigurationId);
            Assert.AreEqual (1, root.SpecVersion.Major);
            Assert.AreEqual (1, root.SpecVersion.Minor);
            Assert.AreEqual (new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-full-device:1"), root.RootDevice.Type);
            Assert.AreEqual ("Mono.Upnp.Tests Full Device", root.RootDevice.FriendlyName);
            Assert.AreEqual ("Mono Project", root.RootDevice.Manufacturer);
            Assert.AreEqual (new Uri ("http://www.mono-project.org/"), root.RootDevice.ManufacturerUrl);
            Assert.AreEqual ("A device description with all optional information.", root.RootDevice.ModelDescription);
            Assert.AreEqual ("Full Device", root.RootDevice.ModelName);
            Assert.AreEqual ("1", root.RootDevice.ModelNumber);
            Assert.AreEqual (new Uri ("http://www.mono-project.org/Mono.Upnp/"), root.RootDevice.ModelUrl);
            Assert.AreEqual ("12345", root.RootDevice.SerialNumber);
            Assert.AreEqual ("uuid:fd1", root.RootDevice.Udn);
            Assert.AreEqual ("67890", root.RootDevice.Upc);
            var icons = root.RootDevice.Icons.GetEnumerator ();
            Assert.IsTrue (icons.MoveNext ());
            Assert.AreEqual ("image/png", icons.Current.MimeType);
            Assert.AreEqual (100, icons.Current.Width);
            Assert.AreEqual (100, icons.Current.Height);
            Assert.AreEqual (32, icons.Current.Depth);
            Assert.AreEqual (new Uri ("http://localhost:8080/icon/0/"), icons.Current.Url);
            Assert.IsTrue (icons.MoveNext ());
            Assert.AreEqual ("image/jpeg", icons.Current.MimeType);
            Assert.AreEqual (100, icons.Current.Width);
            Assert.AreEqual (100, icons.Current.Height);
            Assert.AreEqual (32, icons.Current.Depth);
            Assert.AreEqual (new Uri ("http://localhost:8080/icon/1/"), icons.Current.Url);
            Assert.IsFalse (icons.MoveNext ());
            var services = root.RootDevice.Services.GetEnumerator ();
            Assert.IsTrue (services.MoveNext ());
            Assert.AreEqual (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"), services.Current.Type);
            Assert.AreEqual ("urn:upnp-org:serviceId:testService1", services.Current.Id);
            Assert.AreEqual (new Uri ("http://localhost:8080/service/0/scpd/"), services.Current.ScpdUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/service/0/control/"), services.Current.ControlUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/service/0/event/"), services.Current.EventUrl);
            Assert.IsTrue (services.MoveNext ());
            Assert.AreEqual (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"), services.Current.Type);
            Assert.AreEqual ("urn:upnp-org:serviceId:testService2", services.Current.Id);
            Assert.AreEqual (new Uri ("http://localhost:8080/service/1/scpd/"), services.Current.ScpdUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/service/1/control/"), services.Current.ControlUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/service/1/event/"), services.Current.EventUrl);
            Assert.IsFalse (services.MoveNext ());
            var devices = root.RootDevice.Devices.GetEnumerator ();
            Assert.IsTrue (devices.MoveNext ());
            Assert.AreEqual (new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-full-embedded-device:1"), devices.Current.Type);
            Assert.AreEqual ("Mono.Upnp.Tests Full Embedded Device", devices.Current.FriendlyName);
            Assert.AreEqual ("Mono Project", devices.Current.Manufacturer);
            Assert.AreEqual (new Uri ("http://www.mono-project.org/"), devices.Current.ManufacturerUrl);
            Assert.AreEqual ("An embedded device description with all optional information.", devices.Current.ModelDescription);
            Assert.AreEqual ("Full Embedded Device", devices.Current.ModelName);
            Assert.AreEqual ("1", devices.Current.ModelNumber);
            Assert.AreEqual (new Uri ("http://www.mono-project.org/Mono.Upnp/"), devices.Current.ModelUrl);
            Assert.AreEqual ("12345", devices.Current.SerialNumber);
            Assert.AreEqual ("uuid:fed1", devices.Current.Udn);
            Assert.AreEqual ("67890", devices.Current.Upc);
            icons = devices.Current.Icons.GetEnumerator ();
            Assert.IsTrue (icons.MoveNext ());
            Assert.AreEqual ("image/png", icons.Current.MimeType);
            Assert.AreEqual (100, icons.Current.Width);
            Assert.AreEqual (100, icons.Current.Height);
            Assert.AreEqual (32, icons.Current.Depth);
            Assert.AreEqual (new Uri ("http://localhost:8080/device/0/icon/0/"), icons.Current.Url);
            Assert.IsTrue (icons.MoveNext ());
            Assert.AreEqual ("image/jpeg", icons.Current.MimeType);
            Assert.AreEqual (100, icons.Current.Width);
            Assert.AreEqual (100, icons.Current.Height);
            Assert.AreEqual (32, icons.Current.Depth);
            Assert.AreEqual (new Uri ("http://localhost:8080/device/0/icon/1/"), icons.Current.Url);
            Assert.IsFalse (icons.MoveNext ());
            services = devices.Current.Services.GetEnumerator ();
            Assert.IsTrue (services.MoveNext ());
            Assert.AreEqual (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"), services.Current.Type);
            Assert.AreEqual ("urn:upnp-org:serviceId:testService1", services.Current.Id);
            Assert.AreEqual (new Uri ("http://localhost:8080/device/0/service/0/scpd/"), services.Current.ScpdUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/device/0/service/0/control/"), services.Current.ControlUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/device/0/service/0/event/"), services.Current.EventUrl);
            Assert.IsTrue (services.MoveNext ());
            Assert.AreEqual (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"), services.Current.Type);
            Assert.AreEqual ("urn:upnp-org:serviceId:testService2", services.Current.Id);
            Assert.AreEqual (new Uri ("http://localhost:8080/device/0/service/1/scpd/"), services.Current.ScpdUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/device/0/service/1/control/"), services.Current.ControlUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/device/0/service/1/event/"), services.Current.EventUrl);
            Assert.IsFalse (services.MoveNext ());
            var empty_devices = devices.Current.Devices.GetEnumerator ();
            Assert.IsFalse (empty_devices.MoveNext ());
            Assert.IsFalse (devices.MoveNext ());
        }
        
        [Test]
        public void UrlBaseDeviceDescriptionDeserializationTest ()
        {
            var root = deserializer.DeserializeRoot (Xml.UrlBaseDeviceDescription);
            var icons = root.RootDevice.Icons.GetEnumerator ();
            Assert.IsTrue (icons.MoveNext ());
            Assert.AreEqual (new Uri ("http://www.mono-project.com/icon/0/"), icons.Current.Url);
            var services = root.RootDevice.Services.GetEnumerator ();
            Assert.IsTrue (services.MoveNext ());
            Assert.AreEqual (new Uri ("http://www.mono-project.com/service/0/scpd/"), services.Current.ScpdUrl);
            Assert.AreEqual (new Uri ("http://www.mono-project.com/service/0/control/"), services.Current.ControlUrl);
            Assert.AreEqual (new Uri ("http://www.mono-project.com/service/0/event/"), services.Current.EventUrl);
        }
        
        [Test]
        public void FullDescriptionSerializationTest ()
        {
            var root = CreateRoot ();
            root.Initialize ();
            Assert.AreEqual (Xml.FullDeviceDescription, serializer.GetString<Root> (root));
        }
    }
}
