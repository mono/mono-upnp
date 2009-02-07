//
// ServiceAnnouncement.cs
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
using System.Collections.Generic;

namespace Mono.Upnp.Description
{
	public sealed class ServiceAnnouncement
	{
        readonly UpnpClient client;
        readonly ServiceType type;
        readonly string deviceUdn;
        readonly IEnumerable<string> locations;
        ServiceDescription description;

        internal ServiceAnnouncement (UpnpClient client, ServiceType type, string deviceUdn, IEnumerable<string> locations)
        {
            this.client = client;
            this.type = type;
            this.deviceUdn = deviceUdn;
            this.locations = locations;
        }
        
        public event EventHandler<DisposedEventArgs> Disposed;
        
        public bool IsDisposed { get; private set; }

        public ServiceType Type {
            get { return type; }
        }

        public string DeviceUdn {
            get { return deviceUdn; }
        }

        public IEnumerable<string> Locations {
            get { return locations; }
        }

        public ServiceDescription GetDescription ()
        {
            if (description == null) {
                if (IsDisposed) {
                    throw new ObjectDisposedException (ToString (), "The services has gone off the network.");
                }
                description = client.GetDescription (this);
            }
            return description;
        }

        internal void Dispose ()
        {
            if (IsDisposed) {
                return;
            }

            IsDisposed = true;
            OnDispose (DisposedEventArgs.Empty);

            if (description != null) {
                description.Dispose ();
                description = null;
            }
        }

        void OnDispose (DisposedEventArgs args)
        {
            var handler = Disposed;
            if (handler != null) {
                handler (this, args);
            }
        }

        public override bool Equals (object obj)
        {
            var announcement = obj as ServiceAnnouncement;
            return announcement != null &&
                announcement.type == type &&
                announcement.deviceUdn == deviceUdn;
        }

        public override int GetHashCode ()
        {
            return type.GetHashCode () ^ deviceUdn.GetHashCode ();
        }

        public override string ToString ()
        {
            return string.Format ("ServiceAnnouncement {{ uuid:{0}::{1} }}", deviceUdn, type);
        }
	}
}
