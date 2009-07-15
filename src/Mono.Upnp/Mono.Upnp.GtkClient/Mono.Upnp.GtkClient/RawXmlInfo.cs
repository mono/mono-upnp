// 
// RawXmlInfo.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Thomas
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Mono.Upnp.GtkClient
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class RawXmlInfo : Gtk.Bin
    {
        public RawXmlInfo (Uri location)
        {
            this.Build ();
            
            var request = WebRequest.Create (location);
            using (var response = request.GetResponse ()) {
                using (var stream = response.GetResponseStream ()) {
                    using (var reader = new StreamReader (stream)) {
                        raw.Buffer.Text = reader.ReadToEnd ();
                    }
                }
            }
            
            try {
                using (var string_reader = new StringReader (raw.Buffer.Text)) {
                    using (var xml_reader = XmlReader.Create (string_reader, new XmlReaderSettings { IgnoreWhitespace = true })) {
                        var builder = new StringBuilder ();
                        using (var writer = XmlWriter.Create (builder, new XmlWriterSettings { Indent = true })) {
                            writer.WriteNode (xml_reader, true);
                        }
                        raw.Buffer.Text = builder.ToString ();
                    }
                }
            } catch {
            }
        }
    }
}
