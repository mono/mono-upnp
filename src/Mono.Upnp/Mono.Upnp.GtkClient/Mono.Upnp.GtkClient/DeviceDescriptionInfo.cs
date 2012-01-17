// 
// DeviceDescriptionInfo.cs
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

using Gtk;

namespace Mono.Upnp.GtkClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class DeviceDescriptionInfo : Bin
    {
        public DeviceDescriptionInfo (Device device)
        {
            this.Build ();
            
            deviceType.Text = device.Type.ToString ();
            udn.Text = device.Udn;
            friendlyName.Text = device.FriendlyName;
            manufacturer.Text = device.Manufacturer;
            manufacturerUrl.Text = device.ManufacturerUrl != null ? device.ManufacturerUrl.ToString () : null;
            modelName.Text = device.ModelName;
            modelNumber.Text = device.ModelNumber;
            modelUrl.Text = device.ModelUrl != null ? device.ModelUrl.ToString () : null;
            serialNumber.Text = device.SerialNumber;
            upc.Text = device.Upc;
            foreach (var icon in device.Icons) {
                var expander = new Expander (string.Format ("{0}, {1}x{2}x{3}", icon.MimeType, icon.Width, icon.Height, icon.Depth));
                expander.Add (new LazyIcon (icon));
                iconBox.PackStart (expander, true, true, 0);
            }
        }
    }
}
