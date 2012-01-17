// 
// Arguments.cs
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
    class Arguments : StripedCollection<KeyValuePair<string, string>>, IDictionary<string, string>
    {
        public Arguments (params string[] keyValuePairs)
            : base (keyValuePairs)
        {
        }
        
        public void Add (string key, string value)
        {
            throw new NotSupportedException ();
        }
        
        public bool ContainsKey (string key)
        {
            string value;
            return TryGetValue (key, out value);
        }
        
        
        public bool Remove (string key)
        {
            return false;
        }
        
        public bool TryGetValue (string key, out string value)
        {
            for (var i = 0; i < StripedArray.Length; i += 2) {
                if (StripedArray[i] == key) {
                    value = StripedArray[i + 1];
                    return true;
                }
            }
            value = null;
            return false;
        }
        
        public string this[string key] {
            get {
                string value;
                if (TryGetValue (key, out value)) {
                    return value;
                } else {
                    throw new KeyNotFoundException ();
                }
            }
            set {
                throw new NotSupportedException ();
            }
        }
        
        public ICollection<string> Keys {
            get {
                return new KeyCollection (StripedArray);
            }
        }
        
        public ICollection<string> Values {
            get {
                return new ValueCollection (StripedArray);
            }
        }
        
        protected override KeyValuePair<string, string> GetValue (KeyValuePair<string, string> keyValuePair)
        {
            return keyValuePair;
        }
        
        class KeyCollection : StripedCollection<string>
        {
            public KeyCollection (string[] stripedArray)
                : base (stripedArray)
            {
            }
            
            protected override string GetValue (KeyValuePair<string, string> keyValuePair)
            {
                return keyValuePair.Key;
            }
        }
        
        class ValueCollection : StripedCollection<string>
        {
            public ValueCollection (string[] stripedArray)
                : base (stripedArray)
            {
            }
            
            protected override string GetValue (KeyValuePair<string, string> keyValuePair)
            {
                return keyValuePair.Value;
            }
        }
    }
}
