// 
// Wmp11ContainerBuilder.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Thomas
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;

using Object = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object;

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem.Wmp11
{
    public class Wmp11ContainerBuilder : ContainerBuilder
    {
        readonly Uri url;

        public Wmp11ContainerBuilder (Uri url, string id, string title)
            : base (id, title)
        {
            if (url == null) {
                throw new ArgumentNullException ("url");
            }

            this.url = url;
        }

        public Uri Url {
            get { return url; }
        }

        protected Container Build<T> (Action<ContainerInfo> consumer,
                                      ContainerBuilder<T> containerBuilder)
            where T : ContainerOptions
        {
            if (containerBuilder == null) {
                throw new ArgumentNullException ("containerBuilder");
            }

            return containerBuilder.Build (consumer, Id);
        }

        protected Container Build (Action<ContainerInfo> consumer, IList<Object> children)
        {
            return Build (consumer, Title, Id, Wmp11Ids.Root, children);
        }

        protected Container Build (Action<ContainerInfo> consumer, string title, string id, IList<Object> children)
        {
            return Build (consumer, title, id, this.Id, children);
        }
    }
}
