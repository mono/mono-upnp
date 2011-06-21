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
using System.Collections.Generic;
using System.Threading;

using Gtk;

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;

using Object = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Object;
using Container = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Container;
using Item = Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Item;

namespace Mono.Upnp.Dcp.MediaServer1.GtkClient
{
    public class ContentDirectoryInfo : HBox
    {
        ObjectRow loading = new ObjectRow (new Object ("0", "-1", new ObjectOptions { Title = "Loading..." }));
        RemoteContentDirectory content_directry;
        TreeStore store;
        ObjectInfo object_info;

        public ContentDirectoryInfo (RemoteContentDirectory contentDirectory)
        {
            if (contentDirectory == null) {
                throw new ArgumentNullException ("contentDirectory");
            }

            this.content_directry = contentDirectory;
            this.store = new TreeStore (typeof (ObjectRow));
            var objects = new TreeView ();
            var column = new TreeViewColumn ();
            var cell = new CellRendererText ();
            column.PackStart (cell, true);
            column.SetCellDataFunc (cell, RenderObject);
            column.Title = "Objects";
            objects.AppendColumn (column);
            objects.Selection.Changed += HandleObjectsSelectionChanged;
            objects.RowExpanded += HandleObjectsRowExpanded;
            objects.Model = store;

            var root = contentDirectory.GetRootObject ();
            store.AppendValues (new ObjectRow (root));
            TreeIter iter;
            store.GetIterFirst (out iter);
            store.AppendValues (iter, loading);

            Add (objects);
        }

        void HandleObjectsRowExpanded (object o, RowExpandedArgs args)
        {
            var container = GetContainer (args.Iter);
            if (container == null) {
                return;
            }
            var path = args.Path.Copy ();
            ThreadPool.QueueUserWorkItem (state => {
                try {
                    var children = content_directry.GetChildren<Object> (container);
                    Application.Invoke ((s, a) => {
                        TreeIter iter, loading_iter;
                        store.GetIter (out iter, path);
                        store.IterNthChild (out loading_iter, iter, 0);
                        List<ItemRow> items = null;
                        var position = -1;
                        foreach (var child in children) {
                            position++;
                            var item = child as Item;
                            if (item != null && item.IsReference) {
                                if (items == null) {
                                    items = new List<ItemRow> ();
                                }
                                items.Add (new ItemRow (item, store.GetPath (store.InsertWithValues (iter, position, loading))));
                                continue;
                            }
                            var child_iter = store.InsertWithValues (iter, position, new ObjectRow (child));
                            var c = child as Container;
                            if (c != null && c.ChildCount > 0) {
                                store.AppendValues (child_iter, loading);
                            }
                        }
                        store.Remove (ref loading_iter);
                        if (items != null) {
                            Load (items);
                        }
                    });
                } catch (Exception e) {
                    Console.WriteLine (e);
                }
            });
        }

        void Load (IEnumerable<ItemRow> items)
        {
            ThreadPool.QueueUserWorkItem (state => {
                foreach (var item in items) {
                    var @object = content_directry.GetObject <Item> (item.Item.RefId);
                    Load (@object, item.Path);
                }
            });
        }

        void Load (Item item, TreePath path)
        {
            Application.Invoke ((sender, args) => {
                TreeIter iter;
                if (store.GetIter (out iter, path)) {
                    store.SetValue (iter, 0, new ObjectRow (item));
                }
            });
        }

        Container GetContainer (TreeIter iter)
        {
            var row = ((ObjectRow)store.GetValue (iter, 0));
            if (row.Expanded) {
                return null;
            }
            row.Expanded = true;
            return (Container)row.Object;
        }

        void RenderObject (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
        {
            var @object = ((ObjectRow)model.GetValue (iter, 0)).Object;
            ((CellRendererText)cell).Text = @object.Title;
        }

        void HandleObjectsSelectionChanged (object sender, EventArgs e)
        {
            if (object_info != null) {
                Remove (object_info);
            }

            var selection = (TreeSelection)sender;
            TreeIter iter;
            if (!selection.GetSelected (out iter)) {
                return;
            }
            object_info = new ObjectInfo (((ObjectRow)store.GetValue (iter, 0)).Object);
            Add (object_info);
        }
    }

    class ItemRow
    {
        public ItemRow (Item item, TreePath path)
        {
            Item = item;
            Path = path;
        }

        public Item Item { get; private set; }

        public TreePath Path { get; private set; }
    }

    class ObjectRow
    {
        public ObjectRow (Object @object)
        {
            Object = @object;
        }

        public Object Object { get; private set; }

        public bool Expanded { get; set; }
    }
}
