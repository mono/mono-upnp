using System;
using System.IO;
using System.Xml;

using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.Sharpener
{
	public class ClientRunner
	{
        public static void Run (RunnerContext context)
        {
            while (context.Reader.Read () && context.Reader.NodeType != XmlNodeType.Element) {
            }
            if (context.Reader.Name == "root") {
                RunDevice (context);
            } else if (context.Reader.Name == "scpd") {
                RunService (context);
            }
        }

        private static void RunDevice (RunnerContext context)
        {
            context.Reader.ReadToFollowing ("device");
            OfflineDevice device = new OfflineDevice (context.Reader.ReadSubtree ());
        }

        private static void RunService (RunnerContext context)
        {
            OfflineService service = new OfflineService (context.Reader);
            StreamWriter writer = new StreamWriter (context.ClassName + ".cs");
            CodeMonkey monkey = new CodeMonkey (writer);
            StartWriteService (monkey, service, context);
            WriteMethods (monkey, service);
            WriteEvents (monkey, service);
            EndWriteService (monkey);
            writer.Close ();
        }

        private static void StartWriteService (CodeMonkey monkey, OfflineService service, RunnerContext context)
        {
            monkey.Write ("// {0}.cs auto-generated at {1} by Sharpener", context.ClassName, DateTime.Now);
            monkey.WriteLine ();
            monkey.WriteUsing ("System.Collections.Generic", "System.Net", "System.Xml");
            monkey.WriteLine ();
            if (!context.Namespace.StartsWith ("Mono.Upnp")) {
                monkey.WriteUsing ("Mono.Upnp");
            }
            monkey.WriteUsing ("Mono.Upnp.Control");
            monkey.WriteLine ();
            monkey.StartWriteBlock ("namespace {0}", context.Namespace);
            monkey.StartWriteBlock ("public class {0}", context.ClassName);
            monkey.WriteLine ("internal {0} (Client client, IEnumerable<string> locations)", context.ClassName);
            monkey.StartWriteBlock ("{0}: base (client, locations, {1}Factory.Type)", monkey.Indentation, context.ClassName);
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.WriteLine ("internal {0} (Device device, WebHeaderCollection headers, XmlReader reader)", context.ClassName);
            monkey.StartWriteBlock ("{0}: base (device, headers, reader)", monkey.Indentation);
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
        }

        private static void WriteMethods (CodeMonkey monkey, Service service)
        {
            foreach (Action action in service.Actions.Values) {
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
                    monkey.Write ("string {0}", argument.Name);
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
                    monkey.WriteLine (@"action.InArguments[""{0}""].Value = {0};", argument.Name);
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
        }

        private static void WriteEvents (CodeMonkey monkey, Service service)
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

        private static void EndWriteService (CodeMonkey monkey)
        {
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
        }
	}
}
