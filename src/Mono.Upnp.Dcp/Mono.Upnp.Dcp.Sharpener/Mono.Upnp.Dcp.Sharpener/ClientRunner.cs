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
            monkey.StartWriteBlock ("public class {0}Factory : IServiceFactory", Context.ClassName);
            monkey.WriteLine (@"private static readonly ServiceType type = new ServiceType (""{0}"");", Context.Type);
            monkey.StartWriteBlock ("internal static ServiceType Type", false);
            monkey.WriteLine ("get { return type; }");
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("ServiceType IServiceFactory.Type", false);
            monkey.WriteLine ("get { return Type; }");
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("public Service CreateService (Client client, IEnumerable<string> locations)");
            monkey.WriteLine ("return new {0} (client, locations);", Context.ClassName);
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("public Service CreateService (Device device, WebHeaderCollection headers, XmlReader reader)");
            monkey.WriteLine ("return new {0} (device, headers, reader);", Context.ClassName);
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
            EndWriteService (monkey);
            monkey.Close ();
        }

        private void StartWriteService (CodeMonkey monkey, Service service)
        {
            monkey.Write ("// {0}.cs auto-generated at {1} by Sharpener", Context.ClassName, DateTime.Now);
            monkey.WriteLine ();
            monkey.WriteUsing ("System.Collections.Generic", "System.Net", "System.Xml");
            monkey.WriteLine ();
            if (!Context.Namespace.StartsWith ("Mono.Upnp")) {
                monkey.WriteUsing ("Mono.Upnp");
            }
            monkey.WriteUsing ("Mono.Upnp.Control");
            monkey.WriteLine ();
            monkey.StartWriteBlock ("namespace {0}", Context.Namespace);
            monkey.StartWriteBlock ("public class {0}", Context.ClassName);
            monkey.WriteLine ("internal {0} (Client client, IEnumerable<string> locations)", Context.ClassName);
            monkey.StartWriteBlock ("{0}: base (client, locations, {1}Factory.Type)", monkey.Indentation, Context.ClassName);
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.WriteLine ("internal {0} (Device device, WebHeaderCollection headers, XmlReader reader)", Context.ClassName);
            monkey.StartWriteBlock ("{0}: base (device, headers, reader)", monkey.Indentation);
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
        }

        private void WriteMethods (CodeMonkey monkey, Service service)
        {
            foreach (Action action in service.Actions.Values) {
                WriteMethod (monkey, action);
            }
        }

        private void WriteMethod (CodeMonkey monkey, Action action)
        {
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
                // TODO proper typing and pascal casing
                WriteArgumentParameterDefinition (monkey, argument);
            }
            if (action.OutArguments.Count > 0 && !return_single_out_arg) {
                foreach (Argument argument in action.OutArguments.Values) {
                    if (first) {
                        first = false;
                    } else {
                        monkey.Write (", ");
                    }
                    monkey.Write ("out string {0}", argument.Name);
                }
            }
            monkey.Write (")");
            monkey.StartWriteBlock ();
            monkey.WriteLine (@"Action action = Actions[""{0}""];", action.Name);
            foreach (Argument argument in action.InArguments.Values) {
                monkey.WriteLine (@"action.InArguments[""{0}""].Value = {1};", argument.Name, InArgumentAssignment (argument));
            }
            monkey.WriteLine ("action.Execute ();");
            if (action.ReturnArgument != null) {
                monkey.WriteLine ("return action.ReturnArgument.Value;");
            } else if (return_single_out_arg) {
                Argument out_argument = null;
                foreach (Argument argument in action.OutArguments.Values) {
                    out_argument = argument;
                    break;
                }
                monkey.WriteLine (@"return action.OutArguments[""{0}""].Value;", out_argument.Name);
            } else {
                foreach (Argument argument in action.OutArguments.Values) {
                    monkey.WriteLine (@"{0} = action.OutArguments[""{0}""].Value;", argument.Name);
                }
            }
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
        }

        private void WriteArgumentParameterDefinition (CodeMonkey monkey, Argument argument)
        {
            if (argument.AllowedValues != null) {
                monkey.Write ("{0}AllowedValues {1}", argument.RelatedStateVariable.Name, argument.Name);
            } else {
                monkey.Write ("{0} {1}", GetTypeName (argument.Type), argument.Name);
            }
        }

        private string InArgumentAssignment (Argument argument)
        {
            if (argument.Type == typeof (string) && argument.AllowedValues == null) {
                return argument.Name;
            } else {
                return argument.Name + ".ToString ()";
            }
        }

        private void WriteEvents (CodeMonkey monkey, Service service)
        {
            foreach (StateVariable variable in service.StateVariables.Values) {
                if (variable.SendEvents) {
                    monkey.StartWriteBlock ("public event StateVariableChanged<string> {0}Changed", variable.Name, false);
                    monkey.WriteLine (@"add {{ StateVariables[""{0}""].Changed += value; }}", variable.Name);
                    monkey.WriteLine (@"remove {{ StateVariables[""{0}""].Changed -= value; }}", variable.Name);
                    monkey.EndWriteBlock ();
                }
            }
        }

        private void EndWriteService (CodeMonkey monkey)
        {
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
        }
	}
}
