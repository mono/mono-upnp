//
// Deserializer.cs
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
using System.Xml;

using Mono.Upnp.Control;
using Mono.Upnp.Internal;

namespace Mono.Upnp.Description
{
	public class Deserializer : IDeserializer
	{
        Disposer disposer;
        DeviceDescription root_device;

        protected Version SpecVersion { get; private set; }
        protected Uri UrlBase { get; private set; }

        public DeviceDescription DeserializeDescription (Uri url)
        {
            if (url == null) throw new ArgumentNullException ("url");
			if (disposer != null) throw new InvalidOperationException (
				"Deserializers can only be used to deserialize one device description once.");

            disposer = new Disposer (url);
            UrlBase = url;
            return DeserializeDescriptionCore (url);
        }

        protected virtual DeviceDescription DeserializeDescriptionCore (Uri url)
        {
            if (url == null) throw new ArgumentNullException ("url");

            // TODO handle ACCEPT-LANGUAGE
            using (var response = Helper.GetResponse (url)) {
                using (var stream = response.GetResponseStream ()) {
					using (var reader = XmlReader.Create (stream)) {
						if (reader.ReadToFollowing ("root")) {
							using (var root_reader = reader.ReadSubtree ()) {
								root_reader.Read ();
	                    		return DeserializeDescriptionRootElement (reader);
							}
						} else {
							Log.Exception (new UpnpDeserializationException (
								"The device description does not have a root element."));
							return null;
						}
					}
                }
            }
        }

        protected virtual DeviceDescription DeserializeDescriptionRootElement (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                while (Helper.ReadToNextElement (reader)) {
					var property_reader = reader.ReadSubtree ();
					property_reader.Read ();
                    try {
                        DeserializeDescriptionPropertyElement (property_reader);
                    } catch (Exception e) {
                        Log.Exception (
                            "There was a problem deserializing one of the root description elements.", e);
                    } finally {
						property_reader.Close ();
					}
                }
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deserializing a root XML file.", e);
            }

            return root_device;
        }

        protected virtual void DeserializeDescriptionPropertyElement (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            switch (reader.Name) {
            case "specVersion":
                SpecVersion = Helper.DeserializeSpecVersion (reader.ReadSubtree ());
                break;
            case "URLBase":
                UrlBase = new Uri (reader.ReadString ());
                break;
            case "device":
				using (var device_reader = reader.ReadSubtree ()) {
					device_reader.Read ();
	                root_device = DeserializeDevice (device_reader);
	                disposer.SetRootDevice (root_device);
				}
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
        }

        public ServiceController DeserializeController (ServiceDescription service)
        {
            return DeserializeControllerCore (service);
        }

        protected virtual ServiceController DeserializeControllerCore (ServiceDescription service)
        {
            if (service == null)
                throw new ArgumentNullException ("service");
            if (service.ScpdUrl == null)
                throw new ArgumentException ("The services does not have an SCPDURL.", "service");

            using (var response = Helper.GetResponse (service.ScpdUrl)) {
                using (var stream = response.GetResponseStream ()) {
					using (var reader = XmlReader.Create (stream)) {
						if (reader.ReadToFollowing ("scpd")) {
							using (var controller_reader = reader.ReadSubtree ()) {
								controller_reader.Read ();
                    			return DeserializeControllerCore (service, controller_reader);
							}
						} else {
							Log.Exception (new UpnpDeserializationException (
								"The service description does not have an scpd element."));
							return null;
						}
					}
                }
            }
        }

        protected virtual ServiceController DeserializeControllerCore (ServiceDescription service, XmlReader reader)
        {
			try {
	            var controller = CreateController (service);
				if (controller != null) {
	            	controller.Deserialize (reader);
				}
	            return controller;
			} catch (Exception e) {
				Log.Exception ("There was a problem deserializing a service control description.", e);
                return null;
			}
        }

        protected virtual ServiceController CreateController (ServiceDescription service)
        {
            return new ServiceController (service);
        }

        public DeviceDescription DeserializeDevice (XmlReader reader)
        {
            return DeserializeDeviceCore (disposer, reader);
        }

        protected virtual DeviceDescription DeserializeDeviceCore (IDisposer disposer, XmlReader reader)
        {
            try {
                var device = CreateDevice (disposer);
				if (device != null) {
                	device.Deserialize (this, reader);
				}
                return device;
            } catch (Exception e) {
                Log.Exception ("There was a problem deserializing a device.", e);
                return null;
            }
        }

        protected virtual DeviceDescription CreateDevice (IDisposer disposer)
        {
            return new DeviceDescription (disposer);
        }

        public ServiceDescription DeserializeService (XmlReader reader)
        {
            return DeserializeServiceCore (disposer, reader);
        }

        protected virtual ServiceDescription DeserializeServiceCore (IDisposer disposer, XmlReader reader)
        {
            try {
                var service = CreateService (disposer);
				if (service != null) {
                	service.Deserialize (this, reader);
				}
                return service;
            } catch (Exception e) {
                Log.Exception ("There was a problem deserializing a service.", e);
                return null;
            }
        }

        protected virtual ServiceDescription CreateService (IDisposer disposer)
        {
            return new ServiceDescription (disposer);
        }

        public Uri DeserializeUrl (XmlReader reader)
        {
            return DeserializeUrlCore (reader);
        }

        protected virtual Uri DeserializeUrlCore (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");
			if (UrlBase == null)
				throw new InvalidOperationException ("A description must be deserialized before a URL.");

            try {
                var url = reader.ReadString ();
                return Uri.IsWellFormedUriString (url, UriKind.Absolute)
                    ? new Uri (url) : new Uri (UrlBase, url);
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deserializing a URL.", e);
            }
        }
	}
}
