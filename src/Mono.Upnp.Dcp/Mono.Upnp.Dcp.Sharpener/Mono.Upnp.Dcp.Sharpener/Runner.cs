//
// Runner.cs
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
using System.Text;
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

        private Dictionary<StateVariable, string> enumeration_names = new Dictionary<StateVariable, string> ();
        protected IDictionary<StateVariable, string> EnumerationNames {
            get { return enumeration_names; }
        }

        protected void WriteEnums (Service service)
        {
            foreach (StateVariable variable in service.StateVariables.Values) {
                if (variable.AllowedValues != null) {
                    string name = variable.Name;
                    if (name.StartsWith ("A_ARG_TYPE_")) {
                        name = name.Substring (11);
                    }
                    int count = 1;
                    while (enumeration_names.ContainsValue (name)) {
                        name = variable.Name + count++;
                    }
                    enumeration_names.Add (variable, name);
                    CodeMonkey monkey = new CodeMonkey (name + ".cs");
                    monkey.Write ("// {0}.cs auto-generated at {1} by Sharpener", name, DateTime.Now);
                    monkey.WriteLine ();
                    monkey.StartWriteBlock ("namespace {0}", Context.Namespace);
                    monkey.StartWriteBlock ("public enum {0}", name);
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

        private static Dictionary<string, string> camel_case_map = new Dictionary<string, string> ();

        protected static string ToCamelCase (string s)
        {
            if (camel_case_map.ContainsKey (s)) {
                return camel_case_map[s];
            }
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
            string camel_case = builder.ToString ();
            camel_case_map.Add (s, camel_case);
            return camel_case;
        }

        protected static char ToUpper (char c)
        {
            return (char)(IsUpper (c) ? c : c - 32);
        }

        protected static char ToLower (char c)
        {
            return (char)(IsUpper (c) ? c + 32 : c);
        }

        protected static bool IsUpper (char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        protected readonly static char[] delimiters = { '_', '-', ' ' };

        protected static bool IsCamelCase (string s)
        {
            return !IsUpper (s[0]) && s.IndexOfAny (delimiters) == -1;
        }

        protected static IEnumerable<T> Concat<T> (IEnumerable<T> enumerable1, IEnumerable<T> enumerable2)
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
