// 
// MediaServer.cs
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
using System.Collections.Generic;

using Mono.Upnp.Dcp.MediaServer1.ConnectionManager1;
using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;

namespace Mono.Upnp.Dcp.MediaServer1
{
    public class MediaServer : IDisposable
    {
        public static readonly DeviceType DeviceType = new DeviceType ("urn:schemas-upnp-org:device:MediaServer:1");
        
        readonly Server server;
        readonly ContentDirectory content_directory;
        
        public MediaServer (string udn, string friendlyName, string manufacturer, string modelName, ConnectionManager connectionManager, ContentDirectory contentDirectory)
            : this (udn, friendlyName, manufacturer, modelName, null, connectionManager, contentDirectory)
        {
        }
        
        public MediaServer (string udn, string friendlyName, string manufacturer, string modelName, RootDeviceOptions options, ConnectionManager connectionManager, ContentDirectory contentDirectory)
        {
            if (connectionManager == null) throw new ArgumentNullException ("connnectionManager");
            if (contentDirectory == null) throw new ArgumentNullException ("contentDirectory");
            
            this.content_directory = contentDirectory;
            
            var connectionManagerService = new Service<ConnectionManager> (ConnectionManager.ServiceType, "urn:upnp-org:serviceId:ConnectionManager", connectionManager);
            var contentDirectoryService = new Service<ContentDirectory> (ContentDirectory.ServiceType, "urn:upnp-org:serviceId:ContentDirectory", contentDirectory);
            options.Services = Combine (new Service[] { connectionManagerService, contentDirectoryService }, options.Services);
            server = new Server (new Root (DeviceType, udn, friendlyName, manufacturer, modelName, options));
        }
        
        public void Start ()
        {
            content_directory.Start ();
            server.Start ();
        }
        
        public void Stop ()
        {
            server.Stop ();
            content_directory.Stop ();
        }
        
        public void Dispose ()
        {
            server.Dispose ();
            content_directory.Dispose ();
        }
        
        IEnumerable<Service> Combine (IEnumerable<Service> first, IEnumerable<Service> second)
        {
            foreach (var service in first) {
                yield return service;
            }
            
            if (second != null) {
                foreach (var service in second) {
                    yield return service;
                }
            }
        }
    }
}
