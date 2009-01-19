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
using System.IO;
using System.Net;
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp.Description
{
	public class Icon
	{
        readonly DeviceDescription device;
        string mime_type;
        int? width;
        int? height;
        int? depth;
        Uri url;
        byte[] data;
        IDeserializer deserializer;
        bool loaded;

        protected internal Icon (DeviceDescription device)
        {
            if (device == null) {
                throw new ArgumentNullException ("device");
            }
            this.device = device;
        }

        public DeviceDescription Device {
            get { return device; }
        }

        public bool IsDisposed {
            get { return device.IsDisposed; }
        }

        public string MimeType {
            get { return mime_type; }
            protected set { SetField (ref mime_type, value); }
        }

        public int Width {
            get { return width.Value; }
            protected set { SetField (ref width, value); }
        }

        public int Height {
            get { return height.Value; }
            protected set { SetField (ref height, value); }
        }

        public int Depth {
            get { return depth.Value; }
            protected set { SetField (ref depth, value); }
        }

        public Uri Url {
            get { return url; }
            protected set { SetField (ref url, value); }
        }

        public byte[] GetData ()
        {
            if (data == null) {
                try {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
                    using (HttpWebResponse response = Helper.GetResponse (request)) {
                        data = new byte[response.ContentLength];
                        using (Stream stream = response.GetResponseStream ()) {
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
            byte[] copy = new byte [data.Length];
            Array.Copy (data, copy, data.Length);
            return copy;
        }

        void SetField<T> (ref T field, T value)
        {
            CheckLoaded ();
            field = value;
        }

        void SetField<T> (ref T? field, T value) where T : struct
        {
            CheckLoaded ();
            field = value;
        }

        void CheckLoaded ()
        {
            if (loaded) {
                throw new InvalidOperationException ("The icon has already been deserialized.");
            }
        }

        public override string ToString ()
        {
            return String.Format ("Icon {{ {0}, {1}x{2}x{3}, {4} }}", mime_type, width, height, depth, url);
        }

        public override bool Equals (object obj)
        {
            Icon icon = obj as Icon;
            return icon != null && icon.url == url;
        }

        public override int GetHashCode ()
        {
            return ~url.GetHashCode ();
        }

        public void Deserialize (IDeserializer deserializer, XmlReader reader)
        {
            if (deserializer == null) throw new ArgumentNullException ("deserializer");

            this.deserializer = deserializer;
            DeserializeCore (reader);
            Verify ();

            this.deserializer = null;
            loaded = true;
        }

        protected virtual void DeserializeCore (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                reader.ReadToFollowing ("icon");
                while (Helper.ReadToNextElement (reader)) {
                    try {
                        DeserializeCore (reader.ReadSubtree (), reader.Name);
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing one of the icon description elements.", e);
                    }
                }
                reader.Close ();
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deserializing an icon.", e);
            }
        }

        protected virtual void DeserializeCore (XmlReader reader, string element)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            reader.Read ();
            switch (element.ToLower ()) {
            case "mimetype":
                MimeType = reader.ReadString ();
                break;
            case "width":
                reader.Read ();
                Width = reader.ReadContentAsInt ();
                break;
            case "height":
                reader.Read ();
                Height = reader.ReadContentAsInt ();
                break;
            case "depth":
                reader.Read ();
                Depth = reader.ReadContentAsInt ();
                break;
            case "url":
                Url = deserializer.DeserializeUrl (reader.ReadSubtree ());
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        void Verify ()
        {
            if (url == null) {
                throw new UpnpDeserializationException ("The icon has no URL.");
            }
            if (mime_type == null) {
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
