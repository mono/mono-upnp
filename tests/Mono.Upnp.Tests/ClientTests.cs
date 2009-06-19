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
using Mono.Upnp.Control.Tests;

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
                using (var client = new Client (factory)) {
                    client.Browse (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"));
                    client.Browse (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"));
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
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (5))) {
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
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (5))) {
                            Assert.Fail ("The UPnP server announcement timed out.");
                        }
                    }
                }
            }
        }

        void AnnouncementTestClientServiceAdded (object sender, ServiceEventArgs e)
        {
            lock (mutex) {
                Assert.AreEqual (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"), e.Service.Type);
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
                Assert.AreEqual (new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-device:1"), e.Device.Type);
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
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (5))) {
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
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (5))) {
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
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (5))) {
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
                new DeviceSettings (
                    new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-device:1"),
                    "uuid:d1",
                    "Mono.Upnp.Tests Device",
                    "Mono Project",
                    "Device") {
                    Services = new Service [] {
                        new Service<ControlTestClass> (
                            new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"),
                            "urn:upnp-org:serviceId:testService1",
                            new ControlTestClass ()
                        )
                    }
                }
            );
            
            using (var client = new Client ()) {
                client.Browse (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"));
                client.ServiceAdded += (sender, args) => {
                    lock (mutex) {
                        var controller = args.Service.GetService ().GetController ();
                        var arguments = new Dictionary<string, string> (1);
                        arguments["bar"] = "hello world!";
                        var results = controller.Actions["Foo"].Invoke (arguments);
                        Assert.AreEqual ("You said hello world!", results["result"]);
                        Monitor.Pulse (mutex);
                    }
                };
                
                using (var server = new Server (root)) {
                    lock (mutex) {
                        server.Start ();
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (5))) {
                            Assert.Fail ("The server control timed out.");
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
            
            public EventTestHelperClass (EventTestClass service, object mutex)
            {
                this.service = service;
                this.mutex = mutex;
            }
        
            public void FirstEventHandler (object sender, StateVariableChangedArgs<string> args)
            {
                Assert.AreEqual ("Hello World!", args.NewValue);
                var state_variable = (StateVariable)sender;
                state_variable.ValueChanged -= FirstEventHandler;
                state_variable.ValueChanged += SecondEventHandler;
                service.Foo = "Hello Universe!";
            }
            
            public void SecondEventHandler (object sender, StateVariableChangedArgs<string> args)
            {
                Assert.AreEqual ("Hello Universe!", args.NewValue);
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
                new DeviceSettings (
                    new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-device:1"),
                    "uuid:d1",
                    "Mono.Upnp.Tests Device",
                    "Mono Project",
                    "Device") {
                    Services = new Service [] {
                        new Service<EventTestClass> (
                            new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"),
                            "urn:upnp-org:serviceId:testService1",
                            service
                        )
                    }
                }
            );
            using (var server = new Server (root)) {
                service.Foo = "Hello World!";
                using (var client = new Client ()) {
                    client.ServiceAdded += (sender, args) => {
                        var controller = args.Service.GetService ().GetController ();
                        var helper = new EventTestHelperClass (service, mutex);
                        controller.StateVariables["FooChanged"].ValueChanged += helper.FirstEventHandler;
                    };
                    client.Browse (new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"));
                    lock (mutex) {
                        server.Start ();
                        if (!Monitor.Wait (mutex, TimeSpan.FromSeconds (10))) {
                            Assert.Fail ("The event timed out.");
                        }
                    }
                }
            }
        }
        
        static Root CreateRoot ()
        {
            return new DummyRoot (
                new DeviceSettings (
                    new DeviceType ("urn:schemas-upnp-org:device:mono-upnp-tests-device:1"),
                    "uuid:d1",
                    "Mono.Upnp.Tests Device",
                    "Mono Project",
                    "Device") {
                    Services = new Service[] {
                        new DummyService (
                            new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:1"),
                            "urn:upnp-org:serviceId:testService1"
                        ),
                        new DummyService (
                            new ServiceType ("urn:schemas-upnp-org:service:mono-upnp-test-service:2"),
                            "urn:upnp-org:serviceId:testService2"
                        )
                    }
                }
            );
        }
    }
}
