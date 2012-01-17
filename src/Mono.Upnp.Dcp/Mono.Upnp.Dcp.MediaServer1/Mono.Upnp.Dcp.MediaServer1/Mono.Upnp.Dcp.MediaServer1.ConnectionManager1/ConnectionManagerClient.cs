// ConnectionManager1.cs auto-generated at 1/17/2009 8:08:37 PM by Sharpener

using System;

namespace Mono.Upnp.Dcp.MediaServer1.ConnectionManager1
{
    public class ConnectionManagerClient
    {
        readonly Client client;

        public event EventHandler<DiscoveryEventArgs<ConnectionManager>> ConnectionManagerAdded;

        public ConnectionManagerClient () : this (new Client ())
        {
        }

        public ConnectionManagerClient (Client client)
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
            if (args.Service.Type != ConnectionManager.ServiceType) return;

            try {
                //ConnectionManager service = new ConnectionManager (args.Service);
                //OnConnectionManagerAdded (service);
            }
            catch
            {
            }
        }

        public void Browse ()
        {
            client.Browse (ConnectionManager.ServiceType);
        }

        /*void OnConnectionManagerAdded (ConnectionManager service)
        {
            EventHandler<DiscoveryEventArgs<ConnectionManager>> handler = ConnectionManagerAdded;
            if (handler != null) {
                handler (this, new DiscoveryEventArgs<ConnectionManager> (service));
            }
        }*/
    }
}