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
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

using Mono.Upnp.Control;

namespace Mono.Upnp.Tests
{
    [TestFixture]
    public class ClientTests
    {
        readonly object mutex = new object ();
        volatile bool flag;
        
        [Test]
        public void DescriptionCacheTest()
        {
            var factory = new DummyDeserializerFactory ();
            using (var server = new Server (CreateRoot ())) {
                using (var client = new Client (factory.CreateDeserializer)) {
                    client.Browse (new ServiceType ("schemas-upnp-org", "mono-upnp-test-service", new Version (1, 0)));
                    client.Browse (new ServiceType ("schemas-upnp-org", "mono-upnp-test-service", new Version (2, 0)));
                    flag = false;
                    client.ServiceAdded += (sender, args) => {
                        lock (mutex) {
                            var service = args.Service.GetService ();
                            Assert.IsNotNull (service);
                            if (flag) {
                                Monitor.Pulse (mutex);
                            } else {
                                flag = true;
                            }
                        }
                    };
                    lock (mutex) {
                        server.Start ();
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (30))) {
                            Assert.Fail ("The server announcement timed out.");
                        }
                        Assert.AreEqual (1, factory.InstantiationCount);
                    }
                }
            }
        }
        
        [Test]
        public void AnnouncementTest ()
        {
            using (var server = new Server (CreateRoot ())) {
                using (var client = new Client ()) {
                    client.DeviceAdded += AnnouncementTestClientDeviceAdded;
                    client.ServiceAdded += AnnouncementTestClientServiceAdded;
                    client.BrowseAll ();
                    lock (mutex) {
                        flag = false;
                        server.Start ();
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (30))) {
                            Assert.Fail ("The UPnP server announcement timed out.");
                        }
                    }
                }
            }
        }

        void AnnouncementTestClientServiceAdded (object sender, ServiceEventArgs e)
        {
            lock (mutex) {
                Assert.AreEqual (new ServiceType ("schemas-upnp-org", "mono-upnp-test-service", new Version (1, 0)), e.Service.Type);
                Assert.AreEqual ("uuid:d1", e.Service.DeviceUdn);
                Assert.IsTrue (e.Service.Locations.GetEnumerator ().MoveNext ());
                if (flag) {
                    Monitor.Pulse (mutex);
                } else {
                    flag = true;
                }
            }
        }

        void AnnouncementTestClientDeviceAdded (object sender, DeviceEventArgs e)
        {
            lock (mutex) {
                Assert.AreEqual (new DeviceType ("schemas-upnp-org", "mono-upnp-test-device", new Version (1, 0)), e.Device.Type);
                Assert.AreEqual ("uuid:d1", e.Device.Udn);
                Assert.IsTrue (e.Device.Locations.GetEnumerator ().MoveNext ());
                if (flag) {
                    Monitor.Pulse (mutex);
                } else {
                    flag = true;
                }
            }
        }
        
        [Test]
        public void GetDeviceTest ()
        {
            using (var server = new Server (CreateRoot ())) {
                using (var client = new Client ()) {
                    client.BrowseAll ();
                    client.DeviceAdded += (obj, args) => {
                        lock (mutex) {
                            var device = args.Device.GetDevice ();
                            Assert.AreEqual ("uuid:d1", device.Udn);
                            Monitor.Pulse (mutex);
                        }
                    };
                    lock (mutex) {
                        server.Start ();
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (30))) {
                            Assert.Fail ("The UPnP server announcement timed out.");
                        }
                    }
                }
            }
        }
        
        [Test]
        public void GetServiceTest ()
        {
            using (var server = new Server (CreateRoot ())) {
                using (var client = new Client ()) {
                    client.BrowseAll ();
                    client.ServiceAdded += (obj, args) => {
                        lock (mutex) {
                            var service = args.Service.GetService ();
                            Assert.AreEqual ("urn:upnp-org:serviceId:testService1", service.Id);
                            Monitor.Pulse (mutex);
                        }
                    };
                    lock (mutex) {
                        server.Start ();
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (30))) {
                            Assert.Fail ("The UPnP server announcement timed out.");
                        }
                    }
                }
            }
        }
        
        [Test]
        public void GetServiceControllerTest ()
        {
            using (var server = new Server (CreateRoot ())) {
                using (var client = new Client ()) {
                    client.BrowseAll ();
                    client.ServiceAdded += (obj, args) => {
                        lock (mutex) {
                            var controller = args.Service.GetService ().GetController ();
                            Assert.IsNotNull (controller);
                            Monitor.Pulse (mutex);
                        }
                    };
                    lock (mutex) {
                        server.Start ();
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (30))) {
                            Assert.Fail ("The UPnP server announcement timed out.");
                        }
                    }
                }
            }
        }
        
        class ControlTestClass
        {
            [UpnpAction]
            public string Foo (string bar)
            {
                return string.Format ("You said {0}", bar);
            }
        }
        
        [Test]
        public void ControlTest ()
        {
            var root = new Root (
                new DeviceType ("schemas-upnp-org", "mono-upnp-test-device", new Version (1, 0)),
                "uuid:d1",
                "Mono.Upnp.Tests Device",
                "Mono Project",
                "Device",
                new RootDeviceOptions {
                    Services = new[] {
                        new Service<ControlTestClass> (
                            new ServiceType ("schemas-upnp-org", "mono-upnp-test-service", new Version (1, 0)),
                            "urn:upnp-org:serviceId:testService1",
                            new ControlTestClass ()
                        )
                    }
                }
            );
            
            using (var client = new Client ()) {
                client.Browse (new ServiceType ("schemas-upnp-org", "mono-upnp-test-service", new Version (1, 0)));
                Exception exception = null;
                client.ServiceAdded += (sender, args) => {
                    lock (mutex) {
                        try {
                            var controller = args.Service.GetService ().GetController ();
                            var arguments = new Dictionary<string, string> (1);
                            arguments["bar"] = "hello world!";
                            var results = controller.Actions["Foo"].Invoke (arguments);
                            Assert.AreEqual ("You said hello world!", results["result"]);
                        } catch (Exception e) {
                            exception = e;
                        } finally {
                            Monitor.Pulse (mutex);
                        }
                    }
                };
                
                using (var server = new Server (root)) {
                    lock (mutex) {
                        server.Start ();
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (30))) {
                            Assert.Fail ("The server control timed out.");
                        } else if (exception != null) {
                            throw exception;
                        }
                    }
                }
            }
        }
        
        class EventTestClass
        {
            [UpnpStateVariable]
            public event EventHandler<StateVariableChangedArgs<string>> FooChanged;
            
            string foo;
            public string Foo {
                get {
                    return foo;
                }
                set {
                    foo = value;
                    if (FooChanged != null) {
                        FooChanged (this, new StateVariableChangedArgs<string> (value));
                    }
                }
            }
        }
        
        class EventTestHelperClass
        {
            readonly EventTestClass service;
            readonly object mutex;
            
            public Exception Exception;
            
            public EventTestHelperClass (EventTestClass service, object mutex)
            {
                this.service = service;
                this.mutex = mutex;
            }
        
            public void FirstEventHandler (object sender, StateVariableChangedArgs<string> args)
            {
                try {
                    Assert.AreEqual ("Hello World!", args.NewValue);
                    var state_variable = (StateVariable)sender;
                    state_variable.ValueChanged -= FirstEventHandler;
                    state_variable.ValueChanged += SecondEventHandler;
                    service.Foo = "Hello Universe!";
                } catch (Exception e) {
                    Exception = e;
                    lock (mutex) {
                        Monitor.Pulse (mutex);
                    }
                }
            }
            
            public void SecondEventHandler (object sender, StateVariableChangedArgs<string> args)
            {
                try {
                    Assert.AreEqual ("Hello Universe!", args.NewValue);
                } catch (Exception e) {
                    Exception = e;
                }
                lock (mutex) {
                    Monitor.Pulse (mutex);
                }
            }
        }
        
        [Test]
        public void EventTest ()
        {
            var service = new EventTestClass ();
            var root = new Root (
                new DeviceType ("schemas-upnp-org", "mono-upnp-test-device", new Version (1, 0)),
                "uuid:d1",
                "Mono.Upnp.Tests Device",
                "Mono Project",
                "Device",
                new RootDeviceOptions {
                    Services = new[] {
                        new Service<EventTestClass> (
                            new ServiceType ("schemas-upnp-org", "mono-upnp-test-service", new Version (1, 0)),
                            "urn:upnp-org:serviceId:testService1",
                            service
                        )
                    }
                }
            );
            var helper = new EventTestHelperClass (service, mutex);
            using (var server = new Server (root)) {
                using (var client = new Client ()) {
                    client.ServiceAdded += (sender, args) => {
                        try {
                            var controller = args.Service.GetService ().GetController ();
                            controller.StateVariables["FooChanged"].ValueChanged += helper.FirstEventHandler;
                        } catch (Exception e) {
                            helper.Exception = e;
                            lock (mutex) {
                                Monitor.Pulse (mutex);
                            }
                        }
                    };
                    client.Browse (new ServiceType ("schemas-upnp-org", "mono-upnp-test-service", new Version (1, 0)));
                    lock (mutex) {
                        server.Start ();
                        service.Foo = "Hello World!";
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (30))) {
                            Assert.Fail ("The event timed out.");
                        }
                    }
                }
            }
            if (helper.Exception != null) {
                throw helper.Exception;
            }
        }
        
        static Root CreateRoot ()
        {
            return new DummyRoot (
                new DeviceType ("schemas-upnp-org", "mono-upnp-test-device", new Version (1, 0)),
                "uuid:d1",
                "Mono.Upnp.Tests Device",
                "Mono Project",
                "Device",
                new RootDeviceOptions {
                    Services = new[] {
                        new DummyService (
                            new ServiceType ("schemas-upnp-org", "mono-upnp-test-service", new Version (1, 0)),
                            "urn:upnp-org:serviceId:testService1"
                        ),
                        new DummyService (
                            new ServiceType ("schemas-upnp-org", "mono-upnp-test-service", new Version (2, 0)),
                            "urn:upnp-org:serviceId:testService2"
                        )
                    }
                }
            );
        }
    }
}
