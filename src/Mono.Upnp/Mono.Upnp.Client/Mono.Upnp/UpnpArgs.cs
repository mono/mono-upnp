//
// UpnpArgs.cs
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

using System;

namespace Mono.Upnp
{
    public enum UpnpOperation
    {
        Added,
        Removed
    }

    public class DeviceArgs : DeviceArgs<Device>
    {
        public DeviceArgs (Device device, UpnpOperation operation)
            : base (device, operation)
        {
        }
    }

	public class DeviceArgs<T> : EventArgs where T : Device
	{
        private readonly T device;
        private readonly UpnpOperation operation;

        public DeviceArgs (T device, UpnpOperation operation)
        {
            this.device = device;
            this.operation = operation;
        }

        public T Device {
            get { return device; }
        }

        public UpnpOperation Operation {
            get { return operation; }
        }
	}

    public class ServiceArgs : ServiceArgs<Service>
    {
        public ServiceArgs (Service service, UpnpOperation operation)
            : base (service, operation)
        {
        }
    }

    public class ServiceArgs<T> : EventArgs where T : Service
    {
        private readonly T service;
        private readonly UpnpOperation operation;

        public ServiceArgs (T service, UpnpOperation operation)
        {
            this.service = service;
            this.operation = operation;
        }

        public T Service {
            get { return service; }
        }

        public UpnpOperation Operation {
            get { return operation; }
        }
    }
}
