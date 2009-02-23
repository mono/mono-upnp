// 
// ResourceBuilder.cs
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
using System.Xml.Serialization;

namespace Mono.Upnp.ContentDirectory
{
	[XmlType ("res", Namespace = Schemas.DidlLiteSchema)]
	public sealed class ResourceBuilder
	{
		readonly Uri resource_uri;
		readonly string protocol_info;
		
		public ResourceBuilder (Uri resourceUri, string contentType)
		{
			if (resourceUri == null) throw new ArgumentNullException ("resourceUri");
			if (!resourceUri.IsAbsoluteUri) throw new ArgumentException (
				"The provided URI must be an absolute URI.", "resourceUri");
			
			if (resourceUri.IsFile && resourceUri.IsLoopback) {
				protocol_info = string.Format ("*:*:{0}:*", contentType ?? "*");
			} else {
				protocol_info = string.Format ("{0}:*:{1}:*", resourceUri.Scheme, contentType ?? "*");
			}
			
			resource_uri = resourceUri;
		}
	}
}
