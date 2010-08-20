//
// Client.cs
//
// Author:
//   Scott Thomas <lunchtimemama@gmail.com>
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

using Mono.Ssdp;
using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp
{
    public class Client : IDisposable
    {
        static WeakReference static_deserializer = new WeakReference (null);
        
        readonly Dictionary<DeviceAnnouncement, DeviceAnnouncement> devices =
            new Dictionary<DeviceAnnouncement, DeviceAnnouncement> ();
        readonly Dictionary<ServiceAnnouncement, ServiceAnnouncement> services =
            new Dictionary<ServiceAnnouncement, ServiceAnnouncement> ();
        readonly Dictionary<string, Root> descriptions =
            new Dictionary<string, Root> ();

        readonly Mono.Ssdp.Client client = new Mono.Ssdp.Client ();
        DeserializerProducer deserializer_producer;

        public Client ()
            : this (null)
        {
        }
        
        public Client (DeserializerProducer deserializerProducer)
        {
            DeserializerProducer = deserializerProducer ?? (xmlDeserializer => new Deserializer (xmlDeserializer));
            client.ServiceAdded += ClientServiceAdded;
            client.ServiceRemoved += ClientServiceRemoved;
        }
        
        public DeserializerProducer DeserializerProducer {
            get { return deserializer_producer; }
            set {
                if (value == null) {
                    throw new ArgumentNullException ("value");
                }
                deserializer_producer = value;
            }
        }
        
        public event EventHandler<DeviceEventArgs> DeviceAdded;
        public event EventHandler<DeviceEventArgs> DeviceRemoved;
        public event EventHandler<ServiceEventArgs> ServiceAdded;
        public event EventHandler<ServiceEventArgs> ServiceRemoved;

        public void BrowseRoots ()
        {
            client.Browse ("upnp:rootdevice");
        }
        
        public void BrowseAll ()
        {
            client.BrowseAll ();
        }

        public void Browse (TypeInfo type)
        {
            client.Browse (type.ToString ());
        }

        void ClientServiceAdded (object sender, Mono.Ssdp.ServiceArgs args)
        {
            ClientServiceEvent (args,
                (device) => {
                    if (!devices.ContainsKey (device)) {
                        OnDeviceAdded (new DeviceEventArgs (device, UpnpOperation.Added));
                        devices.Add (device, device);
                    }
                },
                (service) => {
                    if (!services.ContainsKey (service)) {
                        OnServiceAdded (new ServiceEventArgs (service, UpnpOperation.Added));
                        services.Add (service, service);
                    }
                }
            );
        }

        void ClientServiceRemoved (object sender, Mono.Ssdp.ServiceArgs args)
        {
            ClientServiceEvent (args,
                (device) => {
                    if (devices.ContainsKey (device)) {
                        device.Dispose ();
                        OnDeviceRemoved (new DeviceEventArgs (device, UpnpOperation.Removed));
                        devices.Remove (device);
                    }
                },
                (service) => {
                    if (services.ContainsKey (service)) {
                        service.Dispose ();
                        OnServiceRemoved (new ServiceEventArgs (service, UpnpOperation.Removed));
                        services.Remove (service);
                    }
                }
            );
        }

        void ClientServiceEvent (Mono.Ssdp.ServiceArgs args,
                                 Action<DeviceAnnouncement> deviceHandler,
                                 Action<ServiceAnnouncement> serviceHandler)
        {
            if (!args.Usn.StartsWith ("uuid:")) {
                return;
            }

            var colon = args.Usn.IndexOf (':', 5);
            var usn = colon == -1 ? args.Usn : args.Usn.Substring (0, colon);

            if (args.Usn.Contains (":device:")) {
                var type = DeviceType.Parse (args.Service.ServiceType);
                var device = new DeviceAnnouncement (this, type, usn, args.Service.Locations);
                deviceHandler (device);
            } else if (args.Usn.Contains (":service:")) {
                var type = ServiceType.Parse (args.Service.ServiceType);
                var service = new ServiceAnnouncement (this, type, usn, args.Service.Locations);
                serviceHandler (service);
            }
        }
        
        protected virtual void OnDeviceAdded (DeviceEventArgs e)
        {
            OnEvent (DeviceAdded, e);
        }

        protected virtual void OnServiceAdded (ServiceEventArgs e)
        {
            OnEvent (ServiceAdded, e);
        }

        protected virtual void OnDeviceRemoved (DeviceEventArgs e)
        {
            OnEvent (DeviceRemoved, e);
        }

        protected virtual void OnServiceRemoved (ServiceEventArgs e)
        {
            OnEvent (ServiceRemoved, e);
        }
        
        void OnEvent<T> (EventHandler<T> handler, T e)
            where T : EventArgs
        {
            if (handler != null) {
                handler (this, e);
            }
        }

        internal Service GetService (ServiceAnnouncement announcement)
        {
            return GetDescription<ServiceAnnouncement, Service> (announcement.Locations, announcement, GetService);
        }

        internal Device GetDevice (DeviceAnnouncement announcement)
        {
            return GetDescription<DeviceAnnouncement, Device> (announcement.Locations, announcement, GetDevice);
        }
        
        TResult GetDescription<TAnnouncement, TResult> (IEnumerable<string> urls,
                                                        TAnnouncement announcement,
                                                        Func<TAnnouncement, Device, TResult> getter)
            where TResult : class
        {
            foreach (var url in urls) {
                Root root;
                if (descriptions.TryGetValue (url, out root)) {
                    if (!root.IsDisposed) {
                        var result = getter (announcement, root.RootDevice);
                        if (result != null) {
                            return result;
                        }
                    }
                }
                
                try {
                    var deserializer = Helper.Get<XmlDeserializer> (static_deserializer);
                    root = deserializer_producer (deserializer).DeserializeRoot (new Uri (url));
                    if (root == null) {
                        continue;
                    }
                    
                    descriptions[url] = root;
                    if (root.RootDevice == null) {
                        Log.Error (string.Format ("The description root does not have a root device: {0}.", url));
                        continue;
                    }
                    
                    var result = getter (announcement, root.RootDevice);
                    if (result == null) {
                        continue;
                    }
                    
                    return result;
                } catch (Exception e) {
                    Log.Exception (string.Format ("There was a problem fetching the description at {0}.", url), e);
                }
            }
            
            return null;
        }

        Service GetService (ServiceAnnouncement announcement, Device device)
        {
            foreach (var description in device.Services) {
                if (device.Udn == announcement.DeviceUdn && announcement.Type == description.Type) {
                    return description;
                }
            }
            
            foreach (var childDevice in device.Devices) {
                var description = GetService (announcement, childDevice);
                if (description != null) {
                    return description;
                }
            }
            
            return null;
        }

        Device GetDevice (DeviceAnnouncement announcement, Device device)
        {
            if (device.Type == announcement.Type && device.Udn == announcement.Udn) {
                return device;
            }
            
            foreach (var childDevice in device.Devices) {
                var description = GetDevice (announcement, childDevice);
                if (description != null) {
                    return description;
                }
            }
            
            return null;
        }
        
        public void Dispose ()
        {
            // TODO proper dispose pattern
            client.Dispose ();
        }
    }
}
