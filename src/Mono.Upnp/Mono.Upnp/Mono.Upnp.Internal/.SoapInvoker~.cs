//
// ControlClient.cs
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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

using Mono.Upnp.Control;

namespace Mono.Upnp.Internal
{
    class SoapInvoker
    {
        // The UPnP spec defines a fallback scheme for the SOAP requests. Also, SOME PEOPLE *cough*Rhapsody*cough*
        // have decided that they know better than the rest of us and only accept ASCII requests despite the fact
        // that the spec VERY EXPLICTLY calls for UTF-8 encoding ONLY. Ug. Anyway, we do fallback for all of this
        // shit and we remember what we do on a per-host basis so that we do it right the first time the next time.
        class FallbackInfo
        {
            public bool OmitMan = true;
            public bool Chuncked = false;
            public bool UseDefaultUtf8 = true;
            public FallbackInfo Clone ()
            {
                return new FallbackInfo {
                    OmitMan = OmitMan,
                    Chuncked = Chuncked,
                    UseDefaultUtf8 = UseDefaultUtf8
                };
            }
        }
            
        delegate HttpWebRequest RequestProvider (ServiceAction action, WebHeaderCollection headers);
        delegate HttpWebResponse RequestHandler (HttpWebRequest request, ServiceAction action,
                                                 int retry, Stream stream);
        
        static SoapInvoker ()
        {
            ServicePointManager.Expect100Continue = false;
        }

        readonly static Dictionary<string, FallbackInfo> fallbacks = new Dictionary<string, FallbackInfo> ();
        readonly static Encoding fallback_utf8 = new UTF8Encoding (false);

        readonly Uri location;
        readonly FallbackInfo fallback;

        public SoapInvoker (Uri location)
        {
            this.location = location;
            if (fallbacks.ContainsKey (location.Host)) {
                fallback = fallbacks[location.Host].Clone ();
                fallbacks[location.Host] = fallback;
            } else {
                fallback = new FallbackInfo ();
                fallbacks.Add (location.Host, fallback);
            }
        }

        public ActionResult Invoke (ServiceAction action, IDictionary<string, string> arguments, int retry)
        {
            Encoding encoding, fallback_encoding;
            if (fallback.UseDefaultUtf8) {
                encoding = Encoding.UTF8;
                fallback_encoding = fallback_utf8;
            } else {
                encoding = fallback_utf8;
                fallback_encoding = Encoding.UTF8;
            }
            var result = Invoke (action, arguments, encoding, retry, false);
            if (result == null) {
                fallback.UseDefaultUtf8 = !fallback.UseDefaultUtf8;
                result = Invoke (action, arguments, fallback_encoding, retry, true);
            }
            return result;
        }

        ActionResult Invoke (ServiceAction action, IDictionary<string, string> arguments,
                            Encoding encoding, int retry, bool isFallback)
        {
            // Because we may do several fallbacks, we serialize to a memory stream and copy that
            // to the network stream. That way we only have to serialize once (unless we have to
            // try a different encoding scheme).
            using (var stream = new MemoryStream ()) {
                try {
                    var headers = new WebHeaderCollection ();
                    var settings = new XmlWriterSettings { Encoding = encoding };
                    using (var writer = XmlWriter.Create (stream, settings)) {
                        action.SerializeRequest (arguments, headers, writer);
                    }

                    using (var response = GetResponse (action, headers, retry, stream)) {
                        if (response.StatusCode == HttpStatusCode.OK) {
                            return action.DeserializeResponse (response);
                        } else if (response.StatusCode == HttpStatusCode.InternalServerError) {
                            if (isFallback) {
                                action.DeserializeResponseFault (response);
                            }
                            return null;
                        } else if (response.StatusCode == HttpStatusCode.BadRequest) {
                            return null;
                        } else {
                            throw new UpnpException (string.Format (
                                "There was an unknown error while invoking the action: " +
                                "the service returned status code {0}.", response.StatusCode));
                        }
                    }
                } catch (Exception e) {
                    throw new UpnpException ("There was an unknown error while invoking the action.", e);
                }
            }
        }

        HttpWebResponse GetResponse (ServiceAction action, WebHeaderCollection headers,
                                             int retry, Stream stream)
        {
            var response = Stage1 (action, headers, retry, stream);
            if (response.StatusCode == HttpStatusCode.NotImplemented || response.StatusDescription == "Not Extended") {
                // FIXME is this the right exception type?
                throw new UpnpException ("The SOAP request failed.");
            } else {
                return response;
            }
        }

        HttpWebResponse Stage1 (ServiceAction action, WebHeaderCollection headers, int retry, Stream stream)
        {
            if (fallback.OmitMan) {
                return Stage1 (action, headers, retry, stream, CreateRequestWithoutMan, CreateRequestWithMan);
            } else {
                return Stage1 (action, headers, retry, stream, CreateRequestWithMan, CreateRequestWithoutMan);
            }
        }

        HttpWebResponse Stage1 (ServiceAction action, WebHeaderCollection headers, int retry, Stream stream,
                               RequestProvider requestProvider1, RequestProvider requestProvider2)
        {
            var response = Stage2 (requestProvider1, action, headers, retry, stream);
            if (response.StatusCode == HttpStatusCode.MethodNotAllowed) {
                response = Stage2 (requestProvider2, action, headers, retry, stream);
            }
            return response;
        }

        HttpWebRequest CreateRequestWithoutMan (ServiceAction action, WebHeaderCollection headers)
        {
            fallback.OmitMan = true;
            var request = CreateRequest (headers);
            request.Method = "POST";
            request.Headers.Add ("SOAPACTION", string.Format (
                @"""{0}#{1}""", action.Controller.Description.Type, action.Name));
            return request;
        }

        HttpWebRequest CreateRequestWithMan (ServiceAction action, WebHeaderCollection headers)
        {
            fallback.OmitMan = false;
            var request = CreateRequest (headers);
            request.Method = "M-POST";
            request.Headers.Add ("MAN", string.Format (@"""{0}""; ns=01", Protocol.SoapEnvelopeSchema));
            request.Headers.Add ("01-SOAPACTION", string.Format (
                @"""{0}#{1}""", action.Controller.Description.Type, action.Name));
            return request;
        }

        HttpWebRequest CreateRequest (WebHeaderCollection headers)
        {
            var request = (HttpWebRequest)WebRequest.Create (location);
            // TODO ascii?
            request.UserAgent = Protocol.UserAgent;
            request.ContentType = @"text/xml; charset=""utf-8""";
            request.KeepAlive = false;
            foreach (string header in headers.Keys) {
                request.Headers.Add (header, headers[header]);
            }
            return request;
        }

        HttpWebResponse Stage2 (RequestProvider requestProvider, ServiceAction action,
                                        WebHeaderCollection headers, int retry, Stream stream)
        {
            var request = requestProvider (action, headers);
            if (fallback.Chuncked) {
                return Stage2 (request, action, retry, stream, Stage3Chuncked, Stage3Unchunked);
            } else {
                return Stage2 (request, action, retry, stream, Stage3Unchunked, Stage3Chuncked);
            }
        }

        HttpWebResponse Stage2 (HttpWebRequest request, ServiceAction action, int retry, Stream stream,
                                        RequestHandler requestHandler1, RequestHandler requestHandler2)
        {
            try {
                var response = requestHandler1 (request, action, retry, stream);
                if (response.StatusCode == HttpStatusCode.HttpVersionNotSupported) {
                    throw new ProtocolViolationException ();
                }
                return response;
            } catch (ProtocolViolationException) {
                return requestHandler2 (request, action, retry, stream);
            }
        }

        HttpWebResponse Stage3Chuncked (HttpWebRequest request, ServiceAction action,
                                                int retry, Stream stream)
        {
            fallback.Chuncked = true;
            request.ProtocolVersion = new Version (1, 1);
            request.SendChunked = true;
            return Final (request, action, retry, stream);
        }

        HttpWebResponse Stage3Unchunked (HttpWebRequest request, ServiceAction action,
                                                 int retry, Stream stream)
        {
            fallback.Chuncked = false;
            request.ProtocolVersion = new Version (1, 0);
            request.SendChunked = false;
            return Final (request, action, retry, stream);
        }

        HttpWebResponse Final (HttpWebRequest request, ServiceAction action, int retry, Stream stream)
        {
            stream.Seek (0, SeekOrigin.Begin);
            if (!fallback.Chuncked) {
                request.ContentLength = stream.Length;
            }
            using (var output = request.GetRequestStream ()) {
                var buffer = new byte[1024];
                int count;
                while ((count = stream.Read (buffer, 0, 1024)) > 0) {
                    output.Write (buffer, 0, count);
                }
            }
            return Helper.GetResponse (request, retry);
        }
    }
}
