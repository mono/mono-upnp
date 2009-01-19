//
// ServiceDescription.cs
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
using System.Xml;

using Mono.Upnp.Control;
using Mono.Upnp.Internal;

namespace Mono.Upnp.Description
{
	public class ServiceDescription
	{
        readonly IDisposer disposer;
        ServiceController controller;
        ServiceType type;
        string id;
        Uri scpd_url;
        Uri control_url;
        Uri event_url;
        IDeserializer deserializer;
        bool loaded;
        bool is_disposed;

        public event EventHandler<DisposedEventArgs> Disposed;

        protected internal ServiceDescription (IDisposer disposer)
        {
            if (disposer == null) throw new ArgumentNullException ("disposer");

            this.disposer = disposer;
        }

        public ServiceType Type {
            get { return type; }
            protected set { SetField (ref type, value); }
        }

        public string Id {
            get { return id; }
            protected set { SetField (ref id, value); }
        }

        public Uri ScpdUrl {
            get { return scpd_url; }
            protected set { SetField (ref scpd_url, value); }
        }

        public Uri ControlUrl {
            get { return control_url; }
            protected set { SetField (ref control_url, value); }
        }

        public Uri EventUrl {
            get { return event_url; }
            protected set { SetField (ref event_url, value); }
        }

        void SetField<T> (ref T field, T value)
        {
            if (loaded) throw new InvalidOperationException ("The service description has already been deserialized.");
            field = value;
        }

        public bool IsDisposed {
            get { return is_disposed; }
        }

        protected internal void CheckDisposed ()
        {
            disposer.TryDispose (this);
            if (is_disposed) {
                throw new ObjectDisposedException (ToString (), "The service has gone off the network.");
            }
        }

        protected internal void Dispose ()
        {
            if (is_disposed) {
                return;
            }

            is_disposed = true;
            OnDispose ();

            if (controller != null) {
                controller.Dispose ();
                controller = null;
            }
        }

        protected virtual void OnDispose ()
        {
            EventHandler<DisposedEventArgs> handler = Disposed;
            if (handler != null) {
                handler (this, DisposedEventArgs.Empty);
            }
        }

        public ServiceController GetController ()
        {
            if (controller == null) {
                if (is_disposed) {
                    throw new ObjectDisposedException (ToString (), "The service has gone off the network.");
                }
                if (deserializer == null) {
                    throw new InvalidOperationException ("The service description has not been deserialized.");
                }
                if (scpd_url == null) {
                    throw new InvalidOperationException ("Attempting to get a controller from a services with no SCPDURL.");
                }
                controller = deserializer.DeserializeController (this);
                deserializer = null;
            }
            return controller;
        }

        public void Deserialize (IDeserializer deserializer, XmlReader reader)
        {
            if (deserializer == null) throw new ArgumentNullException ("deserializer");

            this.deserializer = deserializer;
            DeserializeCore (reader);
            VerifyDeserialization ();

            loaded = true;
        }

        protected virtual void DeserializeCore (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                reader.ReadToFollowing ("service");
                while (Helper.ReadToNextElement (reader)) {
                    try {
                        DeserializeCore (reader.ReadSubtree (), reader.Name);
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing one of the service description elements.", e);
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
            case "servicetype":
                Type = new ServiceType (reader.ReadString ());
                break;
            case "serviceid":
                // TODO better handling of this complex string
                Id = reader.ReadString ().Trim ();
                break;
            case "scpdurl":
                ScpdUrl = deserializer.DeserializeUrl (reader.ReadSubtree ());
                break;
            case "controlurl":
                ControlUrl = deserializer.DeserializeUrl (reader.ReadSubtree ());
                break;
            case "eventsuburl":
                EventUrl = deserializer.DeserializeUrl (reader.ReadSubtree ());
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        void VerifyDeserialization ()
        {
            if (id == null) {
                string message = type == null
                    ? string.Format ("A service has no ID or type.")
                    : string.Format ("A service of type {0} has no id.", type);
                throw new UpnpDeserializationException (message);
            }
            if (type == null) {
                throw new UpnpDeserializationException (string.Format ("The service {0} has no type.", id));
            }
            if (scpd_url == null) {
                Log.Exception (new UpnpDeserializationException (string.Format ("{0} has no SCPD URL.", ToString ())));
            }
            if (control_url == null) {
                Log.Exception (new UpnpDeserializationException (string.Format ("{0} has no control URL.", ToString ())));
            }
            if (event_url == null) {
                Log.Exception (new UpnpDeserializationException (string.Format ("{0} has no event URL.", ToString ())));
            }
        }

        public override string ToString ()
        {
            return String.Format ("ServiceDescription {{ {0}, {1} }}", id, type);
        }
	}
}
