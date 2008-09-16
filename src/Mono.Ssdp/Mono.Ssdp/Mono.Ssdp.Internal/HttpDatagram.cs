//
// HttpDatagram.cs
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
using System.Net;
using System.Text;

namespace Mono.Ssdp.Internal
{   
    internal class HttpDatagram
    {
        private HttpDatagramType type;
        private WebHeaderCollection headers;
        
        public HttpDatagram (HttpDatagramType type)
        {
            this.type = type;
        }
        
        public HttpDatagramType Type {
            get { return type; }
            set { type = value; }
        }
        
        public WebHeaderCollection Headers {
            get { return headers; }
        }

#region Static Datagram Parser
    
        private static byte [] fingerprint_discover_response = Encoding.ASCII.GetBytes ("HTTP/1.1 200 OK\r\n");
        private static byte [] fingerprint_msearch = Encoding.ASCII.GetBytes ("M-SEARCH * HTTP/1.1\r\n");
        private static byte [] fingerprint_notify = Encoding.ASCII.GetBytes ("NOTIFY * HTTP/1.1\r\n");
    
        public static HttpDatagram Parse (byte [] rawDgram)
        {
            if (rawDgram.Length <= 0) {
                return null;
            }
            
            int headers_offset = 0;
            HttpDatagramType type = DetectDatagramType (rawDgram, out headers_offset);
            if (type == HttpDatagramType.Unknown) {
                return null;
            }
            
            HttpDatagram dgram = new HttpDatagram (type);
            if (ParseHeaders (dgram, rawDgram, headers_offset)) {
                return dgram;
            }
            
            return null;
        }
        
        private static HttpDatagramType DetectDatagramType (byte [] rawDgram, out int offset)
        {
            switch (ToUpper (rawDgram[0])) {
                case (byte)'H':
                    if (Check (rawDgram, fingerprint_discover_response, out offset)) {
                        return HttpDatagramType.DiscoverResponse;
                    }
                    break;
                case (byte)'M':
                    if (Check (rawDgram, fingerprint_msearch, out offset)) {
                        return HttpDatagramType.MSearch;
                    }
                    break;
                case (byte)'N':
                    if (Check (rawDgram, fingerprint_notify, out offset)) {
                        return HttpDatagramType.Notify;
                    }
                    break;
            }
            
            offset = -1;
            return HttpDatagramType.Unknown;
        }
        
        private static bool ParseHeaders (HttpDatagram dgram, byte [] raw, int offset)
        {
            for (int i = offset, line_start = offset, sep_start = -1, n = raw.Length - 1; i < n; i++) {
                if (sep_start < 0 && raw[i] == ':') {
                    // Account for the first : to denote the kvp split
                    sep_start = i;
                } else if (raw[i] == '\r' && raw[i + 1] == '\n') {
                    // Process on the line boundary
                    int line_length = i - line_start - 1;
                    int sep_length = sep_start - line_start;
                    
                    if (line_length > 0 && sep_length > 0) {
                        // Encode the kvp
                        string key = Encoding.ASCII.GetString (raw, line_start, sep_length);
                        string value = Encoding.ASCII.GetString (raw, line_start + sep_length + 1, 
                            line_length - sep_length);
                        
                        // Save the header
                        if (!String.IsNullOrEmpty (key)) {
                            if (dgram.headers == null) {
                                dgram.headers = new WebHeaderCollection ();
                            }
                            
                            dgram.headers.Add (key.Trim (), value.Trim ());
                        }
                    }
                    
                    // Set state for next line
                    line_start = i + 2;
                    sep_start = -1;
                }
            }
            
            return dgram.headers != null;
        }
        
        private static bool Check (byte [] check, byte [] fingerprint, out int offset)
        {
            offset = -1;
            
            if (check.Length < fingerprint.Length) {
                return false;
            }
            
            for (int i = 0; i < fingerprint.Length; i++) {
                if (ToUpper (check[i]) != fingerprint[i]) {
                    return false;
                }
            }
            
            offset = fingerprint.Length;
            return true;
        }
        
        private static byte ToUpper (byte b)
        {
            return (b >= 'a' && b <= 'z') ? (byte)(b - 0x20) : b;
        }

#endregion

    }
}
