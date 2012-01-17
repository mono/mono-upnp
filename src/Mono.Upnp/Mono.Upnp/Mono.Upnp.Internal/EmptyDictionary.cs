// 
// EmptyDictionary.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Thomas
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
    class EmptyDictionary : IDictionary<string, string>
    {
        public void Add (string key, string value)
        {
            throw new NotSupportedException ();
        }

        public bool ContainsKey (string key)
        {
            return false;
        }

        public bool Remove (string key)
        {
            return false;
        }

        public bool TryGetValue (string key, out string value)
        {
            value = null;
            return false;
        }

        public string this[string key] {
            get {
                return null;
            }
            set {
                throw new NotSupportedException ();
            }
        }

        public ICollection<string> Keys {
            get {
                return new string[0];
            }
        }

        public ICollection<string> Values {
            get {
                return new string[0];
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator ()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        public void Add (KeyValuePair<string, string> item)
        {
            throw new NotSupportedException ();
        }

        public void Clear ()
        {
            throw new NotSupportedException ();
        }

        public bool Contains (KeyValuePair<string, string> item)
        {
            return false;
        }

        public void CopyTo (KeyValuePair<string, string>[] array, int arrayIndex)
        {
        }

        public bool Remove (KeyValuePair<string, string> item)
        {
            throw new NotSupportedException ();
        }

        public int Count {
            get {
                return 0;
            }
        }

        public bool IsReadOnly {
            get {
                return true;
            }
        }
    }
}
