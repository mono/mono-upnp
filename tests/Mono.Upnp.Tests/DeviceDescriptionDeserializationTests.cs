// 
// DeviceDescriptionDeserializationTests.cs
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
    public class DeviceDescriptionDeserializationTests
    {
        readonly DummyDeserializer deserializer = new DummyDeserializer (new XmlDeserializer ());
        
        [Test]
        public void FullDescriptionTest ()
        {
            var root = deserializer.DeserializeRoot (DeviceDescriptions.FullDescription);
            Assert.AreEqual ("1", root.ConfigurationId);
            Assert.IsNotNull (root.SpecVersion);
            Assert.AreEqual (1, root.SpecVersion.Major);
            Assert.AreEqual (1, root.SpecVersion.Minor);
            Assert.IsNotNull (root.RootDevice);
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
            Assert.IsNotNull (root.RootDevice.Icons);
            var icons = root.RootDevice.Icons.GetEnumerator ();
            Assert.IsTrue (icons.MoveNext ());
            Assert.AreEqual ("image/png", icons.Current.MimeType);
            Assert.AreEqual (100, icons.Current.Width);
            Assert.AreEqual (100, icons.Current.Height);
            Assert.AreEqual (32, icons.Current.Depth);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/icons/0/"), icons.Current.Url);
            Assert.IsTrue (icons.MoveNext ());
            Assert.AreEqual ("image/jpeg", icons.Current.MimeType);
            Assert.AreEqual (100, icons.Current.Width);
            Assert.AreEqual (100, icons.Current.Height);
            Assert.AreEqual (32, icons.Current.Depth);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/icons/1/"), icons.Current.Url);
            Assert.IsFalse (icons.MoveNext ());
            Assert.IsNotNull (root.RootDevice.Services);
            var services = root.RootDevice.Services.GetEnumerator ();
            Assert.IsTrue (services.MoveNext ());
            Assert.AreEqual (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"), services.Current.Type);
            Assert.AreEqual ("urn:upnp-org:serviceId:testService1", services.Current.Id);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-test-service/1/testService1/scpd/"), services.Current.ScpdUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-test-service/1/testService1/control/"), services.Current.ControlUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-test-service/1/testService1/event/"), services.Current.EventUrl);
            Assert.IsTrue (services.MoveNext ());
            Assert.AreEqual (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"), services.Current.Type);
            Assert.AreEqual ("urn:upnp-org:serviceId:testService2", services.Current.Id);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-test-service/2/testService2/scpd/"), services.Current.ScpdUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-test-service/2/testService2/control/"), services.Current.ControlUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-test-service/2/testService2/event/"), services.Current.EventUrl);
            Assert.IsFalse (services.MoveNext ());
            Assert.IsNotNull (root.RootDevice.Devices);
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
            Assert.IsNotNull (devices.Current.Icons);
            icons = devices.Current.Icons.GetEnumerator ();
            Assert.IsTrue (icons.MoveNext ());
            Assert.AreEqual ("image/png", icons.Current.MimeType);
            Assert.AreEqual (100, icons.Current.Width);
            Assert.AreEqual (100, icons.Current.Height);
            Assert.AreEqual (32, icons.Current.Depth);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-tests-full-embedded-device/1/fed1/icons/0/"), icons.Current.Url);
            Assert.IsTrue (icons.MoveNext ());
            Assert.AreEqual ("image/jpeg", icons.Current.MimeType);
            Assert.AreEqual (100, icons.Current.Width);
            Assert.AreEqual (100, icons.Current.Height);
            Assert.AreEqual (32, icons.Current.Depth);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-tests-full-embedded-device/1/fed1/icons/1/"), icons.Current.Url);
            Assert.IsFalse (icons.MoveNext ());
            Assert.IsNotNull (devices.Current.Services);
            services = devices.Current.Services.GetEnumerator ();
            Assert.IsTrue (services.MoveNext ());
            Assert.AreEqual (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"), services.Current.Type);
            Assert.AreEqual ("urn:upnp-org:serviceId:testService1", services.Current.Id);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-tests-full-embedded-device/1/fed1/schemas-upnp-org/mono-upnp-test-service/1/testService1/scpd/"), services.Current.ScpdUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-tests-full-embedded-device/1/fed1/schemas-upnp-org/mono-upnp-test-service/1/testService1/control/"), services.Current.ControlUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-tests-full-embedded-device/1/fed1/schemas-upnp-org/mono-upnp-test-service/1/testService1/event/"), services.Current.EventUrl);
            Assert.IsTrue (services.MoveNext ());
            Assert.AreEqual (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"), services.Current.Type);
            Assert.AreEqual ("urn:upnp-org:serviceId:testService2", services.Current.Id);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-tests-full-embedded-device/1/fed1/schemas-upnp-org/mono-upnp-test-service/2/testService2/scpd/"), services.Current.ScpdUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-tests-full-embedded-device/1/fed1/schemas-upnp-org/mono-upnp-test-service/2/testService2/control/"), services.Current.ControlUrl);
            Assert.AreEqual (new Uri ("http://localhost:8080/schemas-upnp-org/mono-upnp-tests-full-device/1/fd1/schemas-upnp-org/mono-upnp-tests-full-embedded-device/1/fed1/schemas-upnp-org/mono-upnp-test-service/2/testService2/event/"), services.Current.EventUrl);
            Assert.IsFalse (services.MoveNext ());
            Assert.IsNotNull (devices.Current.Devices);
            var empty_devices = devices.Current.Devices.GetEnumerator ();
            Assert.IsFalse (empty_devices.MoveNext ());
            Assert.IsFalse (devices.MoveNext ());
        }
    }
}
