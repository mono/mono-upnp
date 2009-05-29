// 
// Xml.cs
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

namespace Mono.Upnp.Tests
{
    public static class Xml
    {
        public const string FullDeviceDescription =
           @"<?xml version=""1.0"" encoding=""utf-16""?>" +
           @"<root configId=""1"" xmlns=""urn:schemas-upnp-org:device-1-0"">" +
                "<specVersion>" +
                    "<major>1</major>" +
                    "<minor>1</minor>" +
                "</specVersion>" +
                "<device>" +
                    "<deviceType>urn:schemas-upnp-org:device:mono-upnp-tests-full-device:1</deviceType>" +
                    "<friendlyName>Mono.Upnp.Tests Full Device</friendlyName>" +
                    "<manufacturer>Mono Project</manufacturer>" +
                    "<manufacturerURL>http://www.mono-project.org/</manufacturerURL>" +
                    "<modelDescription>A device description with all optional information.</modelDescription>" +
                    "<modelName>Full Device</modelName>" +
                    "<modelNumber>1</modelNumber>" +
                    "<modelURL>http://www.mono-project.org/Mono.Upnp/</modelURL>" +
                    "<serialNumber>12345</serialNumber>" +
                    "<UDN>uuid:fd1</UDN>" +
                    "<UPC>67890</UPC>" +
                    "<iconList>" +
                        "<icon>" +
                            "<mimetype>image/png</mimetype>" +
                            "<width>100</width>" +
                            "<height>100</height>" +
                            "<depth>32</depth>" +
                            "<url>icon/0/</url>" +
                        "</icon>" +
                        "<icon>" +
                            "<mimetype>image/jpeg</mimetype>" +
                            "<width>100</width>" +
                            "<height>100</height>" +
                            "<depth>32</depth>" +
                            "<url>icon/1/</url>" +
                        "</icon>" +
                    "</iconList>" +
                    "<serviceList>" +
                        "<service>" +
                            "<serviceType>urn:schemas-upnp-org:service:mono-upnp-test-service:1</serviceType>" +
                            "<serviceId>urn:upnp-org:serviceId:testService1</serviceId>" +
                            "<SCPDURL>service/0/scpd/</SCPDURL>" +
                            "<controlURL>service/0/control/</controlURL>" +
                            "<eventSubURL>service/0/event/</eventSubURL>" +
                        "</service>" +
                        "<service>" +
                            "<serviceType>urn:schemas-upnp-org:service:mono-upnp-test-service:2</serviceType>" +
                            "<serviceId>urn:upnp-org:serviceId:testService2</serviceId>" +
                            "<SCPDURL>service/1/scpd/</SCPDURL>" +
                            "<controlURL>service/1/control/</controlURL>" +
                            "<eventSubURL>service/1/event/</eventSubURL>" +
                        "</service>" +
                    "</serviceList>" +
                    "<deviceList>" +
                        "<device>" +
                            "<deviceType>urn:schemas-upnp-org:device:mono-upnp-tests-full-embedded-device:1</deviceType>" +
                            "<friendlyName>Mono.Upnp.Tests Full Embedded Device</friendlyName>" +
                            "<manufacturer>Mono Project</manufacturer>" +
                            "<manufacturerURL>http://www.mono-project.org/</manufacturerURL>" +
                            "<modelDescription>An embedded device description with all optional information.</modelDescription>" +
                            "<modelName>Full Embedded Device</modelName>" +
                            "<modelNumber>1</modelNumber>" +
                            "<modelURL>http://www.mono-project.org/Mono.Upnp/</modelURL>" +
                            "<serialNumber>12345</serialNumber>" +
                            "<UDN>uuid:fed1</UDN>" +
                            "<UPC>67890</UPC>" +
                            "<iconList>" +
                                "<icon>" +
                                    "<mimetype>image/png</mimetype>" +
                                    "<width>100</width>" +
                                    "<height>100</height>" +
                                    "<depth>32</depth>" +
                                    "<url>device/0/icon/0/</url>" +
                                "</icon>" +
                                "<icon>" +
                                    "<mimetype>image/jpeg</mimetype>" +
                                    "<width>100</width>" +
                                    "<height>100</height>" +
                                    "<depth>32</depth>" +
                                    "<url>device/0/icon/1/</url>" +
                                "</icon>" +
                            "</iconList>" +
                            "<serviceList>" +
                                "<service>" +
                                    "<serviceType>urn:schemas-upnp-org:service:mono-upnp-test-service:1</serviceType>" +
                                    "<serviceId>urn:upnp-org:serviceId:testService1</serviceId>" +
                                    "<SCPDURL>device/0/service/0/scpd/</SCPDURL>" +
                                    "<controlURL>device/0/service/0/control/</controlURL>" +
                                    "<eventSubURL>device/0/service/0/event/</eventSubURL>" +
                                "</service>" +
                                "<service>" +
                                    "<serviceType>urn:schemas-upnp-org:service:mono-upnp-test-service:2</serviceType>" +
                                    "<serviceId>urn:upnp-org:serviceId:testService2</serviceId>" +
                                    "<SCPDURL>device/0/service/1/scpd/</SCPDURL>" +
                                    "<controlURL>device/0/service/1/control/</controlURL>" +
                                    "<eventSubURL>device/0/service/1/event/</eventSubURL>" +
                                "</service>" +
                            "</serviceList>" +
                        "</device>" +
                    "</deviceList>" +
                "</device>" +
            "</root>";
        
        public const string FullScpd =
           @"<?xml version=""1.0"" encoding=""utf-16""?>" +
           @"<scpd configId=""1"" xmlns=""urn:schemas-upnp-org:service-1-0"">" +
    }
}
