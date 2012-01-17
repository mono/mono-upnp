// 
// XmlSerializationContext.cs
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
using System.Xml;

namespace Mono.Upnp.Xml
{
    public abstract class XmlSerializationContext
    {
        readonly XmlWriter writer;
        
        internal XmlSerializationContext (XmlWriter writer)
        {
            this.writer = writer;
        }
        
        public XmlWriter Writer {
            get { return writer; }
        }

        public abstract void AutoSerializeObjectStart<TObject> (TObject obj);

        public abstract void AutoSerializeObjectEnd<TObject> (TObject obj);
        
        public abstract void AutoSerializeMembers<TObject> (TObject obj);
        
        public abstract void Serialize<TObject> (TObject obj);
    }
    
    public sealed class XmlSerializationContext<TContext> : XmlSerializationContext
    {
        readonly XmlSerializer<TContext> serializer;
        readonly TContext context;
        
        internal XmlSerializationContext (XmlSerializer<TContext> serializer, XmlWriter writer, TContext context)
            : base (writer)
        {
            this.serializer = serializer;
            this.context = context;
        }
        
        public TContext Context {
            get { return context; }
        }
        
        public override void AutoSerializeObjectStart<TObject> (TObject obj)
        {
            if (obj == null) throw new ArgumentNullException ("obj");
            
            serializer.AutoSerializeObjectStart (obj, this);
        }
        
        public void AutoSerializeObjectStart<TObject> (TObject obj, TContext context)
        {
            if (obj == null) throw new ArgumentNullException ("obj");
            
            serializer.AutoSerializeObjectStart (obj,
                new XmlSerializationContext<TContext> (serializer, Writer, context));
        }
        
        public override void AutoSerializeObjectEnd<TObject> (TObject obj)
        {
            if (obj == null) throw new ArgumentNullException ("obj");
            
            serializer.AutoSerializeObjectEnd (obj, this);
        }
        
        public void AutoSerializeObjectEnd<TObject> (TObject obj, TContext context)
        {
            if (obj == null) throw new ArgumentNullException ("obj");
            
            serializer.AutoSerializeObjectEnd (obj,
                new XmlSerializationContext<TContext> (serializer, Writer, context));
        }
        
        public override void AutoSerializeMembers<TObject> (TObject obj)
        {
            if (obj == null) throw new ArgumentNullException ("obj");
            
            serializer.AutoSerializeMembers (obj, this);
        }
        
        public void AutoSerializeMembers<TObject> (TObject obj, TContext context)
        {
            if (obj == null) throw new ArgumentNullException ("obj");
            
            serializer.AutoSerializeMembers (obj,
                new XmlSerializationContext<TContext> (serializer, Writer, context));
        }
        
        public override void Serialize<TObject> (TObject obj)
        {
            serializer.Serialize (obj, this);
        }
        
        public void Serialize<TObject> (TObject obj, TContext context)
        {
            serializer.Serialize (obj, new XmlSerializationContext<TContext> (serializer, Writer, context));
        }
    }
}
