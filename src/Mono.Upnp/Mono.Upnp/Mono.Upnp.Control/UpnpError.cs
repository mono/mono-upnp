// 
// UpnpError.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Thomas
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Control
{
    [XmlType ("UPnPError", Protocol.ControlSchema)]
    public class UpnpError
    {
        protected internal UpnpError ()
        {
        }
        
        public UpnpError (int errorCode, string errorDescription)
        {
            ErrorCode = errorCode;
            ErrorDescription = errorDescription;
        }
        
        public override string ToString ()
        {
            return string.Format("{0}: {1}", ErrorCode, ErrorDescription);
        }
        
        [XmlElement ("errorCode", Protocol.ControlSchema)]
        public int ErrorCode { get; protected set; }
        
        [XmlElement ("errorDescription", Protocol.ControlSchema, OmitIfNull = true )]
        public string ErrorDescription { get; protected set; }
        
        public static UpnpError Unknown ()
        {
            return Unknown (null);
        }
        
        public static UpnpError Unknown (string message)
        {
            return new UpnpError (0, Helper.MakeErrorDescription ("Unknown", message));
        }
        
        public static UpnpError InvalidAction ()
        {
            return InvalidAction (null);
        }
        
        public static UpnpError InvalidAction (string message)
        {
            return new UpnpError (401, Helper.MakeErrorDescription ("Invalid Action", message));
        }
        
        public static UpnpError InvalidArgs ()
        {
            return InvalidArgs (null);
        }
        
        public static UpnpError InvalidArgs (string message)
        {
            return new UpnpError (402, Helper.MakeErrorDescription ("Invalid Args", message));
        }
        
        public static UpnpError InvalidVar ()
        {
            return InvalidVar (null);
        }
        
        public static UpnpError InvalidVar (string message)
        {
            return new UpnpError (404, Helper.MakeErrorDescription ("Invalid Var", message));
        }
        
        public static UpnpError ActionFailed ()
        {
            return ActionFailed (null);
        }
        
        public static UpnpError ActionFailed (string message)
        {
            return new UpnpError (501, Helper.MakeErrorDescription ("Action Failed", message));
        }
        
        public static UpnpError ArgumentValueInvalid ()
        {
            return ArgumentValueInvalid (null);
        }
        
        public static UpnpError ArgumentValueInvalid (string message)
        {
            return new UpnpError (600, Helper.MakeErrorDescription ("Argument Value Invalid", message));
        }
        
        public static UpnpError ArgumentValueOutOfRange ()
        {
            return ArgumentValueOutOfRange (null);
        }
        
        public static UpnpError ArgumentValueOutOfRange (string message)
        {
            return new UpnpError (601, Helper.MakeErrorDescription ("Argument Value Out Of Range", message));
        }
        
        public static UpnpError OptionalActionNotImplemented ()
        {
            return OptionalActionNotImplemented (null);
        }
        
        public static UpnpError OptionalActionNotImplemented (string message)
        {
            return new UpnpError (602, Helper.MakeErrorDescription ("Optional Action Not Implemented", message));
        }
        
        public static UpnpError OutOfMemory ()
        {
            return OutOfMemory (null);
        }
        
        public static UpnpError OutOfMemory (string message)
        {
            return new UpnpError (603, Helper.MakeErrorDescription ("Out Of Memory", message));
        }
        
        public static UpnpError HumanInterventionRequired ()
        {
            return HumanInterventionRequired (null);
        }
        
        public static UpnpError HumanInterventionRequired (string message)
        {
            return new UpnpError (604, Helper.MakeErrorDescription ("Human Intervention Required", message));
        }
        
        public static UpnpError StringArgumentTooLong ()
        {
            return StringArgumentTooLong (null);
        }
        
        public static UpnpError StringArgumentTooLong (string message)
        {
            return new UpnpError (605, Helper.MakeErrorDescription ("String Argument Too Long", message));
        }
        
        public static UpnpError ActionNotAuthorized ()
        {
            return ActionNotAuthorized (null);
        }
        
        public static UpnpError ActionNotAuthorized (string message)
        {
            return new UpnpError (606, Helper.MakeErrorDescription ("Action Not Authorized", message));
        }
        
        public static UpnpError SignatureFailure ()
        {
            return SignatureFailure (null);
        }
        
        public static UpnpError SignatureFailure (string message)
        {
            return new UpnpError (607, Helper.MakeErrorDescription ("Signature Failure", message));
        }
        
        public static UpnpError SignatureMissing ()
        {
            return SignatureMissing (null);
        }
        
        public static UpnpError SignatureMissing (string message)
        {
            return new UpnpError (608, Helper.MakeErrorDescription ("Signature Missing", message));
        }
        
        public static UpnpError NotEncrypted ()
        {
            return NotEncrypted (null);
        }
        
        public static UpnpError NotEncrypted (string message)
        {
            return new UpnpError (609, Helper.MakeErrorDescription ("Not Encrypted", message));
        }
        
        public static UpnpError InvalidSequence ()
        {
            return InvalidSequence (null);
        }
        
        public static UpnpError InvalidSequence (string message)
        {
            return new UpnpError (610, Helper.MakeErrorDescription ("Invalid Sequence", message));
        }
        
        public static UpnpError InvalidControlUrl ()
        {
            return InvalidControlUrl (null);
        }
        
        public static UpnpError InvalidControlUrl (string message)
        {
            return new UpnpError (611, Helper.MakeErrorDescription ("Invalid Control URL", message));
        }
        
        public static UpnpError NoSuchSession ()
        {
            return NoSuchSession (null);
        }
        
        public static UpnpError NoSuchSession (string message)
        {
            return new UpnpError (612, Helper.MakeErrorDescription ("No Such Session", message));
        }
    }
}
