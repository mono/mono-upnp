// 
// Map.cs
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
using System.Collections;
using System.Collections.Generic;

using Mono.Upnp.Control;

namespace Mono.Upnp.Internal
{
    sealed class Map<TKey, TValue> : ICollection<TValue>, IMap<TKey, TValue>
    {
        public delegate TKey Mapper (TValue value);
        
        const string collection_error = "The collection is read-only.";
        const string dictionary_error = "The dictionary is read-only.";
        readonly Mapper mapper;
        readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue> ();
        bool is_read_only;
        
        public Map (Mapper mapper)
        {
            this.mapper = mapper;
        }
        
        public void Add (TValue value)
        {
            if (is_read_only) {
                throw new NotSupportedException (collection_error);
            }
            dictionary.Add (mapper (value), value);
        }
        
        bool ICollection<TValue>.Remove (TValue value)
        {
            throw new NotSupportedException (collection_error);
        }
        
        bool ICollection<TValue>.Contains (TValue value)
        {
            return dictionary.ContainsKey (mapper (value));
        }
        
        void ICollection<TValue>.Clear ()
        {
            throw new NotSupportedException (collection_error);
        }
        
        void ICollection<TValue>.CopyTo (TValue[] array, int arrayIndex)
        {
            dictionary.Values.CopyTo (array, arrayIndex);
        }
        
        int ICollection<TValue>.Count {
            get { return dictionary.Count; }
        }
        
        bool ICollection<TValue>.IsReadOnly {
            get { return is_read_only; }
        }
        
        int IMap<TKey, TValue>.Count {
            get { return dictionary.Count; }
        }
        
        TValue IMap<TKey, TValue>.this[TKey key] {
            get { return dictionary[key]; }
        }
        
        bool IMap<TKey, TValue>.ContainsKey (TKey key)
        {
            return dictionary.ContainsKey (key);
        }
        
        IEnumerable<TKey> IMap<TKey, TValue>.Keys {
            get { return dictionary.Keys; }
        }
        
        IEnumerable<TValue> IMap<TKey, TValue>.Values {
            get { return dictionary.Values; }
        }
        
        public IEnumerator<TValue> GetEnumerator ()
        {
            return dictionary.Values.GetEnumerator ();
        }
        
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
        {
            return dictionary.GetEnumerator ();
        }
        
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }
        
        public void MakeReadOnly ()
        {
            is_read_only = true;
        }
    }
}
