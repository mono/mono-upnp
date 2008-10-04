using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Mono.Upnp.Dcp.Sharpener
{
	public class CodeMonkey
	{
        private readonly TextWriter writer;
        private int indentation_level;

        public CodeMonkey (string path)
            : this (new StreamWriter (path))
        {
        }

        public CodeMonkey (TextWriter writer)
        {
            if (writer == null) {
                throw new ArgumentNullException ("writer");
            }

            this.writer = writer;
        }

        private string indentation = "    ";
        public string Indentation {
            get { return indentation; }
            set { indentation = value; }
        }

        private void Indent ()
        {
            for (int i = 0; i < indentation_level; i++) {
                writer.Write (indentation);
            }
        }

        public void Write (string format, object obj)
        {
            Write (String.Format (format, obj));
        }

        public void Write (string format, object obj0, object obj1)
        {
            Write (String.Format (format, obj0, obj1));
        }

        public void Write (string format, params object [] objs)
        {
            Write (String.Format (format, objs));
        }

        public void Write (string value)
        {
            writer.Write (value);
        }

        public void WriteLine ()
        {
            writer.WriteLine ();
        }

        public void WriteLine (string format, object obj)
        {
            WriteLine (String.Format (format, obj));
        }

        public void WriteLine (string format, object obj0, object obj1)
        {
            WriteLine (String.Format (format, obj0, obj1));
        }

        public void WriteLine (string value)
        {
            WriteLine ();
            Indent ();
            Write (value);
        }

        public void StartWriteBlock ()
        {
            StartWriteBlock ("", false);
        }

        public void StartWriteBlock (string format, object obj)
        {
            StartWriteBlock (String.Format (format, obj));
        }

        public void StartWriteBlock (string format, object obj0, object obj1)
        {
            StartWriteBlock (String.Format (format, obj0, obj1));
        }

        public void StartWriteBlock (string value)
        {
            StartWriteBlock (value, true);
        }

        public void StartWriteBlock (string format, object obj, bool braceOnNewLine)
        {
            StartWriteBlock (String.Format (format, obj), braceOnNewLine);
        }

        public void StartWriteBlock (string format, object obj0, object obj1, bool braceOnNewLine)
        {
            StartWriteBlock (String.Format (format, obj0, obj1), braceOnNewLine);
        }

        public void StartWriteBlock (string value, bool braceOnNewLine)
        {
            WriteLine ();
            Indent ();
            writer.Write (value);
            if (braceOnNewLine) {
                WriteLine ();
                Indent ();
            } else if (!String.IsNullOrEmpty (value)) {
                writer.Write (' ');
            }
            writer.Write ('{');
            indentation_level++;
        }

        public void EndWriteBlock ()
        {
            indentation_level--;
            WriteLine ();
            Indent ();
            writer.Write ('}');
        }

        public void WriteUsing (string @namespace)
        {
            WriteLine (String.Format ("using {0};", @namespace));
        }

        public void WriteUsing (params string[] namespaces)
        {
            foreach (string @namespace in namespaces) {
                WriteUsing (@namespace);
            }
        }

        public void Close ()
        {
            writer.Close ();
        }
	}
}
