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
using System.IO;
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
        
        const string fake = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><s:Body>
<u:BrowseResponse xmlns:u=""urn:schemas-upnp-org:service:ContentDirectory:1"">
<Result>&lt;DIDL-Lite xmlns=&quot;urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/&quot; xmlns:dc=&quot;http://purl.org/dc/elements/1.1/&quot; xmlns:upnp=&quot;urn:schemas-upnp-org:metadata-1-0/upnp/&quot;&gt;&lt;container id=&quot;1&quot; parentID=&quot;0&quot; childCount=&quot;11&quot; restricted=&quot;true&quot;&gt;&lt;upnp:class&gt;object.container.storageFolder&lt;/upnp:class&gt;&lt;dc:title&gt;.&lt;/dc:title&gt;&lt;/container&gt;&lt;/DIDL-Lite&gt;</Result>
<NumberReturned>1</NumberReturned>
<TotalMatches>1</TotalMatches>
<UpdateID>0</UpdateID>
</u:BrowseResponse>
</s:Body> </s:Envelope>";

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
                // FIXME this is a workaround for mono bug 523151
                if (reader.MoveToContent () != XmlNodeType.Element) {
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
                    var stream = context.Response.OutputStream;
                    try {
                        serializer.Serialize (
                            new SoapEnvelope<Arguments> (new Arguments (service_type, action.Name, action.Execute (arguments.Values), true)),
                            stream
                        );
                        
                        Log.Information (string.Format ("{0} invoked {1} on {2}.",
                            context.Request.RemoteEndPoint, arguments.ActionName, context.Request.Url));
                    } catch {
                        // TODO handle faults
                    } finally {
                        stream.Close ();
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
