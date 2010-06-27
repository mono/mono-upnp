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
using System.Xml;

using Mono.Upnp.Control;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Internal
{
    sealed class ControlClient
    {
        readonly static WeakReference static_serializer = new WeakReference (null);
        
        readonly string service_type;
        readonly Uri url;
        readonly XmlSerializer serializer;
        readonly XmlDeserializer deserializer;
        
        public ControlClient (string serviceType, Uri url, XmlDeserializer deserializer)
        {
            this.service_type = serviceType;
            this.url = url;
            this.deserializer = deserializer;
            
            if (static_serializer.IsAlive) {
                this.serializer = (XmlSerializer)static_serializer.Target;
            } else {
                this.serializer = new XmlSerializer ();
                static_serializer.Target = this.serializer;
            }
        }
        
        public IMap<string, string> Invoke (string actionName, IDictionary<string, string> arguments)
        {
            var request = (HttpWebRequest)WebRequest.Create (url);
            request.Method = "POST";
            request.ContentType = @"text/xml; charset=""utf-8""";
            request.UserAgent = Protocol.UserAgent;
            request.Headers.Add ("SOAPACTION", string.Format (@"""{0}#{1}""", service_type, actionName));
            using (var stream = request.GetRequestStream ()) {
                serializer.Serialize (
                    new SoapEnvelope<Arguments> (new Arguments (service_type, actionName, arguments)), stream);
            }
            
            HttpWebResponse response;
            WebException exception;
            try {
                response = (HttpWebResponse)request.GetResponse ();
                exception = null;
            } catch (WebException e) {
                response = e.Response as HttpWebResponse;
                if (response == null) {
                    // TODO check for timeout
                    throw new UpnpControlException (UpnpError.Unknown(), "The invokation failed.", e);
                }
                exception = e;
            }
            
            using (response) {
                switch (response.StatusCode) {
                case HttpStatusCode.OK:
                    using (var reader = XmlReader.Create (response.GetResponseStream ())) {
                        // FIXME this is a workaround for Mono bug 523151
                        reader.MoveToContent ();
                        var envelope = deserializer.Deserialize<SoapEnvelope<Arguments>> (reader);
                        if (envelope == null) {
                            Log.Error (string.Format (
                                "The response to the {0} action request on {1} has no envelope.", actionName, url));
                            throw new UpnpControlException (UpnpError.Unknown(),
                                "The service did not provide a valid response (unable to deserialize SOAP envelope).");
                        } else if (envelope.Body == null) {
                            Log.Error (string.Format (
                                "The response to the {0} action request on {1} " +
                                "has no envelope body.", actionName, url));
                            throw new UpnpControlException (UpnpError.Unknown(),
                                "The service did not provide a valid response " +
                                "(unable to deserialize SOAP envelope body).");
                        }
                        return new Map<string, string> (envelope.Body.Values);
                    }
                case HttpStatusCode.InternalServerError:
                    using (var reader = XmlReader.Create (response.GetResponseStream ())) {
                        // FIME this is a workaround for Mono bug 523151
                        reader.MoveToContent ();
                        var envelope = deserializer
                            .Deserialize<SoapEnvelope<XmlShell<SoapFault<XmlShell<UpnpError>>>>> (reader);
                        if (envelope == null) {
                            Log.Error (string.Format (
                                "The faulty response to the {0} action request " +
                                "on {1} has no envelope.", actionName, url));
                            throw new UpnpControlException (UpnpError.Unknown(),
                                "The invokation failed but the service did not provide valid fault information " +
                                "(unable to deserialize SOAP envelope).", exception);
                        } else if (envelope.Body == null) {
                            Log.Error (string.Format (
                                "The faulty response to the {0} action request on {1} " +
                                "has no envelope body.", actionName, url));
                            throw new UpnpControlException (UpnpError.Unknown(),
                                "The invokation failed but the service did not provide valid fault information " +
                                "(unable to deserialize SOAP envelope body).", exception);
                        } else if (envelope.Body.Value.Detail == null || envelope.Body.Value.Detail.Value == null) {
                            Log.Error (string.Format (
                                "The faulty response to the {0} action request on {1} has no UPnPError. " +
                                @"The faultcode and faultstring are ""{2}"" and ""{3}"" respectively.",
                                actionName, url, envelope.Body.Value.FaultCode, envelope.Body.Value.FaultString));
                            throw new UpnpControlException (UpnpError.Unknown(),
                                "The invokation failed but the service did not provide valid fault information " +
                                "(unable to deserialize a UPnPError from the SOAP envelope).", exception);
                        }
                        throw new UpnpControlException (envelope.Body.Value.Detail.Value,
                            "The invokation failed.", exception);
                    }
                default:
                    Log.Error (string.Format (
                        "The response to the {0} action request on {1} returned with status code {2}: {3}.",
                        actionName, url, (int)response.StatusCode, response.StatusDescription));
                    throw new UpnpControlException (UpnpError.Unknown(), "The invokation failed.", exception);
                }
            }
        }
    }
}
