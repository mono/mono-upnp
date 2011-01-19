// 
// NetworkInterfaceInfo.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Peterson
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
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Mono.Ssdp.Internal
{
    struct NetworkInterfaceInfo
    {
        public readonly IPAddress Address;
        public readonly int Index;
        
        public NetworkInterfaceInfo (IPAddress address, int index)
        {
            Address = address;
            Index = index;
        }
        
        public static NetworkInterfaceInfo GetNetworkInterfaceInfo (NetworkInterface networkInterface)
        {
            if (networkInterface == null) {
                return new NetworkInterfaceInfo (IPAddress.Any, 0);
            }
            var properties = networkInterface.GetIPProperties ();
            var ipv4_properties = properties.GetIPv4Properties ();
            if (ipv4_properties == null) {
                throw new ArgumentException ("The specified network interface does not support IPv4.", "networkInterface");
            }
            var host_name = Dns.GetHostName ();
            foreach (var address in properties.UnicastAddresses) {
                string addressHostname = null;
                try {
                    addressHostname = Dns.GetHostEntry (address.Address).HostName;
                } catch (SocketException) {
                }
                if (address.Address.AddressFamily == AddressFamily.InterNetwork && addressHostname == host_name) {
                    return new NetworkInterfaceInfo (address.Address, ipv4_properties.Index);
                }
            }
            throw new ArgumentException (string.Format (
                "The specified network interface does not have a suitable address for the local hostname: {0}.", host_name), "networkInterface");
        }
    }
}

