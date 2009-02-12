// 
// StorageSystem.cs
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
using System.Xml;

namespace Mono.Upnp.ContentDirectory.Metadata.Av
{
	public class StorageSystem : Container
	{
		protected StorageSystem ()
		{
		}
		
		public long StorageTotal { get; private set; }
		public long StorageUsed { get; private set; }
		public long StorageFree { get; private set; }
		public long StorageMaxPartition { get; private set; }
		public string StorageMedium { get; private set; }
		
		public Browser<StorageSystem> BrowseStorageSystems ()
		{
		}
		
		public Browser<StorageVolume> BrowseStorageVolumes ()
		{
		}
		
		public Browser<StorageFolder> BrowseStorageFolders ()
		{
		}
		
		protected override void DeserializePropertyElement (XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException ("reader");
			
			if (reader.NamespaceURI == Schemas.UpnpSchema) {
				switch (reader.Name) {
				case "storageTotal":
					StorageTotal = reader.ReadElementContentAsLong ();
					break;
				case "storageUsed":
					StorageUsed = reader.ReadElementContentAsLong ();
					break;
				case "storageFree":
					StorageFree = reader.ReadElementContentAsLong ();
					break;
				case "storageMaxPartition":
					StorageMaxPartition = reader.ReadElementContentAsLong ();
					break;
				case "storageMedium":
					StorageMedium = reader.ReadString ();
					break;
				default:
					base.DeserializePropertyElement (reader);
					break;
				}
			} else {
				base.DeserializePropertyElement (reader);
			}
		}
		
//		protected override void VerifyDeserialization ()
//		{
//			if (StorageTotal == null)
//				throw new DeserializationException ("The storage system does not have a total value.");
//			if (StorageUsed == null)
//				throw new DeserializationException ("The storage system does not have a used value.");
//			if (StorageFree == null)
//				throw new DeserializationException ("The storage system does not have a free value.");
//			if (StorageMaxPartition == null)
//				throw new DeserializationException ("The storage system does not have a max partition value.");
//			if (StorageMedium == null)
//				throw new DeserializationException ("The storage system does not have a medium value.");
//			if (Volumes.Count > 0 && WriteStatus.HasValue) {
//				var write_status = Volumes[0].WriteStatus;
//				for (var i = 1; i < Volumes.Count; i++) {
//					if (Volumes[0].WriteStatus != write_status && WriteStatus.Value != Mono.Upnp.DidlLite.WriteStatus.Mixed) {
//						throw new DeserializationException ("The storage system must have a mixed write status if its volumes' write status are heterogenius.");
//					}
//				}
//				if (WriteStatus != write_status) {
//					throw new DeserializationException ("The storage system has a different write status than its volumes.");
//				}
//			}
//		}
	}
}
