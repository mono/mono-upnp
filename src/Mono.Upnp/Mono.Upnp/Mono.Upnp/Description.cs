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

namespace Mono.Upnp
{
    public abstract class Description : XmlAutomatable
    {
        readonly Deserializer deserializer;
        bool is_disposed;
        
        internal Description ()
        {
        }
        
        internal Description (Deserializer deserializer)
        {
            if (deserializer == null) throw new ArgumentNullException ("deserializer");
            
            this.deserializer = deserializer;
            Root = deserializer.Root;
        }
        
        internal Deserializer Deserializer {
            get { return deserializer; }
        }
        
        internal Root Root { get; private set; }
        
        internal void Initialize (Root root)
        {
            Root = root;
        }
        
        public bool IsDisposed {
            get { return deserializer != null ? deserializer.IsDisposed : is_disposed; }
        }
        
        internal virtual void Dispose ()
        {
            is_disposed = true;
        }
        
        event EventHandler<DisposedEventArgs> disposed;
        
        public event EventHandler<DisposedEventArgs> Disposed {
            add {
                if (deserializer != null) {
                    deserializer.Disposed += value;
                } else {
                    disposed += value;
                }
            }
            remove {
                if (deserializer != null) {
                    deserializer.Disposed -= value;
                } else {
                    disposed -= value;
                }
            }
        }
        
        internal Uri ExpandUrl (string urlFragment)
        {
            if (Uri.IsWellFormedUriString (urlFragment, UriKind.Relative)) {
                // TODO handle NREs
                return new Uri (Root.UrlBase, urlFragment);
            } else if (Uri.IsWellFormedUriString (urlFragment, UriKind.Absolute)) {
                return new Uri (urlFragment);
            } else {
                throw new UpnpDeserializationException ("The URL fragment is not valid.");
            }
        }
        
        internal string CollapseUrl (Uri url)
        {
            if (Root.UrlBase.IsBaseOf (url)) {
                return Root.UrlBase.MakeRelativeUri (url).ToString ();
            } else {
                // TODO throw
                return url.ToString ();
            }
        }
    }
}
