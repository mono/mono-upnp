// 
// DeviceSettings.cs
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

namespace Mono.Upnp
{
    public class DeviceSettings
    {
        readonly DeviceType type;
        readonly string udn;
        readonly string friendly_name;
        readonly string manufacturer;
        readonly string model_name;
        readonly List<Service> services = new List<Service> ();
        readonly List<Icon> icons = new List<Icon> ();
        
        protected DeviceSettings ()
        {
        }
        
        public DeviceSettings (DeviceType type, string udn, string friendlyName, string manufacturer, string modelName)
        {
            if (type == null) throw new ArgumentNullException ("type");
            if (udn == null) throw new ArgumentNullException ("udn");
            if (!udn.StartsWith ("uuid:")) throw new ArgumentException (@"The udn must begin with ""uuid:"".", "udn");
            if (friendlyName == null) throw new ArgumentNullException ("friendlyName");
            if (manufacturer == null) throw new ArgumentException ("manufacturer");
            if (modelName == null) throw new ArgumentNullException ("modelName");
            
            this.type = type;
            this.udn = udn;
            this.friendly_name = friendlyName;
            this.manufacturer = manufacturer;
            this.model_name = modelName;
        }
        
        public DeviceType Type {
            get { return type; }
        }
        
        public string Udn {
            get { return udn; }
        }
        
        public string FriendlyName {
            get { return friendly_name; }
        }
        
        public string Manufacturer {
            get { return manufacturer; }
        }
        
        public string ModelName {
            get { return model_name; }
        }
        
        public IList<Service> Services {
            get { return services; }
        }
        
        public IList<Icon> Icons {
            get { return icons; }
        }
        
        public Uri ManufacturerUrl { get; set; }
        
        public string ModelDescription { get; set; }
        
        public string ModelNumber { get; set; }
        
        public Uri ModelUrl { get; set; }
        
        public string SerialNumber { get; set; }
        
        public string Upc { get; set; }
    }
}
