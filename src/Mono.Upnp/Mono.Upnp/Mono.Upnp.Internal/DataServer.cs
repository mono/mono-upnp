//
// DataServer.cs
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

using Mono.Upnp.Control;

namespace Mono.Upnp.Internal
{
    sealed class DataServer : UpnpServer
    {
        readonly byte[] data;
        readonly string content_type;

        public DataServer (byte[] data, string contentType, Uri url)
            : base (url)
        {
            this.data = data;
            this.content_type = contentType;
        }

        protected override void HandleContext (HttpListenerContext context)
        {
            base.HandleContext (context);
            
            context.Response.SendChunked = false;
            context.Response.ContentLength64 = data.LongLength;
            context.Response.ContentType = content_type;
            context.Response.OutputStream.Write (data, 0, data.Length);
            
            Log.Information (string.Format (
                "{0} requested {1}.", context.Request.RemoteEndPoint, context.Request.Url));
        }
    }
}
