// 
// Resource.cs
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
using System.Xml;

namespace Mono.Upnp.DidlLite
{
	public class Resource
	{
		readonly ulong? size;
		readonly TimeSpan? duration;
		readonly uint? bitrate;
		readonly uint? sample_frequency;
		readonly uint? bits_per_sample;
		readonly uint? nr_audio_channels;
		readonly Resolution? resolution;
		readonly uint? color_depth;
		readonly string protocol_info;
		readonly string protection;
		readonly Uri import_uri;
		readonly Uri uri;
		
		internal Resource (XmlReader reader)
		{
			ulong size;
			TimeSpan duration;
			Resolution resolution;
			uint bitrate, sample_frequency, bits_per_sample, nr_audio_channels, color_depth;
			
			if (ulong.TryParse (reader["size", Protocol.DidlLiteSchema], out size))
				this.size = size;
			
			if (TimeSpan.TryParse (reader["duration", Protocol.DidlLiteSchema], out duration)) // TODO our own parsing
				this.duration = duration;
			
			if (uint.TryParse (reader["bitrate", Protocol.DidlLiteSchema], out bitrate))
				this.bitrate = bitrate;
			
			if (uint.TryParse (reader["sampleFrequency", Protocol.DidlLiteSchema], out sample_frequency))
				this.sample_frequency = sample_frequency;
			
			if (uint.TryParse (reader["bitsPerSecond", Protocol.DidlLiteSchema], out bits_per_sample))
				this.bits_per_sample = bits_per_sample;
			
			if (uint.TryParse (reader["nrAudioChannels", Protocol.DidlLiteSchema], out nr_audio_channels))
				this.nr_audio_channels = nr_audio_channels;
			
			if (Mono.Upnp.DidlLite.Resolution.TryParse (reader["resolution", Protocol.DidlLiteSchema], out resolution))
				this.resolution = resolution;
			
			if (uint.TryParse (reader["colorDepth", Protocol.DidlLiteSchema], out color_depth))
				this.color_depth = color_depth;
			
			protocol_info = reader["protocolInfo", Protocol.DidlLiteSchema];
			
			protection = reader["protection", Protocol.DidlLiteSchema];
			
			var import_uri = reader["importUri", Protocol.DidlLiteSchema];
			if (Uri.IsWellFormedUriString (import_uri, UriKind.Absolute)) {
				this.import_uri = new Uri (import_uri);
			}
			
			uri = new Uri (reader.ReadString ());
		}
		
		public ulong? Size { get { return size; } }
		public TimeSpan? Duration { get { return duration; } }
		public uint? Bitrate { get { return bitrate; } }
		public uint? SampleFrequency { get { return sample_frequency; } }
		public uint? BitsPerSample { get { return bits_per_sample; } }
		public uint? NrAudioChannels { get { return nr_audio_channels; } }
		public Resolution? Resolution { get { return resolution; } }
		public uint? ColorDepth { get { return color_depth; } }
		public string ProtocolInfo { get { return protocol_info; } }
		public string Protection { get { return protection; } }
		public Uri ImportUri { get { return import_uri; } }
		public Uri Uri { get { return uri; } }
	}
}
