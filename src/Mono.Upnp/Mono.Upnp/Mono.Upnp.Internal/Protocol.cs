//
// Protocol.cs
//
// Author:
//   Scott Peterson <lunchtimemama@gmail.com>
//
// Copyright (C) 2008 S&S Black Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
    
using System;

namespace Mono.Upnp.Internal
{
    static class Protocol
    {
        public const string DeviceSchema = "urn:schemas-upnp-org:device-1-0";
        public const string ServiceSchema = "urn:schemas-upnp-org:service-1-0";
        public const string ControlSchema = "urn:schemas-upnp-org:control-1-0";
        public const string EventSchema = "urn:schemas-upnp-org:event-1-0";
        public const string SoapEnvelopeSchema = "http://schemas.xmlsoap.org/soap/envelope/";
        public const string SoapEncodingSchema = "http://schemas.xmlsoap.org/soap/encoding/";
        // TODO make this better
        public readonly static string UserAgent = string.Format (
            "{0}/{1} UPnp/1.1 Mono.Upnp/1.0", Environment.OSVersion.Platform, Environment.OSVersion.Version);
    }
}
