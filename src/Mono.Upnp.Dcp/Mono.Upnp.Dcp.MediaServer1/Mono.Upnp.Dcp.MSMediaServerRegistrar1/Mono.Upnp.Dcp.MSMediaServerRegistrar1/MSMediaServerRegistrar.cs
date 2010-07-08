// 
// MSMediaServerRegistrar.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Thomas
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

using System;

using Mono.Upnp.Control;

namespace Mono.Upnp.Dcp.MSMediaServerRegistrar1
{
    public abstract class MSMediaServerRegistrar
    {
        public static readonly ServiceType ServiceType = new ServiceType ("microsoft.com", "X_MS_MediaReceiverRegistrar", new Version (1, 0));
        
        [UpnpAction]
        public virtual void IsAuthorized ([UpnpArgument ("DeviceId")] string deviceId, [UpnpArgument ("Result")] out long result)
        {
            result = IsAuthorized (deviceId);
        }
        
        protected abstract long IsAuthorized (string deviceId);
        
        [UpnpAction]
        public virtual void RegisterDevice ([UpnpArgument ("RegistrationRegMsg")]
                                            [UpnpRelatedStateVariable (DataType = "bin.base64")] byte[] registrationRequestMessage,
                                            [UpnpArgument ("RegistrationRespMsg")]
                                            [UpnpRelatedStateVariable (DataType = "bin.base64")] out byte [] registrationResponseMessage)
        {
            registrationResponseMessage = RegisterDevice (registrationRequestMessage);
        }
        
        protected abstract byte[] RegisterDevice (byte[] registrationRequestMessage);
        
        [UpnpAction]
        public virtual void IsValidated ([UpnpArgument ("DeviceId")] string deviceId, [UpnpArgument ("Result")] out long result)
        {
            result = IsAuthorized (deviceId);
        }
        
        protected abstract long IsValidated (string deviceId);
    }
}
