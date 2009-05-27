//
// Service.cs
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

using Mono.Upnp.Control;
using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp
{
    [XmlType ("service", Protocol.DeviceUrn)]
    public class Service : DeviceDescriptionBase
    {
        ServiceController controller;
        DataServer scpd_server;
        ControlServer control_server;
        EventServer event_server;

        protected internal Service (Deserializer deserializer)
            : base (deserializer)
        {
        }
        
        protected Service (ServiceController controller)
        {
            if (controller == null) throw new ArgumentNullException ("controller");
            
            this.controller = controller;
        }
        
        protected internal Service (ServiceController controller, ServiceType type, string id)
            : this (controller)
        {
            Type = type;
            Id = id;
        }
        
        [XmlElement ("serviceType", Protocol.DeviceUrn)]
        public virtual ServiceType Type { get; protected set; }
        
        [XmlElement ("serviceId", Protocol.DeviceUrn)]
        public virtual string Id { get; protected set; }
        
        [XmlElement ("SCPDURL", Protocol.DeviceUrn)]
        public virtual Uri ScpdUrl { get; protected set; }
        
        [XmlElement ("controlURL", Protocol.DeviceUrn)]
        public virtual Uri ControlUrl { get; protected set; }
        
        [XmlElement ("eventSubURL", Protocol.DeviceUrn)]
        public virtual Uri EventUrl { get; protected set; }
        
        protected internal virtual void Initialize (Uri serviceUrl)
        {
            if (serviceUrl == null) throw new ArgumentNullException ("serviceUrl");
            if (Deserializer != null) throw new InvalidOperationException ("This service has been constructred for deserialization - it cannot be initialized. Use one of the other constructor.");
            
            ScpdUrl = new Uri (serviceUrl, "scpd/");
            ControlUrl = new Uri (serviceUrl, "control/");
            EventUrl = new Uri (serviceUrl, "event/");
            //scpd_server = new DataServer (controller, ScpdUrl);
            control_server = new ControlServer (controller, ControlUrl);
            event_server = new EventServer (controller, EventUrl);
        }
        
        protected internal virtual void Start ()
        {
        }
        
        protected internal virtual void Stop ()
        {
        }

        public ServiceController GetController ()
        {
            if (controller == null) {
                if (IsDisposed) {
                    throw new ObjectDisposedException (ToString (), "The service has gone off the network.");
                }
                if (ScpdUrl == null) {
                    throw new InvalidOperationException (
                        "Attempting to get a controller from a services with no SCPDURL.");
                }
                //controller = deserializer.DeserializeController (this);
            }
            
            return controller;
        }
        
        [XmlTypeDeserializer]
        protected virtual ServiceType DeserializeServiceType (XmlDeserializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            return new ServiceType (context.Reader.ReadElementContentAsString ());
        }

        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoDeserializeElement (this);
        }
        
        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoSerializeObjectAndMembers (this);
        }
        
        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoSerializeMembersOnly (this);
        }

        public override string ToString ()
        {
            return string.Format ("ServiceDescription {{ {0}, {1} }}", Id, Type);
        }
    }
}
