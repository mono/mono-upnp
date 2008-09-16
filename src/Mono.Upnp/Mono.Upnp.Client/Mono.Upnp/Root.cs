//
// Root.cs
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
using System.Net;
using System.Xml;

using Mono.Ssdp;
using Mono.Upnp.Internal;

namespace Mono.Upnp
{
	public class Root
	{
        private Client client;

        internal Root (Client client, Uri url_base, WebHeaderCollection headers, XmlReader reader)
        {
            this.client = client;
            this.url_base = url_base;
            Deserialize (headers, reader);
        }

        private Uri url_base;
        internal Uri UrlBase {
            get { return url_base; }
        }

        private Version spec_version;
        public Version SpecVersion {
            get { return spec_version; }
        }

        private Device device;
        public Device Device {
            get { return device; }
        }

        #region Deserialization

        private void Deserialize (WebHeaderCollection headers, XmlReader reader)
        {
            try {
                reader.ReadToFollowing ("root");
                while (Helper.ReadToNextElement (reader)) {
                    Deserialize (headers, reader.ReadSubtree (), reader.Name);
                }
                reader.Close ();
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deserializing a root XML file.", e);
            }
            Verify ();
        }

        private void Deserialize (WebHeaderCollection headers, XmlReader reader, string element)
        {
            reader.Read ();
            switch (element) {
            case "specVersion":
                spec_version = Helper.DeserializeSpecVersion (reader.ReadSubtree ());
                break;
            case "URLBase":
                url_base = new Uri (reader.ReadString ());
                break;
            case "device":
                if (device != null) {
                    throw new UpnpDeserializationException ("There are multiple root devices.");
                }
                device = new Device (client, this, headers, reader.ReadSubtree ());
                if (client.Devices.ContainsKey (device.Udn)) {
                    foreach (Device d in client.Devices[device.Udn]) {
                        if (d.Type == device.Type) {
                            d.CopyFrom (device);
                            device = d;
                        }
                    }
                }
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        internal Uri DeserializeUrl (XmlReader reader)
        {
            try {
                reader.Read ();
                string url = reader.ReadString ();
                Uri uri = Uri.IsWellFormedUriString (url, UriKind.Absolute) ? new Uri (url) : new Uri (url_base, url);
                reader.Close ();
                return uri;
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deserializing a URL.", e);
            }
        }

        private void Verify ()
        {
            if (device == null) {
                throw new UpnpDeserializationException ("There is no root device.");
            }
            if (spec_version == null) {
                throw new UpnpDeserializationException ("The UPnP spec version is not specified.");
            }
            if (spec_version.Major > 1) {
                throw new UpnpDeserializationException (String.Format (
                    "The UPnP spec version {0} is not supported.", spec_version));
            }
        }

        #endregion

    }
}
