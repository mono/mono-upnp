// 
// AlbumOptions.cs
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

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV
{
    public class AlbumOptions : ContainerOptions
    {
        IEnumerable<string> publishers;
        IEnumerable<string> contributors;
        IEnumerable<Uri> relations;
        IEnumerable<string> rights;

        public IEnumerable<string> Publishers {
            get { return GetEnumerable (publishers); }
            set { publishers = value; }
        }

        public IEnumerable<string> Contributors {
            get { return GetEnumerable (contributors); }
            set { contributors = value; }
        }

        public IEnumerable<Uri> Relations {
            get { return GetEnumerable (relations); }
            set { relations = value; }
        }

        public IEnumerable<string> Rights {
            get { return GetEnumerable (rights); }
            set { rights = value; }
        }

        public string StorageMedium { get; set; }

        public string LongDescription { get; set; }

        public string Description { get; set; }

        public string Date { get; set; }
    }
}
