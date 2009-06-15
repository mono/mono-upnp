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

namespace Mono.Upnp.Internal
{
    abstract class UpnpServer : IDisposable
    {
        readonly HttpListener listener;

        protected UpnpServer (Uri url)
        {
            listener = new HttpListener { IgnoreWriteExceptions = true };
            listener.Prefixes.Add (url.ToString ());
        }

        public virtual void Start ()
        {
            lock (listener) {
                listener.Start ();
                listener.BeginGetContext (OnGetContext, null);
            }
        }

        void OnGetContext (IAsyncResult asyncResult)
        {
            lock (listener) {
                if (!listener.IsListening) {
                    return;
                }
                var context = listener.EndGetContext (asyncResult);
                HandleContext (context);
                context.Response.Close ();
                listener.BeginGetContext (OnGetContext, null);
            }
        }

        protected virtual void HandleContext (HttpListenerContext context)
        {
            context.Response.AppendHeader ("SERVER", Protocol.UserAgent);
            context.Response.AppendHeader ("DATE", DateTime.Now.ToUniversalTime ().ToString ("r"));
        }

        public virtual void Stop ()
        {
            lock (listener) {
                listener.Stop ();
            }
        }

        public virtual void Dispose ()
        {
            lock (listener) {
                listener.Close ();
            }
        }
    }
}
