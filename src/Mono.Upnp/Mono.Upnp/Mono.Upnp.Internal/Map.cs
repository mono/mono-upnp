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

namespace Mono.Upnp.Internal
{
    class Map<TKey, TValue> : ICollection<TValue>, IDictionary<TKey, TValue>
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
        
        void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> value)
        {
            throw new NotSupportedException (collection_error);
        }
        
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> value)
        {
            throw new NotSupportedException (collection_error);
        }
        
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> value)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains (value);
        }
        
        void ICollection<KeyValuePair<TKey, TValue>>.Clear ()
        {
            throw new NotSupportedException (collection_error);
        }
        
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo (array, arrayIndex);
        }
        
        int ICollection<KeyValuePair<TKey, TValue>>.Count {
            get { return dictionary.Count; }
        }
        
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
            get { return true; }
        }
        
        TValue IDictionary<TKey, TValue>.this[TKey key] {
            get { return dictionary[key]; }
            set { throw new NotSupportedException (dictionary_error); }
        }
        
        bool IDictionary<TKey, TValue>.TryGetValue (TKey key, out TValue value)
        {
            return dictionary.TryGetValue (key, out value);
        }
        
        void IDictionary<TKey, TValue>.Add (TKey key, TValue value)
        {
            throw new NotSupportedException (dictionary_error);
        }
        
        bool IDictionary<TKey, TValue>.Remove (TKey key)
        {
            throw new NotSupportedException (dictionary_error);
        }
        
        bool IDictionary<TKey, TValue>.ContainsKey (TKey key)
        {
            return dictionary.ContainsKey (key);
        }
        
        ICollection<TKey> IDictionary<TKey, TValue>.Keys {
            get { return dictionary.Keys; }
        }
        
        ICollection<TValue> IDictionary<TKey, TValue>.Values {
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
