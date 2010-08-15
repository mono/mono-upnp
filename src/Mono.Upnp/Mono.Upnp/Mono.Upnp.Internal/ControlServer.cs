//
// ControlServer.cs
//
// Author:
//   Scott Thomas <lunchtimemama@gmail.com>
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
using System.IO;
using System.Net;
using System.Reflection;
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
        
        public ControlServer (IMap<string, ServiceAction> actions,
                              string serviceType,
                              Uri url,
                              XmlSerializer serializer)
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
            context.Response.AddHeader ("EXT", string.Empty);
            
            using (var reader = XmlReader.Create (context.Request.InputStream)) {
                // FIXME this is a workaround for mono bug 523151
                if (reader.MoveToContent () != XmlNodeType.Element) {
                    Log.Error (string.Format (
                        "A control request from {0} to {1} does not have a SOAP envelope.",
                        context.Request.RemoteEndPoint, context.Request.Url));
                    return;
                }

                SoapEnvelope<Arguments> requestEnvelope;

                try {
                    requestEnvelope = deserializer.Deserialize<SoapEnvelope<Arguments>> (reader);
                } catch (Exception e) {
                    Log.Exception (string.Format (
                        "Failed to deserialize a control request from {0} to {1}.",
                        context.Request.RemoteEndPoint, context.Request.Url), e);
                    return;
                }
                
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

                if (arguments.ActionName == null) {
                    Log.Error (string.Format (
                        "A control request from {0} to {1} does not have an action name.",
                        context.Request.RemoteEndPoint, context.Request.Url));
                    return;
                }
                
                ServiceAction action;

                try {
                    if (actions.TryGetValue (arguments.ActionName, out action)) {
                        Log.Information (string.Format ("{0} invoked {1} on {2}.",
                            context.Request.RemoteEndPoint, arguments.ActionName, context.Request.Url));

                        Arguments result;

                        try {
                            result = new Arguments (
                                service_type, action.Name, action.Execute (arguments.Values), true);
                        } catch (UpnpControlException) {
                            throw;
                        } catch (Exception e) {
                            throw new UpnpControlException (UpnpError.Unknown (), "Unexpected exception.", e);
                        }

                        // TODO If we're allowing consumer code to subclass Argument, then we need to expose that in a
                        // Mono.Upnp.Serializer class. We would then need to put this in a try/catch because custom
                        // serialization code could throw.
                        serializer.Serialize (new SoapEnvelope<Arguments> (result), context.Response.OutputStream);
                    } else {
                        throw new UpnpControlException (UpnpError.InvalidAction (), string.Format (
                            "{0} attempted to invoke the non-existant action {1} on {2}.",
                            context.Request.RemoteEndPoint, arguments.ActionName, context.Request.Url));
                    }
                } catch (UpnpControlException e) {
                    Log.Exception (e);

                    context.Response.StatusCode = 500;
                    context.Response.StatusDescription = "Internal Server Error";

                    // TODO This needs to be a try/catch in the future too.
                    serializer.Serialize (new SoapEnvelope<SoapFault<UpnpError>> (
                        new SoapFault<UpnpError> (e.UpnpError)), context.Response.OutputStream);
                }
            }
        }
    }
}
