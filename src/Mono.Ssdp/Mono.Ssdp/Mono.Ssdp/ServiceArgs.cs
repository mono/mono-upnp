//
// ServiceArgs.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2008 Novell, Inc.
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

namespace Mono.Ssdp
{
    public sealed class ServiceArgs : EventArgs
    {
        private readonly ServiceOperation operation;
        private readonly string usn;
        private readonly Service service;
        
        public ServiceArgs (string usn) : this (ServiceOperation.Removed, usn, null)
        {
        }
        
        public ServiceArgs (ServiceOperation operation, Service service) : this (operation, service.Usn, service)
        {
        }
        
        public ServiceArgs (ServiceOperation operation, string usn, Service service)
        {
            this.operation = operation;
            this.usn = usn;
            this.service = service;
        }
        
        public ServiceOperation Operation {
            get { return operation; }
        }
        
        public string Usn {
            get { return usn; }
        }
        
        public Service Service {
            get { return service; }
        }
    }
}
