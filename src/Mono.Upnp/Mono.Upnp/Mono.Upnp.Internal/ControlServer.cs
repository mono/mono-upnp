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

        public ControlServer (IMap<string, ServiceAction> actions, string serviceType, Uri url, XmlSerializer serializer)
            : base (url)
        {
            this.actions = actions;
            this.service_type = serviceType;
            this.serializer = serializer;
            this.deserializer = Helper.Get<XmlDeserializer> (static_deserializer);
        }
        
        protected override void HandleContext (HttpListenerContext context)
        {
            base.HandleContext (context);
            
            context.Response.ContentType = @"text/xml; charset=""utf-8""";
            
            using (var reader = XmlReader.Create (context.Request.InputStream)) {
                if (!reader.ReadToFollowing ("Envelope", Protocol.SoapEnvelopeSchema)) {
                    Log.Error (string.Format (
                        "A control request from {0} to {1} does not have a SOAP envelope.",
                        context.Request.RemoteEndPoint, context.Request.Url));
                    return;
                }
                
                var requestEnvelope = deserializer.Deserialize<SoapEnvelope<Arguments>> (reader);
                
                if (requestEnvelope == null) {
                    Log.Error (string.Format (
                        "A control request from {0} to {1} does not have a valid SOAP envelope.",
                        context.Request.RemoteEndPoint, context.Request.Url));
                    return;
                }
                
                var arguments = requestEnvelope.Body;
                
                if (arguments == null) {
                    Log.Error (string.Format (
                        "A control request from {0} to {1} does not have a valid argument list.",
                        context.Request.RemoteEndPoint, context.Request.Url));
                    return;
                }
                
                ServiceAction action;
                if (actions.TryGetValue (arguments.ActionName, out action)) {
                    try {
                        serializer.Serialize (
                            new SoapEnvelope<Arguments> (new Arguments (service_type, action.Name, action.Execute (arguments.Values), true)),
                            context.Response.OutputStream
                        );
                        
                        Log.Information (string.Format ("{0} invoked {1} on {2}.",
                            context.Request.RemoteEndPoint, arguments.ActionName, context.Request.Url));
                    } catch {
                        // TODO handle faults
                    }
                } else {
                    Log.Error (string.Format (
                        "{0} attempted to invoke the non-existant action {1} on {2}.",
                        context.Request.RemoteEndPoint, arguments.ActionName, context.Request.Url));
                    // TODO generate fault
                }
            }
        }
    }
}
