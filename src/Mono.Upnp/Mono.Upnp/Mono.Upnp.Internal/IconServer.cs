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
using System.Collections.Generic;
using System.Net;

namespace Mono.Upnp.Internal
{
    sealed class IconServer : UpnpServer
    {
        readonly IList<Icon> icons;

        public IconServer (Uri url, IList<Icon> icons)
            : base (url)
        {
            this.icons = icons;
        }

        protected override void HandleContext (HttpListenerContext context)
        {
            using (var stream = context.Response.OutputStream) {
                var url = context.Request.Url.ToString ();
                var slash = url.LastIndexOf ('/', 1);
                var index = int.Parse (url.Substring (slash, url.Length - slash - 1));
                var data = icons[index].Data;
                context.Response.ContentType = icons[index].MimeType;
                stream.Write (data, 0, data.Length);
            }
        }
    }
}
