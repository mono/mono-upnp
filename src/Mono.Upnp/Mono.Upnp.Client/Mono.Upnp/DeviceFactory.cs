//
// DeviceFactory.cs
//
// Author:
//   Scott Peterson <lunchtimemama@gmail.com>
//
// Copyright (C) 2008 S&S Black Ltd.
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

using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Mono.Upnp
{
	public abstract class DeviceFactory
	{
        private bool verify;

        protected DeviceFactory ()
            : this (true)
        {
        }

        protected DeviceFactory (bool verify)
        {
            this.verify = verify;
        }

        public bool Verify {
            get { return verify; }
            set { verify = value; }
        }

        public abstract DeviceType Type { get; }
        protected abstract Device CreateDeviceCore (Client client, string udn, IEnumerable<string> locations);
        protected abstract Device CreateDeviceCore (DeviceFactory factory, Client client, Root root, XmlReader reader, WebHeaderCollection headers);
        public abstract IEnumerable<ServiceFactory> Services { get; }
        public abstract IEnumerable<DeviceFactory> Devices { get; }

        internal Device CreateDevice (Client client, string udn, IEnumerable<string> locations)
        {
            Device device = CreateDeviceCore (client, udn, locations);
            if (verify) {
                device.VerifyContract ();
            }
            return device;
        }

        internal Device CreateDevice (Client client, Root root, XmlReader reader, WebHeaderCollection headers)
        {
            Device device = CreateDeviceCore (this, client, root, reader, headers);
            if (verify) {
                device.VerifyContract ();
            }
            return device;
        }
	}
}
