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

namespace Mono.Upnp.Description
{
	public class Icon
	{
        readonly DeviceDescription device;
        int? width;
        int? height;
        int? depth;
        byte[] data;
        IDeserializer deserializer;

        protected internal Icon (DeviceDescription device)
        {
            if (device == null) throw new ArgumentNullException ("device");
            this.device = device;
        }

        public DeviceDescription Device { get { return device; } }
        public bool IsDisposed { get { return device.IsDisposed; } }
		public Uri Url { get; private set; }
        public string MimeType { get; private set; }
        public int Width { get { return width.Value; } }
        public int Height { get { return height.Value; } }
        public int Depth { get { return depth.Value; } }

        public byte[] GetData ()
        {
            if (data == null) {
                try {
                    var request = (HttpWebRequest)WebRequest.Create (Url);
                    using (var response = Helper.GetResponse (request)) {
                        data = new byte[response.ContentLength];
                        using (var stream = response.GetResponseStream ()) {
                            stream.Read (data, 0, (int)response.ContentLength);
                        }
                    }
                } catch (WebException e) {
                    if (e.Status == WebExceptionStatus.Timeout) {
                        device.CheckDisposed ();
                    }
                    throw e;
                }
            }
            var copy = new byte [data.Length];
            Array.Copy (data, copy, data.Length);
            return copy;
        }

        public override string ToString ()
        {
            return string.Format ("Icon {{ {0}, {1}x{2}x{3}, {4} }}", MimeType, width, height, depth, Url);
        }

        public override bool Equals (object obj)
        {
            var icon = obj as Icon;
            return icon != null && icon.Url == Url;
        }

        public override int GetHashCode ()
        {
            return ~Url.GetHashCode ();
        }

        public void Deserialize (IDeserializer deserializer, XmlReader reader)
        {
            if (deserializer == null) throw new ArgumentNullException ("deserializer");

            this.deserializer = deserializer;
            DeserializeCore (reader);
            Verify ();
            this.deserializer = null;
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
                            "There was a problem deserializing one of the icon description elements.", e);
                    }
                }
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deserializing an icon.", e);
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
                case "mimetype":
                    MimeType = reader.ReadString ();
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
                    Url = deserializer.DeserializeUrl (reader.ReadSubtree ());
                    break;
                default: // This is a workaround for Mono bug 334752
                    reader.Skip ();
                    break;
                }
            }
        }

        void Verify ()
        {
            if (Url == null) {
                throw new UpnpDeserializationException ("The icon has no URL.");
            }
            if (MimeType == null) {
                Log.Exception (new UpnpDeserializationException ("The icon has no mime-type."));
            }
            if (width == null) {
                Log.Exception (new UpnpDeserializationException ("The icon has no width."));
                width = 0;
            }
            if (height == null) {
                Log.Exception (new UpnpDeserializationException ("The icon has no height."));
                height = 0;
            }
            if (depth == null) {
                Log.Exception (new UpnpDeserializationException ("The icon has no depth."));
                depth = 0;
            }
        }
    }
}
