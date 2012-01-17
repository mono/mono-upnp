// 
// ContainerBuilder.cs
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

namespace Mono.Upnp.Dcp.MediaServer1.FileSystem
{
    public class ContainerBuilder
    {
        readonly string id;
        readonly string title;

        public ContainerBuilder (string id, string title)
        {
            if (id == null) {
                throw new ArgumentNullException ("id");
            }

            this.id = id;
            this.title = title;
        }

        public string Id {
            get { return id; }
        }

        public string Title {
            get { return title; }
        }

        public Container Build (Action<ContainerInfo> consumer,
                                string title,
                                string id,
                                string parentId,
                                IList<Object> children)
        {
            if (consumer == null) {
                throw new ArgumentNullException ("consumer");
            } else if (children == null) {
                throw new ArgumentNullException ("children");
            }

            var container = new Container (id, parentId, new ContainerOptions {
                Title = title,
                ChildCount = children.Count
            });

            consumer (new ContainerInfo (container, children));

            return container;
        }
    }

    public class ContainerBuilder<T> : ContainerBuilder
        where T : ContainerOptions
    {
        // Public inner types are almost always Bad News. BUT in the interest of avoiding a 3.5 dependency, I am
        // willing to do this because these type names should never be needed outside of this file due to inference.
        public delegate T OptionsProducer (T options);
        public delegate Container ContainerProducer (string id, T options);
        public delegate string IdProducer ();

        readonly IdProducer id_producer;
        readonly ContainerProducer container_producer;

        public ContainerBuilder (string id, string title, IdProducer idProducer, ContainerProducer containerProducer)
            : base (id, title)
        {
            if (idProducer == null) {
                throw new ArgumentNullException ("idProducer");
            } else if (containerProducer == null) {
                throw new ArgumentNullException ("containerProducer");
            }

            id_producer = idProducer;
            container_producer = containerProducer;
        }

        Dictionary<string, ContainerOptionsInfo<T>> containers = new Dictionary<string, ContainerOptionsInfo<T>> ();

        public void OnItem (string container, Item item, Action<Object> consumer, OptionsProducer optionsProducer)
        {
            // And have I mentioned how much I LOVE null checking! It's 2010: do you know where your type system is?
            if (container == null) {
                return;
            } else if (item == null) {
                throw new ArgumentNullException ("item");
            } else if (consumer == null) {
                throw new ArgumentNullException ("consumer");
            } else if (optionsProducer == null) {
                throw new ArgumentNullException ("optionsProducer");
            }

            ContainerOptionsInfo<T> container_options_info;
            if (containers.TryGetValue (container, out container_options_info)) {
                container_options_info.Options = optionsProducer (container_options_info.Options);
            } else {
                container_options_info = new ContainerOptionsInfo<T> (id_producer (), optionsProducer (null));
                containers[container] = container_options_info;
            }
            var reference = new Item (id_producer (), container_options_info.Id, new ItemOptions { RefId = item.Id });
            container_options_info.Children.Add (reference);
            consumer (reference);
        }

        public Container Build (Action<ContainerInfo> consumer, string parentId)
        {
            if (consumer == null) {
                throw new ArgumentNullException ("consumer");
            }

            var children = new List<Object> (containers.Count);

            foreach (var container_info in containers.Values) {
                container_info.Options.ChildCount = container_info.Children.Count;
                var container = container_producer (Id, container_info.Options);
                consumer (new ContainerInfo (container, container_info.Children));
                children.Add (container);
            }

            return Build (consumer, Title, Id, parentId, children);
        }
    }
}
