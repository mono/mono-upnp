//
// Client.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2008 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

using Mono.Ssdp.Internal;

namespace Mono.Ssdp
{
    public class Client : IDisposable
    {
        public static bool StrictProtocol { get; set; }

        private bool disposed;
        private readonly object mutex = new object();
        private NotifyListener notify_listener;
        private Dictionary<string, Browser> browsers;
        
        private readonly NetworkInterfaceInfo network_interface_info;
        internal NetworkInterfaceInfo NetworkInterfaceInfo {
            get { return network_interface_info; }
        }

        private readonly TimeoutDispatcher dispatcher = new TimeoutDispatcher ();
        internal TimeoutDispatcher Dispatcher {
            get { return dispatcher; }
        }
        
        private ServiceCache service_cache;
        internal ServiceCache ServiceCache {
            get { lock (service_cache) { return service_cache; } }
        }
        
        public bool Started { get; private set; }
        
        public event EventHandler<ServiceArgs> ServiceAdded;

        public event EventHandler<ServiceArgs> ServiceUpdated;

        public event EventHandler<ServiceArgs> ServiceRemoved;
        
        public Client ()
            : this (null)
        {
        }
    
        public Client (NetworkInterface networkInterface)
        {
            network_interface_info = NetworkInterfaceInfo.GetNetworkInterfaceInfo (networkInterface);
            service_cache = new ServiceCache (this);
            notify_listener = new NotifyListener (this);
            browsers = new Dictionary<string, Browser> ();
        }
        
        public void Start ()
        {
            Start (true);
        }
        
        public void Start (bool startBrowsers)
        {
            lock (mutex) {
                CheckDisposed ();

                if (Started) {
                    throw new InvalidOperationException ("The Client is already started.");
                }

                Started = true;
                notify_listener.Start ();
                if (startBrowsers) {
                    foreach (var browser in browsers.Values) {
                        if (!browser.Started) {
                            browser.Start ();
                        }
                    }
                }
            }
        }
        
        public void Stop ()
        {
            Stop (true);
        }
        
        public void Stop (bool stopBrowsers)
        {
            lock (mutex) {
                CheckDisposed ();
                Started = false;
                notify_listener.Stop ();
                if (stopBrowsers) {
                    foreach (var browser in browsers.Values) {
                        browser.Stop ();
                    }
                }
            }
        }

        public Browser BrowseAll ()
        {
            return BrowseAll (true);
        }

        public Browser BrowseAll (bool autoStart)
        {
            return Browse (Protocol.SsdpAll, autoStart);
        }
        
        public Browser Browse (string serviceType)
        {
            return Browse (serviceType, true);
        }
        
        public Browser Browse (string serviceType, bool autoStart)
        {
            lock (mutex) {
                CheckDisposed ();
            
                Browser browser;
                if (browsers.TryGetValue (serviceType, out browser)) {
                    if (!browser.Started && autoStart) {
                        browser.Start ();
                    }
                    return browser;
                }
                
                browser = new Browser (this, serviceType);
                browsers.Add (serviceType, browser);
                
                if (autoStart) {
                    browser.Start ();
                }
                
                return browser;
            }
        }
        
        internal void RemoveBrowser (Browser browser)
        {
            lock (mutex) {
                foreach (var entry in browsers) {
                    if (entry.Value == browser) {
                        browsers.Remove (entry.Key);
                        return;
                    }
                }
            }
        }
        
        internal bool ServiceTypeRegistered (string serviceType)
        {
            lock (mutex) {
                return serviceType != null && browsers != null && 
                    (browsers.ContainsKey (serviceType) || browsers.ContainsKey (Protocol.SsdpAll));
            }
        }
        
        internal void CacheServiceAdded (Service service)
        {
            OnServiceAdded (service);
        }
        
        internal void CacheServiceUpdated (Service service)
        {
            OnServiceUpdated (service);
        }
        
        internal void CacheServiceRemoved (string usn)
        {
            OnServiceRemoved (usn);
        }
                
        protected virtual void OnServiceAdded (Service service)
        {
            var handler = ServiceAdded;
            if (handler != null) {
                handler (this, new ServiceArgs (ServiceOperation.Added, service));
            }
        }
        
        protected virtual void OnServiceUpdated (Service service)
        {
            var handler = ServiceUpdated;
            if (handler != null) {
                handler (this, new ServiceArgs (ServiceOperation.Updated, service));
            }
        }
        
        protected virtual void OnServiceRemoved (string usn)
        {
            var handler = ServiceRemoved;
            if (handler != null) {
                handler (this, new ServiceArgs (usn));
            }
        }

        private void CheckDisposed ()
        {
            if (disposed) {
                throw new ObjectDisposedException ("Client has been Disposed");
            }
        }

        public void Dispose ()
        {
            lock (mutex) {
                if (disposed) {
                    return;
                }

                notify_listener.Stop ();
                foreach (var browser in browsers.Values) {
                    browser.Dispose (false);
                }

                browsers.Clear ();
                service_cache.Dispose ();

                service_cache = null;
                notify_listener = null;
                browsers = null;

                disposed = true;
            }
        }
    }
}
