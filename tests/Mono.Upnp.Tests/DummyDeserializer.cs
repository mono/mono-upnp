// 
// DummyDeserializer.cs
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
using System.IO;
using System.Xml;

using Mono.Upnp.Control;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Tests
{
    public class DummyDeserializer : Deserializer
    {
        static readonly XmlDeserializer deserializer = new XmlDeserializer ();
        
        public DummyDeserializer ()
            : base (deserializer)
        {
        }
        
        public Root DeserializeRoot (string xml)
        {
            using (var string_reader = new StringReader (xml)) {
                using (var xml_reader = XmlReader.Create (string_reader)) {
                    return DeserializeRoot (xml_reader);
                }
            }
        }
        
        public Root DeserializeRoot (XmlReader reader)
        {
            return DeserializeRoot (reader, new Uri ("http://localhost:8080"));
        }
        
        public Root DeserializeRoot (XmlReader reader, Uri url)
        {
            reader.ReadToFollowing ("root");
            return XmlDeserializer.Deserialize (reader, context => DeserializeRoot (url, context));
        }
        
        public ServiceController DeserializeServiceController (string xml)
        {
            using (var string_reader = new StringReader (xml)) {
                using (var xml_reader = XmlReader.Create (string_reader)) {
                    return DeserializeServiceController (xml_reader);
                }
            }
        }
        
        public ServiceController DeserializeServiceController (XmlReader reader)
        {
            reader.ReadToFollowing ("scpd");
            return XmlDeserializer.Deserialize (reader, context => DeserializeServiceController (
                new DummyService (new ServiceType ("schemas-upnp-org", "mono-upnp-test-service", new Version (1, 0)), "testService1"), context));
        }
    }
}
