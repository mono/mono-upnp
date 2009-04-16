//
// UpnpServer.cs
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

namespace Mono.Upnp.Server.Internal
{
	internal abstract class UpnpServer : IDisposable
	{
        static UpnpServer ()
        {
        }

        readonly HttpListener listener;
        readonly Uri url;

        protected UpnpServer (Uri url)
            : this (url.ToString ())
        {
            this.url = url;
        }

        protected UpnpServer (string prefix)
        {
            listener = new HttpListener ();
            listener.Prefixes.Add (prefix);
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
            } finally {
                try {
                    context.Response.Close ();
                } catch {
                }
            }
            listener.BeginGetContext (OnGetContext, listener);
        }

        protected abstract void HandleContext (HttpListenerContext context);

        public void Stop ()
        {
            listener.Stop ();
        }

        public Uri Url {
            get { return url; }
        }

        public void Dispose ()
        {
            listener.Close ();
        }
    }
}
