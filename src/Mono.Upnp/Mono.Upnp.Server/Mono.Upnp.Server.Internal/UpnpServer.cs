using System;
using System.Net;

namespace Mono.Upnp.Server.Internal
{
	internal abstract class UpnpServer : IDisposable
	{
        static UpnpServer ()
        {
            ServicePointManager.Expect100Continue = false;
        }

        private readonly HttpListener listener;

        protected UpnpServer (Uri url)
        {
            this.url = url;
            listener = new HttpListener ();
            listener.Prefixes.Add (url.ToString ());
        }

        public void Start ()
        {
            listener.Start ();
            listener.BeginGetContext (OnGetContext, listener);
        }

        private void OnGetContext (IAsyncResult asyncResult)
        {
            HttpListener listener = (HttpListener)asyncResult.AsyncState;
            HttpListenerContext context = listener.EndGetContext (asyncResult);
            try {
                HandleContext (context);
            } catch {
                // TODO log 
            }
            listener.BeginGetContext (OnGetContext, listener);
        }

        protected abstract void HandleContext (HttpListenerContext context);

        public void Stop ()
        {
            listener.Stop ();
        }

        private Uri url;
        public Uri Url {
            get { return url; }
        }

        public void Dispose ()
        {
            listener.Close ();
        }
    }
}
