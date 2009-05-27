//
// ServiceCache.cs
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

namespace Mono.Ssdp.Internal
{
    internal class ServiceCache : IDisposable
    {
        private readonly object mutex = new object ();
        private Client client;
        private TimeoutDispatcher timeouts = new TimeoutDispatcher ();
    
        private Dictionary<string, BrowseService> services;
        private Dictionary<string, BrowseService> Services {
            get {
                if (services == null) {
                    services = new Dictionary<string, BrowseService> ();
                }
                
                return services;
            }
        }
        
        public ServiceCache (Client client)
        {
            this.client = client;
        }
        
        public void Dispose ()
        {
            lock (mutex) {
                if (services != null) {
                    services.Clear ();
                    services = null;
                }
            }
        }

        public void Remove (Service service)
        {
            Remove (service.Usn);
        }
        
        public void Remove (string usn)
        {
            Remove (usn, true);
        }
        
        private void Remove (string usn, bool fromTimeout)
        {
            lock (mutex) {
                BrowseService service;
                if (services.TryGetValue (usn, out service)) {
                    Services.Remove (usn);
                    client.CacheServiceRemoved (usn);
                    timeouts.Remove (service.TimeoutId);
                }
            }
        }
        
        public void Add (BrowseService service)
        {
            lock (mutex) {
                Services.Add (service.Usn, service);
                client.CacheServiceAdded (service);
                service.TimeoutId = timeouts.Add (service.Expiration, TimeoutHandler, service);
            }
        }
        
        public bool Update (string usn, HttpDatagram dgram)
        {
            lock (mutex) {
                BrowseService service;
                if (!Services.TryGetValue (usn, out service)) {
                    return false;
                }
                
                timeouts.Remove (service.TimeoutId);
                service.Update (dgram, true);
                service.TimeoutId = timeouts.Add (service.Expiration, TimeoutHandler, service);
                
                client.CacheServiceUpdated (service);
                return true;
            }
        }
        
        private bool TimeoutHandler (object state, ref TimeSpan interval)
        {
            Remove (((Service)state).Usn, false);
            return false;
        }
        
        public BrowseService this [string usn] {
            get {
                lock (mutex) {
                    BrowseService service;
                    return Services.TryGetValue (usn, out service) ? service : null;
                }
            }
        }
    }
}
