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

using Mono.Ssdp.Internal;

namespace Mono.Ssdp
{
    public class Client : IDisposable
    {
        private static bool strict_protocol = true;
        public static bool StrictProtocol {
            get { return strict_protocol; }
            set { strict_protocol = value; }
        }
        
        private bool disposed;
        private NotifyListener notify_listener;
        private Dictionary<string, Browser> browsers;
        
        private ServiceCache service_cache;
        internal ServiceCache ServiceCache {
            get { lock (service_cache) { return service_cache; } }
        }
        
        private bool started;
        public bool Started {
            get { return started; }
        }
        
        private event ServiceHandler service_added;
        public event ServiceHandler ServiceAdded {
            add { service_added += value; }
            remove { service_added -= value; }
        }
        
        private event ServiceHandler service_updated;
        public event ServiceHandler ServiceUpdated {
            add { service_updated += value; }
            remove { service_updated -= value; }
        }
        
        private event ServiceHandler service_removed;
        public event ServiceHandler ServiceRemoved {
            add { service_removed += value; }
            remove { service_removed -= value; }
        }
    
        public Client ()
        {
            service_cache = new ServiceCache (this);
            notify_listener = new NotifyListener (this);
            browsers = new Dictionary<string, Browser> ();
        }
        
        public void Dispose ()
        {
            lock (this) {
                if (disposed) {
                    return;
                }
                
                notify_listener.Stop ();
                foreach (Browser browser in browsers.Values) {
                    browser.Dispose ();
                }
                
                browsers.Clear ();
                service_cache.Dispose ();
                
                service_cache = null;
                notify_listener = null;
                browsers = null;
                
                disposed = true;
            }
        }
        
        private void CheckDisposed ()
        {
            if (disposed) {
                throw new InvalidOperationException ("Client has been Disposed");
            }
        }
        
        public void Start ()
        {
            Start (true);
        }
        
        public void Start (bool startBrowsers)
        {
            lock (this) {
                CheckDisposed ();
                started = true;
                notify_listener.Start ();
                if (startBrowsers) {
                    foreach (Browser browser in browsers.Values) {
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
            lock (this) {
                CheckDisposed ();
                started = false;
                notify_listener.Stop ();
                if (stopBrowsers) {
                    foreach (Browser browser in browsers.Values) {
                        browser.Stop ();
                    }
                }
            }
        }
        
        public Browser Browse (string serviceType)
        {
            return Browse (serviceType, true);
        }
        
        public Browser Browse (string serviceType, bool autoStart)
        {
            lock (this) {
                CheckDisposed ();
            
                Browser browser;
                if (browsers.TryGetValue (serviceType, out browser)) {
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
            lock (this) {
                foreach (KeyValuePair<string, Browser> entry in browsers) {
                    if (entry.Value == browser) {
                        browsers.Remove (entry.Key);
                        return;
                    }
                }
            }
        }
        
        internal bool ServiceTypeRegistered (string serviceType)
        {
            lock (this) {
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
            ServiceHandler handler = service_added;
            if (handler != null) {
                handler (this, new ServiceArgs (ServiceOperation.Added, service));
            }
        }
        
        protected virtual void OnServiceUpdated (Service service)
        {
            ServiceHandler handler = service_updated;
            if (handler != null) {
                handler (this, new ServiceArgs (ServiceOperation.Updated, service));
            }
        }
        
        protected virtual void OnServiceRemoved (string usn)
        {
            ServiceHandler handler = service_removed;
            if (handler != null) {
                handler (this, new ServiceArgs (usn));
            }
        }
    }
}
