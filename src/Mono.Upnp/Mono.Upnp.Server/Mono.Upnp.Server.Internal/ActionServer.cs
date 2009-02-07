//
// ActionServer.cs
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
using System.Text;
using System.Xml;

namespace Mono.Upnp.Server.Internal
{
	internal class ActionServer : UpnpServer
	{
        private Service service;

        public ActionServer (Service service, Uri url)
            : base (url)
        {
            this.service = service;
        }

        protected override void HandleContext (HttpListenerContext context)
        {
            try {
                HandleContextCore (context);
            } catch (UpnpControlException e) {
                HandleFault (context, e);
            }
        }

        private void HandleContextCore (HttpListenerContext context)
        {
            XmlReader reader = XmlReader.Create (context.Request.InputStream);
            reader.ReadToFollowing ("Body", Protocol.SoapEnvelopeSchema);
            while (reader.ReadToNextElement ()) {
                if (!service.Actions.ContainsKey (reader.LocalName)) {
                    throw UpnpControlException.InvalidAction ();
                }
                Action action = service.Actions[reader.LocalName];
                BriefAction (reader.ReadSubtree (), action);
                try {
                    action.Execute ();
                } catch (UpnpControlException) {
                    throw;
                } catch (ArgumentOutOfRangeException e) {
                    throw UpnpControlException.ArgumentValueOutOfRange (e.Message);
                } catch (ArgumentException e) {
                    throw UpnpControlException.ArgumentValueInvalid (e.Message);
                } catch (NotImplementedException e) {
                    throw UpnpControlException.OptionalActionNotImplimented (e.Message);
                } catch (Exception e) {
                    throw UpnpControlException.ActionFailed (e.Message);
                }
                DebriefAction (context, action);
            }
        }

        private void BriefAction (XmlReader reader, Action action)
        {
            reader.Read ();
            foreach (Argument argument in action.Arguments.Values) {
                argument.Value = argument.RelatedStateVariable.DefaultValue;
            }
            while (reader.ReadToNextElement ()) {
                if (!action.Arguments.ContainsKey (reader.Name)) {
                    throw UpnpControlException.InvalidArgs ();
                }
                Argument argument = action.Arguments[reader.Name];
                if (argument.RelatedStateVariable.DataType.IsEnum) {
                    try {
                        argument.Value = Enum.Parse (argument.RelatedStateVariable.DataType, reader.ReadString ());
                    } catch {
                        throw UpnpControlException.ArgumentValueOutOfRange ();
                    }
                } else {
                    try {
                        argument.Value = Convert.ChangeType (reader.ReadString (), argument.RelatedStateVariable.DataType);
                    } catch {
                        throw UpnpControlException.InvalidArgs ();
                    }
                    // TODO handle min, max, and step
                }
            }
            reader.Close ();
        }

        private void DebriefAction (HttpListenerContext context, Action action)
        {
            context.Response.ContentType = @"text/xml; charset=""utf-8""";
            context.Response.Headers.Add ("Date", DateTime.Now.ToString ("r"));
            context.Response.Headers.Add ("EXT:");
            context.Response.AddHeader ("SERVER", ServerString);

            Stream stream = context.Response.OutputStream;
            XmlWriterSettings settings = new XmlWriterSettings ();
            settings.Encoding = Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create (stream, settings);
            Helper.WriteStartSoapBody (writer);
            writer.WriteStartElement (action.Name + "Response", action.Service.Type.ToString ());

            foreach (Argument argument in action.Arguments.Values) {
                if (argument.Direction == ArgumentDirection.Out) {
                    DebriefArgument (writer, argument);
                }
            }
            if (action.ReturnArgument != null) {
                DebriefArgument (writer, action.ReturnArgument);
            }

            writer.WriteEndElement ();
            Helper.WriteEndSoapBody (writer);
            writer.Flush ();
            context.Response.ContentLength64 = stream.Length;
        }

        private void DebriefArgument (XmlWriter writer, Argument argument)
        {
            writer.WriteStartElement (argument.Name);
            writer.WriteValue (argument.Value);
            writer.WriteEndElement ();
        }

        private void HandleFault (HttpListenerContext context, UpnpControlException e)
        {
            context.Response.StatusCode = 500;
            context.Response.StatusDescription = "Internal Server Error";
            context.Response.ContentType = @"text/xml; charset=""utf-8""";
            context.Response.Headers.Add ("Date", DateTime.Now.ToString ("r"));
            context.Response.Headers.Add ("EXT:");
            context.Response.AddHeader ("SERVER", ServerString);

            Stream stream = context.Response.OutputStream;
            XmlWriterSettings settings = new XmlWriterSettings ();
            settings.Encoding = Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create (stream, settings);
            Helper.WriteStartSoapBody (writer);
            writer.WriteStartElement ("Fault", Protocol.SoapEnvelopeSchema);

            writer.WriteStartElement ("faultcode");
            writer.WriteValue (String.Format ("{0}:Client", writer.LookupPrefix (Protocol.SoapEnvelopeSchema)));
            writer.WriteEndElement ();
            writer.WriteStartElement ("faultstring");
            writer.WriteValue ("UPnPError");
            writer.WriteEndElement ();
            writer.WriteStartElement ("detail");
            writer.WriteStartElement ("UPnPError", Protocol.ControlUrn);
            writer.WriteStartElement ("errorCode");
            writer.WriteValue (e.Code);
            writer.WriteEndElement ();
            writer.WriteStartElement ("errorDescription");
            writer.WriteValue (e.Message);
            writer.WriteEndElement ();
            writer.WriteEndElement ();
            writer.WriteEndElement ();
            writer.WriteEndElement ();

            writer.WriteEndElement ();
            Helper.WriteEndSoapBody (writer);
            writer.Flush ();
            context.Response.ContentLength64 = stream.Length;
        }

        // TODO move to Protocol.cs when it exists
        private string ServerString {
            get { return String.Format ("{0}/{1} UPnP/1.1 Mono.Upnp/1.0", Environment.OSVersion.Platform, Environment.OSVersion.Version); }
        }
    }
}
