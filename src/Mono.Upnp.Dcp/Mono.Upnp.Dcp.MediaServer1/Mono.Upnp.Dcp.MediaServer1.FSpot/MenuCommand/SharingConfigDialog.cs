// 
// SharingConfigDialog.cs
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
using Gdk;
using System.Collections.Generic;
using System.Collections;

namespace Mono.Upnp.Dcp.MediaServer1.FSpot
{
    public partial class SharingConfigDialog : Dialog
    {
        GConf.Client client = new GConf.Client();
        private List<uint> selected_tags = new List<uint> ();
        bool selected_tags_changed = false;
        TreeStore model = new TreeStore (typeof(uint), typeof(string));
        TagStore tag_store = App.Instance.Database.Tags;
        Pixbuf empty_pixbuf = new Pixbuf (Colorspace.Rgb, true, 8, 1, 1);

        public SharingConfigDialog ()
        {
            this.Build ();
            categoriesTreeView.Model = model;

            var complete_column = new TreeViewColumn ();

            var toggle_render = new CellRendererToggle ();
            toggle_render.Toggled += CellRendererToggle_Toggled;
            complete_column.PackStart (toggle_render, false);
            complete_column.SetCellDataFunc (toggle_render, new TreeCellDataFunc (ToggleDataFunc));

            var pix_render = new CellRendererPixbuf () { Mode = CellRendererMode.Inert };
            complete_column.PackStart (pix_render, false);
            complete_column.SetCellDataFunc (pix_render, new TreeCellDataFunc (IconDataFunc));

            var text_render = new CellRendererText () { Mode = CellRendererMode.Inert };
            complete_column.PackStart (text_render, true);
            complete_column.SetCellDataFunc (text_render, new TreeCellDataFunc (NameDataFunc));
    
            categoriesTreeView.AppendColumn (complete_column);

            empty_pixbuf.Fill(0xffffff00);

            Update ();

            LoadPreferences ();
        }

        protected virtual void OnShareMyLibraryCheckboxToggled (object sender, System.EventArgs e)
        {
            if (shareMyLibraryCheckbox.Active) {
                shareEntireLibraryRadioButton.Sensitive = true;
                shareSelectedCategoriesRadioButton.Sensitive = true;
                if (shareSelectedCategoriesRadioButton.Active) {
                    categoriesTreeView.Sensitive = true;
                }
            } else {
                shareEntireLibraryRadioButton.Sensitive = false;
                shareSelectedCategoriesRadioButton.Sensitive = false;
                categoriesTreeView.Sensitive = false;
            }
        }

        protected virtual void OnShareSelectedCategoriesRadioButtonToggled (object sender, System.EventArgs e)
        {
            if (shareSelectedCategoriesRadioButton.Active) {
                categoriesTreeView.Sensitive = true;
            } else {
                categoriesTreeView.Sensitive = false;
            }
        }

        protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
        {
            client.Set (GConfConstants.LOOK_FOR_LIBRARIES_KEY, lookForSharedLibrariesCheckbox.Active);
            client.Set (GConfConstants.SHARE_LIBRARY_KEY, shareMyLibraryCheckbox.Active);
            client.Set (GConfConstants.SHARE_ALL_CATEGORIES_KEY, shareEntireLibraryRadioButton.Active);

            var list = new List<int> ();
            foreach (var tag_id in selected_tags) {
                list.Add (unchecked((int)tag_id));
            }

            client.Set (GConfConstants.SHARED_CATEGORIES_KEY, list.ToArray ());

            client.SuggestSync ();
        }

        void LoadPreferences ()
        {
            try {
                lookForSharedLibrariesCheckbox.Active = (bool)client.Get (GConfConstants.LOOK_FOR_LIBRARIES_KEY);
                shareMyLibraryCheckbox.Active = (bool)client.Get (GConfConstants.SHARE_LIBRARY_KEY);
                shareSelectedCategoriesRadioButton.Active = !(bool)client.Get (GConfConstants.SHARE_ALL_CATEGORIES_KEY);
                var list = client.Get (GConfConstants.SHARED_CATEGORIES_KEY);
                if (list != null) {
                    foreach (var item in (IEnumerable)list)
                    {
                        selected_tags.Add (Convert.ToUInt32 (item));
                    }
                }
            } catch (GConf.NoSuchKeyException) {
            }
        }

        void Update ()
        {
            model.Clear ();

            foreach (Tag t in tag_store.RootCategory.Children) {
                TreeIter iter = model.AppendValues (t.Id, t.Name);
                if (t is Category)
                    LoadCategory (t as Category, iter);
            }
        }

        void LoadCategory (Category category, TreeIter parent_iter)
        {
            foreach (var t in category.Children) {
                var iter = model.AppendValues (parent_iter, t.Id, t.Name);
                if (t is Category)
                    LoadCategory (t as Category, iter);
            }
        }

        void IconDataFunc (TreeViewColumn column,
                       CellRenderer renderer,
                       TreeModel model,
                       TreeIter iter)
        {
            var value = new GLib.Value ();
            model.GetValue (iter, 0, ref value);
            var tag_id = (uint) value;
            var tag = tag_store.Get (tag_id) as Tag;
    
            if (tag.SizedIcon != null) {
                (renderer as CellRendererPixbuf).Pixbuf = tag.SizedIcon;
            } else
                (renderer as CellRendererPixbuf).Pixbuf = empty_pixbuf;
        }
    
        void NameDataFunc (TreeViewColumn column,
                       CellRenderer renderer,
                       TreeModel model,
                       TreeIter iter)
        {
            // FIXME not sure why it happens...
            if (model == null)
                return;

            var value = new GLib.Value ();
            model.GetValue (iter, 0, ref value);
            var tag_id = (uint) value;

            var tag = tag_store.Get (tag_id) as Tag;

            (renderer as CellRendererText).Text = tag.Name;
        }

        void ToggleDataFunc (TreeViewColumn column,
                       CellRenderer renderer,
                       TreeModel model,
                       TreeIter iter)
        {
            // FIXME not sure why it happens...
            if (model == null)
                return;

            var value = new GLib.Value ();
            model.GetValue (iter, 0, ref value);
            var tag_id = (uint) value;

            (renderer as CellRendererToggle).Active = selected_tags.Contains (tag_id);
        }

        void CellRendererToggle_Toggled (object o, ToggledArgs args)
        {
            TreeIter iter;
            if (model.GetIterFromString (out iter, args.Path))
            {
                var tag_id = (uint)model.GetValue (iter, 0);
                ToggleTag (tag_id);

                selected_tags_changed = true;
            }
        }

        void ToggleTag (uint tag_id)
        {
            var tag = tag_store.Get (tag_id);
            if (tag != null) {
                if (selected_tags.Contains (tag_id)) {
                    selected_tags.Remove (tag_id);
                } else {
                    selected_tags.Add (tag_id);
                }

                if (tag is Category) {
                    foreach (var child in (tag as Category).Children) {
                        ToggleTag (child.Id);
                    }
                }
            }
        }
    }
}