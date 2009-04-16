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
using System.Xml.Serialization;

namespace Mono.Upnp.Server
{
    [XmlType ("icon")]
	public class Icon
	{
        readonly string mimetype;
        readonly int width;
        readonly int height;
        readonly int depth;
        Uri url;
        string filename;
        byte[] data;
        

        public Icon (int width, int height, int depth, string format, byte[] data)
             : this (width, height, depth, format)
        {
            if (data == null) throw new ArgumentNullException ("data");
            
            this.data = data;
        }

        public Icon (int width, int height, int depth, string format, string filename)
            : this (width, height, depth, format)
        {
            if (filename == null) throw new ArgumentNullException ("filename");
            if (!File.Exists (filename)) throw new ArgumentException ("The specified filename does not exist on the file system.", "path");
            
            this.filename = filename;
        }

        protected Icon (int width, int height, int depth, string format)
        {
            if (format == null) {
                throw new ArgumentNullException ("format");
            }

            if (format.StartsWith ("image/")) {
                mimetype = format;
            } else {
                mimetype = string.Format ("image/{0}", format);
            }

            this.width = width;
            this.height = height;
            this.depth = depth;
        }
        
        internal void Initialize (Uri iconUrl)
        {
            url = iconUrl;
            Initialize ();
        }
        
        protected virtual void Initialize ()
        {
        }
        
        [XmlElement ("mimetype")]
        public string MimeType {
            get { return mimetype; }
        }
        
        [XmlElement ("width")]
        public int Width {
            get { return width; }
        }
        
        [XmlElement ("height")]
        public int Height {
            get { return height; }
        }
        
        [XmlElement ("depth")]
        public int Depth {
            get { return depth; }
        }
        
        [XmlElement ("url")]
        public Uri Url {
            get { return url; }
        }
        
        internal byte[] GetData ()
        {
            return GetDataCore ();
        }

        protected virtual byte[] GetDataCore ()
        {
            if (data == null) {
                data = File.ReadAllBytes (filename);
                filename = null;
            }
            return data;
        }
	}
}
