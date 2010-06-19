// 
// UpdateContext.cs
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
using System.IO;
using System.Text;
using System.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.Xml
{
    public class UpdateContext
    {
        readonly object other_value;
        readonly Stream stream;
        byte[] delineator;
        bool delineated;
        
        public UpdateContext ()
        {
        }
        
        public UpdateContext (object otherValue, Stream stream, Encoding encoding)
        {
            if (otherValue == null) {
                throw new ArgumentNullException ("otherValue");
            } else if (stream == null) {
                throw new ArgumentNullException ("stream");
            } else if (encoding == null) {
                throw new ArgumentNullException ("encoding");
            }
            
            this.other_value = otherValue;
            this.stream = stream;
            this.delineator = encoding.GetBytes (",");
        }
        
        public object OtherValue {
            get { return other_value; }
        }
        
        public bool Delineated {
            get { return delineated; }
        }
        
        public void DelineateUpdate ()
        {
            if (stream == null) {
                throw new InvalidOperationException ("You cannot call DelineateUpdate unless a Stream and Encoding were passed to the constructor.");
            }
            
            if (delineated) {
                stream.Write (delineator, 0, delineator.Length);
            } else {
                delineated = true;
            }
        }
    }
}
