//
// ClientRunner.cs
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

using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.Sharpener
{
	public class ClientRunner : Runner
	{
        public ClientRunner (RunnerContext context)
            : base (context)
        {
        }

        protected override void RunDevice ()
        {
            Context.Reader.ReadToFollowing ("device");
            Device device = new OfflineDevice (Context.Reader.ReadSubtree ());
        }

        protected override void RunService ()
        {
            Service service = new OfflineService (Context.Reader);
            WriteEnums (service);
            WriteService (service);
            WriteServiceFactory (service);
        }

        private void WriteServiceFactory (Service service)
        {
            CodeMonkey monkey = new CodeMonkey (Context.ClassName + "Factory.cs");
            monkey.Write ("// {0}.cs auto-generated at {1} by Sharpener", Context.ClassName, DateTime.Now);
            monkey.WriteLine ();
            monkey.WriteUsing ("System.Collections.Generic", "System.Net", "System.Xml");
            monkey.WriteLine ();
            if (!Context.Namespace.StartsWith ("Mono.Upnp")) {
                monkey.WriteUsing ("Mono.Upnp");
                monkey.WriteLine ();
            }
            monkey.StartWriteBlock ("namespace {0}", Context.Namespace);
            monkey.StartWriteBlock ("public class {0}Factory : ServiceFactory", Context.ClassName);
            monkey.WriteLine (@"private static readonly ServiceType type = new ServiceType (""{0}"");", Context.Type);
            monkey.StartWriteBlock ("internal static ServiceType ServiceType", false);
            monkey.WriteLine ("get { return type; }");
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("public override ServiceType Type", false);
            monkey.WriteLine ("get { return ServiceType; }");
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("protected override Service CreateServiceCore (Client client, string deviceId, IEnumerable<string> locations)");
            monkey.WriteLine ("return new {0} (client, deviceId, locations);", Context.ClassName);
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("protected override Service CreateServiceCore (Device device, XmlReader reader, WebHeaderCollection headers)");
            monkey.WriteLine ("return new {0} (device, reader, headers);", Context.ClassName);
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
            monkey.Close ();
        }

        private void WriteService (Service service)
        {
            CodeMonkey monkey = new CodeMonkey (Context.ClassName + ".cs");
            StartWriteService (monkey, service);
            WriteMethods (monkey, service);
            WriteEvents (monkey, service);
            WriteVerify (monkey, service);
            EndWriteService (monkey);
            monkey.Close ();
        }

        private void StartWriteService (CodeMonkey monkey, Service service)
        {
            monkey.Write ("// {0}.cs auto-generated at {1} by Sharpener", Context.ClassName, DateTime.Now);
            monkey.WriteLine ();
            monkey.WriteUsing ("System", "System.Collections.Generic", "System.Net", "System.Xml");
            monkey.WriteLine ();
            if (!Context.Namespace.StartsWith ("Mono.Upnp")) {
                monkey.WriteUsing ("Mono.Upnp");
            }
            monkey.WriteUsing ("Mono.Upnp.Control");
            monkey.WriteLine ();
            monkey.StartWriteBlock ("namespace {0}", Context.Namespace);
            monkey.StartWriteBlock ("public class {0} : Service", Context.ClassName);
            monkey.WriteLine ("internal {0} (Client client, string deviceId, IEnumerable<string> locations)", Context.ClassName);
            monkey.StartWriteBlock ("{0}: base (client, deviceId, locations, {1}Factory.ServiceType)", monkey.Indentation, Context.ClassName);
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.WriteLine ("internal {0} (Device device, XmlReader reader, WebHeaderCollection headers)", Context.ClassName);
            monkey.StartWriteBlock ("{0}: base (device, reader, headers)", monkey.Indentation);
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
        }

        private void WriteMethods (CodeMonkey monkey, Service service)
        {
            foreach (OfflineAction action in service.Actions.Values) {
                WriteMethod (monkey, action);
            }
        }

        private void WriteMethod (CodeMonkey monkey, OfflineAction action)
        {
            if (action.IsOptional) {
                monkey.WriteLine (@"public bool Can{0} {{ get {{ return Actions.ContainsKey(""{0}""); }} }}", action.Name);
            }
            bool return_single_out_arg = action.ReturnArgument == null && action.OutArguments.Count == 1;
            string return_type = action.ReturnArgument != null || return_single_out_arg
                ? "string" : "void";
            monkey.WriteLine ("public {0} {1} (", return_type, action.Name);
            bool first = true;
            foreach (Argument argument in action.InArguments.Values) {
                if (first) {
                    first = false;
                } else {
                    monkey.Write (", ");
                }
                // TODO proper typing
                WriteArgumentParameterDefinition (monkey, argument);
            }
            if (action.OutArguments.Count > 0 && !return_single_out_arg) {
                foreach (Argument argument in action.OutArguments.Values) {
                    if (first) {
                        first = false;
                    } else {
                        monkey.Write (", ");
                    }
                    monkey.Write ("out string {0}", ToCamelCase (argument.Name));
                }
            }
            monkey.Write (")");
            monkey.StartWriteBlock ();
            if (action.IsOptional) {
                monkey.WriteLine ("if (!Can{0}) throw new NotImplementedException ();", action.Name);
            }
            monkey.WriteLine (@"Action action = Actions[""{0}""];", action.Name);
            foreach (Argument argument in action.InArguments.Values) {
                monkey.WriteLine (@"action.InArguments[""{0}""].Value = {1};", argument.Name, InArgumentAssignment (argument));
            }
            monkey.WriteLine ("action.Invoke ();");
            if (!return_single_out_arg) {
                foreach (Argument argument in action.OutArguments.Values) {
                    monkey.WriteLine (@"{0} = action.OutArguments[""{1}""].Value;", ToCamelCase (argument.Name), argument.Name);
                }
            }
            if (action.ReturnArgument != null) {
                monkey.WriteLine ("return action.ReturnArgument.Value;");
            } else if (return_single_out_arg) {
                Argument out_argument = null;
                foreach (Argument argument in action.OutArguments.Values) {
                    out_argument = argument;
                    break;
                }
                monkey.WriteLine (@"return action.OutArguments[""{0}""].Value;", out_argument.Name);
            }
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
        }

        private void WriteArgumentParameterDefinition (CodeMonkey monkey, Argument argument)
        {
            if (argument.AllowedValues != null) {
                monkey.Write ("{0} {1}", EnumerationNames[argument.RelatedStateVariable], ToCamelCase (argument.Name));
            } else {
                monkey.Write ("{0} {1}", GetTypeName (argument.Type), ToCamelCase (argument.Name));
            }
        }

        private string InArgumentAssignment (Argument argument)
        {
            if (argument.Type == typeof (string) && argument.AllowedValues == null) {
                return ToCamelCase (argument.Name);
            } else {
                return ToCamelCase (argument.Name) + ".ToString ()";
            }
        }

        private void WriteEvents (CodeMonkey monkey, Service service)
        {
            foreach (OfflineStateVariable variable in service.StateVariables.Values) {
                if (variable.SendEvents) {
                    if (variable.IsOptional) {
                        monkey.WriteLine (@"public bool Has{0} {{ get {{ return StateVariables.ContainsKey (""{0}""); }} }}", variable.Name);
                    }
                    monkey.StartWriteBlock ("public event EventHandler<StateVariableChangedArgs<string>> {0}Changed", variable.Name, false);
                    if (variable.IsOptional) {
                        monkey.StartWriteBlock ("add", false);
                        monkey.WriteLine ("if (!Has{0}) throw new NotImplementedException ();", variable.Name);
                        monkey.WriteLine (@"StateVariables[""{0}""].Changed += value;", variable.Name);
                        monkey.EndWriteBlock ();

                        monkey.StartWriteBlock ("remove", false);
                        monkey.WriteLine ("if (!Has{0}) throw new NotImplementedException ();", variable.Name);
                        monkey.WriteLine (@"StateVariables[""{0}""].Changed -= value;", variable.Name);
                        monkey.EndWriteBlock ();
                    } else {
                        monkey.WriteLine (@"add {{ StateVariables[""{0}""].Changed += value; }}", variable.Name);
                        monkey.WriteLine (@"remove {{ StateVariables[""{0}""].Changed -= value; }}", variable.Name);
                    }
                    monkey.EndWriteBlock ();
                    monkey.WriteLine ();
                }
            }
        }

        private void WriteVerify (CodeMonkey monkey, Service service)
        {
            monkey.StartWriteBlock ("protected override void VerifyContract ()");
            monkey.WriteLine ("base.VerifyContract ();");
            foreach (OfflineAction action in service.Actions.Values) {
                if (!action.IsOptional) {
                    monkey.WriteLine (@"if (!Actions.ContainsKey (""{0}"")) throw new UpnpDeserializationException (String.Format (""The service {{0}} claims to be of type {1} but it does not have the required action {0}."", Id));", action.Name, Context.Type);
                }
            }
            foreach (OfflineStateVariable state_variable in service.StateVariables.Values) {
                if (state_variable.SendEvents && state_variable.IsOptional) {
                    monkey.WriteLine (@"if (!StateVariables.ContainsKey (""{0}"")) throw new UpnpDeserializationException (String.Format (""The service {{0}} claims to be of type {1} but it does not have the required state variable {0}."", Id));", state_variable.Name, Context.Type);
                }
            }
            monkey.EndWriteBlock ();
        }

        private void EndWriteService (CodeMonkey monkey)
        {
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
        }
	}
}
