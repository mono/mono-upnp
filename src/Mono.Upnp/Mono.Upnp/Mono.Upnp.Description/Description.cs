// 
// Description.cs
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

using Mono.Upnp.Xml;

namespace Mono.Upnp.Description
{
    public abstract class Description : XmlSerializable, IXmlDeserializable
    {
        readonly Root root;
        
        internal Description ()
        {
        }
        
        internal Description (Root root)
        {
            if (root == null) throw new ArgumentNullException ("root");
            
            this.root = root;
        }
        
        public bool IsReadOnly { get; private set; }
        
        internal Root Root { get { return root; } }
        
        [XmlTypeDeserializer]
        public virtual Uri DeserializeUri (XmlDeserializationContext context)
        {
            if (root != null) {
                return root.DeserializeUrl (context);
            }
            return null;
        }
        
        internal virtual void Initialize (Uri url)
        {
            state = State.Initialized;
        }
        
        void IXmlDeserializer.Deserialize (XmlDeserializationContext context)
        {
            Deserialize (context);
            IsReadOnly = true;
        }
        
        void IXmlDeserializer.DeserializeAttribute (XmlDeserializationContext context)
        {
            DeserializeAttribute (context);
        }

        void IXmlDeserializer.DeserializeElement (Mono.Upnp.Xml.XmlDeserializationContext context)
        {
            DeserializeElement (context);
        }
        
        protected abstract void Deserialize (XmlDeserializationContext context);
        
        protected abstract void DeserializeAttribute (XmlDeserializationContext context);
        
        protected abstract void DeserializeElement (XmlDeserializationContext context);
    }
}
