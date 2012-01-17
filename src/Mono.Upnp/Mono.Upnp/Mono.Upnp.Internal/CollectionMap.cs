// 
// CollectionMap.cs
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
using System.Collections;
using System.Collections.Generic;

namespace Mono.Upnp.Internal
{
    sealed class CollectionMap<TKey, TValue> : Map<TKey, TValue>, ICollection<TValue>
        where TValue : IMappable<TKey>
    {
        bool is_read_only;
        
        public CollectionMap ()
            : base (new Dictionary<TKey, TValue> ())
        {
        }
        
        public void Add (TValue value)
        {
            CheckReadOnly ();
            if (value == null) {
                return;
            }
            Dictionary.Add (value.Map (), value);
        }
        
        public void Clear ()
        {
            CheckReadOnly ();
            Dictionary.Clear ();
        }
        
        public bool Remove (TValue value)
        {
            CheckReadOnly ();
            if (value == null) {
                return false;
            }
            return Dictionary.Remove (value.Map ());
        }
        
        public bool Contains (TValue value)
        {
            if (value == null) {
                return false;
            }
            return Dictionary.ContainsKey (value.Map ());
        }
        
        public void CopyTo (TValue[] array, int arrayIndex)
        {
            Dictionary.Values.CopyTo (array, arrayIndex);
        }
        
        public bool IsReadOnly {
            get { return is_read_only; }
        }
        
        public new IEnumerator<TValue> GetEnumerator ()
        {
            return Dictionary.Values.GetEnumerator ();
        }
        
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }
        
        public void MakeReadOnly ()
        {
            is_read_only = true;
        }
        
        void CheckReadOnly ()
        {
            if (is_read_only) {
                throw new NotSupportedException ("The collection is read-only.");
            }
        }
    }
}
