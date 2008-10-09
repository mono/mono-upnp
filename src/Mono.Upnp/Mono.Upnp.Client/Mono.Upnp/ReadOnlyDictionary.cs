//
// ReadOnlyDictionary.cs
//
// Author:
//   Scott Peterson <lunchtimemama@gmail.com>
//
// Copyright (C) 2008 S&S Black Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace Mono.Upnp
{
	public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private const string error = "Dictionary is read-only.";
        private readonly IDictionary<TKey, TValue> dictionary;

        public ReadOnlyDictionary (IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null) {
                throw new ArgumentNullException ("dictionary");
            }
            this.dictionary = dictionary;
        }

        void IDictionary<TKey, TValue>.Add (TKey key, TValue value)
        {
            throw new NotSupportedException (error);
        }

        public bool ContainsKey (TKey key)
        {
            return dictionary.ContainsKey (key);
        }

        public ICollection<TKey> Keys
        {
            get { return dictionary.Keys; }
        }

        bool IDictionary<TKey, TValue>.Remove (TKey key)
        {
            throw new NotSupportedException (error);
        }

        public bool TryGetValue (TKey key, out TValue value)
        {
            return dictionary.TryGetValue (key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return dictionary.Values; }
        }

        public TValue this[TKey key] {
            get { return dictionary[key]; }
            set { throw new NotSupportedException (error); }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException (error);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear ()
        {
            throw new NotSupportedException (error);
        }

        public bool Contains (KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Contains (item);
        }

        public void CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dictionary.CopyTo (array, arrayIndex);
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException (error);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
        {
            return dictionary.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        public override bool Equals (object obj)
        {
            ReadOnlyDictionary<TKey, TValue> dict = obj as ReadOnlyDictionary<TKey, TValue>;
            return dict != null && dict.dictionary.Equals (dictionary);
        }

        public override int GetHashCode ()
        {
            return dictionary.GetHashCode ();
        }
    }
}
