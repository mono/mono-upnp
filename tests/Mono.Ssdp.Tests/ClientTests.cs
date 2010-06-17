// 
// ClientTests.cs
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
using System.Threading;
using NUnit.Framework;

namespace Mono.Ssdp.Tests
{
    [TestFixture]
    public class ClientTests
    {
        readonly object mutex = new object ();
        
        [Test]
        public void BrowseAllAnnoucementTest ()
        {
            using (var client = new Client ()) {
                using (var server = new Server ()) {
                    client.ServiceAdded += ClientServiceAdded;
                    client.BrowseAll ();
                    lock (mutex) {
                        server.Announce ("upnp:test", "uuid:mono-upnp-tests:test", "http://localhost/");
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (30))) {
                            Assert.Fail ("The announcement timed out.");
                        }
                    }
                }
            }
        }
        
        [Test]
        public void BrowseAllSearchTest ()
        {
            using (var client = new Client ()) {
                using (var server = new Server ()) {
                    server.Announce ("upnp:test", "uuid:mono-upnp-tests:test", "http://localhost/");
                    client.ServiceAdded += ClientServiceAdded;
                    lock (mutex) {
                        client.BrowseAll ();
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (30))) {
                            Assert.Fail ("The announcement timed out.");
                        }
                    }
                }
            }
        }

        void ClientServiceAdded (object sender, ServiceArgs e)
        {
            lock (mutex) {
                Assert.AreEqual (ServiceOperation.Added, e.Operation);
                Assert.AreEqual ("upnp:test", e.Service.ServiceType);
                Assert.AreEqual ("uuid:mono-upnp-tests:test", e.Usn);
                Monitor.Pulse (mutex);
            }
        }
        
        [Test]
        public void BrowseAnnouncementInclusionTest ()
        {
            using (var client = new Client ()) {
                using (var server = new Server ()) {
                    client.ServiceAdded += ClientServiceAdded;
                    client.Browse ("upnp:test");
                    lock (mutex) {
                        server.Announce ("upnp:test", "uuid:mono-upnp-tests:test", "http://localhost/");
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (30))) {
                            Assert.Fail ("The announcement timed out.");
                        }
                    }
                }
            }
        }
        
        [Test]
        public void BrowseAnnouncementExclusionTest ()
        {
            using (var client = new Client ()) {
                using (var server = new Server ()) {
                    client.ServiceAdded += (sender, args) => {
                        lock (mutex) {
                            if (args.Service.ServiceType != "upnp:test") {
                                Monitor.Pulse (mutex);
                            }
                        }
                    };
                    client.Browse ("upnp:test");
                    lock (mutex) {
                        server.Announce ("upnp:test:fail", "uuid:mono-upnp-tests:test1", "http://localhost/");
                        server.Announce ("upnp", "uuid:mono-upnp-tests:test2", "http://localhost/");
                        server.Announce ("upnp:test", "uuid:mono-upnp-tests:test3", "http://localhost/");
                        if (Monitor.Wait (mutex, TimeSpan.FromSeconds (30))) {
                            Assert.Fail ("The client recieved invalid announcements.");
                        }
                    }
                }
            }
        }
    }
}
