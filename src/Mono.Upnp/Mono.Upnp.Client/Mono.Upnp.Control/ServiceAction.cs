//
// ServiceAction.cs
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
using System.Net;
using System.Reflection;
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp.Control
{
	public class ServiceAction
    {
        readonly static Dictionary<string, string> emptyArguments = new Dictionary<string, string> ();
        readonly ServiceController controller;
        readonly Dictionary<string, Argument> in_argument_dict = new Dictionary<string,Argument> ();
        readonly ReadOnlyDictionary<string, Argument> in_arguments;
        readonly Dictionary<string, Argument> out_argument_dict = new Dictionary<string,Argument> ();
        readonly ReadOnlyDictionary<string, Argument> out_arguments;
        bool bypass_return_argument;

        protected internal ServiceAction (ServiceController serviceController)
        {
            if (serviceController == null) throw new ArgumentNullException ("serviceController");

            controller = serviceController;
            in_arguments = new ReadOnlyDictionary<string, Argument> (in_argument_dict);
            out_arguments = new ReadOnlyDictionary<string, Argument> (out_argument_dict);
        }

        public ServiceController Controller {
            get { return controller; }
        }

        public string Name { get; private set; }
        
        public Argument ReturnArgument { get; private set; }

        public ReadOnlyDictionary<string, Argument> InArguments {
            get { return in_arguments; }
        }

        public ReadOnlyDictionary<string, Argument> OutArguments {
            get { return out_arguments; }
        }

        public int Retry { get; set; }

        public bool IsDisposed {
            get { return controller.IsDisposed; }
        }

        public ActionResult Invoke ()
        {
            return Invoke (emptyArguments);
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

        void VerifyArguments (IDictionary<string, string> arguments)
        {
            foreach (var pair in arguments) {
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
            
            var type = argument.RelatedStateVariable.Type;
            var values = argument.RelatedStateVariable.AllowedValues;
            if (values != null && type == typeof (string) && !values.Contains (value)) {
                throw new ArgumentException (
                    string.Format ("The value {0} is not allowed for the argument {1}.", value, argument.Name));
            }
            
            var range = argument.RelatedStateVariable.AllowedValueRange;
            if (range != null && type is IComparable) {
                var parse = type.GetMethod ("Parse", BindingFlags.Public | BindingFlags.Static);
                var arg = parse.Invoke (null, new object[] { value });
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
        
        internal void SerializeRequest (IDictionary<string, string> arguments,
                                         WebHeaderCollection headers, XmlWriter writer)
        {
            SerializeRequestCore (arguments, headers, writer);
        }

        protected virtual void SerializeRequestCore (IDictionary<string, string> arguments,
                                                       WebHeaderCollection headers, XmlWriter writer)
        {
            Helper.WriteStartSoapBody (writer);
            SerializeRequestCore (arguments, writer);
            Helper.WriteEndSoapBody (writer);
        }

        protected virtual void SerializeRequestCore (IDictionary<string, string> arguments, XmlWriter writer)
        {
            writer.WriteStartElement ("u", Name, controller.Description.Type.ToString ());
            foreach (var argument in arguments) {
                writer.WriteStartElement (argument.Key);
                writer.WriteValue (argument.Value ?? "");
                writer.WriteEndElement ();
            }
            writer.WriteEndElement ();
        }
        
        internal ActionResult DeserializeResponse (HttpWebResponse response)
        {
            return DeserializeResponseCore (response);
        }

        protected virtual ActionResult DeserializeResponseCore (HttpWebResponse response)
        {
            using (var reader = XmlReader.Create (response.GetResponseStream ())) {
                reader.ReadToFollowing (Name + "Response", controller.Description.Type.ToString ());
                var out_args = new Dictionary<string, string> ();
                string return_value = null;
                while (Helper.ReadToNextElement (reader)) {
                    if (ReturnArgument != null && ReturnArgument.Name == reader.Name) {
                        return_value = reader.ReadString ();
                    } else {
                        out_args [reader.Name] = reader.ReadString ();
                    }
                }
                return new ActionResult (return_value, out_args);
            }
        }
        
        internal void DeserializeResponseFault (HttpWebResponse response)
        {
            DeserializeResponseFaultCore (response);
        }

        protected virtual void DeserializeResponseFaultCore (HttpWebResponse response)
        {
            throw new UpnpControlException (XmlReader.Create (response.GetResponseStream ()));
        }

        public void Deserialize (XmlReader reader)
        {
            DeserializeCore (reader);
            VerifyDeserialization ();
        }

        protected virtual void DeserializeCore (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                reader.Read ();
                while (Helper.ReadToNextElement (reader)) {
                    try {
                        DeserializeCore (reader.ReadSubtree (), reader.Name);
                    } catch (Exception e) {
                        Log.Exception (
                            "There was a problem deserializing one of the action description elements.", e);
                    }
                }
            } catch (Exception e) {
                throw new UpnpDeserializationException (
                    string.Format ("There was a problem deserializing {0}.", ToString ()), e);
            } finally {
                reader.Close ();
            }
        }

        protected virtual void DeserializeCore (XmlReader reader, string element)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            using (reader) {
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
            }
        }

        protected virtual void DeserializeArguments (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            using (reader) {
                while (reader.ReadToFollowing ("argument")) {
                    try {
                        DeserializeArgument (reader.ReadSubtree ());
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing an argument list element.", e);
                    }
                }
            }
        }

        protected virtual void DeserializeArgument (XmlReader reader)
        {
            var argument = CreateArgument ();
            if (argument != null) {
                argument.Deserialize (reader);
                AddArgument (argument);
            }
        }

        protected virtual Argument CreateArgument ()
        {
            return new Argument (this);
        }

        protected void AddArgument (Argument argument)
        {
            if (argument == null) throw new ArgumentNullException ("argument");
            
            if (argument.Direction == ArgumentDirection.In) {
                in_argument_dict [argument.Name] = argument;
            } else {
                if (argument.IsReturnValue && !bypass_return_argument) {
                    if (ReturnArgument == null) {
                        ReturnArgument = argument;
                    } else {
                        out_argument_dict [ReturnArgument.Name] = ReturnArgument;
                        out_argument_dict [argument.Name] = argument;
                        ReturnArgument = null;
                        bypass_return_argument = true;
                        Log.Exception (new UpnpDeserializationException (
                            string.Format ("{0} has multiple return values.", ToString ())));
                    }
                } else {
                    out_argument_dict [argument.Name] = argument;
                }
            }
        }

        void VerifyDeserialization ()
        {
            if (Name == null) {
                throw new UpnpDeserializationException (
                    string.Format ("The action on {0} has no name.", controller));
            }
            if (Name.Length == 0) {
                Log.Exception (new UpnpDeserializationException (
                    string.Format ("The action on {0} has an empty name.", controller)));
            }
        }

        public override string ToString ()
        {
            return String.Format ("ServiceAction {{ {0} }}", Name);
        }
    }
}
