// 
// ResourceSettings.cs
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

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public class ResourceSettings
    {
        readonly Uri uri;
        
        public ResourceSettings (Uri uri)
        {
            if (uri == null) throw new ArgumentNullException ("uri");
            
            this.uri = uri;
        }
        
        public Uri Uri {
            get { return uri; }
        }
        
        public ulong? Size { get; set; }
        
        public TimeSpan? Duration { get; set; }

        public uint? Bitrate { get; set; }

        public uint? SampleFrequency { get; set; }

        public uint? BitsPerSample { get; set; }

        public uint? NrAudioChannels { get; set; }
        
        public Resolution? Resolution { get; set; }

        public uint? ColorDepth { get; set; }

        public string ProtocolInfo { get; set; }
        
        public string Protection { get; set; }

        public Uri ImportUri { get; set; }
    }
}
