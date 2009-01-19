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
using System.IO;
using System.Net;
using System.Xml;

using Mono.Upnp.Control;
using Mono.Upnp.Internal;

namespace Mono.Upnp.Description
{
	public class Deserializer : IDeserializer
	{
        Uri url_base;
        Version spec_version;
        DeviceDescription root_device;
        Disposer disposer;

        protected Uri UrlBase {
            get { return url_base; }
            set {
                if (value == null) throw new ArgumentNullException ("value");
                url_base = value;
            }
        }

        protected Version SpecVersion {
            get { return spec_version; }
            set { spec_version = value; }
        }

        protected DeviceDescription RootDevice {
            get { return root_device; }
            set {
                if (root_device != null) {
                    throw new UpnpDeserializationException ("There are multiple root devices.");
                }

                root_device = value;
                disposer.SetRootDevice (value);
            }
        }

        public DeviceDescription DeserializeDescription (Uri url)
        {
            if (url == null) throw new ArgumentNullException ("url");

            disposer = new Disposer (url);
            url_base = url;
            return DeserializeDescriptionCore (url);
        }

        protected virtual DeviceDescription DeserializeDescriptionCore (Uri url)
        {
            if (url == null) throw new ArgumentNullException ("url");

            // TODO handle ACCEPT-LANGUAGE
            using (WebResponse response = Helper.GetResponse (url)) {
                using (Stream stream = response.GetResponseStream ()) {
                    return DeserializeDescriptionCore (XmlReader.Create (stream));
                }
            }
        }

        protected virtual DeviceDescription DeserializeDescriptionCore (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                reader.ReadToFollowing ("root");
                while (Helper.ReadToNextElement (reader)) {
                    try {
                        DeserializeDescriptionCore (reader.ReadSubtree (), reader.Name);
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing one of the root description elements.", e);
                    }
                }
                reader.Close ();
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deserializing a root XML file.", e);
            }

            return root_device;
        }

        protected virtual void DeserializeDescriptionCore (XmlReader reader, string element)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            reader.Read ();
            switch (element.ToLower ()) {
            case "specversion":
                SpecVersion = Helper.DeserializeSpecVersion (reader.ReadSubtree ());
                break;
            case "urlbase":
                UrlBase = new Uri (reader.ReadString ());
                break;
            case "device":
                RootDevice = DeserializeDevice (reader.ReadSubtree ());
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        public ServiceController DeserializeController (ServiceDescription service)
        {
            return DeserializeControllerCore (service);
        }

        protected virtual ServiceController DeserializeControllerCore (ServiceDescription service)
        {
            if (service == null) throw new ArgumentNullException ("service");
            if (service.ScpdUrl == null) throw new ArgumentException ("The services does not have an SCPDURL.", "service");

            using (WebResponse response = Helper.GetResponse (service.ScpdUrl)) {
                using (Stream stream = response.GetResponseStream ()) {
                    return DeserializeControllerCore (service, XmlReader.Create (stream));
                }
            }
        }

        protected virtual ServiceController DeserializeControllerCore (ServiceDescription service, XmlReader reader)
        {
            ServiceController controller = CreateController (service);
            controller.Deserialize (reader);
            return controller;
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
                DeviceDescription device = CreateDevice (disposer);
                device.Deserialize (this, reader);
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
                ServiceDescription service = CreateService (disposer);
                service.Deserialize (this, reader);
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

            try {
                reader.Read ();
                string url = reader.ReadString ();
                Uri uri = Uri.IsWellFormedUriString (url, UriKind.Absolute) ? new Uri (url) : new Uri (url_base, url);
                reader.Close ();
                return uri;
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deserializing a URL.", e);
            }
        }
	}
}
