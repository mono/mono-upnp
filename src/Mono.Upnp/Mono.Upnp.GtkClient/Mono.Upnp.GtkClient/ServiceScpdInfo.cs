// 
// ServiceScpdInfo.cs
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

using Mono.Unix;
using Mono.Upnp.Control;

namespace Mono.Upnp.GtkClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class ServiceScpdInfo : Gtk.Bin
    {
        ServiceController service;
        TreeStore actionModel;
        TreeStore stateVariableModel;
        
        public ServiceScpdInfo (ServiceController service)
        {
            this.Build ();
            
            this.service = service;
            
            actionModel = new TreeStore (typeof (string));
            stateVariableModel = new TreeStore (typeof (string));
            
            foreach (var action in service.Actions) {
                var iter = actionModel.AppendValues (action.Key);
                
                foreach (var argument in action.Value.Arguments) {
                    var argument_iter = actionModel.AppendValues (iter, argument.Key);
                    
                    actionModel.AppendValues (argument_iter, Catalog.GetString ("Direction: ") +
                        (argument.Value.Direction == ArgumentDirection.In ? "In" : "Out"));
                    actionModel.AppendValues (argument_iter, Catalog.GetString ("Is Return Value: ") + argument.Value.IsReturnValue);
                    actionModel.AppendValues (argument_iter, Catalog.GetString ("Related State Variable: ") + argument.Value.RelatedStateVariable);
                }
            }
            
            foreach (var stateVariable in service.StateVariables) {
                var iter = stateVariableModel.AppendValues (stateVariable.Key);
                
                stateVariableModel.AppendValues (iter, Catalog.GetString ("Data Type: ") + stateVariable.Value.DataType);
                stateVariableModel.AppendValues (iter, Catalog.GetString ("Sends Events: ") + stateVariable.Value.SendsEvents);
                stateVariableModel.AppendValues (iter, Catalog.GetString ("Is Multicast: ") + stateVariable.Value.IsMulticast);
                
                if (stateVariable.Value.DefaultValue != null) {
                    stateVariableModel.AppendValues (iter, Catalog.GetString ("Default Value: ") + stateVariable.Value.DefaultValue);
                }
                
                if (stateVariable.Value.AllowedValues != null) {
                    var allowed_values_iter = stateVariableModel.AppendValues (iter, Catalog.GetString ("Allowed Values"));
                    foreach (var value in stateVariable.Value.AllowedValues) {
                        stateVariableModel.AppendValues (allowed_values_iter, value);
                    }
                }
                
                if (stateVariable.Value.AllowedValueRange != null) {
                    var allowed_value_range_iter = stateVariableModel.AppendValues (iter, Catalog.GetString ("Allowed Value Range"));
                    stateVariableModel.AppendValues (allowed_value_range_iter,
                        "Minimum: " + stateVariable.Value.AllowedValueRange.Minimum);
                    stateVariableModel.AppendValues (allowed_value_range_iter,
                        "Maximum: " + stateVariable.Value.AllowedValueRange.Maximum);
                    if (stateVariable.Value.AllowedValueRange.Step != null) {
                        stateVariableModel.AppendValues (allowed_value_range_iter,
                            "Step: " + stateVariable.Value.AllowedValueRange.Step);
                    }
                }
            }
            
            actions.AppendColumn (Catalog.GetString ("Actions"), new CellRendererText (), "text", 0);
            actions.Model = actionModel;
            actions.Selection.Changed += ActionsSelectionChanged;
            
            stateVariables.AppendColumn (Catalog.GetString ("State Variables"), new CellRendererText (), "text", 0);
            stateVariables.Model = stateVariableModel;
        }

        void ActionsSelectionChanged (object sender, EventArgs e)
        {
            TreeIter iter;
            if (!actions.Selection.GetSelected (out iter)) {
                return;
            }
            
            switch (actionModel.IterDepth (iter)) {
            case 1:
                actionModel.IterNthChild (out iter, iter, 2);
                break;
            case 2:
                actionModel.IterParent (out iter, iter);
                goto case 1;
            default:
                return;
            }
            
            var value = (string)actionModel.GetValue (iter, 0);
            var related_state_variable = value.Substring (24);
            if (!stateVariableModel.GetIterFirst (out iter)) {
                return;
            }
            
            while (related_state_variable != (string)stateVariableModel.GetValue (iter, 0) && stateVariableModel.IterNext (ref iter)) { }
            stateVariables.Selection.SelectIter (iter);
        }

        protected virtual void OnActionsRowActivated (object o, Gtk.RowActivatedArgs args)
        {
            TreeIter iter;
            if (!actionModel.GetIter (out iter, args.Path)) {
                return;
            }
            
            if (actionModel.IterDepth (iter) == 0) {
                var value = (string)actionModel.GetValue (iter, 0);
                var window = new ActionInvocationWindow (service, service.Actions[value]);
                window.ShowAll ();
            }
        }
    }
}
