// 
// PlaylistContainerOptions.cs
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

using System.Collections.Generic;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.AV
{
    public class PlaylistContainerOptions : ContainerOptions
    {
        IEnumerable<PersonWithRole> artists;
        IEnumerable<string> genres;
        IEnumerable<string> producers;
        IEnumerable<string> contributors;
        IEnumerable<string> rights;

        public IEnumerable<PersonWithRole> Artists {
            get { return GetEnumerable (artists); }
            set { artists = value; }
        }

        public IEnumerable<string> Genres {
            get { return GetEnumerable (genres); }
            set { genres = value; }
        }

        public IEnumerable<string> Producers {
            get { return GetEnumerable (producers); }
            set { producers = value; }
        }

        public IEnumerable<string> Contributors {
            get { return GetEnumerable (contributors); }
            set { contributors = value; }
        }

        public IEnumerable<string> Rights {
            get { return GetEnumerable (rights); }
            set { rights = value; }
        }

        public virtual string LongDescription { get; set; }

        public virtual string StorageMedium { get; set; }

        public virtual string Description { get; set; }

        public virtual string Date { get; set; }

        public virtual string Language { get; set; }
    }
}
