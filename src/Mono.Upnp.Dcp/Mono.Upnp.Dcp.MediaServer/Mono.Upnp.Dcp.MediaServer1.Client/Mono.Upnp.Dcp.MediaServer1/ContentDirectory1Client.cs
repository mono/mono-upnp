// ContentDirectory1.cs auto-generated at 1/17/2009 8:08:36 PM by Sharpener

using System;

namespace Mono.Upnp.Dcp.MediaServer1
{
    public class ContentDirectory1Client
    {
        readonly UpnpClient client;

        public event EventHandler<DiscoveryEventArgs<ContentDirectory1>> ContentDirectory1Added;

        public ContentDirectory1Client () : this (new UpnpClient ())
        {
        }

        public ContentDirectory1Client (UpnpClient client)
        {
            if (client == null) throw new ArgumentNullException ("client");
            this.client = client;
            client.ServiceAdded += ClientServiceAdded;
        }

        public UpnpClient Client {
            get { return client; }
        }

        void ClientServiceAdded (object sender, ServiceEventArgs args)
        {
            if (args.Service.Type != ContentDirectory1.ServiceType) return;

            try {
                ContentDirectory1 service = new ContentDirectory1 (args.Service);
                OnContentDirectory1Added (service);
            }
            catch
            {
            }
        }

        public void Browse ()
        {
            client.Browse (ContentDirectory1.ServiceType);
        }

        void OnContentDirectory1Added (ContentDirectory1 service)
        {
            EventHandler<DiscoveryEventArgs<ContentDirectory1>> handler = ContentDirectory1Added;
            if (handler != null) {
                handler (this, new DiscoveryEventArgs<ContentDirectory1> (service));
            }
        }
    }
}