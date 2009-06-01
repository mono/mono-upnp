// 
// ServerTests.cs
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
using System.Net;
using System.Threading;
using System.Xml;

using Mono.Ssdp;
using Mono.Upnp.Control;

using NUnit.Framework;

namespace Mono.Upnp.Tests
{
    [TestFixture]
    public class ServerTests
    {
        readonly object mutex = new object ();
        
        Root CreateRoot ()
        {
            return CreateRoot (null, null);
        }
        
        Root CreateRoot (IEnumerable<Icon> icons1, IEnumerable<Icon> icons2)
        {
            return new DummyRoot (
                new DeviceSettings (
                    new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-device:1"),
                    "uuid:d1",
                    "Mono.Upnp.Tests Device",
                    "Mono Project",
                    "Device") {
                    Icons = icons1,
                    Services = new Service[] {
                        new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"), "urn:upnp-org:serviceId:testService1"),
                        new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"), "urn:upnp-org:serviceId:testService2"),
                    }
                }, 
                new Device[] {
                    new Device (new DeviceSettings (
                        new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-embedded-device:1"),
                        "uuid:ed1",
                        "Mono.Upnp.Tests Embedded Device",
                        "Mono Project",
                        "Embedded Device") {
                        Icons = icons2,
                        Services = new Service[] {
                            new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"), "urn:upnp-org:serviceId:testService1"),
                            new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"), "urn:upnp-org:serviceId:testService2"),
                        }
                    })
                }
            );
        }
        
        [Test]
        public void DescriptionTest ()
        {
            var root = CreateRoot ();
            using (var server = new Server (root)) {
                server.Start ();
                var request = WebRequest.Create (root.UrlBase);
                using (var response = (HttpWebResponse)request.GetResponse ()) {
                    Assert.AreEqual (HttpStatusCode.OK, response.StatusCode);
                }
            }
        }
        
        [Test]
        public void ScpdTest ()
        {
            var root = new DummyRoot (
                new DeviceSettings (
                    new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-device:1"),
                    "uuid:d1",
                    "Mono.Upnp.Tests Device",
                    "Mono Project",
                    "Device") {
                    Services = new Service[] {
                        new Service (
                            new ServiceController (new object (), null, null),
                            new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"),
                            "urn:upnp-org:serviceId:testService1"
                        )
                    }
                }
            );
            using (var server = new Server (root)) {
                server.Start ();
                var request = WebRequest.Create (new Uri (root.UrlBase, "service/0/scpd/"));
                using (var response = (HttpWebResponse)request.GetResponse ()) {
                    Assert.AreEqual (HttpStatusCode.OK, response.StatusCode);
                }
            }
        }
            
        [Test]
        public void IconTest ()
        {
            var root = CreateRoot (
                new Icon[] {
                    new Icon (100, 100, 32, "image/jpeg", new byte[] { 0 }),
                    new Icon (100, 100, 32, "image/png", new byte[] { 1 })
                },
                new Icon[] {
                    new Icon (100, 100, 32, "image/jpeg", new byte[] { 2 }),
                    new Icon (100, 100, 32, "image/png", new byte[] { 3 })
                }
            );
            using (var server = new Server (root)) {
                server.Start ();
                var url = new Uri (root.UrlBase, "icon/");
                AssertEquality (url, 0);
                AssertEquality (url, 1);
                url = new Uri (root.UrlBase, "device/0/icon/");
                AssertEquality (url, 2);
                AssertEquality (url, 3);
            }
        }
                    
        void AssertEquality (Uri url, int iconIndex)
        {
            var request = WebRequest.Create (new Uri (url, iconIndex.ToString ()));
            using (var response = (HttpWebResponse)request.GetResponse ()) {
                Assert.AreEqual (HttpStatusCode.OK, response.StatusCode);
                using (var stream = response.GetResponseStream ()) {
                    Assert.AreNotEqual (iconIndex, stream.ReadByte ());
                    Assert.AreEqual (-1, stream.ReadByte ());
                }
            }
        }
        
        [Test]
        public void AnnouncementTest ()
        {
            var root = CreateRoot ();
            using (var server = new Server (root)) {
                using (var client = new Mono.Ssdp.Client ()) {
                    var announcements = new Dictionary<string,string> ();
                    announcements.Add ("upnp:rootdevice/uuid:d1::upnp:rootdevice", null);
                    announcements.Add ("uuid:d1/uuid:d1", null);
                    announcements.Add ("urn:schemas-upnp-org:device:mono-upnp-tests-device:1/uuid:d1::urn:schemas-upnp-org:device:mono-upnp-tests-device:1", null);
                    announcements.Add ("uuid:ed1/uuid:ed1", null);
                    announcements.Add ("urn:schemas-upnp-org:device:mono-upnp-tests-embedded-device:1/uuid:ed1::urn:schemas-upnp-org:device:mono-upnp-tests-embedded-device:1", null);
                    announcements.Add ("urn:schemas-upnp-org:service:mono-upnp-test-service:1/uuid:d1::urn:schemas-upnp-org:service:mono-upnp-test-service:1", null);
                    announcements.Add ("urn:schemas-upnp-org:service:mono-upnp-test-service:2/uuid:d1::urn:schemas-upnp-org:service:mono-upnp-test-service:2", null);
                    announcements.Add ("urn:schemas-upnp-org:service:mono-upnp-test-service:1/uuid:ed1::urn:schemas-upnp-org:service:mono-upnp-test-service:1", null);
                    announcements.Add ("urn:schemas-upnp-org:service:mono-upnp-test-service:2/uuid:ed1::urn:schemas-upnp-org:service:mono-upnp-test-service:2", null);
                    client.ServiceAdded += (obj, args) => {
                        lock (mutex) {
                            Assert.AreEqual (ServiceOperation.Added, args.Operation);
                            var announcement = string.Format ("{0}/{1}", args.Service.ServiceType, args.Service.Usn);
                            if (announcements.ContainsKey (announcement)) {
                                announcements.Remove (announcement);
                            }
                            if (announcements.Count == 0) {
                                Monitor.Pulse (mutex);
                            }
                        }
                    };
                    client.BrowseAll ();
                    lock (mutex) {
                        server.Start ();
                        if (!Monitor.Wait (mutex, new TimeSpan (0, 0, 5))) {
                            Assert.Fail ("The UPnP server announcement timed out.");
                        }
                    }
                }
            }
        }
    }
}
