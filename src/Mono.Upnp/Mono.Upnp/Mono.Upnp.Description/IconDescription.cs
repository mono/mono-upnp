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
	public class IconDescription : Description
	{
        readonly DeviceDescription device;
        Uri url;
        string mimetype;
        int width;
        int height;
        int depth;
        byte[] data;

        protected internal IconDescription (DeviceDescription device)
            : base (GetRoot (device))
        {
            this.device = device;
        }
        
        public Icon (int width, int height, int depth, string mimetype, byte[] data)
             : this (width, height, depth, mimetype)
        {
            if (data == null) throw new ArgumentNullException ("data");
            
            this.data = data;
        }

        public Icon (int width, int height, int depth, string format, string filename)
            : this (width, height, depth, format)
        {
            if (filename == null) throw new ArgumentNullException ("filename");
            //if (!File.Exists (filename)) throw new ArgumentException ("The specified filename does not exist on the file system.", "path");
            
            this.filename = filename;
        }
        
        static Root GetRoot (DeviceDescription device)
        {
            if (device == null) throw new ArgumentNullException ("device");
            return device.Root;
        }
        
        protected Icon (int width, int height, int depth, string mimetype)
        {
            Width = width;
            Height = height;
            Depth = depth;
            MimeType = mimetype;
        }

        //public DeviceDescription Device { get { return device; } }
        public bool IsDisposed { get { return device.IsDisposed; } }
        
		[XmlElement ("url")]
        public virtual Uri Url {
            get { return url; }
            set {
                CheckReadOnly ();
                url = value;
            }
        }
        
        [XmlElement ("mimetype")]
        public virtual string MimeType {
            get { return mimetype; }
            set {
                CheckReadOnly ();
                if (value == null) throw new ArgumentNullException ("mimetype");
                mimetype = value;
            }
        }
        
        [XmlElement ("width")]
        public virtual int Width {
            get { return width; }
            set {
                CheckReadOnly ();
                width = value;
            }
        }
        
        [XmlElement ("height")]
        public virtual int Height {
            get { return height; }
            set {
                CheckReadOnly ();
                height = value;
            }
        }
        
        [XmlElement ("depth")]
        public virtual int Depth {
            get { return depth; }
            set {
                CheckReadOnly ();
                depth = value;
            }
        }
        
        void CheckReadOnly ()
        {
            if (IsReadOnly) {
                throw new InvalidOperationException ("The icon is read-only and cannot be modified.");
            }
        }
        
        internal override void Initialize (Uri url)
        {
            InitializeCore (url);
            base.Initialize (url);
        }
        
        protected virtual void Initializecore (Uri iconUrl)
        {
            Url = url;
        }

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
        
        protected override void Deserialize (XmlDeserializationContext context)
        {
            context.AutoDeserialize (this);
        }

        protected override void DeserializeAttribute (XmlDeserializationContext context)
        {
            context.AutoDeserializeAttribute (this);
        }

        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            context.AutoDeserializeElement (this);
        }
        
        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            context.AutoSerializeObjectAndMembers (this);
        }
        
        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            context.AutoSerializeMembersOnly (this);
        }
    }
}
