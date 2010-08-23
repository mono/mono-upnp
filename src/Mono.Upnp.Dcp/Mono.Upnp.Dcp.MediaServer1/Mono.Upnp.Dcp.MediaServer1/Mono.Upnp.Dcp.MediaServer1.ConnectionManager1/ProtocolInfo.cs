// 
// ProtocolInfo.cs
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

namespace Mono.Upnp.Dcp.MediaServer1.ConnectionManager1
{
    public class ProtocolInfo
    {
        public ProtocolInfo (string protocol)
        {
            if (string.IsNullOrEmpty (protocol) || protocol == "*") {
                throw new ArgumentException ("Protocol must be defined.", "protocol");
            }
            this.Protocol = protocol;
        }

        public ProtocolInfo (string protocol, string contentFormat)
            : this (protocol)
        {
            ContentFormat = contentFormat;
        }

        public ProtocolInfo (string protocol, string network, string contentFormat, string additionalInfo)
            : this (protocol, contentFormat)
        {
            Network = network;
            AdditionalInfo = additionalInfo;
        }

        public string Protocol { get; private set; }

        public string Network { get; private set; }

        public string ContentFormat { get; private set; }

        public string AdditionalInfo { get; private set; }

        public override string ToString ()
        {
            var network = string.IsNullOrEmpty (Network) ? "*" : Network;
            var content_format = string.IsNullOrEmpty (ContentFormat) ? "*" : ContentFormat;
            var additional_info = string.IsNullOrEmpty (AdditionalInfo) ? "*" : AdditionalInfo;

            return string.Format ("{0}:{1}:{2}:{3}", Protocol, network, content_format, additional_info);
        }

        public static ProtocolInfo Parse (string text)
        {
            if (text == null) {
                throw new ArgumentNullException ("text");
            }

            var tokens = text.Split (':');
            if (tokens.Length != 4) {
                throw new ArgumentException ("text",
                    @"The string must be of the form ""protocol:network:contentFormat:additionalInfo"".");
            }

            var result = new ProtocolInfo (tokens [0]);

            if (!string.IsNullOrEmpty (tokens [1]) && tokens [1] != "*") {
                result.Network = tokens [1];
            }
            if (!string.IsNullOrEmpty (tokens [2]) && tokens [2] != "*") {
                result.ContentFormat = tokens [2];
            }
            if (!string.IsNullOrEmpty (tokens [3]) && tokens [3] != "*") {
                result.AdditionalInfo = tokens [3];
            }

            return result;
        }
    }

    public static class Protocols
    {
        public const string HttpGet = "http-get";

        public const string RtspUdp = "rtsp-rtp-udp";

        public const string Internal = "internal";

        public const string IEC61883 = "iec61883";
    }
}
