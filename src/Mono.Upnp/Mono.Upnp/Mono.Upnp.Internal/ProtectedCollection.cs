// 
// ProtectedCollection.cs
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

namespace Mono.Upnp.Internal
{
    class ProtectedCollection<T> : ICollection<T>
    {
        const string error = "Collection is read-only.";
        readonly List<T> list = new List<T> ();
        bool locked;
        
        public ProtectedCollection ()
        {
        }
        
        public ProtectedCollection (bool locked)
        {
            this.locked = locked;
        }
        
        public void Lock ()
        {
            locked = true;
        }
        
        public void Add (T item)
        {
            list.Add (item);
        }
        
        void ICollection<T>.Add (T item)
        {
            if (locked) {
                throw new NotSupportedException (error);
            } else {
                Add (item);
            }
        }
        
        void ICollection<T>.Remove (T item)
        {
            throw new NotSupportedException (error);
        }
        
        void ICollection<T>.Clear (T item)
        {
            throw new NotSupportedException (error);
        }
        
        void ICollection<T>.CopyTo (T[] array, int arrayIndex)
        {
            throw new NotSupportedException (error);
        }
        
        bool ICollection<T>.Contains (T item)
        {
            return list.Contains (item);
        }
        
        int ICollection<T>.Count { get { return list.Count; } }
        
        bool ICollection<T>.IsReadOnly { get { return locked; } }
    }
}
