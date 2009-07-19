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
                serializer.Serialize (new SoapEnvelope<Arguments> (new Arguments (service_type, actionName, arguments)), stream,
                                      new XmlSerializationSettings { XmlDeclarationType = XmlDeclarationType.None });
            }
            using (var response = (HttpWebResponse)request.GetResponse ()) {
                if (response.StatusCode == HttpStatusCode.OK) {
                    using (var reader = XmlReader.Create (response.GetResponseStream ())) {
                        // FIXME this is a workaround for Mono bug 523151
                        reader.MoveToContent ();
                        var envelope = deserializer.Deserialize<SoapEnvelope<Arguments>> (reader);
                        if (envelope == null) {
                            Log.Error (string.Format ("The response to the {0} action request on {1} has no envelope.", actionName, url));
                        }
                        if (envelope.Body == null) {
                            Log.Error (string.Format ("The response to the {0} action request on {1} has no envelope body.", actionName, url));
                        }
                        return new Map<string, string> (envelope.Body.Values);
                    }
                } else {
                    // TODO handle else
                    return null;
                }
            }
        }
    }
}
