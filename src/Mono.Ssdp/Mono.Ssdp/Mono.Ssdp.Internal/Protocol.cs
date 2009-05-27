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
        public const string SsdpDiscoverMan = @"""ssdp:discover""";
        
        public const ushort DefaultMaxAge = 1800;
        public const ushort DefaultMx = 3;
        public const ushort MaxMX = 120;
        
        public const ushort SocketTtl = 4;

        private static string os = String.Format ("{0}/{1}", Environment.OSVersion.Platform, Environment.OSVersion.Version);
        
        private static string discovery_request =
            "M-SEARCH * HTTP/1.1\r\n" + 
            "HOST: {0}:{1}\r\n" + 
            "MAN: \"ssdp:discover\"\r\n" +
            "ST: {2}\r\n" +
            "MX: {3}\r\n" +
            "\r\n";

        private static string alive_notify =
            "NOTIFY * HTTP/1.1\r\n" +
            "HOST: {0}:{1}\r\n" +
            "CACHE-CONTROL: max-age = {2}\r\n" +
            "LOCATION: {3}\r\n" +
            "NT: {4}\r\n" +
            "NTS: ssdp:alive\r\n" +
            "SERVER: {5} UPnP/1.1 {6}\r\n" +
            "USN: {7}\r\n" +
            "\r\n";

        private static string alive_response =
            "HTTP/1.1 200 OK\r\n" +
            "CACHE-CONTROL: max-age = {0}\r\n" +
            "DATE: {1}\r\n" +
            "EXT:\r\n" +
            "LOCATION: {2}\r\n" +
            "SERVER: {3} UPnP/1.1 {4}\r\n" +
            "ST: {5}\r\n" +
            "USN: {6}\r\n" +
            "\r\n";

        private static string byebye_notify =
            "NOTIFY * HTTP/1.1\r\n" +
            "HOST: {0}:{1}\r\n" +
            "NT: {2}\r\n" +
            "NTS: ssdp:byebye\r\n" +
            "USN: {3}\r\n" +
            "\r\n";
            
        public static byte[] CreateDiscoveryRequest (string serviceType, ushort mx)
        {
            return Encoding.ASCII.GetBytes (String.Format (discovery_request, Address, Port, serviceType, mx));
        }

        public static byte[] CreateAliveNotify (string location, string notificationType, string usn, ushort maxAge)
        {
            return Encoding.ASCII.GetBytes (String.Format (
                alive_notify, Address, Port, maxAge, location, notificationType, os, user_agent, usn));
        }

        public static byte[] CreateAliveResponse (string location, string searchType, string usn, ushort maxAge)
        {
            return Encoding.ASCII.GetBytes (String.Format (
                alive_response, maxAge, DateTime.Now.ToString ("r"), location, os, user_agent, searchType, usn));
        }

        public static byte[] CreateByeByeNotify (string notificationType, string usn)
        {
            return Encoding.ASCII.GetBytes (String.Format (byebye_notify, Address, Port, notificationType, usn));
        }
    }
}
