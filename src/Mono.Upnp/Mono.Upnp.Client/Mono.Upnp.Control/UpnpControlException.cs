//
// UpnpControlException.cs
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
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp.Control
{
	public class UpnpControlException : Exception
	{
        private struct FaultCode
        {
            public string Message;
            public UpnpControlExceptionStatus Status;
        }

        public UpnpControlException (XmlReader reader)
            : this (Deserialize(reader))
        {
        }

        private UpnpControlException (FaultCode code)
            : base (code.Message)
        {
            this.status = code.Status;
        }

        private UpnpControlExceptionStatus status;
        public UpnpControlExceptionStatus Status {
            get { return status; }
        }

        private static FaultCode Deserialize (XmlReader reader)
        {
            FaultCode code = new FaultCode ();
            reader.ReadToFollowing ("UPnPError", "urn:schemas-upnp-org:control-1-0");
            while (Helper.ReadToNextElement (reader)) {
                Deserialize (reader.ReadSubtree (), reader.Name, ref code);
            }
            reader.Close ();
            return code;
        }

        private static void Deserialize(XmlReader reader, string element, ref FaultCode code)
        {
            reader.Read ();
            switch (element) {
            case "errorCode":
                reader.Read ();
                code.Status = (UpnpControlExceptionStatus)reader.ReadContentAsInt ();
                break;
            case "errorDescription":
                code.Message = reader.ReadString ();
                break;
            default:
                reader.Skip (); // This is a workaround for Mono bug 334752
                break;
            }
            reader.Close ();
        }
	}
}
