// 
// Deserializer.cs
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
using System.Net;
using System.Xml;

using Mono.Upnp.Control;
using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp
{
    public class Deserializer
    {
        readonly XmlDeserializer deserializer;
        Root root;
        
        public Deserializer (XmlDeserializer xmlDeserializer)
        {
            if (xmlDeserializer == null) {
                throw new ArgumentNullException ("xmlDeserializer");
            }
            
            this.deserializer = xmlDeserializer;
        }
        
        public XmlDeserializer XmlDeserializer {
            get { return deserializer; }
        }
        
        public Root Root {
            get { return root; }
        }
        
        public virtual Root DeserializeRoot (Uri url)
        {
            // TODO retry fallback
            var request = WebRequest.Create (url);
            using (var response = request.GetResponse ()) {
                using (var stream = response.GetResponseStream ()) {
                    using (var reader = XmlReader.Create (stream)) {
                        // FIXME this is a workaround for Mono bug 523151
                        reader.MoveToContent ();
                        return deserializer.Deserialize (reader, context => DeserializeRoot (url, context));
                    }
                }
            }
        }
        
        protected virtual Root DeserializeRoot (Uri url, XmlDeserializationContext context)
        {
            root = CreateRoot (url);
            Deserialize (root, context);
            return root;
        }
        
        protected virtual Root CreateRoot (Uri url)
        {
            return new Root (this, url);
        }
        
        public virtual Device DeserializeDevice (XmlDeserializationContext context)
        {
            var device = CreateDevice ();
            Deserialize (device, context);
            return device;
        }
        
        protected virtual Device CreateDevice ()
        {
            return new Device (this);
        }
        
        public virtual Service DeserializeService (XmlDeserializationContext context)
        {
            var service = CreateService ();
            Deserialize (service, context);
            return service;
        }
        
        protected virtual Service CreateService ()
        {
            return new Service (this);
        }
        
        public virtual Icon DeserializeIcon (XmlDeserializationContext context)
        {
            var icon = CreateIcon ();
            Deserialize (icon, context);
            return icon;
        }
        
        protected virtual Icon CreateIcon ()
        {
            return new Icon (this);
        }
        
        public virtual ServiceAction DeserializeAction (ServiceController controller,
                                                        XmlDeserializationContext context)
        {
            var action = CreateAction (controller);
            Deserialize (action, context);
            return action;
        }
        
        protected virtual ServiceAction CreateAction (ServiceController controller)
        {
            return new ServiceAction (this, controller);
        }
        
        public virtual Argument DeserializeArgument (XmlDeserializationContext context)
        {
            var argument = CreateArgument ();
            Deserialize (argument, context);
            return argument;
        }
        
        protected virtual Argument CreateArgument ()
        {
            return new Argument ();
        }
        
        public virtual StateVariable DeserializeStateVariable (ServiceController controller,
                                                               XmlDeserializationContext context)
        {
            var state_variable = CreateStateVariable (controller);
            Deserialize (state_variable, context);
            return state_variable;
        }
        
        protected virtual StateVariable CreateStateVariable (ServiceController controller)
        {
            return new StateVariable (controller);
        }
        
        public virtual ServiceController GetServiceController (Service service)
        {
            if (service == null) {
                throw new ArgumentNullException ("service");
            } else if (service.ScpdUrl == null) {
                throw new ArgumentException ("The service has no ScpdUrl", "service");
            }
            
            // TODO retry fallback
            var request = WebRequest.Create (service.ScpdUrl);
            using (var response = request.GetResponse ()) {
                using (var stream = response.GetResponseStream ()) {
                    using (var reader = XmlReader.Create (stream)) {
                        // FIXME this is a workaround for Mono bug 523151
                        reader.MoveToContent ();
                        return deserializer.Deserialize (reader, context => DeserializeServiceController (service, context));
                    }
                }
            }
        }
        
        protected virtual ServiceController DeserializeServiceController (Service service, XmlDeserializationContext context)
        {
            var service_controller = CreateServiceController (service);
            Deserialize (service_controller, context);
            return service_controller;
        }
        
        protected virtual ServiceController CreateServiceController (Service service)
        {
            return new ServiceController (this, service);
        }
        
        void Deserialize (IXmlDeserializable deserializable, XmlDeserializationContext context)
        {
            if (deserializable != null) {
                deserializable.Deserialize (context);
            }
        }
        
        internal bool IsDisposed { get; private set; }
        
        internal event EventHandler<DisposedEventArgs> Disposed;
        
        internal void Dispose ()
        {
        }
        
        internal void CheckDisposed ()
        {
        }
    }
}
