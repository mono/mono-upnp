//
// Service.cs
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
using System.Text.RegularExpressions;

using Mono.Ssdp.Internal;

namespace Mono.Ssdp
{
    public abstract class Service
    {
        private List<string> locations = new List<string> ();
        
        private string usn;
        private string service_type;
        private DateTime expiration;
        private uint timeout_id;
        
        internal Service ()
        {
        }
        
        public void AddLocation (string location)
        {
            lock (locations) {
                locations.Add (location);
            }
        }
        
        public void RemoveLocation (string location)
        {
            lock (locations) {
                locations.Remove (location);
            }
        }
        
        public void ClearLocations ()
        {
            lock (locations) {
                locations.Clear ();
            }
        }
        
        public string GetLocation (int index)
        {
            lock (locations) {
                return locations[index];
            }
        }
        
        public int LocationCount {
            get { lock (locations) { return locations.Count; } }
        }

        public IEnumerable<string> Locations {
            get {
                lock (locations) {
                    foreach (string location in locations) {
                        yield return location;
                    }
                }
            }
        }
        
        public override string ToString ()
        {
            return Usn;
        }
        
        public string Usn {
            get { return usn; }
            set { usn = value; }
        }

        public string ServiceType {
            get { return service_type; }
            set { service_type = value; }
        }
        
        public DateTime Expiration {
            get { return expiration; }
            set { expiration = value; }
        }
        
        internal uint TimeoutId {
            get { return timeout_id; }
            set { timeout_id = value; }
        }
    }
}
