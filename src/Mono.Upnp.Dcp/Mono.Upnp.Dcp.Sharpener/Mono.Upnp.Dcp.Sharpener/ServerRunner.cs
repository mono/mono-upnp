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
            Service service = new OfflineService (Context.Reader);
            WriteEnums (service);
            WriteService (service);
        }

        private void WriteService (Service service)
        {
            CodeMonkey monkey = new CodeMonkey (Context.ClassName + ".cs");
            StartWriteService (monkey, service);
            WriteMethods (monkey, service);
            WriteStateVariables (monkey, service);
            EndWriteService (monkey, service);
            monkey.Close ();
        }

        private void StartWriteService (CodeMonkey monkey, Service service)
        {
            monkey.Write ("// {0}.cs auto-generated at {1} by Sharpener", Context.ClassName, DateTime.Now);
            monkey.WriteLine ();
            if (!Context.Namespace.StartsWith ("Mono.Upnp")) {
                monkey.WriteUsing ("Mono.Upnp");
            }
            monkey.WriteUsing ("Mono.Upnp.Server");
            monkey.WriteLine ();
            monkey.StartWriteBlock ("namespace {0}", Context.Namespace);
            monkey.StartWriteBlock ("public abstract class {0} : Service", Context.ClassName);
        }

        private void WriteMethods (CodeMonkey monkey, Service service)
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
            monkey.Write (action.ReturnArgument != null ? GetTypeName (action.ReturnArgument.Type) : "void");
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
            if (argument.DefaultValue != null) {
                string value;
                if (argument.Type == typeof (string)) {
                    if (argument.AllowedValues != null) {
                        value = String.Format (@"""{0}""", argument.DefaultValue);
                    } else {
                        value = String.Format ("{0}.{1}", argument.RelatedStateVariable.Name + "AllowedValues", argument.DefaultValue);
                    }
                } else {
                    value = argument.DefaultValue;
                }
                monkey.Write ("[UpnpArgument (DefaultValue = {0}", value);
                writen = true;
            }
            if (argument.AllowedValueRange != null) {
                if (!writen) {
                    monkey.Write ("[UpnpArgument (");
                } else {
                    monkey.Write (", ");
                }
                monkey.Write ("AllowedValueRange = new AllowedValueRange ({0}, {1}, {2})", argument.AllowedValueRange.Minimum, argument.AllowedValueRange.Maximum, argument.AllowedValueRange.Step);
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
            if (argument.AllowedValues != null) {
                monkey.Write ("{0}AllowedValues {1}", argument.RelatedStateVariable.Name, ToCamelCase (argument.Name));
            } else {
                monkey.Write ("{0} {1}", GetTypeName (argument.Type), ToCamelCase (argument.Name));
            }
        }

        private void WriteStateVariables (CodeMonkey monkey, Service service)
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
            monkey.WriteLine ("handler (value);");
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
            monkey.WriteLine ();
        }

        private void EndWriteService (CodeMonkey monkey, Service service)
        {
            monkey.EndWriteBlock ();
            monkey.EndWriteBlock ();
        }

        private static string ToCamelCase (string s)
        {
            if (IsCamelCase (s)) {
                return s;
            }
            StringBuilder builder = new StringBuilder (s.Length);
            builder.Append (ToLower (s[0]));
            for (int i = 1; i < s.Length; i++) {
                bool replaced = false;
                foreach (char delimiter in delimiters) {
                    if (s[i] == delimiter) {
                        builder.Append (ToUpper (s[i + 1]));
                        i++;
                        replaced = true;
                        break;
                    }
                }
                if (!replaced) {
                    builder.Append (s[i]);
                }
            }
            return builder.ToString ();
        }

        private static char ToUpper (char c)
        {
            return (char)(IsUpper (c) ? c : c - 32);
        }

        private static char ToLower (char c)
        {
            return (char)(IsUpper (c) ? c + 32 : c);
        }

        private static bool IsUpper (char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        private readonly static char[] delimiters = { '_', '-', ' ' };

        private static bool IsCamelCase (string s)
        {
            return !IsUpper (s[0]) && s.IndexOfAny (delimiters) == -1;
        }

        private static IEnumerable<T> Concat<T> (IEnumerable<T> enumerable1, IEnumerable<T> enumerable2)
        {
            foreach (T value in enumerable1) {
                yield return value;
            }
            foreach (T value in enumerable2) {
                yield return value;
            }
        }
	}
}
