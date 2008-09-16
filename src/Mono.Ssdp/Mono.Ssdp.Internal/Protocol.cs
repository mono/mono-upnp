//
// Protocol.cs
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
    internal static class Protocol
    {
        private const string user_agent = "Mono.Ssdp/1.0";
    
        public const string Address = "239.255.255.250";
        public const ushort Port = 1900;
        
        public static readonly IPAddress IPAddress = IPAddress.Parse (Address);
        
        public const string SsdpSearchMethod = "M-SEARCH";
        public const string GenaNotifyMethod = "NOTIFY";
        
        public const string SsdpAliveNts = "ssdp:alive";
        public const string SsdpByeByeNts = "ssdp:byebye";
        public const string SsdpAll = "ssdp:all";
        
        public const ushort DefaultMaxAge = 1800;
        public const ushort DefaultMx = 3;
        
        public const ushort SocketTtl = 4;
        
        private static string discovery_request =
            "M-SEARCH * HTTP/1.1\r\n" + 
            "Host: {0}:{1}\r\n" + 
            "Man: \"ssdp:discover\"\r\n" +
            "ST: {2}\r\n" +
            "MX: {3}\r\n" +
            "User-Agent: " + user_agent + "\r\n" +
            "\r\n";
            
        public static byte [] CreateDiscoveryRequest (string serviceType)
        {
            return CreateDiscoveryRequest (serviceType, DefaultMx);
        }
        
        public static byte [] CreateDiscoveryRequest (string serviceType, ushort mx)
        {
            return Encoding.ASCII.GetBytes (String.Format (discovery_request, Address, Port, serviceType, mx));
        }
    }
}
