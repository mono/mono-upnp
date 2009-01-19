//
// Action.cs
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
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp.Control
{
	public class Action
    {
        readonly static Dictionary<string, string> emptyArguments = new Dictionary<string, string> ();
        readonly ServiceController controller;
        string name;
        Argument return_argument;
        readonly Dictionary<string, Argument> in_argument_dict = new Dictionary<string,Argument> ();
        readonly ReadOnlyDictionary<string, Argument> in_arguments;
        readonly Dictionary<string, Argument> out_argument_dict = new Dictionary<string,Argument> ();
        readonly ReadOnlyDictionary<string, Argument> out_arguments;
        bool bypass_return_argument;
        bool loaded;
        int retry;

        protected internal Action (ServiceController service)
        {
            if (service == null) throw new ArgumentNullException ("service");

            in_arguments = new ReadOnlyDictionary<string, Argument> (in_argument_dict);
            out_arguments = new ReadOnlyDictionary<string, Argument> (out_argument_dict);

            this.controller = service;
        }

        public ServiceController Controller {
            get { return controller; }
        }

        public string Name {
            get { return name; }
            protected set {
                CheckLoaded ();
                name = value;
            }
        }
        
        public Argument ReturnArgument {
            get { return return_argument; }
        }

        public ReadOnlyDictionary<string, Argument> InArguments {
            get { return in_arguments; }
        }

        public ReadOnlyDictionary<string, Argument> OutArguments {
            get { return out_arguments; }
        }

        public int Retry {
            get { return retry; }
            set { retry = value; }
        }

        public bool IsDisposed {
            get { return controller.IsDisposed; }
        }

        public ActionResult Invoke ()
        {
            return Invoke (emptyArguments);
        }

        public ActionResult Invoke (params string[] arguments)
        {
            return Invoke (DictionarifyArguments (arguments));
        }

        public ActionResult Invoke (IDictionary<string, string> arguments)
        {
            VerifyArguments (arguments);
            CheckDisposed ();
            return InvokeCore (arguments);
        }

        protected virtual ActionResult InvokeCore (IDictionary<string, string> arguments)
        {
            if (arguments == null) throw new ArgumentNullException ("arguments");
            return controller.Invoke (this, arguments);
        }

        IDictionary<string, string> DictionarifyArguments (string[] arguments)
        {
            if (arguments == null) {
                return new Dictionary<string, string> (0);
            }
            if (arguments.Length % 2 != 0) {
                throw new ArgumentException ("There are an uneven number of keys and values in the arguments list.");
            }
            Dictionary<string, string> args = new Dictionary<string, string> (arguments.Length << 1);
            for (int i = 0; i < arguments.Length; i += 2) {
                if (args.ContainsKey (arguments[i])) {
                    throw new ArgumentException (String.Format (
                        "All arguments must have a unique name. {0} appears more than once.", arguments[i]));
                }
                args.Add (arguments[i], arguments[i + 1]);
            }
            return args;
        }

        void VerifyArguments (IDictionary<string, string> arguments)
        {
            foreach (KeyValuePair<string, string> pair in arguments) {
                if (!in_arguments.ContainsKey (pair.Key)) {
                    throw new ArgumentException ("This action does not have an in argument called {0}.", pair.Key);
                }
                VerifyArgumentValue (in_arguments[pair.Key], pair.Value);
            }
        }

        void VerifyArgumentValue (Argument argument, string value)
        {
            if (argument.RelatedStateVariable == null) {
                return;
            }
            Type type = argument.RelatedStateVariable.Type;
            ReadOnlyCollection<string> values = argument.RelatedStateVariable.AllowedValues;
            if (values != null && type == typeof (string) && !values.Contains (value)) {
                throw new ArgumentException (
                    string.Format ("The value {0} is not allowed for the argument {1}.", value, argument.Name));
            }
            AllowedValueRange range = argument.RelatedStateVariable.AllowedValueRange;
            if (range != null && type is IComparable) {
                MethodInfo parse = type.GetMethod ("Parse", BindingFlags.Public | BindingFlags.Static);
                object arg = parse.Invoke (null, new object[] { value });
                if (range.Min == null) {
                    range.Min = (IComparable)parse.Invoke (null, new object[] { range.Minimum });
                    range.Max = (IComparable)parse.Invoke (null, new object[] { range.Maximum });
                }
                if (range.Min.CompareTo (arg) > 0) {
                    throw new ArgumentOutOfRangeException (argument.Name, value, string.Format (
                        "The value is less than {0}.", range.Minimum));
                } else if (range.Max.CompareTo (arg) < 0) {
                    throw new ArgumentOutOfRangeException (argument.Name, value, string.Format (
                        "The value is greater than {0}.", range.Maximum));
                }
            }
        }

        protected void CheckDisposed ()
        {
            if (IsDisposed) {
                throw new ObjectDisposedException (ToString (),
                    "This action is no longer available because its service has gone off the network.");
            }
        }

        void CheckLoaded ()
        {
            if (loaded) {
                throw new InvalidOperationException ("The action has already been deserialized.");
            }
        }

        protected internal virtual void SerializeRequest (IDictionary<string, string> arguments, WebHeaderCollection headers, XmlWriter writer)
        {
            Helper.WriteStartSoapBody (writer);
            SerializeRequestSoapBody (arguments, writer);
            Helper.WriteEndSoapBody (writer);
        }

        protected virtual void SerializeRequestSoapBody (IDictionary<string, string> arguments, XmlWriter writer)
        {
            writer.WriteStartElement ("u", name, controller.Description.Type.ToString ());
            foreach (KeyValuePair<string, string> argument in arguments) {
                writer.WriteStartElement (argument.Key);
                writer.WriteValue (argument.Value ?? "");
                writer.WriteEndElement ();
            }
            writer.WriteEndElement ();
        }

        protected internal virtual ActionResult DeserializeResponse (HttpWebResponse response)
        {
            XmlReader reader = XmlReader.Create (response.GetResponseStream ());
            reader.ReadToFollowing (name + "Response", controller.Description.Type.ToString ());
            string return_value = null;
            Dictionary<string, string> out_args = new Dictionary<string, string> ();
            while (Helper.ReadToNextElement (reader)) {
                if (return_argument != null && return_argument.Name == reader.Name) {
                    return_value = reader.ReadString ();
                } else if (out_arguments.ContainsKey (reader.Name)) {
                    out_args.Add (reader.Name, reader.ReadString ());
                }
            }
            reader.Close ();
            return new ActionResult (return_value, out_args);
        }

        protected internal virtual Exception DeserializeResponseFault (HttpWebResponse response)
        {
            return new UpnpControlException (XmlReader.Create (response.GetResponseStream ()));
        }

        public void Deserialize (XmlReader reader)
        {
            DeserializeCore (reader);
            VerifyDeserialization ();
            loaded = true;
        }

        protected virtual void DeserializeCore (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                reader.ReadToFollowing ("action");
                while (Helper.ReadToNextElement (reader)) {
                    try {
                        DeserializeCore (reader.ReadSubtree (), reader.Name);
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing one of the action description elements.", e);
                    }
                }
                reader.Close ();
            } catch (Exception e) {
                throw new UpnpDeserializationException (string.Format ("There was a problem deserializing {0}.", ToString ()), e);
            }
        }

        protected virtual void DeserializeCore (XmlReader reader, string element)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            reader.Read ();
            switch (element.ToLower ()) {
            case "name":
                Name = reader.ReadString ();
                break;
            case "argumentlist":
                DeserializeArguments (reader.ReadSubtree ());
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        protected virtual void DeserializeArguments (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            while (reader.ReadToFollowing ("argument")) {
                try {
                    DeserializeArgument (reader.ReadSubtree ());
                } catch (Exception e) {
                    Log.Exception ("There was a problem deserializing an argument list element.", e);
                }
            }
            reader.Close ();
        }

        protected virtual void DeserializeArgument (XmlReader reader)
        {
            Argument argument = CreateArgument ();
            argument.Deserialize (reader);
            AddArgument (argument);
        }

        protected virtual Argument CreateArgument ()
        {
            return new Argument (this);
        }

        protected void AddArgument (Argument argument)
        {
            CheckLoaded ();
            if (argument.Direction == ArgumentDirection.In) {
                in_argument_dict.Add (argument.Name, argument);
            } else {
                if (argument.IsReturnValue && !bypass_return_argument) {
                    if (return_argument == null) {
                        return_argument = argument;
                    } else {
                        Log.Exception (new UpnpDeserializationException (
                            string.Format ("{0} has multiple return values.", ToString ())));
                        out_argument_dict.Add (return_argument.Name, return_argument);
                        out_argument_dict.Add (argument.Name, argument);
                        return_argument = null;
                        bypass_return_argument = true;
                    }
                } else {
                    out_argument_dict.Add (argument.Name, argument);
                }
            }
        }

        void VerifyDeserialization ()
        {
            if (name == null) {
                throw new UpnpDeserializationException (string.Format ("The action on {0} has no name.", controller));
            }
            if (name.Length == 0) {
                Log.Exception (new UpnpDeserializationException (string.Format ("The action on {0} has an empty name.", controller)));
            }
        }

        public override string ToString ()
        {
            return String.Format ("Action {{ {0}, {1} }}", controller, name);
        }
    }
}
