// ConnectionManager1.cs auto-generated at 1/17/2009 8:08:37 PM by Sharpener

using System;

namespace Mono.Upnp.Dcp.MediaServer1
{
    public class ConnectionManager1Client
    {
        readonly Client client;

        public event EventHandler<DiscoveryEventArgs<ConnectionManager1>> ConnectionManager1Added;

        public ConnectionManager1Client () : this (new Client ())
        {
        }

        public ConnectionManager1Client (Client client)
        {
            if (client == null) throw new ArgumentNullException ("client");
            this.client = client;
            client.ServiceAdded += ClientServiceAdded;
        }

        public Client Client {
            get { return client; }
        }

        void ClientServiceAdded (object sender, ServiceEventArgs args)
        {
            if (args.Service.Type != ConnectionManager1.ServiceType) return;

            try {
                ConnectionManager1 service = new ConnectionManager1 (args.Service);
                OnConnectionManager1Added (service);
            }
            catch
            {
            }
        }

        public void Browse ()
        {
            client.Browse (ConnectionManager1.ServiceType);
        }

        void OnConnectionManager1Added (ConnectionManager1 service)
        {
            EventHandler<DiscoveryEventArgs<ConnectionManager1>> handler = ConnectionManager1Added;
            if (handler != null) {
                handler (this, new DiscoveryEventArgs<ConnectionManager1> (service));
            }
        }
    }
}