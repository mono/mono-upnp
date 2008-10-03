using System;
using System.IO;
using System.Net;
using System.Xml;

namespace Mono.Upnp.Server.Internal
{
    internal delegate void DescriptionSerializer (XmlWriter writer);

	internal class DescriptionServer : UpnpServer
	{
        private readonly DescriptionSerializer serializer;
        private byte[] description;

        public DescriptionServer (DescriptionSerializer serializer, Uri url)
            : base (url)
        {
            this.serializer = serializer;
        }

        protected override void HandleContext (HttpListenerContext context)
        {
            context.Response.StatusCode = 200;
            context.Response.StatusDescription = "OK";
            if (description != null) {
                context.Response.OutputStream.Write (description, 0, description.Length);
            } else {
                Stream stream = new MemoryStream ();
                XmlWriter writer = XmlWriter.Create (stream);
                writer.WriteStartDocument ();
                serializer (writer);
                writer.WriteEndDocument ();
                writer.Flush ();
                stream.Seek (0, SeekOrigin.Begin);
                description = new byte[stream.Length];
                stream.Read (description, 0, description.Length);
                context.Response.OutputStream.Write (description, 0, description.Length);
            }
            context.Response.Close ();
        }
    }
}
