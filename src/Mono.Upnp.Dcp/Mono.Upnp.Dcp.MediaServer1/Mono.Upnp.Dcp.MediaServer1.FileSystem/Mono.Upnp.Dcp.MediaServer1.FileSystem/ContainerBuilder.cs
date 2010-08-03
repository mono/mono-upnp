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
    public class ContainerBuilder<T>
        where T : ContainerOptions
    {
        // Public inner types are almost always Bad News. BUT in the interest of avoiding a 3.5 dependency, I am
        // willing to do this because these type names should never be needed outside of this file due to inference.
        public delegate T OptionsProducer (T options);
        public delegate Container ContainerProducer (T options);

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
                container_options_info = new ContainerOptionsInfo<T> (GetId (), optionsProducer (null));
                containers[container] = container_options_info;
            }
            var reference = new Item (GetId (), container_options_info.Id, new ItemOptions { RefId = item.Id });
            container_options_info.Children.Add (reference);
            consumer (reference);
        }

        public IList<Object> OnDone (Action<ContainerInfo> consumer, ContainerProducer containerProducer)
        {
            if (consumer == null) {
                throw new ArgumentNullException ("consumer");
            } else if (containerProducer == null) {
                throw new ArgumentNullException ("containerProducer");
            }

            var children = new List<Object> (containers.Count);

            foreach (var container_info in containers.Values) {
                container_info.Options.ChildCount = container_info.Children.Count;
                var container = containerProducer (container_info.Options);
                consumer (new ContainerInfo (container, container_info.Children));
                children.Add (container);
            }

            return children;
        }

        string GetId ()
        {
            return string.Empty;
        }
    }
}
