//
// IconServer.cs
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
using System.IO;
using System.Net;

namespace Mono.Upnp.Server.Internal
{
	internal class IconServer : UpnpServer
	{
        private readonly Device device;
        private readonly Uri baseUrl;

        public IconServer (Device device, Uri url)
            : base (url.ToString ())
        {
            this.device = device;
            this.baseUrl = url;
        }

        protected override void HandleContext (HttpListenerContext context)
        {
            string id = baseUrl.MakeRelativeUri (context.Request.Url).ToString ().Trim ('/');
            Icon icon = device.Icons[int.Parse (id)];
            context.Response.ContentType = icon.MimeType;
            Stream stream = context.Response.OutputStream;
            byte[] data = icon.Data;
            context.Response.ContentLength64 = data.Length;
            stream.Write (data, 0, data.Length);
            stream.Close ();
        }
	}
}
