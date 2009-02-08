// ConnectionManager1.cs auto-generated at 1/17/2009 8:30:43 PM by Sharpener
using System;

namespace Mono.Upnp.Dcp.MediaServer1
{
    public class DiscoveryEventArgs<T> : EventArgs
    {
        readonly T item;
        internal DiscoveryEventArgs (T item)
        {
            this.item = item;
        }
        public T Item {
            get { return item; }
        }
    }
}