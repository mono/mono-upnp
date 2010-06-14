// 
// UpdateXmlSerializer.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Peterson
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
using System.Text;
using System.Xml;

using Mono.Upnp.Xml;
using Mono.Upnp.Xml.Compilation;

namespace Mono.Upnp.Dcp.MediaServer1.Xml
{
    public class UpdateXmlSerializer
    {
        static readonly UTF8Encoding utf8 = new UTF8Encoding (false);
        
        readonly XmlSerializer<UpdateContext> xml_serializer;
        
        public UpdateXmlSerializer ()
            : this (new UpdateDelegateSerialzationCompilerFactory ())
        {
        }
        
        public UpdateXmlSerializer (SerializationCompilerFactory<UpdateContext> compilerFactory)
        {
            this.xml_serializer = new XmlSerializer<UpdateContext> (compilerFactory);
        }
        
        public void Serialize<T> (T obj1, T obj2, Stream stream)
        {
            Serialize (obj1, obj2, stream, null);
        }
        
        public void Serialize<T> (T obj1, T obj2, Stream stream, XmlSerializationOptions options)
        {
            if (stream == null) {
                throw new ArgumentNullException ("stream");
            }
            
            var encoding = options != null ? options.Encoding ?? utf8 : utf8;
            var update_writer = new UpdateTextWriter (new StreamWriter (stream, encoding));
            using (var xml_writer = XmlWriter.Create (update_writer, new XmlWriterSettings {
                Encoding = encoding, OmitXmlDeclaration = true })) {
                xml_serializer.Serialize (obj1, xml_writer, new UpdateContext (obj2, stream, encoding));
            }
        }
    }
}
