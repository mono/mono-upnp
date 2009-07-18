// 
// StorageVolume.cs
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

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Av
{
    public class StorageVolume : Container
    {
        public StorageVolume (ContentDirectory contentDirectory)
            : base (contentDirectory, null)
        {
        }
        
        public StorageVolume (ContentDirectory contentDirectory, StorageSystem parent)
            : base (contentDirectory, parent)
        {
        }
        
        public StorageVolume (ContentDirectory contentDirectory, StorageVolume parent)
            : base (contentDirectory, parent)
        {
        }
        
        [XmlElement ("storageTotal", Schemas.UpnpSchema)]
        public virtual long StorageTotal { get; protected set; }
        
        [XmlElement ("storageUsed", Schemas.UpnpSchema)]
        public virtual long StorageUsed { get; protected set; }
        
        [XmlElement ("storageFree", Schemas.UpnpSchema)]
        public virtual long StorageFree { get; protected set; }
        
        [XmlElement ("storageMedium", Schemas.UpnpSchema)]
        public virtual string StorageMedium { get; protected set; }
    
        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            context.AutoDeserializeElement (this);
        }

        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            context.AutoSerializeMembersOnly (this);
        }
    }
}
