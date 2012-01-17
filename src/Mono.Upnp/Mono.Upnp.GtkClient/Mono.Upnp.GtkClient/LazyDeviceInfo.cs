// 
// LazyDeviceInfo.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Thomas
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
using System.Threading;

using Gtk;

using Mono.Unix;

namespace Mono.Upnp.GtkClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class LazyDeviceInfo : Gtk.Bin
    {
        readonly IDeviceInfoProvider provider;
        readonly DeviceAnnouncement device;
        bool mapped;
        
        public LazyDeviceInfo (IDeviceInfoProvider provider, DeviceAnnouncement device)
        {
            this.provider = provider;
            this.device = device;
            
            this.Build ();
            
            loading.Text = Catalog.GetString ("Loading " + provider.Name);
        }

        protected virtual void OnMapped (object sender, System.EventArgs e)
        {
            if (!mapped) {
                mapped = true;
                ThreadPool.QueueUserWorkItem (state => {
                    Gtk.Widget widget;
                    try {
                        widget = provider.ProvideInfo (device);
                    } catch (Exception exception) {
                        var box = new VBox ();
                        box.PackStart (new Gtk.Label ("Failed to Load " + provider.Name), true, true, 0);
                        var expander = new Expander ("Error");
                        expander.Add (new Label (exception.ToString ()));
                        box.PackStart (expander, true, true, 0);
                        widget = box;
                    }
                    Gtk.Application.Invoke ((o, a) => {
                        alignment.Remove (alignment.Child);
                        alignment.Add (widget);
                        ShowAll ();
                    });
                });
            }
        }
    }
}
