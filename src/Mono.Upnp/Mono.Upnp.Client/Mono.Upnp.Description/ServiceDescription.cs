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
        IDeserializer deserializer;
        ServiceController controller;

        protected internal ServiceDescription (IDisposer disposer)
        {
            if (disposer == null) throw new ArgumentNullException ("disposer");

            this.disposer = disposer;
        }
        
        public event EventHandler<DisposedEventArgs> Disposed;

        public ServiceType Type { get; private set; }
        public string Id { get; private set; }
        public Uri ScpdUrl { get; private set; }
        public Uri ControlUrl { get; private set; }
        public Uri EventUrl { get; private set; }
        public bool IsDisposed { get; private set; }

        protected internal void CheckDisposed ()
        {
            disposer.TryDispose (this);
            if (IsDisposed) {
                throw new ObjectDisposedException (ToString (), "The service has gone off the network.");
            }
        }

        protected internal void Dispose ()
        {
            if (IsDisposed) {
                return;
            }

            IsDisposed = true;
            OnDispose (DisposedEventArgs.Empty);

            if (controller != null) {
                controller.Dispose ();
                controller = null;
            }
        }

        protected virtual void OnDispose (DisposedEventArgs e)
        {
            var handler = Disposed;
            if (handler != null) {
                handler (this, e);
            }
        }

        public ServiceController GetController ()
        {
            if (controller == null && deserializer != null) {
                if (IsDisposed) {
                    throw new ObjectDisposedException (ToString (), "The service has gone off the network.");
                }
                if (ScpdUrl == null) {
                    throw new InvalidOperationException (
                        "Attempting to get a controller from a services with no SCPDURL.");
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
                            "There was a problem deserializing one of the service description elements.", e);
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
            }
        }

        void VerifyDeserialization ()
        {
            if (Id == null) {
                var message = Type == null
                    ? string.Format ("The service has no ID or type.")
                    : string.Format ("The service of type {0} has no id.", Type);
                throw new UpnpDeserializationException (message);
            }
            if (Type == null) {
                throw new UpnpDeserializationException (
					string.Format ("The service {0} has no type.", Id));
            }
            if (ScpdUrl == null) {
                Log.Exception (new UpnpDeserializationException (
					string.Format ("{0} has no SCPD URL.", ToString ())));
            }
            if (ControlUrl == null) {
                Log.Exception (new UpnpDeserializationException (
					string.Format ("{0} has no control URL.", ToString ())));
            }
            if (EventUrl == null) {
                Log.Exception (new UpnpDeserializationException (
					string.Format ("{0} has no event URL.", ToString ())));
            }
        }

        public override string ToString ()
        {
            return string.Format ("ServiceDescription {{ {0}, {1} }}", Id, Type);
        }
	}
}
