// 
// ContentDirectoryInfo.cs
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
using System.Threading;

using Gtk;

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;

using Object = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object;
using Container = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Container;

namespace Mono.Upnp.Dcp.MediaServer1.GtkClient
{
    public class ContentDirectoryInfo : HBox
    {
        Object loading = new Object ("0", "-1", new ObjectOptions { Title = "Loading..." });
        RemoteContentDirectory content_directry;
        TreeStore store;

        public ContentDirectoryInfo (RemoteContentDirectory contentDirectory)
        {
            if (contentDirectory == null) {
                throw new ArgumentNullException ("contentDirectory");
            }

            this.content_directry = contentDirectory;
            this.store = new TreeStore (typeof (Object));
            var objects = new TreeView ();
            var column = new TreeViewColumn ();
            var cell = new CellRendererText ();
            column.SetCellDataFunc (cell, RenderObject);
            column.PackStart (cell, true);
            column.Title = "Objects";
            objects.AppendColumn (column);
            objects.Selection.Changed += HandleObjectsSelectionChanged;
            objects.RowExpanded += HandleObjectsRowExpanded;
            objects.Model = store;

            var root = contentDirectory.GetRootObject ();
            store.AppendValues (root);
            TreeIter iter;
            store.GetIterFirst (out iter);
            store.AppendValues (iter, loading);

            Add (objects);
            Add (new Label ("bye bye"));
        }

        void HandleObjectsRowExpanded (object o, RowExpandedArgs args)
        {
            if (store.IterHasChild (args.Iter)) {
                return;
            }
            var container = ((Container)store.GetValue (args.Iter, 0));
            var path = args.Path;
            ThreadPool.QueueUserWorkItem (state => {
                try {
                    var children = content_directry.GetChildren<Object> (container);
                    Application.Invoke ((s, a) => {
                        TreeIter iter, loading_iter;
                        store.GetIter (out iter, path);
                        store.IterNthChild (out loading_iter, iter, 0);
                        store.Remove (ref loading_iter);
                        foreach (var child in children) {
                            var child_iter = store.AppendValues (iter, child);
                            var c = child as Container;
                            if (c != null && c.ChildCount > 0) {
                                store.AppendValues (child_iter, loading);
                            }
                        }
                    });
                } catch (Exception e) {
                }
            });
        }

        void RenderObject (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
        {
            var @object = (Object)model.GetValue (iter, 0);
            ((CellRendererText)cell).Text = @object.Title;
        }

        void HandleObjectsSelectionChanged (object sender, EventArgs e)
        {

        }
    }
}

