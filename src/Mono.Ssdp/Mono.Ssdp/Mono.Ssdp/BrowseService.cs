//
// BrowseService.cs
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
    public class BrowseService : Service
    {
        private static Regex cc_max_age_regex;
        
        private BrowseService ()
        {
        }
        
        internal BrowseService (HttpDatagram dgram, bool isGena)
        {
            Update (dgram, isGena);    
        }
        
        internal void Update (HttpDatagram dgram, bool isGena)
        {
            if (isGena) {
                ServiceType = dgram.Headers.Get ("NT");
                if (Client.StrictProtocol && String.IsNullOrEmpty (dgram.Headers.Get ("Host"))) {
                    throw new ApplicationException ("Service did not send Host header");
                }
            } else {
                ServiceType = dgram.Headers.Get ("ST");
                if (Client.StrictProtocol && dgram.Headers.Get ("Ext") == null) {
                    throw new ApplicationException ("Service did not send an Ext header " + 
                        "acknowledging 'Man: \"ssdp:discover\"' in request");
                }
            }
            
            if (String.IsNullOrEmpty (ServiceType)) {
                throw new ApplicationException (String.Format ("Service did not send {0} header", 
                    isGena ? "NT" : "ST"));
            }
            
            Usn = dgram.Headers.Get ("USN");
            if (String.IsNullOrEmpty (Usn)) {
                throw new ApplicationException ("Service did not send USN header");
            }
            
            if (Client.StrictProtocol) {
                if (String.IsNullOrEmpty (dgram.Headers.Get ("Server"))) {
                    throw new ApplicationException ("Service did not send Server header");
                }
                
                if (String.IsNullOrEmpty (dgram.Headers.Get ("Server"))) {
                    throw new ApplicationException ("Service did not send Server header");
                }
            }

            ParseExpiration (dgram);
            ParseLocations (dgram);
        }
        
        private void ParseLocations (HttpDatagram dgram)
        {
            ClearLocations ();
            
            string location = dgram.Headers.Get ("Location");
            if (!String.IsNullOrEmpty (location)) {
                AddLocation (location);
            }
            
            string [] als = dgram.Headers.GetValues ("AL");
            if (als != null) {
                foreach (string al in als) {
                    AddLocation (al);
                }
            }
            
            if (LocationCount == 0) {
                throw new ApplicationException ("No Location/AL found in header");
            }
        }
        
        private void ParseExpiration (HttpDatagram dgram)
        {
            // Prefer the "Cache-Control: max-age=SECONDS" directive
            string cc_max_age = dgram.Headers.Get ("Cache-Control");
            if (!String.IsNullOrEmpty (cc_max_age)) {
                if (cc_max_age_regex == null) {
                    cc_max_age_regex = new Regex (@"max-age\s*=\s*([0-9]+)", RegexOptions.IgnoreCase);
                }
                
                Match match = cc_max_age_regex.Match (cc_max_age);
                if (match.Success && match.Groups.Count == 2) {
                    ushort expire = 0;
                    if (UInt16.TryParse (match.Groups[1].Value, out expire)) {
                        Expiration = DateTime.Now + TimeSpan.FromSeconds (expire);
                        return;
                    }
                }
            }
            
            // Fall back to possible Expires header 
            string expires = dgram.Headers.Get ("Expires");
            if (String.IsNullOrEmpty (expires)) {
                DateTime expire_date;
                if (DateTime.TryParse (expires, out expire_date)) {
                    DateTime now = DateTime.Now;
                    if (expire_date <= now || expire_date >= now.AddYears (1)) {
                        Expiration = expire_date;
                        return;
                    }
                }
            }
            
            if (Client.StrictProtocol) {
                throw new ApplicationException ("Service does not specifiy a cache expiration");
            }
            
            // Fall back to the default expiration value
            Expiration = DateTime.Now + TimeSpan.FromSeconds (Protocol.DefaultMaxAge);
        }
    }
}
