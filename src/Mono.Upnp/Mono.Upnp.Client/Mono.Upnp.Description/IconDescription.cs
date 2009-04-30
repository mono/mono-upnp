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

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Description
{
	public class IconDescription : Description, IXmlDeserializable
	{
        readonly DeviceDescription device;
        byte[] data;

        protected internal IconDescription (DeviceDescription device)
        {
            if (device == null) throw new ArgumentNullException ("device");
            
            this.device = device;
        }

        public DeviceDescription Device { get { return device; } }
        public bool IsDisposed { get { return device.IsDisposed; } }
		[XmlElement ("url")] public Uri Url { get; set; }
        [XmlElement ("mimetype")] public string MimeType { get; set; }
        [XmlElement ("width")] public int Width { get; set; }
        [XmlElement ("height")] public int Height { get; set; }
        [XmlElement ("depth")] public int Depth { get; set; }

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
            return string.Format ("Icon {{ {0}, {1}x{2}x{3}, {4} }}", MimeType, Width, Height, Depth, Url);
        }

        public override bool Equals (object obj)
        {
            var icon = obj as IconDescription;
            return icon != null && icon.Url == Url;
        }

        public override int GetHashCode ()
        {
            return ~Url.GetHashCode ();
        }

        protected virtual void VerifyDeserialization ()
        {
            if (Url == null) {
                throw new UpnpDeserializationException ("The icon has no URL.");
            }
            if (MimeType == null) {
                Log.Exception (new UpnpDeserializationException ("The icon has no mime-type."));
            }
            if (!has_width) {
                Log.Exception (new UpnpDeserializationException ("The icon has no width."));
            }
            if (!has_height) {
                Log.Exception (new UpnpDeserializationException ("The icon has no height."));
            }
            if (!has_depth) {
                Log.Exception (new UpnpDeserializationException ("The icon has no depth."));
            }
			verified = true;
        }
    }
}
