// 
// XmlSerializer.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Peterson
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Mono.Upnp.Xml.Internal;

namespace Mono.Upnp.Xml
{
    public sealed class XmlSerializer
    {
        static UTF8Encoding utf8 = new UTF8Encoding (false);
        
        readonly Dictionary<Type, SerializationInfo> infos = new Dictionary<Type, SerializationInfo> ();
        
        public void Serialize<T> (T obj, XmlWriter writer)
        {
            if (writer == null) throw new ArgumentNullException ("writer");

            SerializeCore (obj, writer);
        }
        
        public void Serialize<T> (T obj, Stream stream)
        {
            if (stream == null) throw new ArgumentNullException ("stream");
            
            using (var writer = XmlWriter.Create (stream, new XmlWriterSettings { Encoding = utf8 })) {
                SerializeCore (obj, writer);
            }
        }
                
        public byte[] GetBytes<T> (T obj)
        {
            using (var stream = new MemoryStream ()) {
                using (var writer = XmlWriter.Create (stream, new XmlWriterSettings { Encoding = utf8 })) {
                    SerializeCore (obj, writer);
                }
                return stream.ToArray ();
            }
        }
        
        public string GetString<T> (T obj)
        {
            return utf8.GetString (GetBytes (obj));
        }
        
        void SerializeCore<T> (T obj, XmlWriter writer)
        {
            if (obj == null) throw new ArgumentNullException ("obj");
            
            var serializer = GetInfo (typeof (T)).TypeSerializer;
            serializer (obj, new XmlSerializationContext (this, writer));
        }

        internal void AutoSerializeObjectAndMembers<T> (T obj, XmlSerializationContext context)
        {
            var serializer = GetInfo (typeof (T)).TypeAutoSerializer;
            serializer (obj, context);
        }
        
        internal void AutoSerializeMembersOnly<T> (T obj, XmlSerializationContext context)
        {
            var serialzer = GetInfo (typeof (T)).MemberAutoSerializer;
            serialzer (obj, context);
        }
        
        internal SerializationInfo GetInfo (Type type)
        {
            SerializationInfo info;
            if (!infos.TryGetValue (type, out info)) {
                info = new SerializationInfo (this, type);
                infos[type] = info;
            }
            return info;
        }
    }
}
