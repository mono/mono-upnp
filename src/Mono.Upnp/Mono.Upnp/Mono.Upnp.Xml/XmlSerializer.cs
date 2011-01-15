// 
// XmlSerializer.cs
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Mono.Upnp.Internal;
using Mono.Upnp.Xml.Compilation;

namespace Mono.Upnp.Xml
{
    public sealed class XmlSerializer
    {
        struct Nothing
        {
        }
        
        readonly XmlSerializer<Nothing> serializer = new XmlSerializer<Nothing> ();
        
        public void Serialize<TObject> (TObject obj, XmlWriter writer)
        {
            serializer.Serialize (obj, writer, new Nothing ());
        }
        
        public void Serialize<TObject> (TObject obj, Stream stream)
        {
            Serialize (obj, stream, null);
        }
        
        public void Serialize<TObject> (TObject obj, Stream stream, XmlSerializationOptions options)
        {
            if (options == null) {
                serializer.Serialize (obj, stream);
            } else {
                serializer.Serialize (obj, stream, new XmlSerializationOptions<Nothing> {
                    Encoding = options.Encoding, XmlDeclarationType = options.XmlDeclarationType });
            }
        }
        
        public byte[] GetBytes<TObject> (TObject obj)
        {
            return GetBytes (obj, null);
        }
        
        public byte[] GetBytes<TObject> (TObject obj, XmlSerializationOptions options)
        {
            if (options == null) {
                return serializer.GetBytes (obj);
            } else {
                return serializer.GetBytes (obj, new XmlSerializationOptions<Nothing> {
                    Encoding = options.Encoding, XmlDeclarationType = options.XmlDeclarationType }); 
            }
        }
        
        public string GetString<TObject> (TObject obj)
        {
            return GetString (obj, null);
        }
        
        public string GetString<TObject> (TObject obj, XmlSerializationOptions options)
        {
            if (options == null) {
                return serializer.GetString (obj);
            } else {
                return serializer.GetString (obj, new XmlSerializationOptions<Nothing> {
                    Encoding = options.Encoding, XmlDeclarationType = options.XmlDeclarationType }); 
            }
        }
    }
    
    public sealed class XmlSerializer<TContext>
    {
        readonly SerializationCompilerProvider<TContext> compiler_provider;
        readonly Dictionary<Type, SerializationCompiler<TContext>> compilers =
            new Dictionary<Type, SerializationCompiler<TContext>> ();
        
        public XmlSerializer ()
            : this (null)
        {
        }
        
        public XmlSerializer (SerializationCompilerProvider<TContext> compilerProvider)
        {
            if (compilerProvider == null) {
                compiler_provider = (serializer, type) =>
                    new DelegateSerializationCompiler<TContext> (serializer, type);
            } else {
                compiler_provider = compilerProvider;
            }
        }
        
        public void Serialize<TObject> (TObject obj, XmlWriter writer)
        {
            Serialize (obj, writer, default (TContext));
        }
        
        public void Serialize<TObject> (TObject obj, XmlWriter writer, TContext context)
        {
            if (writer == null) throw new ArgumentNullException ("writer");
            
            SerializeCore (obj, writer, context);
        }
        
        public void Serialize<TObject> (TObject obj, Stream stream)
        {
            Serialize (obj, stream, null);
        }
        
        public void Serialize<TObject> (TObject obj, Stream stream, XmlSerializationOptions<TContext> options)
        {
            if (stream == null) throw new ArgumentNullException ("stream");
            
            var serializationOptions = new XmlSerializationOptions(options);
            
            using (var writer = XmlWriter.Create (stream, new XmlWriterSettings {
                Encoding = serializationOptions.Encoding, OmitXmlDeclaration = true })) {
                WriteXmlDeclaration (writer, serializationOptions);
                SerializeCore (obj, writer, serializationOptions.Context);
            }
        }
        
        public byte[] GetBytes<TObject> (TObject obj)
        {
            return GetBytes (obj, null);
        }
        
        public byte[] GetBytes<TObject> (TObject obj, XmlSerializationOptions<TContext> options)
        {
            var serializationOptions = new XmlSerializationOptions(options);
            
            using (var stream = new MemoryStream ()) {
                using (var writer = XmlWriter.Create (stream, new XmlWriterSettings {
                    Encoding = serializationOptions.Encoding ?? Helper.UTF8Unsigned,
                    OmitXmlDeclaration = serializationOptions.XmlDeclarationType == XmlDeclarationType.None})) {
                    WriteXmlDeclaration (writer, serializationOptions);
                    SerializeCore (obj, writer, serializationOptions.Context);
                }
                return stream.ToArray ();
            }
        }
        
        public string GetString<TObject> (TObject obj)
        {
            return GetString (obj, null);
        }
        
        public string GetString<TObject> (TObject obj, XmlSerializationOptions<TContext> options)
        {
            if (options == null) {
                options = new XmlSerializationOptions<TContext> ();
            }
            
            var encoding = options.Encoding ?? Helper.UTF8Unsigned;
            
            return encoding.GetString (GetBytes (obj, options));
        }
        
        void WriteXmlDeclaration (XmlWriter writer, XmlSerializationOptions options)
        {
            switch (options.XmlDeclarationType) {
            case XmlDeclarationType.Version:
                writer.WriteProcessingInstruction ("xml", @"version=""1.0""");
                break;
            case XmlDeclarationType.VersionAndEncoding:
                writer.WriteProcessingInstruction ("xml", string.Format(
                    @"version=""1.0"" encoding=""{0}""", options.Encoding.HeaderName));
                break;
            }
        }
        
        void SerializeCore<TObject> (TObject obj, XmlWriter writer, TContext context)
        {
            Serialize (obj, new XmlSerializationContext<TContext> (this, writer, context));
        }
        
        internal void Serialize<TObject> (TObject obj, XmlSerializationContext<TContext> context)
        {
            if (obj == null) throw new ArgumentNullException ("obj");
            
            var serializer = GetCompilerForType (typeof (TObject)).TypeSerializer;
            serializer (obj, context);
        }

        internal void AutoSerializeObjectStart<TObject> (TObject obj, XmlSerializationContext<TContext> context)
        {
            var serializer = GetCompilerForType (typeof (TObject)).TypeStartAutoSerializer;
            serializer (obj, context);
        }

        internal void AutoSerializeObjectEnd<TObject> (TObject obj, XmlSerializationContext<TContext> context)
        {
            var serializer = GetCompilerForType (typeof (TObject)).TypeEndAutoSerializer;
            serializer (obj, context);
        }
        
        internal void AutoSerializeMembers<TObject> (TObject obj, XmlSerializationContext<TContext> context)
        {
            var serialzer = GetCompilerForType (typeof (TObject)).MemberAutoSerializer;
            serialzer (obj, context);
        }
        
        internal SerializationCompiler<TContext> GetCompilerForType (Type type)
        {
            SerializationCompiler<TContext> compiler;
            if (!compilers.TryGetValue (type, out compiler)) {
                compiler = compiler_provider (this, type);
                compilers[type] = compiler;
            }
            return compiler;
        }
        
        struct XmlSerializationOptions
        {
            public readonly Encoding Encoding;
            public readonly TContext Context;
            public readonly XmlDeclarationType XmlDeclarationType;
            
            public XmlSerializationOptions (XmlSerializationOptions<TContext> options)
            {
                if (options == null) {
                    Encoding = Helper.UTF8Unsigned;
                    Context = default (TContext);
                    XmlDeclarationType = 0;
                } else {
                    Encoding = options.Encoding ?? Helper.UTF8Unsigned;
                    Context = options.Context;
                    XmlDeclarationType = options.XmlDeclarationType;
                }
            }
        }
    }
}
