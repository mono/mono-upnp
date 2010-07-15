// 
// UpnpServiceWidget.cs
//  
// Author:
//       Yavor Georgiev <fealebenpae@gmail.com>
// 
// Copyright (c) 2010 Yavor Georgiev
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
using System.Linq;
using Gtk;
using FSpot;
using System.Collections.Generic;
namespace Mono.Upnp.Dcp.MediaServer1.FSpot
{
    public partial class UpnpServiceWidget : ScrolledWindow
    {
        TreeStore model;
        Client client;
        Dictionary <string, TreeIter> devices = new Dictionary<string, TreeIter> ();

        public UpnpServiceWidget ()
        {
            this.Build ();
            model = new TreeStore (typeof (string));
            treeview1.Model = model;

            var col = new TreeViewColumn ();
            col.Sizing = TreeViewColumnSizing.Autosize;
            var celr = new CellRendererText ();
            col.PackStart (celr, false);

            col.AddAttribute (celr, "markup", 0);

            treeview1.AppendColumn (col);

            GenerateTree();
        }

        private void GenerateTree ()
        {
            client = new Client ();
            client.DeviceAdded += OnDeviceAdded;
            client.DeviceRemoved += OnDeviceRemoved;
            client.BrowseAll ();
        }

        void OnDeviceRemoved (object sender, DeviceEventArgs e)
        {
            if (devices.ContainsKey (e.Device.Udn)) {
                var iter = devices [e.Device.Udn];
                model.Remove (ref iter);
            }
        }

        void OnDeviceAdded (object sender, DeviceEventArgs e)
        {
            var device = e.Device.GetDevice ();
            if (device.Services.Any ((s) => s.Type == ContentDirectory1.ContentDirectory.ServiceType)
                && !device.Udn.EndsWith (FSpotUpnpService.ServiceGuid.ToString ())) {
                var iter = model.AppendValues (device.FriendlyName);
                devices.Add (e.Device.Udn, iter);
            }
        }

        public FSpotUpnpSidebarPage Page { get; set; }

        public override void Dispose ()
        {
            // client.Stop (); ?

            base.Dispose ();
        }
    }
}

