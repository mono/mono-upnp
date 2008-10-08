//
// CodeMonkey.cs
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

        public void WriteLine (string format, params object[] objs)
        {
            WriteLine (String.Format (format, objs));
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
