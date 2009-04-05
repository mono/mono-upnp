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
using System.Collections.Generic;

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
            //Context.Reader.ReadToFollowing ("device");
            //Device device = new OfflineDevice (Context.Reader.ReadSubtree ());
        }

        protected override void RunService ()
        {
            ServiceController service = new OfflineServiceController ();
            using (var reader = Context.Reader.ReadSubtree ()) {
                reader.Read ();
                service.Deserialize (reader);
            }
            WriteServiceClient ();
            WriteEnums (service);
            WriteService (service);
            WriteHelpers ();
        }

        void WriteServiceClient ()
        {
            CodeMonkey monkey = new CodeMonkey (Context.ClassName + "Client.cs");
            monkey.Write ("// {0}.cs auto-generated at {1} by Sharpener", Context.ClassName, DateTime.Now);
            monkey.WriteLine ();
            monkey.WriteUsing ("System");
            monkey.WriteLine ();
            if (!Context.Namespace.StartsWith ("Mono.Upnp")) {
                monkey.WriteUsing ("Mono.Upnp");
                monkey.WriteLine ();
            }
            monkey.StartWriteBlock ("namespace {0}", Context.Namespace);
            monkey.StartWriteBlock ("public class {0}Client", Context.ClassName);
            monkey.WriteLine ("readonly UpnpClient client;");
            monkey.WriteLine ();
            monkey.WriteLine ("public event EventHandler<DiscoveryEventArgs<{0}>> {0}Added;", Context.ClassName);
            monkey.WriteLine ();
            monkey.StartWriteBlock ("public {0}Client () : this (null)", Context.ClassName);
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("public {0}Client (UpnpClient client)", Context.ClassName);
            monkey.WriteLine ("this.client = client ?? new UpnpClient ();");
            monkey.WriteLine ("client.ServiceAdded += ClientServiceAdded;");
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("public UpnpClient Client", false);
            monkey.WriteLine ("get { return client; }");
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("void ClientServiceAdded (object sender, ServiceEventArgs args)");
            monkey.WriteLine ("if (args.Service.Type != {0}.ServiceType) return;", Context.ClassName);
            monkey.WriteLine ();
            monkey.StartWriteBlock ("try", false);
            monkey.WriteLine ("{0} service = new {0} (args.Service);", Context.ClassName);
            monkey.WriteLine ("On{0}Added (service);", Context.ClassName);
            monkey.EndWriteBlock ();
            monkey.StartWriteBlock ("catch");
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("public void Browse ()");
            monkey.WriteLine ("client.Browse ({0}.ServiceType);", Context.ClassName);
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("void On{0}Added ({0} service)", Context.ClassName);
            monkey.WriteLine ("EventHandler<DiscoveryEventArgs<{0}>> handler = {0}Added;", Context.ClassName);
            monkey.StartWriteBlock ("if (handler != null)", false);
            monkey.WriteLine ("handler (this, new DiscoveryEventArgs<{0}> (service));", Context.ClassName);
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
            monkey.Close ();
        }

        private void WriteService (ServiceController service)
        {
            CodeMonkey monkey = new CodeMonkey (Context.ClassName + "Controller.cs");
            StartWriteService (monkey, service);
            WriteMethods (monkey, service);
            WriteEvents (monkey, service);
            WriteVerify (monkey, service);
            EndWriteService (monkey);
            monkey.Close ();
        }

        private void StartWriteService (CodeMonkey monkey, ServiceController service)
        {
            monkey.Write ("// {0}Controller.cs auto-generated at {1} by Sharpener", Context.ClassName, DateTime.Now);
            monkey.WriteLine ();
            monkey.WriteUsing ("System", "System.Collections.Generic");
            monkey.WriteLine ();
            if (!Context.Namespace.StartsWith ("Mono.Upnp")) {
                monkey.WriteUsing ("Mono.Upnp");
            }
            monkey.WriteUsing ("Mono.Upnp.Discovery");
            monkey.WriteUsing ("Mono.Upnp.Description");
            monkey.WriteUsing ("Mono.Upnp.Control");
            monkey.WriteLine ();
            monkey.StartWriteBlock ("namespace {0}", Context.Namespace);
            monkey.StartWriteBlock ("public class {0}Controller", Context.ClassName);
            monkey.WriteLine (@"public static readonly ServiceType ServiceType = new ServiceType (""{0}"");", Context.Type);
            monkey.WriteLine ("readonly ServiceController controller;");
            monkey.StartWriteBlock ("public {0}Controller (ServiceAnnouncement announcement)", Context.ClassName);
            monkey.WriteLine (@"if (announcement == null) throw new ArgumentNullException (""announcement"");");
            monkey.WriteLine ("ServiceDescription description = announcement.GetDescription ();");
            monkey.WriteLine ("controller = description.GetController ();");
            monkey.WriteLine (@"if (controller == null) throw new UpnpDeserializationException (string.Format (""{0} has no controller."", description));");
            monkey.WriteLine ("Verify ();");
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("public ServiceDescription ServiceDescription", false);
            monkey.WriteLine ("get { return controller.Description; }");
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
        }

        private void WriteMethods (CodeMonkey monkey, ServiceController service)
        {
            foreach (OfflineAction action in service.Actions.Values) {
                WriteMethod (monkey, action);
            }
        }

        private void WriteMethod (CodeMonkey monkey, OfflineAction action)
        {
            if (action.IsOptional) {
                monkey.WriteLine (@"public bool Can{0} {{ get {{ return controller.Actions.ContainsKey(""{0}""); }} }}", action.Name);
            }
            bool return_single_out_arg = action.ReturnArgument == null && action.OutArguments.Count == 1;
            string return_type = action.ReturnArgument != null || return_single_out_arg
                ? "string" : "void";
            monkey.WriteLine ("public {0} {1} (", return_type, action.Name);
            bool first = true;
            List<Argument> enums = new List<Argument>();
            foreach (Argument argument in action.InArguments.Values) {
                if (first) {
                    first = false;
                } else {
                    monkey.Write (", ");
                }
                // TODO proper typing
                WriteArgumentParameterDefinition (monkey, argument);
                if(argument.RelatedStateVariable.AllowedValues != null) {
                    enums.Add(argument);
                }
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
            foreach (Argument argument in enums) {
                IList<string> values = argument.RelatedStateVariable.AllowedValues;
                monkey.WriteLine (@"if ({0} < {1}.{2} || {1}.{3} < {0}) throw new ArgumentOutOfRangeException (""{0}"");", ToCamelCase(argument.Name), EnumerationNames[argument.RelatedStateVariable], values[0], values[values.Count - 1]);
            }
            if (action.InArguments.Count > 0) {
                monkey.WriteLine ("Dictionary<string, string> in_arguments = new Dictionary<string, string> ({0});", action.InArguments.Count);
                foreach (Argument argument in action.InArguments.Values) {
                    monkey.WriteLine (@"in_arguments.Add (""{0}"", {1});", argument.Name, InArgumentAssignment (argument));
                }
                monkey.WriteLine (@"ActionResult action_result = controller.Actions[""{0}""].Invoke (in_arguments);", action.Name);
            } else {
                monkey.WriteLine (@"ActionResult action_result = controller.Actions[""{0}""].Invoke ();", action.Name);
            }
            if (!return_single_out_arg) {
                foreach (Argument argument in action.OutArguments.Values) {
                    monkey.WriteLine (@"{0} = action_result.OutValues[""{1}""];", ToCamelCase (argument.Name), argument.Name);
                }
            }
            if (action.ReturnArgument != null) {
                monkey.WriteLine ("return action_result.ReturnValue;");
            } else if (return_single_out_arg) {
                Argument out_argument = null;
                foreach (Argument argument in action.OutArguments.Values) {
                    out_argument = argument;
                    break;
                }
                monkey.WriteLine (@"return action_result.OutValues[""{0}""];", out_argument.Name);
            }
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
        }

        private void WriteArgumentParameterDefinition (CodeMonkey monkey, Argument argument)
        {
            if (argument.RelatedStateVariable.AllowedValues != null) {
                monkey.Write ("{0} {1}", EnumerationNames[argument.RelatedStateVariable], ToCamelCase (argument.Name));
            } else {
                monkey.Write ("{0} {1}", GetTypeName (argument.RelatedStateVariable.Type), ToCamelCase (argument.Name));
            }
        }

        private string InArgumentAssignment (Argument argument)
        {
            if (argument.RelatedStateVariable.Type == typeof (string) && argument.RelatedStateVariable.AllowedValues == null) {
                return ToCamelCase (argument.Name);
            } else {
                return ToCamelCase (argument.Name) + ".ToString ()";
            }
        }

        private void WriteEvents (CodeMonkey monkey, ServiceController service)
        {
            foreach (OfflineStateVariable variable in service.StateVariables.Values) {
                if (variable.SendsEvents) {
                    if (variable.IsOptional) {
                        monkey.WriteLine (@"public bool Has{0} {{ get {{ return controller.StateVariables.ContainsKey (""{0}""); }} }}", variable.Name);
                    }
                    monkey.StartWriteBlock ("public event EventHandler<StateVariableChangedArgs<string>> {0}Changed", variable.Name, false);
                    if (variable.IsOptional) {
                        monkey.StartWriteBlock ("add", false);
                        monkey.WriteLine ("if (!Has{0}) return;", variable.Name);
                        monkey.WriteLine (@"controller.StateVariables[""{0}""].Changed += value;", variable.Name);
                        monkey.EndWriteBlock ();

                        monkey.StartWriteBlock ("remove", false);
                        monkey.WriteLine ("if (!Has{0}) return;", variable.Name);
                        monkey.WriteLine (@"controller.StateVariables[""{0}""].Changed -= value;", variable.Name);
                        monkey.EndWriteBlock ();
                    } else {
                        monkey.WriteLine (@"add {{ controller.StateVariables[""{0}""].Changed += value; }}", variable.Name);
                        monkey.WriteLine (@"remove {{ controller.StateVariables[""{0}""].Changed -= value; }}", variable.Name);
                    }
                    monkey.EndWriteBlock ();
                    monkey.WriteLine ();
                }
            }
        }

        private void WriteVerify (CodeMonkey monkey, ServiceController service)
        {
            monkey.StartWriteBlock ("void Verify ()");
            foreach (OfflineAction action in service.Actions.Values) {
                WriteVerifyAction (monkey, action);
            }
            foreach (OfflineStateVariable state_variable in service.StateVariables.Values) {
                if (state_variable.SendsEvents && !state_variable.IsOptional) {
                    monkey.WriteLine (@"if (!controller.StateVariables.ContainsKey (""{0}"")) throw new UpnpDeserializationException (string.Format (""The service {{0}} claims to be of type {1} but it does not have the required state variable {0}."", controller.Description.Id));", state_variable.Name, Context.Type);
                }
            }
            monkey.EndWriteBlock ();
        }
        
        private void WriteVerifyAction (CodeMonkey monkey, OfflineAction action)
        {
            if (!action.IsOptional) {
                monkey.WriteLine (@"if (!controller.Actions.ContainsKey (""{0}"")) throw new UpnpDeserializationException (string.Format (""The service {{0}} claims to be of type {1} but it does not have the required action {0}."", controller.Description.Id));", action.Name, Context.Type);
            } else {
                monkey.StartWriteBlock ("if (Can{0})", action.Name);
            }
            foreach (Argument argument in action.InArguments.Values) {
                monkey.WriteLine (@"if (!controller.Actions[""{0}""].InArguments.ContainersKey (""{1}"")) throw new UpnpDeserializationException (string.Format (""The service {{0}} claims to be of type {2} but it does not have the required argument {1} in action {0}."", controller.Description.Id));", action.Name, argument.Name, Context.Type);
            }
            foreach (Argument argument in action.OutArguments.Values) {
                monkey.WriteLine (@"if (!controller.Actions[""{0}""].OutArguments.ContainersKey (""{1}"")) throw new UpnpDeserializationException (string.Format (""The service {{0}} claims to be of type {2} but it does not have the required argument {1} in action {0}."", controller.Description.Id));", action.Name, argument.Name, Context.Type);
            }
            if (action.ReturnArgument != null) {
                monkey.WriteLine (@"if (controller.Actions[""{0}""].ReturnArgument == null || controller.Actions[""{0}""].ReturnArgument.Name != ""{1}"") throw new UpnpDeserializationException (string.Format (""The service {{0}} claims to be of type {2} but it does not have the required return argument {1} in action {0}."", controller.Description.Id));", action.Name, action.ReturnArgument.Name, Context.Type);
            }
            if (action.IsOptional) {
                monkey.EndWriteBlock ();
            }
        }

        private void EndWriteService (CodeMonkey monkey)
        {
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
        }
	}
}
