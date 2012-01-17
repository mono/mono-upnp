// 
// Map.cs
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

using Mono.Upnp.Control;

namespace Mono.Upnp.Internal
{
    class Map<TKey, TValue> : IMap<TKey, TValue>
    {
        readonly IDictionary<TKey, TValue> dictionary;
        
        protected IDictionary<TKey, TValue> Dictionary {
            get { return dictionary; }
        }
        
        public Map (IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }
        
        public int Count {
            get { return dictionary.Count; }
        }
        
        public bool ContainsKey (TKey key)
        {
            return dictionary.ContainsKey (key);
        }
        
        public TValue this[TKey key] {
            get { return dictionary[key]; }
        }
        
        public bool TryGetValue (TKey key, out TValue value)
        {
            return dictionary.TryGetValue (key, out value);
        }
        
        public IEnumerable<TKey> Keys {
            get { return dictionary.Keys; }
        }
        
        public IEnumerable<TValue> Values {
            get { return dictionary.Values; }
        }
        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
        {
            return dictionary.GetEnumerator ();
        }
        
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }
    }
}
