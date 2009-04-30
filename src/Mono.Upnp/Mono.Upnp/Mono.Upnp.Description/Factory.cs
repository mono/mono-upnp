// 
// Factory.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Peterson
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
using Mono.Upnp.Xml;

namespace Mono.Upnp.Description
{
    public class Factory
        : IFactory<DeviceDescription>, IFactory<ServiceDescription>, IFactory<ServiceController>,
          IFactory<ServiceAction>, IFactory<Argument>, IFactory<StateVariable>, IFactory<IconDescription>
    {
        DeviceDescription IFactory<DeviceDescription>.Create ()
        {
            return CreateDeviceDescription ();
        }
        
        ServiceDescription IFactory<ServiceDescription>.Create ()
        {
            return CreateServiceDescription ();
        }
        
        ServiceController IFactory<ServiceController>.Create ()
        {
            return CreateServiceController ();
        }
        
        ServiceAction IFactory<ServiceAction>.Create ()
        {
            return CreateServiceAction ();
        }
        
        Argument IFactory<Argument>.Create ()
        {
            return CreateArgument ();
        }
        
        StateVariable IFactory<StateVariable>.Create ()
        {
            return CreateStateVariable ();
        }
        
        IconDescription IFactory<IconDescription>.Create ()
        {
            return CreateIcon ();
        }
        
        protected virtual DeviceDescription CreateDeviceDescription ()
        {
            return null;
        }
        
        protected virtual ServiceDescription CreateServiceDescription ()
        {
            return null;
        }
        
        protected virtual ServiceController CreateServiceController ()
        {
            return null;
        }
        
        protected virtual ServiceAction CreateServiceAction ()
        {
            return null;
        }
        
        protected virtual Argument CreateArgument ()
        {
            return null;
        }
        
        protected virtual StateVariable CreateStateVariable ()
        {
            return null;
        }
        
        protected virtual IconDescription CreateIcon ()
        {
            return null;
        }
    }
}
