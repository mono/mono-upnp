// 
// RootDevice.cs
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

using System;
using System.Collections.Generic;

using Mono.Upnp.Server.Serialization;

namespace Mono.Upnp.Server
{
    public class RootDevice : Device
    {
        readonly List<Device> devices = new List<Device> ();
        
        public RootDevice(DeviceType type, string id, string friendlyName, string manufacturer, string modelName)
            : base (type, id, friendlyName, manufacturer, modelName)
        {
        }
        
        [XmlArray ("deviceList")]
        public virtual IEnumerable<Device> Devices {
            get { return devices; }
        }
        
        public virtual void AddDevice (Device device)
        {
            CheckInitialized ();
            devices.Add (device);
        }
        
        protected override void InitializeCore (Uri deviceUrl)
        {
            base.InitializeCore (baseUrl);
            
            foreach (var device in devices) {
                device.Initialize (new Uri (deviceUrl, string.Format ("{0}/{1}/", device.Type.ToUrlString (), device.Id)));
            }
        }

        protected internal override void Start ()
        {
            base.Start ();
            
            foreach (var device in devices) {
                device.Start ();
            }
        }
        
        protected internal override void Stop ()
        {
            base.Stop ();
            
            foreach (var device in devices) {
                device.Stop ();
            }
        }
        
        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            context.AutoSerializeObjectAndMembers (this);
        }
        
        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            context.AutoSerializeMembersOnly (this);
        }
        
        protected override void Dispose (bool disposing)
        {
            base.Dispose (disposing);
            
            if (!disposing) {
                return;
            }
            
            foreach (var device in devices) {
                device.Dispose ();
            }
        }
    }
}
