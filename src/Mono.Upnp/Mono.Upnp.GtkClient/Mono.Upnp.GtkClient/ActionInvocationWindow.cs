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

using Gtk;

using Mono.Upnp.Control;

namespace Mono.Upnp.GtkClient
{
    public partial class ActionInvocationWindow : Window
    {
        public ActionInvocationWindow (ServiceAction action) : base (WindowType.Toplevel)
        {
            this.Build ();
            
            this.action.Text = action.Name;
            
//            var in_arguments = new List<Argument> ();
//            var out_arguments = new List<Argument> ();
//            
//            foreach (var argument in action.Arguments) {
//                if (argument.Value.Direction == ArgumentDirection.In) {
//                    in_arguments.Add (argument.Value);
//                } else {
//                    out_arguments.Add (argument.Value);
//                }
//            }
//            
//            var table = new Table ((uint)2, (uint)in_arguments.Count, false);
//            var row = (uint)0;
//            foreach (var argument in in_arguments) {
//                table.Attach (new Label (argument.Name), (uint)0, (uint)1, row, row + 1);
//                table.Attach (new Entry (), (uint)1, (uint)2, row, row + 1);
//                row++;
//            }
//            
//            inputsBox.PackStart (table);
//            inputsBox.ShowAll ();
        }
    }
}
