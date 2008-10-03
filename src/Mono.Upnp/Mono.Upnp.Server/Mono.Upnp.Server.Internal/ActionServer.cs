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
            XmlReader reader = XmlReader.Create (context.Request.InputStream);
            // TODO a Protocol.cs file
            reader.ReadToFollowing ("Body", "http://schemas.xmlsoap.org/soap/envelope/");
            while (Helper.ReadToNextElement (reader)) {
                if (!service.Actions.ContainsKey (reader.LocalName)) {
                }
                Action action = service.Actions[reader.LocalName];
                BriefAction (reader.ReadSubtree (), action);
                try {
                    action.Execute ();
                    DebriefAction (context, action);
                } catch {
                }
            }
            context.Response.Close ();
        }

        private void BriefAction (XmlReader reader, Action action)
        {
            reader.Read ();
            foreach (Argument argument in action.Arguments.Values) {
                argument.Value = argument.RelatedStateVariable.DefaultValue;
            }
            while (Helper.ReadToNextElement (reader)) {
                if (!action.Arguments.ContainsKey (reader.Name)) {
                    // TODO die
                }
                Argument argument = action.Arguments[reader.Name];
                if (argument.RelatedStateVariable.DataType.IsEnum) {
                    try {
                        argument.Value = Enum.Parse (argument.RelatedStateVariable.DataType, reader.ReadString ());
                    } catch {
                    }
                } else {
                    try {
                        argument.Value = Convert.ChangeType (reader.ReadString (), argument.RelatedStateVariable.DataType);
                    } catch {
                    }
                    // TODO handle min, max, and step
                }
            }
            reader.Close ();
        }

        private void DebriefAction (HttpListenerContext context, Action action)
        {
            context.Response.StatusCode = 200;
            context.Response.StatusDescription = "OK";
            context.Response.ContentType = @"text/xml; charset=""utf-8""";
            
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
                    writer.WriteStartElement (argument.Name);
                    writer.WriteValue (argument.Value);
                    writer.WriteEndElement ();
                }
            }
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
