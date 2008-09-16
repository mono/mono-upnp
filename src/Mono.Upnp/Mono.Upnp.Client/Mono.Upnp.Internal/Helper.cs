//
// Helper.cs
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
using System.Net;
using System.Net.Sockets;
using System.Xml;

namespace Mono.Upnp.Internal
{
	internal class Helper
	{
        //public static string HostAddress {
        //    get {
        //        foreach (IPAddress address in Dns.GetHostAddresses (Dns.GetHostName ())) {
        //            if (address.AddressFamily == AddressFamily.InterNetwork) {
        //                return address.ToString ();
        //            }
        //        }
        //    }
        //}

        public static Version DeserializeSpecVersion (XmlReader reader)
        {
            try {
                // We assume the elements appear in this order
                reader.ReadToFollowing ("major");
                reader.Read ();
                int major = reader.ReadContentAsInt ();
                reader.ReadToFollowing ("minor");
                reader.Read ();
                int minor = reader.ReadContentAsInt ();
                reader.Close ();
                return new Version (major, minor);
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deserializing a spec version.", e);
            }
        }

        public static bool ReadToNextElement (XmlReader reader)
        {
            while (reader.Read ()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    return true;
                }
            }
            return false;
        }

        public static HttpWebResponse GetResponse (Uri location)
        {
            return GetResponse ((HttpWebRequest)WebRequest.Create (location), 1);
        }

        public static HttpWebResponse GetResponse (HttpWebRequest request, int retry)
        {
            request.Timeout = 30000;
            while(true) {
                try {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
                    if (response.StatusCode == HttpStatusCode.GatewayTimeout) {
                        throw new WebException ("", WebExceptionStatus.Timeout);
                    }
                    return response;
                } catch (WebException e) {
                    if (e.Status == WebExceptionStatus.Timeout && retry > 0) {
                        retry--;
                    } else if (e.Response != null) {
                        return (HttpWebResponse)e.Response;
                    } else {
                        throw;
                    }
                }
            }
        }

        public static void WriteStartSoapBody (XmlWriter writer)
        {
            writer.WriteStartDocument ();
            writer.WriteStartElement ("s", "Envelope", Protocol.SoapEnvelopeSchema);
            writer.WriteAttributeString ("encodingStyle", Protocol.SoapEnvelopeSchema, Protocol.SoapEncodingSchema);
            writer.WriteStartElement ("Body", Protocol.SoapEnvelopeSchema);
        }

        public static void WriteEndSoapBody (XmlWriter writer)
        {
            writer.WriteEndElement ();
            writer.WriteEndElement ();
            writer.WriteEndDocument ();
        }
	}
}
