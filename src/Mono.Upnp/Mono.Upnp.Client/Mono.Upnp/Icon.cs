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
        internal Icon (Device device)
        {
            this.device = device;
        }

        private Device device;

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
                    WebRequest request = WebRequest.Create (Url);
                    WebResponse response =  request.GetResponse ();
                    data = new byte[response.ContentLength];
                    response.GetResponseStream ().Read (data, 0, (int)response.ContentLength);
                    response.Close ();
                }
                return data;
            }
        }

        #region Deserialize

        internal void Deserialize (XmlReader reader)
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

        private void Deserialize (XmlReader reader, string element)
        {
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

        private void Verify ()
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

        #endregion

	}
}
