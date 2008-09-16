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
using System.Text;
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp.Control
{
	public class Action
	{
        protected internal Action (Service service, WebHeaderCollection headers, XmlReader reader)
        {
            this.service = service;
            Deserialize (headers, reader);
        }

        private Service service;
        internal Service Service {
            get { return service; }
        }

        private string name;
        public string Name {
            get { return name; }
        }

        private bool bypass_return_argument;

        private Argument return_argument;
        public Argument ReturnArgument {
            get { return return_argument; }
        }

        private Dictionary<string, Argument> in_argument_dict;
        private ReadOnlyDictionary<string, Argument> in_arguments;
        public ReadOnlyDictionary<string, Argument> InArguments {
            get { return in_arguments; }
        }

        private Dictionary<string, Argument> out_argument_dict;
        private ReadOnlyDictionary<string, Argument> out_arguments;
        public ReadOnlyDictionary<string, Argument> OutArguments {
            get { return out_arguments; }
        }

        private int retry;
        public int Retry {
            get { return retry; }
            set { retry = value; }
        }

        public virtual void Execute ()
        {
            service.SoapAdapter.Invoke (this);
        }

        #region Deserialization

        protected internal virtual void SerializeRequest (WebHeaderCollection headers, XmlWriter writer)
        {
            Helper.WriteStartSoapBody (writer);
            SerializeRequestSoapBody (writer);
            Helper.WriteEndSoapBody (writer);
        }

        protected virtual void SerializeRequestSoapBody (XmlWriter writer)
        {
            writer.WriteStartElement ("u", name, service.Type.ToString ());
            foreach (Argument argument in in_arguments.Values) {
                writer.WriteStartElement (argument.Name);
                writer.WriteValue (argument.Value ?? "");
                writer.WriteEndElement ();
            }
            writer.WriteEndElement ();
        }

        protected internal virtual void DeserializeResponse (HttpWebResponse response)
        {
            XmlReader reader = XmlReader.Create (response.GetResponseStream ());
            reader.ReadToFollowing (name + "Response", service.Type.ToString ());
            while (Helper.ReadToNextElement (reader)) {
                DeserializeResponse (reader.ReadSubtree ());
            }
            reader.Close ();
        }

        private void DeserializeResponse (XmlReader reader)
        {
            reader.Read ();
            out_arguments[reader.Name].Value = reader.ReadString ();
            reader.Close ();
        }

        protected internal virtual void DeserializeResponseFault (HttpWebResponse response)
        {
            throw new UpnpControlException (XmlReader.Create (response.GetResponseStream ()));
        }

        private void Deserialize (WebHeaderCollection headers, XmlReader reader)
        {
            in_argument_dict = new Dictionary<string, Argument> ();
            in_arguments = new ReadOnlyDictionary<string,Argument> (in_argument_dict);
            out_argument_dict = new Dictionary<string, Argument> ();
            out_arguments = new ReadOnlyDictionary<string,Argument> (out_argument_dict);

            Deserialize (headers);
            Deserialize (reader);
            Verify ();
        }

        protected virtual void Deserialize (WebHeaderCollection headers)
        {
        }

        protected virtual void Deserialize (XmlReader reader)
        {
            try {
                reader.Read ();
                while (Helper.ReadToNextElement (reader)) {
                    Deserialize (reader.ReadSubtree (), reader.Name);
                }
                reader.Close ();
            } catch (Exception e) {
                string message = String.IsNullOrEmpty (name)
                    ? "There was a problem deserializing an action."
                    : String.Format ("There was a problem deserializing the action {0}.", name);
                throw new UpnpDeserializationException (message, e);
            }
        }

        protected virtual void Deserialize (XmlReader reader, string element)
        {
            reader.Read ();
            switch (element) {
            case "name":
                name = reader.ReadString ();
                break;
            case "argumentList":
                DeserializeArgumentList (reader.ReadSubtree ());
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        private void DeserializeArgumentList (XmlReader reader)
        {
            while (reader.ReadToFollowing ("argument") && reader.NodeType == XmlNodeType.Element) {
                Argument argument = new Argument (this, reader.ReadSubtree ());
                if (argument.Direction == ArgumentDirection.In) {
                    in_argument_dict.Add (argument.Name, argument);
                } else {
                    if (argument.IsReturnValue && !bypass_return_argument) {
                        if (return_argument == null) {
                            return_argument = argument;
                        } else {
                            string message = String.IsNullOrEmpty (name)
                                ? "An action has multiple return values."
                                : String.Format ("The action {0} has multiple return values.", name);
                            Log.Exception (new UpnpDeserializationException (message));
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
            reader.Close ();
        }

        protected virtual void Verify ()
        {
            if (String.IsNullOrEmpty (name)) {
                throw new UpnpDeserializationException ("The action has no name.");
            }
        }

        #endregion

    }
}
