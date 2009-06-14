//
// ControlServer.cs
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
using System.Xml;

using Mono.Upnp.Control;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Internal
{
    sealed class ControlServer : UpnpServer
    {
        static readonly WeakReference static_deserializer = new WeakReference (null);
        
        readonly IMap<string, ServiceAction> actions;
        readonly string service_type;
        readonly XmlSerializer serializer;
        readonly XmlDeserializer deserializer;

        public ControlServer (IMap<string, ServiceAction> actions, string serviceType, XmlSerializer serializer, Uri url)
            : base (url)
        {
            this.actions = actions;
            this.service_type = serviceType;
            this.serializer = serializer;
            if (static_deserializer.IsAlive) {
                this.deserializer = (XmlDeserializer)static_deserializer.Target;
            } else {
                this.deserializer = new XmlDeserializer ();
                static_deserializer.Target = this.deserializer;
            }
        }
        
        protected override void HandleContext (HttpListenerContext context)
        {
            base.HandleContext (context);
            
            context.Response.ContentType = @"text/xml; charset=""utf-8""";
            using (var reader = XmlReader.Create (context.Request.InputStream)) {
                reader.ReadToFollowing ("Envelope", Protocol.SoapEnvelopeSchema);
                var requestEnvelope = deserializer.Deserialize<SoapEnvelope<Arguments>> (reader);
                var arguments = requestEnvelope.Body;
                ServiceAction action;
                if (actions.TryGetValue (arguments.ActionName, out action)) {
                    try {
                        serializer.Serialize (
                            new SoapEnvelope<Arguments> (new Arguments (service_type, action.Name, action.Execute (arguments.Values), true)),
                            context.Response.OutputStream
                        );
                    } catch (Exception e) {
                        // TODO handle faults
                    }
                } else {
                    // TODO report fault
                }
            }
        }
    }
}
