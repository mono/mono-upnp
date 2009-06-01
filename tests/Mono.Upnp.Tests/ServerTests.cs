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
using System.Threading;

using Mono.Ssdp;
using NUnit.Framework;

namespace Mono.Upnp.Tests
{
    [TestFixture]
    public class ServerTests
    {
        readonly object mutex = new object ();
        [Test]
        public void AnnouncementTest ()
        {
            var root = new DummyRoot (
                new DeviceSettings (
                    new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-device:1"),
                    "uuid:d1",
                    "Mono.Upnp.Tests Device",
                    "Mono Project",
                    "Device") {
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
                        Services = new Service[] {
                            new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"), "urn:upnp-org:serviceId:testService1"),
                            new DummyService (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"), "urn:upnp-org:serviceId:testService2"),
                        }
                    })
                }
            );
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
