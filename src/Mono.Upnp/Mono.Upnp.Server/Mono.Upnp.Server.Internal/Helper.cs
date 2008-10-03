using System;
using System.Xml;

namespace Mono.Upnp.Server.Internal
{
	internal static class Helper
	{
        public static string GetDataType (Type type)
        {
            if (type == typeof (byte)) {
                return "ui1";
            } else if (type == typeof (ushort)) {
                return "ui2";
            } else if (type == typeof (uint)) {
                return "ui4";
            } else if (type == typeof (sbyte)) {
                return "i1";
            } else if (type == typeof (short)) {
                return "i2";
            } else if (type == typeof (int)) {
                return "i4";
            } else if (type == typeof (long)) {
                return "int";
            } else if (type == typeof (float)) {
                return "r4";
            } else if (type == typeof (double)) {
                return "r8";
            } else if (type == typeof (char)) {
                return "char";
            } else if (type == typeof (string)) {
                return "string";
            }
            // TODO handle this better
            else if (type == typeof (DateTime)) {
                return "dateTime";
            } else if (type == typeof (bool)) {
                return "boolean";
            } else if (type == typeof (byte[])) {
                return "bin.base64";
            } else if (type == typeof (Uri)) {
                return "uri";
            } else if (type.IsEnum) {
                return "string";
            } else {
                throw new UpnpException (String.Format ("The type {0} is not a supported UPnP type.", type));
            }
        }

        public static bool ReadToNextElement (XmlReader reader)
        {
            while (reader.Read ()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    return true;
                }
            }
            return false;
        }

        public static void WriteStartSoapBody (XmlWriter writer)
        {
            writer.WriteStartDocument ();
            writer.WriteStartElement ("s", "Envelope", Protocol.SoapEnvelopeSchema);
            writer.WriteAttributeString ("encodingStyle", Protocol.SoapEnvelopeSchema, Protocol.SoapEncodingSchema);
            writer.WriteStartElement ("Body", Protocol.SoapEnvelopeSchema);
        }

        public static void WriteEndSoapBody (XmlWriter writer)
        {
            writer.WriteEndElement ();
            writer.WriteEndElement ();
            writer.WriteEndDocument ();
        }

        public static void WriteSpecVersion (XmlWriter writer)
        {
            writer.WriteStartElement ("specVersion");
            writer.WriteStartElement ("major");
            writer.WriteValue (1);
            writer.WriteEndElement ();
            writer.WriteStartElement ("minor");
            writer.WriteValue (1);
            writer.WriteEndElement ();
            writer.WriteEndElement ();
        }
    }
}
