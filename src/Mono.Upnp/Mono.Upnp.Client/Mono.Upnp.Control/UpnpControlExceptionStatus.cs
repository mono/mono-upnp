//
// UpnpControlExceptionStatus.cs
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

namespace Mono.Upnp.Control
{
	public enum UpnpControlExceptionStatus
	{
        InvalidAction = 401,
        InvalidArgs = 402,
        InvalidVar = 404,
        ActionFailed = 501,
        ArgumentValueInvalid = 600,
        ArgumentValueOutOfRange = 601,
        OptionalActionNotImplimented = 602,
        OutOfMemory = 603,
        HumanInterventionRequired = 604,
        StringArgumentTooLong = 605,
        ActionNotAuthorized = 606,
        SignatureFailure = 607,
        SignatureMissing = 608,
        NotEncrypted = 609,
        InvalidSequence = 610,
        InvalidControlUrl = 611,
        NoSuchSession = 612
	}
}
