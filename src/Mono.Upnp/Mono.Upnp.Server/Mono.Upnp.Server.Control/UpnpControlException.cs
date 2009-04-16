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

namespace Mono.Upnp.Server
{
	public class UpnpControlException : Exception
	{
        private readonly int code;

        public UpnpControlException (int code, string message)
            : base (message)
        {
            this.code = code;
        }

        public int Code {
            get { return code; }
        }

        public static UpnpControlException InvalidAction ()
        {
            return InvalidAction ("No action by that name at this service.");
        }

        public static UpnpControlException InvalidAction (string message)
        {
            return new UpnpControlException (401, message);
        }

        public static UpnpControlException InvalidArgs ()
        {
            return InvalidArgs ("Could be any of the following: not enough in args, too many in args, no in arg by that name, one or more in args are of the wrong data type.");
        }

        public static UpnpControlException InvalidArgs (string message)
        {
            return new UpnpControlException (402, message);
        }

        public static UpnpControlException ActionFailed ()
        {
            return ActionFailed("May be returned in current state of service prevents invoking that action.");
        }

        public static UpnpControlException ActionFailed (string message)
        {
            return new UpnpControlException (501, message);
        }

        public static UpnpControlException ArgumentValueInvalid ()
        {
            return ArgumentValueInvalid ("The argument value is invalid.");
        }

        public static UpnpControlException ArgumentValueInvalid (string message)
        {
            return new UpnpControlException (600, message);
        }

        public static UpnpControlException ArgumentValueOutOfRange ()
        {
            return ArgumentValueOutOfRange ("An argument value is less than the minimum or more than the maximum value of the allowedValueRange, or is not in the allowedValueList.");
        }

        public static UpnpControlException ArgumentValueOutOfRange (string message)
        {
            return new UpnpControlException (601, message);
        }

        public static UpnpControlException OptionalActionNotImplimented ()
        {
            return OptionalActionNotImplimented ("The requested action is optional and is not implemented by the device.");
        }

        public static UpnpControlException OptionalActionNotImplimented (string message)
        {
            return new UpnpControlException (602, message);
        }

        public static UpnpControlException OutOfMemory ()
        {
            return OutOfMemory ("The device does not have sufficient memory available to complete the action. This may be a temporary condition; the control point may choose to retry the unmodified request again later and it may succeed if memory is available.");
        }

        public static UpnpControlException OutOfMemory (string message)
        {
            return new UpnpControlException (603, message);
        }

        public static UpnpControlException HumanInterventionRequired ()
        {
            return HumanInterventionRequired ("The device has encountered an error condition which it cannot resolve itself and required human intervention such as a reset or power cycle. See the device display or documentation for further guidance.");
        }

        public static UpnpControlException HumanInterventionRequired (string message)
        {
            return new UpnpControlException (604, message);
        }

        public static UpnpControlException StringArgumentTooLong ()
        {
            return StringArgumentTooLong ("A string argument is too long for the device to handle properly.");
        }

        public static UpnpControlException StringArgumentTooLong (string message)
        {
            return new UpnpControlException (605, message);
        }

        public static UpnpControlException ActionNotAuthorized ()
        {
            return ActionNotAuthorized ("The action requested requires authorization and the sender was not authorized.");
        }

        public static UpnpControlException ActionNotAuthorized (string message)
        {
            return new UpnpControlException (606, message);
        }

        public static UpnpControlException SignatureFailure ()
        {
            return SignatureFailure ("The sender's signature failed to verify.");
        }

        public static UpnpControlException SignatureFailure (string message)
        {
            return new UpnpControlException (607, message);
        }

        public static UpnpControlException SignatureMissing ()
        {
            return SignatureMissing ("The action requested requires a digital signature and there was none provided.");
        }

        public static UpnpControlException SignatureMissing (string message)
        {
            return new UpnpControlException (608, message);
        }

        public static UpnpControlException NotEncrypted ()
        {
            return NotEncrypted ("This action requires confidentiality but the action was not delivered encrypted.");
        }

        public static UpnpControlException NotEncrypted (string message)
        {
            return new UpnpControlException (609, message);
        }

        public static UpnpControlException InvalidSequence ()
        {
            return InvalidSequence ("The <sequence> provided was not valid.");
        }

        public static UpnpControlException InvalidSequence (string message)
        {
            return new UpnpControlException (610, message);
        }

        public static UpnpControlException InvalidControlUrl ()
        {
            return InvalidControlUrl ("The controlURL within the <freshness> element does not match the controlURL of the action actually invoked (or the controlURL in the HTTP header).");
        }

        public static UpnpControlException InvalidControlUrl (string message)
        {
            return new UpnpControlException (611, message);
        }

        public static UpnpControlException NoSuchSession ()
        {
            return NoSuchSession ("The session key reference is to a non-existent session. This could be because the device has expired a session, in which case the control point needs to open a new one.");
        }

        public static UpnpControlException NoSuchSession (string message)
        {
            return new UpnpControlException (612, message);
        }
    }
}
