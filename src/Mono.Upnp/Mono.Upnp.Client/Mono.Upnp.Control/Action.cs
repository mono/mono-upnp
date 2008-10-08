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
        #region Constructors

        protected Action (Service service, XmlReader reader)
            : this (service, reader, null)
        {
        }

        protected internal Action (Service service, XmlReader reader, WebHeaderCollection headers)
        {
            if (service == null) {
                throw new ArgumentNullException ("service");
            }
            this.service = service;
            Deserialize (reader, headers);
        }

        #endregion

        #region Data

        private Service service;
        public Service Service {
            get { return service; }
        }

        public bool Disposed {
            get { return service.Disposed; }
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

        #endregion

        #region Methods

        public virtual void Invoke ()
        {
            CheckDisposed ();
            service.Invoke (this);
        }

        protected void CheckDisposed ()
        {
            if (Disposed) {
                throw new ObjectDisposedException (ToString (),
                    "This action is no longer available because its service has gone off the network.");
            }
        }

        #region Overrides

        public override string ToString ()
        {
            return String.Format ("Action {{ {0}, {1} }}", service, name);
        }

        public override bool Equals (object obj)
        {
            Action action = obj as Action;
            return action != null &&
                action.service.Equals (service) &&
                action.name == name &&
                action.in_arguments.Equals (in_arguments) &&
                action.out_arguments.Equals (out_arguments);
        }

        public override int GetHashCode ()
        {
            return service.GetHashCode () ^
                (name == null ? 0 : name.GetHashCode ()) ^
                in_arguments.GetHashCode () ^
                out_arguments.GetHashCode ();
        }

        #endregion

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
            string value = reader.ReadString ();
            if (return_argument != null && return_argument.Name == reader.Name) {
                return_argument.Value = value;
            } else if (out_arguments.ContainsKey (reader.Name)) {
                out_arguments[reader.Name].Value = value;
            }
            reader.Close ();
        }

        protected internal virtual void DeserializeResponseFault (HttpWebResponse response)
        {
            throw new UpnpControlException (XmlReader.Create (response.GetResponseStream ()));
        }

        private WebHeaderCollection headers;

        private void Deserialize (XmlReader reader, WebHeaderCollection headers)
        {
            this.headers = headers;
            in_argument_dict = new Dictionary<string, Argument> ();
            in_arguments = new ReadOnlyDictionary<string,Argument> (in_argument_dict);
            out_argument_dict = new Dictionary<string, Argument> ();
            out_arguments = new ReadOnlyDictionary<string,Argument> (out_argument_dict);

            Deserialize (headers);
            Deserialize (reader);
            VerifyDeserialization ();

            this.headers = null;
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
            while (reader.ReadToFollowing ("argument") && reader.NodeType == XmlNodeType.Element) {
                DeserializeArgument (reader.ReadSubtree ());
            }
            reader.Close ();
        }

        protected virtual void DeserializeArgument (XmlReader reader)
        {
            AddArgument (new Argument (this, reader, headers));
        }

        protected void AddArgument (Argument argument)
        {
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

        protected virtual void VerifyDeserialization ()
        {
            if (String.IsNullOrEmpty (name)) {
                throw new UpnpDeserializationException ("The action has no name.");
            }
        }

        protected internal virtual void VerifyContract ()
        {
            foreach (Argument argument in in_arguments.Values) {
                argument.VerifyContract ();
            }
            foreach (Argument argument in out_arguments.Values) {
                argument.VerifyContract ();
            }
            if (return_argument != null) {
                return_argument.VerifyContract ();
            }
        }

        #endregion

        #endregion

    }
}
