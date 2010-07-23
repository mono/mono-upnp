// 
// ObjectOptions.cs
//  
// Author:
//       Yavor Georgiev <fealebenpae@gmail.com>
// 
// Copyright (c) 2010 Yavor Georgiev
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
namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public class ObjectOptions
    {
        public ObjectOptions ()
        {
            ResourceCollection = new List<Resource> ();
        }

        public ObjectOptions (Object obj)
        {
            GetOptionsFrom (obj);
        }

        protected virtual void GetOptionsFrom (Object obj)
        {
            Title = obj.Title;
            Creator = obj.Creator;
            WriteStatus = obj.WriteStatus;

            ResourceCollection = new List<Resource> (obj.Resources);
        }

        public virtual List<Resource> ResourceCollection { get; set; }

        public virtual string Title { get; set; }

        public virtual string Creator { get; set; }

        public virtual WriteStatus? WriteStatus { get; set; }
    }
}


