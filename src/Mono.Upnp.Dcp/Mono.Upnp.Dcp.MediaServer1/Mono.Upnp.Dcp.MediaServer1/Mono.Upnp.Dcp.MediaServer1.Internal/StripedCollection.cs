// 
// StripedCollection.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Peterson
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

namespace Mono.Upnp.Dcp.MediaServer1.Internal
{
    abstract class StripedCollection<T> : ICollection<T>
    {
        protected string[] StripedArray;
        
        protected StripedCollection (string[] striped_array)
        {
            StripedArray = striped_array;
        }
        
        protected abstract T GetValue (KeyValuePair<string, string> keyValuePair);
        
        public IEnumerator<T> GetEnumerator ()
        {
            for (var i = 0; i < StripedArray.Length; i += 2) {
                yield return GetValue (new KeyValuePair<string, string> (StripedArray[i], StripedArray[i + 1]));
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }
        
        public void Add (T item)
        {
            throw new NotSupportedException ();
        }
        
        public void Clear ()
        {
            throw new NotSupportedException ();
        }
        
        public bool Contains (T item)
        {
            foreach (var keyValuePair in this) {
                if (keyValuePair.Equals (item)) {
                    return true;
                }
            }
            return false;
        }
        
        public void CopyTo (T[] array, int arrayIndex)
        {
            if (arrayIndex + Count >= array.Length) {
                throw new ArgumentException ("The array is too small.");
            }
            
            foreach (var keyValuePair in this) {
                array[arrayIndex] = keyValuePair;
                arrayIndex++;
            }
        }
        
        public bool Remove (T item)
        {
            throw new NotSupportedException ();
        }
        
        public int Count {
            get {
                return StripedArray.Length / 2;
            }
        }
        
        public bool IsReadOnly {
            get {
                return true;
            }
        }
    }
}
