// ContentDirectory1.cs auto-generated at 1/17/2009 8:08:36 PM by Sharpener

using System;

using Mono.Upnp.Dcp.MediaServer1;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public class ContentDirectoryClient : IDisposable
    {
        readonly Client client;

        public event EventHandler<DiscoveryEventArgs<LocalContentDirectory>> ContentDirectoryAdded;

        public ContentDirectoryClient ()
            : this (null)
        {
        }

        public ContentDirectoryClient (Client client)
        {
            this.client = client ?? new Client ();
            this.client.ServiceAdded += ClientServiceAdded;
        }

        public Client Client {
            get { return client; }
        }

        void ClientServiceAdded (object sender, ServiceEventArgs args)
        {
            if (args.Service.Type != ContentDirectory.ServiceType) return;

            try {
                var description = args.Service.GetService ();
                if (description != null) {
                    var controller = description.GetController ();
                    if (controller != null) {
                        //var service = new Deserializer (new ContentDirectoryController (controller));
                        //OnContentDirectoryAdded (new DiscoveryEventArgs<Deserializer> (service));
                    }
                }
            } catch {
            }
        }

        public void Browse ()
        {
            client.Browse (ContentDirectory.ServiceType);
        }

        /*void OnContentDirectoryAdded (DiscoveryEventArgs<ContentDirectory> e)
        {
            var handler = ContentDirectoryAdded;
            if (handler != null) {
                handler (this, e);
            }
        }*/
        
        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }
        
        protected virtual void Dispose (bool disposing)
        {
            if (disposing) {
                client.Dispose ();
            }
        }
    }
}