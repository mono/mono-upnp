// 
// MainWindow.cs
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
using System.Collections.Generic;

using Gtk;

using Mono.Addins;
using Mono.Unix;

namespace Mono.Upnp.GtkClient
{
    public partial class MainWindow : Gtk.Window
    {
        readonly Client client;
        readonly ListStore model;
        
        public MainWindow () : base (Gtk.WindowType.Toplevel)
        {
            Build ();
            
            list.AppendColumn (Catalog.GetString ("Icon"), new CellRendererPixbuf (), "pixbuf", 0);
            list.AppendColumn (Catalog.GetString ("UPnP Devices and Services"), new CellRendererText (), RenderCell);
            
            model = new ListStore (typeof (Gdk.Pixbuf), typeof (DeviceAnnouncement));
            list.Model = model;
            list.Selection.Changed += ListSelectionChanged;
            
            client = new Client ();
            client.DeviceAdded += ClientDeviceAdded;
            client.ServiceAdded += ClientServiceAdded;
            client.DeviceRemoved += ClientDeviceRemoved;
            client.ServiceRemoved += ClientServiceRemoved;
            client.BrowseAll ();
        }

        void ListSelectionChanged (object sender, EventArgs e)
        {
            infoBox.Remove (infoBox.Children[0]);
            
            TreeIter iter;
            if (!list.Selection.GetSelected (out iter)) {
                infoBox.Add (infoFiller);
                return;
            }
            
            var value = model.GetValue (iter, 1);
            var service = value as ServiceAnnouncement;
            if (service != null) {
                infoBox.Add (CreateNotebook (service));
            } else {
                infoBox.Add (CreateNotebook ((DeviceAnnouncement)value));
            }
            infoBox.ShowAll ();
        }

        void ClientDeviceAdded (object sender, DeviceEventArgs e)
        {
            model.AppendValues (Style.LookupIconSet ("Device").RenderIcon (Style, TextDirection.Ltr, StateType.Normal, IconSize.Menu, this, null), e.Device);
        }

        void ClientServiceAdded (object sender, ServiceEventArgs e)
        {
            model.AppendValues (Style.LookupIconSet ("Service").RenderIcon (Style, TextDirection.Ltr, StateType.Normal, IconSize.Menu, this, null), e.Service);
        }
        
        void ClientDeviceRemoved (object sender, DeviceEventArgs e)
        {
            Remove (e.Device);
        }

        void ClientServiceRemoved (object sender, ServiceEventArgs e)
        {
            Remove (e.Service);
        }
        
        void Remove<T> (T item)
            where T : class
        {
            TreeIter iter;
            if (!model.GetIterFirst (out iter)) {
                return;
            }
            
            do {
                var value = model.GetValue (iter, 1) as T;
                if (item.Equals (value)) {
                    model.Remove (ref iter);
                    break;
                }
            } while (model.IterNext (ref iter));
        }
        
        void RenderCell (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
        {
            var value = model.GetValue (iter, 1);
            
            if (value == null) return;
            
            var service = value as ServiceAnnouncement;
            if (service != null) {
                ((CellRendererText)cell).Text = service.Type.ToString ();
            } else {
                ((CellRendererText)cell).Text = ((DeviceAnnouncement)value).Type.ToString ();
            }
        }
    
        protected void OnDeleteEvent (object sender, Gtk.DeleteEventArgs a)
        {
            Gtk.Application.Quit ();
            a.RetVal = true;
        }
        
        Widget CreateNotebook (DeviceAnnouncement device)
        {
            var notebook = new Notebook ();
            foreach (var provider in DeviceInfoProviders) {
                if (provider.CanProvideInfo (device.Type)) {
                    notebook.AppendPage (new LazyDeviceInfo (provider, device), new Label (provider.Name));
                }
            }
            
            return notebook;
        }
        
        Widget CreateNotebook (ServiceAnnouncement service)
        {
            var notebook = new Notebook ();
            foreach (var provider in ServiceInfoProviders) {
                if (provider.CanProvideInfo (service.Type)) {
                    notebook.AppendPage (new LazyServiceInfo (provider, service), new Label (provider.Name));
                }
            }
            return notebook;
        }
        
        static IEnumerable<IDeviceInfoProvider> DeviceInfoProviders {
            get {
                yield return new DeviceAnnouncementInfoProvider ();
                yield return new DeviceDescriptionInfoProvider ();
                yield return new RawDeviceDescriptionInfoProvider ();
                //foreach (IDeviceInfoProvider extension in AddinManager.GetExtensionObjects (typeof (IDeviceInfoProvider))) {
                 //   yield return extension;
                //}
            }
        }
        
        static IEnumerable<IServiceInfoProvider> ServiceInfoProviders {
            get {
                yield return new ServiceAnnouncementInfoProvider ();
                yield return new ServiceDescriptionInfoProvider ();
                yield return new ServiceScpdInfoProvider ();
                yield return new RawServiceDescriptionInfoProvider ();
                yield return new RawServiceScdpInfoProvider ();
                foreach (IServiceInfoProvider extension in AddinManager.GetExtensionObjects (typeof (IServiceInfoProvider))) {
                    yield return extension;
                }
            }
        }
    }
}
