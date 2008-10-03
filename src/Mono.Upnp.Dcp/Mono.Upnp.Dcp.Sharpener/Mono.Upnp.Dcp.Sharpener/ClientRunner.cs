using System;
using System.IO;
using System.Xml;

using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.Sharpener
{
	public class ClientRunner
	{
        private RunnerContext context;

        public ClientRunner (RunnerContext context)
        {
            this.context = context;
        }

        public void Run ()
        {
            while (context.Reader.Read () && context.Reader.NodeType != XmlNodeType.Element) {
            }
            if (context.Reader.Name == "root") {
                RunDevice ();
            } else if (context.Reader.Name == "scpd") {
                RunService ();
            }
        }

        private void RunDevice ()
        {
            context.Reader.ReadToFollowing ("device");
            OfflineDevice device = new OfflineDevice (context.Reader.ReadSubtree ());
        }

        private void RunService ()
        {
            Service service = new OfflineService (context.Reader);
            WriteEnums (service);
            WriteService (service);
            WriteServiceFactory (service);
        }

        private void WriteServiceFactory (Service service)
        {
            StreamWriter writer = new StreamWriter (context.ClassName + "Factory.cs");
            CodeMonkey monkey = new CodeMonkey (writer);
            monkey.Write ("// {0}.cs auto-generated at {1} by Sharpener", context.ClassName, DateTime.Now);
            monkey.WriteLine ();
            monkey.WriteUsing ("System.Collections.Generic", "System.Net", "System.Xml");
            monkey.WriteLine ();
            if (!context.Namespace.StartsWith ("Mono.Upnp")) {
                monkey.WriteUsing ("Mono.Upnp");
                monkey.WriteLine ();
            }
            monkey.StartWriteBlock ("namespace {0}", context.Namespace);
            monkey.StartWriteBlock ("public class {0}Factory : IServiceFactory", context.ClassName);
            monkey.WriteLine (@"private static readonly ServiceType type = new ServiceType (""{0}"");", context.Type);
            monkey.StartWriteBlock ("internal static ServiceType Type", false);
            monkey.WriteLine ("get { return type; }");
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("ServiceType IServiceFactory.Type", false);
            monkey.WriteLine ("get { return Type; }");
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("public Service CreateService (Client client, IEnumerable<string> locations)");
            monkey.WriteLine ("return new {0} (client, locations);", context.ClassName);
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
            monkey.StartWriteBlock ("public Service CreateService (Device device, WebHeaderCollection headers, XmlReader reader)");
            monkey.WriteLine ("return new {0} (device, headers, reader);", context.ClassName);
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
            writer.Close ();
        }

        private void WriteService (Service service)
        {
            StreamWriter writer = new StreamWriter (context.ClassName + ".cs");
            CodeMonkey monkey = new CodeMonkey (writer);
            StartWriteService (monkey, service);
            WriteMethods (monkey, service);
            WriteEvents (monkey, service);
            EndWriteService (monkey);
            writer.Close ();
        }

        private void WriteEnums (Service service)
        {
            foreach (StateVariable variable in service.StateVariables.Values) {
                if (variable.AllowedValues != null) {
                    StreamWriter writer = new StreamWriter (variable.Name + "AllowedValues.cs");
                    CodeMonkey monkey = new CodeMonkey (writer);
                    monkey.Write ("// {0}AllowedValues.cs auto-generated at {1} by Sharpener", variable.Name, DateTime.Now);
                    monkey.WriteLine ();
                    monkey.StartWriteBlock ("namespace {0}", context.Namespace);
                    monkey.StartWriteBlock ("public enum {0}AllowedValues", variable.Name);
                    bool first = true;
                    foreach (string value in variable.AllowedValues) {
                        if (first) {
                            first = false;
                        } else {
                            monkey.Write (",");
                        }
                        monkey.WriteLine (value);
                    }
                    monkey.EndWriteBlock ();
                    monkey.EndWriteBlock ();
                    writer.Close ();
                }
            }
        }

        private void StartWriteService (CodeMonkey monkey, Service service)
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

        private void WriteMethods (CodeMonkey monkey, Service service)
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
                    WriteInArgumentParameterDefinition (monkey, argument);
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
        }

        private string InArgumentAssignment (Argument argument)
        {
            if (argument.Type == typeof (string) && argument.AllowedValues == null) {
                return argument.Name;
            } else {
                return argument.Name + ".ToString ()";
            }
        }

        private void WriteInArgumentParameterDefinition (CodeMonkey monkey, Argument argument)
        {
            if (argument.AllowedValues != null) {
                monkey.Write ("{0}AllowedValues {1}", argument.RelatedStateVariable.Name, argument.Name);
            } else{
                monkey.Write ("{0} {1}", GetTypeName (argument.Type), argument.Name);
            }
        }

        private string GetTypeName (Type type)
        {
            if (type == typeof (string)) {
                return "string";
            } else if (type == typeof (byte)) {
                return "byte";
            } else if (type == typeof (sbyte)) {
                return "sbyte";
            } else if (type == typeof (short)) {
                return "short";
            } else if (type == typeof (ushort)) {
                return "ushort";
            } else if (type == typeof (int)) {
                return "int";
            } else if (type == typeof (uint)) {
                return "uint";
            } else if (type == typeof (long)) {
                return "long";
            } else if (type == typeof (ulong)) {
                return "ulong";
            } else if (type == typeof (float)) {
                return "float";
            } else if (type == typeof (double)) {
                return "double";
            } else if (type == typeof (char)) {
                return "char";
            } else {
                return type.Name;
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
