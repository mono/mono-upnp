using System;
using System.Xml;

using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.Sharpener
{
	public abstract class Runner
	{
        private RunnerContext context;
        protected RunnerContext Context {
            get { return context; }
        }

        protected Runner (RunnerContext context)
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

        protected abstract void RunDevice ();
        protected abstract void RunService ();

        protected void WriteEnums (Service service)
        {
            foreach (StateVariable variable in service.StateVariables.Values) {
                if (variable.AllowedValues != null) {
                    CodeMonkey monkey = new CodeMonkey (variable.Name + "AllowedValues.cs");
                    monkey.Write ("// {0}AllowedValues.cs auto-generated at {1} by Sharpener", variable.Name, DateTime.Now);
                    monkey.WriteLine ();
                    monkey.StartWriteBlock ("namespace {0}", Context.Namespace);
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
                    monkey.Close ();
                }
            }
        }

        protected string GetTypeName (Type type)
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
	}
}
