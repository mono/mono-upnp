// 
// Service.cs
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

using Mono.Upnp.Xml;
using Mono.Upnp.Server.Control;

namespace Mono.Upnp.Server
{
    [XmlType ("service")]
    public class Service : XmlSerializable
    {
        readonly ServiceType type;
        readonly string id;
        readonly ServiceController controller;
        DescriptionServer description_server;
        ActionServer action_server;
        EventServer event_server;
        
        protected Service (ServiceType type, string id, ServiceController controller)
        {
            if (type == null) throw new ArgumentNullException ("type");
            if (id == null) throw new ArgumentNullException ("id");
            if (id.Length == 0) throw new ArgumentException ("The id cannot be an empty string.", "id");
            if (controller == null) throw new ArgumentNullException ("controller");

            this.type = type;
            this.id = id;
            this.controller = controller;
        }
        
        protected internal virtual void Initialize (Uri serviceUrl)
        {
            description_server = new DescriptionServer (controller, new Uri (serviceUrl, "description/"));
            action_server = new ActionServer (controller, new Uri (serviceUrl, "control/"));
            event_server = new EventServer (controller, new Uri (serviceUrl, "event/"));
        }
        
        [XmlElement ("serviceType")]
        public virtual ServiceType Type {
            get { return type; }
        }
        
        [XmlElement ("serviceId")]
        public virtual string Id {
            get { return id; }
        }
        
        [XmlElement ("SCPDURL")]
        public virtual Uri DescriptionUrl {
            get { return description_server.Url; }
        }
        
        [XmlElement ("controlURL")]
        public virtual Uri ActionUrl {
            get { return action_server.Url; }
        }
        
        [XmlElement ("eventSubURL")]
        public virtual Uri EventUrl {
            get { return event_server.Url; }
        }
        
        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            context.AutoSerializeObjectAndMembers (this);
        }
        
        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            context.AutoSerializeMembersOnly (this);
        }
    }
}
