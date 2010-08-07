// 
// Error.cs
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

using Mono.Upnp.Control;
using Mono.Upnp.Internal;

namespace Mono.Upnp.Dcp.MediaServer1
{
    public static class Error
    {
        public static UpnpError NoSuchObject ()
        {
            return NoSuchObject (null);
        }

        public static UpnpError NoSuchObject (string message)
        {
            return new UpnpError (701, Helper.MakeErrorDescription ("No such object", message));
        }

        public static UpnpError InvalidCurrentTagValue ()
        {
            return InvalidCurrentTagValue (null);
        }

        public static UpnpError InvalidCurrentTagValue (string message)
        {
            return new UpnpError (702, Helper.MakeErrorDescription ("Invalid CurrentTagValue", message));
        }

        public static UpnpError InvalidNewTagValue ()
        {
            return InvalidNewTagValue (null);
        }

        public static UpnpError InvalidNewTagValue (string message)
        {
            return new UpnpError (703, Helper.MakeErrorDescription ("Invalid NewTagValue", message));
        }

        public static UpnpError RequiredTag ()
        {
            return RequiredTag (null);
        }

        public static UpnpError RequiredTag (string message)
        {
            return new UpnpError (704, Helper.MakeErrorDescription ("Required tag", message));
        }

        public static UpnpError ReadOnlyTag ()
        {
            return ReadOnlyTag (null);
        }

        public static UpnpError ReadOnlyTag (string message)
        {
            return new UpnpError (705, Helper.MakeErrorDescription ("Read only tag", message));
        }

        public static UpnpError ParameterMismatch ()
        {
            return ParameterMismatch (null);
        }

        public static UpnpError ParameterMismatch (string message)
        {
            return new UpnpError (706, Helper.MakeErrorDescription ("Parameter mismatch", message));
        }

        public static UpnpError UnsupportedOrInvalidSearchCriteria ()
        {
            return UnsupportedOrInvalidSearchCriteria (null);
        }

        public static UpnpError UnsupportedOrInvalidSearchCriteria (string message)
        {
            return new UpnpError (709, Helper.MakeErrorDescription (
                "Unsupported or invalid search criteria", message));
        }

        public static UpnpError NoSuchContainer ()
        {
            return NoSuchContainer (null);
        }

        public static UpnpError NoSuchContainer (string message)
        {
            return new UpnpError (710, Helper.MakeErrorDescription ("No such container", message));
        }

        public static UpnpError RestrictedObject ()
        {
            return RestrictedObject (null);
        }

        public static UpnpError RestrictedObject (string message)
        {
            return new UpnpError (711, Helper.MakeErrorDescription ("Restricted object", message));
        }

        public static UpnpError BadMetadata ()
        {
            return BadMetadata (null);
        }

        public static UpnpError BadMetadata (string message)
        {
            return new UpnpError (712, Helper.MakeErrorDescription ("Bad metadata", message));
        }

        public static UpnpError RestrictedParentObject ()
        {
            return RestrictedParentObject (null);
        }

        public static UpnpError RestrictedParentObject (string message)
        {
            return new UpnpError (713, Helper.MakeErrorDescription ("Restricted parent object", message));
        }

        public static UpnpError NoSuchSourceResource ()
        {
            return NoSuchSourceResource (null);
        }

        public static UpnpError NoSuchSourceResource (string message)
        {
            return new UpnpError (714, Helper.MakeErrorDescription ("No such source resource", message));
        }

        public static UpnpError SourceResourceAccessDenied ()
        {
            return SourceResourceAccessDenied (null);
        }

        public static UpnpError SourceResourceAccessDenied (string message)
        {
            return new UpnpError (715, Helper.MakeErrorDescription ("Source resource access denied", message));
        }

        public static UpnpError TransferBusy ()
        {
            return TransferBusy (null);
        }

        public static UpnpError TransferBusy (string message)
        {
            return new UpnpError (716, Helper.MakeErrorDescription ("Transfer busy", message));
        }

        public static UpnpError NoSuchFileTransfer ()
        {
            return NoSuchFileTransfer (null);
        }

        public static UpnpError NoSuchFileTransfer (string message)
        {
            return new UpnpError (717, Helper.MakeErrorDescription ("No such file transfer", message));
        }

        public static UpnpError NoSuchDestinationResource ()
        {
            return NoSuchDestinationResource (null);
        }

        public static UpnpError NoSuchDestinationResource (string message)
        {
            return new UpnpError (718, Helper.MakeErrorDescription ("No such destination resource", message));
        }

        public static UpnpError DestinationResourceAccessDenied ()
        {
            return DestinationResourceAccessDenied (null);
        }

        public static UpnpError DestinationResourceAccessDenied (string message)
        {
            return new UpnpError (719, Helper.MakeErrorDescription ("Destination resource access denied", message));
        }

        public static UpnpError CannotProcessTheRequest ()
        {
            return CannotProcessTheRequest (null);
        }

        public static UpnpError CannotProcessTheRequest (string message)
        {
            return new UpnpError (720, Helper.MakeErrorDescription ("Cannot process the request", message));
        }
    }
}
