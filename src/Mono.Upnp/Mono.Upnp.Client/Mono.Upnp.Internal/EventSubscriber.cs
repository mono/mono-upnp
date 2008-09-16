//
// EventSubscriber.cs
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
using System.Net;
using System.Net.Sockets;

namespace Mono.Upnp.Internal
{
	internal class EventSubscriber : IDisposable
	{
        private readonly object mutex = new object ();
        private Service service;
        private TimeoutDispatcher dispatcher = new TimeoutDispatcher();
        private bool subscribed;
        private int timeout_id;

        private string delivery_url;
        private string subscription_uuid;
        private uint event_key;

        public EventSubscriber (Service service)
        {
            this.service = service;
        }

        public void Subscribe ()
        {
            lock (mutex) {
                if (subscribed) {
                    return;
                }

                WebRequest request = WebRequest.Create (service.EventUrl);
                request.Method = "SUBSCRIBE";
                request.Headers.Add ("CALLBACK", delivery_url);
                request.Headers.Add ("NT", "upnp:event");

                subscribed = true;
            }
        }

        public void Unsubscribe ()
        {
            lock (mutex) {
                if (!subscribed) {
                    return;
                }
                subscribed = false;
            }
        }

        public void Dispose ()
        {
            Unsubscribe ();
        }
	}
}
