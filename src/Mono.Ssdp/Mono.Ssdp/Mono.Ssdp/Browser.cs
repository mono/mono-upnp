//
// Browser.cs
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

using Mono.Ssdp.Internal;

namespace Mono.Ssdp
{
    public class Browser : MulticastReader, IDisposable
    {
        private SsdpSocket socket;
        private bool disposed;
        private readonly object mutex = new object ();
        private uint timout_id;
        
        private Client client;
        public Client Client {
            get { return client; }
        }

        private ushort mx = Protocol.DefaultMx;
        [CLSCompliantAttribute (false)]
        public ushort MX {
            get { return mx; }
            set { mx = System.Math.Min (value, Protocol.MaxMX); }
        }
        
        private string service_type;
        public string ServiceType {
            get { lock (mutex) { return service_type; } }
            set { lock (mutex) { service_type = value; } }
        }
        
        public bool Started {
            get { return socket != null; }
        }

        private bool auto_stop = false;
        public bool AutoStop {
            get { return auto_stop; }
            set { auto_stop = value; }
        }
        
        internal Browser (Client client, string serviceType)
        {
            this.client = client;
            this.service_type = serviceType;
        }
        
        public void Dispose ()
        {
            Dispose (true);
        }
        
        internal void Dispose (bool removeFromClient)
        {
            Stop ();
            if (removeFromClient) {
                client.RemoveBrowser (this);
            }
            client = null;
            disposed = true;
        }
        
        private void CheckDisposed ()
        {
            if (disposed) {
                throw new ObjectDisposedException ("Browser has been Disposed");
            }
        }
                         
        public void Start ()
        {
            lock (mutex) {
                CheckDisposed ();

                if (Started) {
                    throw new InvalidOperationException ("A browse operation is already running. Cancel it first.");
                } else if (String.IsNullOrEmpty (service_type)) {
                    throw new ArgumentNullException ("ServiceType");
                }
                
                socket = new MulticastSsdpSocket (client.NetworkInterfaceInfo);
                socket.BeginSendTo (Protocol.CreateDiscoveryRequest (service_type, MX), OnBrowseRequestFinished);

                // wait for 4 times MX
                if (AutoStop) {
                    timout_id = client.Dispatcher.Add (TimeSpan.FromSeconds (MX * 3), OnTimeout);
                }
                
                if (!client.Started) {
                    client.Start (false);
                }
            }
        }

        private bool OnTimeout (object state, ref TimeSpan interval)
        {
            lock (mutex) {
                timout_id = 0;
                Stop ();
                return false;
            }
        }
        
        public void Stop ()
        {
            lock (mutex) {
                CheckDisposed ();

                if (timout_id != 0) {
                    client.Dispatcher.Remove (timout_id);
                    timout_id = 0;
                }
                
                if (socket != null) {
                    socket.Close ();
                    socket = null;
                }
            }
        }
        
        internal override bool OnAsyncResultReceived (AsyncReceiveBuffer result)
        {
            try {
                HttpDatagram dgram = HttpDatagram.Parse (result.Buffer);
                if (dgram == null) {
                    return true;
                }
                
                try {
                    client.ServiceCache.Add (new BrowseService (dgram, false));
                } catch (Exception e) {
                    Log.Exception ("Invalid browse response", e);
                }
            } catch (Exception e) {
                Log.Exception ("Invalid HTTPU datagram", e);
            }
            
            return true;
        }
        
        private void OnBrowseRequestFinished (IAsyncResult asyncResult)
        {
            SsdpSocket socket = (SsdpSocket)asyncResult.AsyncState;
            socket.EndSendTo (asyncResult);
            
            AsyncReadResult (socket);
        }
    }
}
