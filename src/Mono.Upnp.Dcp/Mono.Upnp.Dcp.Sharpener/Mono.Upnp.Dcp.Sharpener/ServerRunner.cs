//
// ServerRunner.cs
//
// Author:
//   Scott Peterson <lunchtimemama@gmail.com>
//
// Copyright (C) 2008 S&S Black Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.Sharpener
{
	public class ServerRunner : Runner
	{
        public ServerRunner (RunnerContext context)
            : base (context)
        {
        }

        protected override void RunDevice ()
        {
            throw new NotImplementedException ();
        }

        protected override void RunService ()
        {
            ServiceController service = new OfflineServiceController ();
            service.Deserialize (Context.Reader);
            WriteEnums (service);
            WriteService (service);
        }

        private void WriteService (ServiceController service)
        {
            CodeMonkey monkey = new CodeMonkey (Context.ClassName + ".cs");
            StartWriteService (monkey, service);
            WriteMethods (monkey, service);
            WriteStateVariables (monkey, service);
            EndWriteService (monkey, service);
            monkey.Close ();
        }

        private void StartWriteService (CodeMonkey monkey, ServiceController service)
        {
            monkey.Write ("// {0}.cs auto-generated at {1} by Sharpener", Context.ClassName, DateTime.Now);
            monkey.WriteLine ();
            monkey.WriteUsing ("System");
            monkey.WriteLine ();
            if (!Context.Namespace.StartsWith ("Mono.Upnp")) {
                monkey.WriteUsing ("Mono.Upnp");
            }
            monkey.WriteUsing ("Mono.Upnp.Server");
            monkey.WriteLine ();
            monkey.StartWriteBlock ("namespace {0}", Context.Namespace);
            monkey.StartWriteBlock ("public abstract class {0} : Service", Context.ClassName);
            monkey.WriteLine ("protected {0} (string id)", Context.ClassName);
            ServiceType type = new ServiceType (Context.Type);
            monkey.WriteLine (@"{0}: base (new ServiceType (""{1}"", ""{2}"", new Version ({3}, {4})), id)",
                monkey.Indentation, type.DomainName, type.Type, type.Version.Major, type.Version.Minor);
            monkey.WriteLine ("{");
            monkey.WriteLine ("}");
        }

        private void WriteMethods (CodeMonkey monkey, ServiceController service)
        {
            foreach (Action action in service.Actions.Values) {
                WriteMethod (monkey, action);
            }
        }

        private void WriteMethod (CodeMonkey monkey, Action action)
        {
            monkey.WriteLine ("[UpnpAction]");
            WriteArgumentAttribute (monkey, action.ReturnArgument);
            WriteMethodSig (monkey, action, "public ", action.Name, true);
            monkey.StartWriteBlock ();
            monkey.WriteLine ("{0}{1}Core (", action.ReturnArgument == null ? "" : "return ", action.Name);
            WriteMethodParameters (monkey, action, false, false);
            monkey.Write (");");
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            WriteMethodSig (monkey, action, "public abstract ", action.Name + "Core", false);
            monkey.Write (";");
            monkey.WriteLine ();
        }

        private void WriteMethodSig (CodeMonkey monkey, Action action, string modifiers, string name, bool includeAttributes)
        {
            monkey.WriteLine (modifiers);
            monkey.Write (action.ReturnArgument != null ? GetTypeName (action.ReturnArgument.RelatedStateVariable.Type) : "void");
            monkey.Write (" {0} (", name);
            WriteMethodParameters (monkey, action, true, includeAttributes);
            monkey.Write (")");
        }

        private void WriteMethodParameters (CodeMonkey monkey, Action action, bool definition, bool includeAttributes)
        {
            bool first = true;
            foreach (Argument argument in Concat (action.InArguments.Values, action.OutArguments.Values)) {
                if (first) {
                    first = false;
                } else {
                    monkey.Write (", ");
                }
                if (definition) {
                    if (includeAttributes) {
                        WriteArgumentAttribute (monkey, argument);
                    }
                    WriteMethodParameter (monkey, argument);
                } else {
                    monkey.Write ("{0}{1}", argument.Direction == ArgumentDirection.Out ? "out " : "", ToCamelCase (argument.Name));
                }
            }
        }

        private void WriteArgumentAttribute (CodeMonkey monkey, Argument argument)
        {
            if (argument == null) {
                return;
            }
            bool writen = false;
            if (argument.RelatedStateVariable.DefaultValue != null) {
                string value;
                if (argument.RelatedStateVariable.Type == typeof (string)) {
                    if (argument.RelatedStateVariable.AllowedValues != null) {
                        value = String.Format (@"""{0}""", argument.RelatedStateVariable.DefaultValue);
                    } else {
                        value = String.Format ("{0}.{1}", argument.RelatedStateVariable.Name + "AllowedValues", argument.RelatedStateVariable.DefaultValue);
                    }
                } else {
                    value = argument.RelatedStateVariable.DefaultValue;
                }
                monkey.Write ("[UpnpArgument (DefaultValue = {0}", value);
                writen = true;
            }
            if (argument.RelatedStateVariable.AllowedValueRange != null) {
                if (!writen) {
                    monkey.Write ("[UpnpArgument (");
                } else {
                    monkey.Write (", ");
                }
                monkey.Write ("AllowedValueRange = new AllowedValueRange ({0}, {1}, {2})", argument.RelatedStateVariable.AllowedValueRange.Minimum, argument.RelatedStateVariable.AllowedValueRange.Maximum, argument.RelatedStateVariable.AllowedValueRange.Step);
                writen = true;
            }
            if (!IsCamelCase (argument.Name)) {
                if (!writen) {
                    monkey.Write (@"[UpnpArgument (""{0}"")]", argument.Name);
                } else {
                    monkey.Write (@", Name = ""{0}""", argument.Name);
                }
            }
            if (writen) {
                monkey.Write (")]");
            }
        }

        private void WriteMethodParameter (CodeMonkey monkey, Argument argument)
        {
            if (argument.Direction == ArgumentDirection.Out) {
                monkey.Write ("out ");
            }
            if (argument.RelatedStateVariable.AllowedValues != null) {
                monkey.Write ("{0}AllowedValues {1}", argument.RelatedStateVariable.Name, ToCamelCase (argument.Name));
            } else {
                monkey.Write ("{0} {1}", GetTypeName (argument.RelatedStateVariable.Type), ToCamelCase (argument.Name));
            }
        }

        private void WriteStateVariables (CodeMonkey monkey, ServiceController service)
        {
            foreach (StateVariable state_variable in service.StateVariables.Values) {
                if (state_variable.SendEvents) {
                    WriteStateVariable (monkey, state_variable);
                }
            }
        }

        private void WriteStateVariable (CodeMonkey monkey, StateVariable state_variable)
        {
            string type = GetTypeName (state_variable.Type);

            monkey.WriteLine (@"[UpnpStateVariable (""{0}"")]", state_variable.Name);
            monkey.WriteLine ("public event EventHandler<StateVariableChangedArgs<{0}>> {1}Changed;", type, state_variable.Name);

            monkey.StartWriteBlock ("protected {0} {1}", type, state_variable.Name, false);
            monkey.StartWriteBlock ("set", false);
            monkey.WriteLine ("EventHandler<StateVariableChangedArgs<{0}>> handler = {1}Changed;", type, state_variable.Name);
            monkey.StartWriteBlock ("if (handler != null)", false);
            monkey.WriteLine ("handler (this, new StateVariableChangedArgs<{0}> (value));", type);
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
        }

        private void EndWriteService (CodeMonkey monkey, ServiceController service)
        {
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
        }
	}
}
