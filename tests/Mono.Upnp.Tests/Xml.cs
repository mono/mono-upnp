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
           @"<?xml version=""1.0""?>" +
           @"<root configId="""" xmlns=""urn:schemas-upnp-org:device-1-0"">" +
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
                    "<modelURL>http://www.mono-project.org/Mono.Upnp</modelURL>" +
                    "<serialNumber>12345</serialNumber>" +
                    "<UDN>uuid:fd1</UDN>" +
                    "<UPC>67890</UPC>" +
                    "<iconList>" +
                        "<icon>" +
                            "<mimetype>image/png</mimetype>" +
                            "<width>100</width>" +
                            "<height>100</height>" +
                            "<depth>32</depth>" +
                            "<url>/icon/0/</url>" +
                        "</icon>" +
                        "<icon>" +
                            "<mimetype>image/jpeg</mimetype>" +
                            "<width>100</width>" +
                            "<height>100</height>" +
                            "<depth>32</depth>" +
                            "<url>/icon/1/</url>" +
                        "</icon>" +
                    "</iconList>" +
                    "<serviceList>" +
                        "<service>" +
                            "<serviceType>urn:schemas-upnp-org:service:mono-upnp-test-service:1</serviceType>" +
                            "<serviceId>urn:upnp-org:serviceId:testService1</serviceId>" +
                            "<SCPDURL>/service/0/scpd/</SCPDURL>" +
                            "<controlURL>/service/0/control/</controlURL>" +
                            "<eventSubURL>/service/0/event/</eventSubURL>" +
                        "</service>" +
                        "<service>" +
                            "<serviceType>urn:schemas-upnp-org:service:mono-upnp-test-service:2</serviceType>" +
                            "<serviceId>urn:upnp-org:serviceId:testService2</serviceId>" +
                            "<SCPDURL>/service/1/scpd/</SCPDURL>" +
                            "<controlURL>/service/1/control/</controlURL>" +
                            "<eventSubURL>/service/1/event/</eventSubURL>" +
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
                            "<modelURL>http://www.mono-project.org/Mono.Upnp</modelURL>" +
                            "<serialNumber>12345</serialNumber>" +
                            "<UDN>uuid:fed1</UDN>" +
                            "<UPC>67890</UPC>" +
                            "<iconList>" +
                                "<icon>" +
                                    "<mimetype>image/png</mimetype>" +
                                    "<width>100</width>" +
                                    "<height>100</height>" +
                                    "<depth>32</depth>" +
                                    "<url>/device/0/icon/0/</url>" +
                                "</icon>" +
                                "<icon>" +
                                    "<mimetype>image/jpeg</mimetype>" +
                                    "<width>100</width>" +
                                    "<height>100</height>" +
                                    "<depth>32</depth>" +
                                    "<url>/device/0/icon/1/</url>" +
                                "</icon>" +
                            "</iconList>" +
                            "<serviceList>" +
                                "<service>" +
                                    "<serviceType>urn:schemas-upnp-org:service:mono-upnp-test-service:1</serviceType>" +
                                    "<serviceId>urn:upnp-org:serviceId:testService1</serviceId>" +
                                    "<SCPDURL>/device/0/service/0/scpd/</SCPDURL>" +
                                    "<controlURL>/device/0/service/0/control/</controlURL>" +
                                    "<eventSubURL>/device/0/service/0/event/</eventSubURL>" +
                                "</service>" +
                                "<service>" +
                                    "<serviceType>urn:schemas-upnp-org:service:mono-upnp-test-service:2</serviceType>" +
                                    "<serviceId>urn:upnp-org:serviceId:testService2</serviceId>" +
                                    "<SCPDURL>/device/0/service/1/scpd/</SCPDURL>" +
                                    "<controlURL>/device/0/service/1/control/</controlURL>" +
                                    "<eventSubURL>/device/0/service/1/event/</eventSubURL>" +
                                "</service>" +
                            "</serviceList>" +
                        "</device>" +
                    "</deviceList>" +
                "</device>" +
            "</root>";
        
        public const string UrlBaseDeviceDescription =
           @"<?xml version=""1.0""?>" +
           @"<root configId="""" xmlns=""urn:schemas-upnp-org:device-1-0"">" +
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
                    "<modelURL>http://www.mono-project.org/Mono.Upnp</modelURL>" +
                    "<serialNumber>12345</serialNumber>" +
                    "<UDN>uuid:fd1</UDN>" +
                    "<UPC>67890</UPC>" +
                    "<iconList>" +
                        "<icon>" +
                            "<mimetype>image/png</mimetype>" +
                            "<width>100</width>" +
                            "<height>100</height>" +
                            "<depth>32</depth>" +
                            "<url>/icon/0/</url>" +
                        "</icon>" +
                    "</iconList>" +
                    "<serviceList>" +
                        "<service>" +
                            "<serviceType>urn:schemas-upnp-org:service:mono-upnp-test-service:1</serviceType>" +
                            "<serviceId>urn:upnp-org:serviceId:testService1</serviceId>" +
                            "<SCPDURL>/service/0/scpd/</SCPDURL>" +
                            "<controlURL>/service/0/control/</controlURL>" +
                            "<eventSubURL>/service/0/event/</eventSubURL>" +
                        "</service>" +
                    "</serviceList>" +
                "</device>" +
                "<URLBase>http://www.mono-project.com</URLBase>" +
            "</root>";
        
        public const string FullScpd =
           @"<?xml version=""1.0""?>" +
           @"<scpd configId="""" xmlns=""urn:schemas-upnp-org:service-1-0"">" +
                "<specVersion>" +
                    "<major>1</major>" +
                    "<minor>1</minor>" +
                "</specVersion>" +
                "<actionList>" +
                    "<action>" +
                        "<name>Browse</name>" +
                        "<argumentList>" +
                            "<argument>" +
                                "<name>browseFlag</name>" +
                                "<direction>in</direction>" +
                                "<relatedStateVariable>A_ARG_TYPE_BrowseFlag</relatedStateVariable>" +
                            "</argument>" +
                            "<argument>" +
                                "<name>offset</name>" +
                                "<direction>in</direction>" +
                                "<relatedStateVariable>A_ARG_TYPE_Offset</relatedStateVariable>" +
                            "</argument>" +
                            "<argument>" +
                                "<name>requestCount</name>" +
                                "<direction>in</direction>" +
                                "<relatedStateVariable>A_ARG_TYPE_RequestCount</relatedStateVariable>" +
                            "</argument>" +
                            "<argument>" +
                                "<name>resultCount</name>" +
                                "<direction>out</direction>" +
                                "<relatedStateVariable>A_ARG_TYPE_ResultCount</relatedStateVariable>" +
                            "</argument>" +
                            "<argument>" +
                                "<name>result</name>" +
                                "<direction>out</direction>" +
                                "<retval />" +
                                "<relatedStateVariable>A_ARG_TYPE_Result</relatedStateVariable>" +
                            "</argument>" +
                        "</argumentList>" +
                    "</action>" +
                    "<action>" +
                        "<name>Search</name>" +
                        "<argumentList>" +
                            "<argument>" +
                                "<name>searchFlag</name>" +
                                "<direction>in</direction>" +
                                "<relatedStateVariable>A_ARG_TYPE_SearchFlag</relatedStateVariable>" +
                            "</argument>" +
                            "<argument>" +
                                "<name>offset</name>" +
                                "<direction>in</direction>" +
                                "<relatedStateVariable>A_ARG_TYPE_Offset</relatedStateVariable>" +
                            "</argument>" +
                            "<argument>" +
                                "<name>requestCount</name>" +
                                "<direction>in</direction>" +
                                "<relatedStateVariable>A_ARG_TYPE_RequestCount</relatedStateVariable>" +
                            "</argument>" +
                            "<argument>" +
                                "<name>resultCount</name>" +
                                "<direction>out</direction>" +
                                "<relatedStateVariable>A_ARG_TYPE_ResultCount</relatedStateVariable>" +
                            "</argument>" +
                            "<argument>" +
                                "<name>result</name>" +
                                "<direction>out</direction>" +
                                "<retval />" +
                                "<relatedStateVariable>A_ARG_TYPE_Result</relatedStateVariable>" +
                            "</argument>" +
                        "</argumentList>" +
                    "</action>" +
                "</actionList>" +
                "<serviceStateTable>" +
                    "<stateVariable>" +
                        "<name>A_ARG_TYPE_BrowseFlag</name>" +
                        "<dataType>string</dataType>" +
                        "<allowedValueList>" +
                            "<allowedValue>BrowseMetadata</allowedValue>" +
                            "<allowedValue>BrowseObjects</allowedValue>" +
                        "</allowedValueList>" +
                    "</stateVariable>" +
                    "<stateVariable>" +
                        "<name>A_ARG_TYPE_SearchFlag</name>" +
                        "<dataType>string</dataType>" +
                        "<allowedValueList>" +
                            "<allowedValue>SearchMetadata</allowedValue>" +
                            "<allowedValue>SearchObjects</allowedValue>" +
                        "</allowedValueList>" +
                    "</stateVariable>" +
                    "<stateVariable>" +
                        "<name>A_ARG_TYPE_Offset</name>" +
                        "<dataType>ui4</dataType>" +
                    "</stateVariable>" +
                    "<stateVariable>" +
                        "<name>A_ARG_TYPE_RequestCount</name>" +
                        "<dataType>ui4</dataType>" +
                        "<defaultValue>50</defaultValue>" +
                        "<allowedValueRange>" +
                            "<minimum>1</minimum>" +
                            "<maximum>100</maximum>" +
                        "</allowedValueRange>" +
                    "</stateVariable>" +
                    "<stateVariable>" +
                        "<name>A_ARG_TYPE_ResultCount</name>" +
                        "<dataType>ui4</dataType>" +
                    "</stateVariable>" +
                    "<stateVariable>" +
                        "<name>A_ARG_TYPE_Result</name>" +
                        "<dataType>string</dataType>" +
                    "</stateVariable>" +
                   @"<stateVariable sendEvents=""yes"">" +
                        "<name>SystemId</name>" +
                        "<dataType>ui4</dataType>" +
                    "</stateVariable>" +
                "</serviceStateTable>" +
            "</scpd>";
        
        public const string SimpleSoapRequest =
           @"<?xml version=""1.0""?>" +
           @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">" +
                "<s:Body>" +
                   @"<u:Foo xmlns:u=""urn:schemas-upnp-org:service:mono-upnp-test-service:1"">" +
                        "<bar>hello world!</bar>" +
                    "</u:Foo>" +
                "</s:Body>" +
            "</s:Envelope>";
        
        public const string SingleEventReport =
           @"<?xml version=""1.0""?>" +
           @"<e:propertyset xmlns:e=""urn:schemas-upnp-org:event-1-0"">" +
                "<e:property>" +
                    "<Foo>foo</Foo>" +
                "</e:property>" +
            "</e:propertyset>";
        
        public const string DoubleEventReport =
           @"<?xml version=""1.0""?>" +
           @"<e:propertyset xmlns:e=""urn:schemas-upnp-org:event-1-0"">" +
                "<e:property>" +
                    "<Foo>foo</Foo>" +
                "</e:property>" +
                "<e:property>" +
                    "<Bar>bar</Bar>" +
                "</e:property>" +
            "</e:propertyset>";
    }
}
