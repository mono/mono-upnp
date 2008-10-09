//
// Icon.cs
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
using System.Net;
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp
{
	public class Icon
	{
        protected internal Icon (Device device, XmlReader reader, WebHeaderCollection headers)
        {
            if (device == null) {
                throw new ArgumentNullException ("device");
            }
            this.device = device;
            Deserialize (reader, headers);
            loaded = true;
        }

        #region Data

        private readonly bool loaded;

        private readonly Device device;
        public Device Device {
            get { return device; }
        }

        public bool Disposed {
            get { return device.Disposed; }
        }

        private string mime_type;
        public string MimeType {
            get { return mime_type; }
        }

        private int? width;
        public int Width {
            get { return width.Value; }
        }

        private int? height;
        public int Height {
            get { return height.Value; }
        }

        private int? depth;
        public int Depth {
            get { return depth.Value; }
        }

        private Uri url;
        public Uri Url {
            get { return url; }
        }

        private byte[] data;
        public byte[] Data {
            get {
                if (data == null) {
                    CheckDisposed ();
                    WebRequest request = WebRequest.Create (Url);
                    WebResponse response =  request.GetResponse ();
                    data = new byte[response.ContentLength];
                    response.GetResponseStream ().Read (data, 0, (int)response.ContentLength);
                    response.Close ();
                }
                return data;
            }
        }

        #endregion

        #region Methods

        protected void CheckDisposed ()
        {
            if (Disposed) {
                throw new ObjectDisposedException (ToString (),
                    "This icon is longer available because its device has gone off the network.");
            }
        }

        #region Overrides

        public override string ToString ()
        {
            return String.Format ("Icon {{ {0}, {1}x{2}x{3}, {4} }}", mime_type, Width, Height, Depth, url);
        }

        public override bool Equals (object obj)
        {
            Icon icon = obj as Icon;
            return icon != null &&
                icon.device.Equals (device) &&
                icon.mime_type == mime_type &&
                icon.width == width &&
                icon.height == height &&
                icon.depth == depth &&
                icon.url == url;
        }

        private int hash;
        private bool computed_hash;
        public override int GetHashCode ()
        {
            if (!computed_hash) {
                hash = device.GetHashCode () ^
                    (mime_type == null ? 0 : mime_type.GetHashCode ()) ^
                    (width.HasValue ? width.Value : 0) ^
                    (height.HasValue ? (height.Value << 8 | height.Value >> 24) : 0) ^
                    (depth.HasValue ? (depth.Value << 16 | depth.Value >> 16) : 0) ^
                    (url == null ? 0 : url.GetHashCode ());
                computed_hash = true;
            }
            return hash;
        }

        #endregion

        #region Deserialize

        private void Deserialize (XmlReader reader, WebHeaderCollection headers)
        {
            Deserialize (headers);
            Deserialize (reader);
            VerifyDeserialization ();
        }

        protected virtual void Deserialize (WebHeaderCollection headers)
        {
        }

        protected virtual void Deserialize (XmlReader reader)
        {
            try {
                reader.Read ();
                while (reader.Read () && reader.NodeType == XmlNodeType.Element) {
                    Deserialize (reader.ReadSubtree (), reader.Name);
                }
                reader.Close ();
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deserializing an icon.", e);
            }
        }

        protected virtual void Deserialize (XmlReader reader, string element)
        {
            if (loaded) {
                throw new InvalidOperationException ("The icon has already been deserialized.");
            }

            reader.Read ();
            switch (element) {
            case "mimetype":
                mime_type = reader.ReadString ();
                break;
            case "width":
                reader.Read ();
                width = reader.ReadContentAsInt ();
                break;
            case "height":
                reader.Read ();
                height = reader.ReadContentAsInt ();
                break;
            case "depth":
                reader.Read ();
                depth = reader.ReadContentAsInt ();
                break;
            case "url":
                url = device.Root.DeserializeUrl (reader.ReadSubtree ());
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        protected virtual void VerifyDeserialization ()
        {
            if (mime_type == null) {
                throw new UpnpDeserializationException ("The icon has no mime-type.");
            }
            if (width == null) {
                throw new UpnpDeserializationException ("The icon has no width.");
            }
            if (height == null) {
                throw new UpnpDeserializationException ("The icon has no height.");
            }
            if (depth == null) {
                throw new UpnpDeserializationException ("The icon has no depth.");
            }
            if (url == null) {
                throw new UpnpDeserializationException ("The icon has no URL.");
            }
        }

        protected internal virtual void VerifyContract ()
        {
        }

        #endregion

        #endregion

    }
}
