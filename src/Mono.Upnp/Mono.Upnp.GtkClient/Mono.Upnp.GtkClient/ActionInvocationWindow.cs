// 
// ActionInvocationWindow.cs
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
using System.Threading;

using Gtk;

using Mono.Upnp.Control;

namespace Mono.Upnp.GtkClient
{
    public partial class ActionInvocationWindow : Window
    {
        readonly ServiceAction action;
        readonly Table table;
        
        public ActionInvocationWindow (ServiceController service, ServiceAction action)
            : base (WindowType.Toplevel)
        {
            this.action = action;
            
            this.Build ();
            
            name.Markup = string.Format ("<big><b>{0}</b></big>", action.Name);
            
            var arguments = new List<Argument> ();
            
            foreach (var argument in action.Arguments) {
                if (argument.Value.Direction == ArgumentDirection.In) {
                    arguments.Add (argument.Value);
                }
            }
            
            table = new Table ((uint)2, (uint)arguments.Count, false);
            var row = (uint)0;
            
            foreach (var argument in arguments) {
                table.Attach (new Label (argument.Name), (uint)0, (uint)1, row, row + 1);
                Widget widget;
                var related_state_variable = service.StateVariables[argument.RelatedStateVariable];
                if (related_state_variable.AllowedValues != null) {
                    var combobox = ComboBox.NewText ();
                    var i = 0;
                    var index = i;
                    foreach (var allowed_value in related_state_variable.AllowedValues) {
                        combobox.AppendText (allowed_value);
                        if (allowed_value == related_state_variable.DefaultValue) {
                            index = i;
                        }
                        i++;
                    }
                    combobox.Active = index;
                    widget = combobox;
                } else if (related_state_variable.AllowedValueRange != null) {
                    widget = new SpinButton (
                        double.Parse (related_state_variable.AllowedValueRange.Minimum),
                        double.Parse (related_state_variable.AllowedValueRange.Maximum),
                        double.Parse (related_state_variable.AllowedValueRange.Step));
                } else if (related_state_variable.DataType == "i4") {
                    widget = new SpinButton (int.MinValue, int.MaxValue, 1.0) {
                        Value = related_state_variable.DefaultValue != null
                            ? double.Parse (related_state_variable.DefaultValue) : 0
                    };
                } else if (related_state_variable.DataType == "ui4") {
                    widget = new SpinButton (int.MinValue, int.MaxValue, 1.0) {
                        Value = related_state_variable.DefaultValue != null
                            ? double.Parse (related_state_variable.DefaultValue) : 0
                    };
                } else {
                    widget = new Entry {
                        Text = related_state_variable.DefaultValue != null
                            ? related_state_variable.DefaultValue : ""
                    };
                }
                table.Attach (widget, (uint)1, (uint)2, row, row + 1);
                row++;
            }
            
            inputsBox.PackStart (table, true, true, 0);
            inputsBox.ShowAll ();
        }

        protected virtual void OnInvokeClicked (object sender, System.EventArgs e)
        {
            foreach (var child in outputsBox.Children) {
                outputsBox.Remove (child);
            }
            
            var arguments = new Dictionary<string, string> ();
            var children = table.Children;
            for (var i = children.Length - 1; i > 0; i -= 2) {
                string value;
                var child = children[i - 1];
                var entry = child as Entry;
                if (entry != null) {
                    value = entry.Text;
                } else {
                    value = ((ComboBox)child).ActiveText;
                }
                arguments[((Label)children[i]).Text] = value;
            }
            
            inputsBox.Sensitive = false;
            invoke.Sensitive = false;
            
            ThreadPool.QueueUserWorkItem (state => {
                try  {
                    var results = action.Invoke (arguments);
                    if (results == null) {
                        return;
                    }
                    Application.Invoke ((o, a) => {
                        foreach (var result in results) {
                            var expander = new Expander (result.Key);
                            expander.Add (new Label (result.Value) { LineWrap = true, Selectable = true });
                            outputsBox.PackStart (expander, false, false, 0);
                        }
                        outputsBox.ShowAll ();
                        inputsBox.Sensitive = true;
                        invoke.Sensitive = true;
                    });
                } catch (UpnpControlException exception) {
                    // TODO report error to the UI
                    Console.WriteLine ("Upnp error: " + exception.UpnpError);
                }
            });
        }
    }
}
