// 
// ContentDirectory.cs
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
using System.IO;
using System.Xml.XPath;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
	public class ContentDirectory
	{
		public static readonly ServiceType ServiceType = new ServiceType (
			"urn:schemas-upnp-org:service:ContentDirectory:1");
		
		readonly Dictionary<string, Dictionary<string, WeakReference>> object_cache =
			new Dictionary<string, Dictionary<string, WeakReference>> ();
		readonly Dictionary<string, uint> container_update_ids = new Dictionary<string, uint> ();
		
		readonly ContentDirectoryController controller;
        readonly string search_capabilities;
        readonly string sort_capabilities;
		Container root_container;
		
		public ContentDirectory (ContentDirectoryController controller)
		{
			if (controller == null) throw new ArgumentNullException ("controller");
			
			this.controller = controller;
            search_capabilities = controller.GetSearchCapabilities ();
            sort_capabilities = controller.GetSortCapabilities ();
		}
		
		public ContentDirectoryController Controller { get { return controller; } }
		
		public Container GetRootContainer ()
		{
			if (root_container == null) {
				root_container = GetObject<Container> ("0");
			}
			return root_container;
		}
		
		internal IEnumerable<T> Deserialize<T> (string filter, string xml) where T : Object
		{
			if (!object_cache.ContainsKey (filter)) {
				object_cache[filter] = new Dictionary<string, WeakReference> ();
			}
			
			using (var reader = new StringReader (xml)) {
				var navigator = new XPathDocument (reader).CreateNavigator ();
				if (navigator.MoveToChild ("DIDL-Lite", Schemas.DidlLiteSchema) && navigator.MoveToFirstChild ()) {
					do {
						yield return DerserializeObject<T> (filter, navigator);
					} while (navigator.MoveToNext ());
				}
			}
		}
		
		T DerserializeObject<T> (string filter, XPathNavigator navigator) where T : Object
		{
			return GetObjectFromCache<T> (filter, navigator) ?? CreateObject<T> (filter, navigator);
		}
		
		T GetObjectFromCache<T> (string filter, XPathNavigator navigator) where T : Object
		{
			if (navigator.MoveToAttribute ("id", Schemas.DidlLiteSchema)) {
				var id = navigator.Value;
				WeakReference weak_reference;
				if (object_cache.ContainsKey (filter) && object_cache[filter].ContainsKey (id)) {
					weak_reference = object_cache[filter][id];
				} else if (filter != "*" && object_cache.ContainsKey ("*") && object_cache["*"].ContainsKey (id)) {
					weak_reference = object_cache["*"][id];
				} else {
					return null;
				}
				if (weak_reference.IsAlive) {
					var @object = (T)weak_reference.Target;
					if (!CheckIfObjectIsOutOfDate (@object)) {
						return @object;
					}
				}
			}
			return null;
		}
		
		T CreateObject<T> (string filter, XPathNavigator navigator) where T : Object
		{
			navigator.MoveToChild ("class", Schemas.UpnpSchema);
			var type = ClassManager.GetTypeFromClass (navigator.Value);
			navigator.MoveToParent ();
			
			if (type == null || (type != typeof (T) && !type.IsSubclassOf (typeof (T)))) {
				return null;
			}
			
			var @object = (T)Activator.CreateInstance (type, true);
            using (var reader = navigator.ReadSubtree ()) {
                reader.Read ();
                @object.Deserialize (this, reader);
            }
			if (container_update_ids.ContainsKey (@object.ParentId)) {
				@object.ParentUpdateId = container_update_ids[@object.ParentId];
			}
			
			object_cache[filter][@object.Id] = new WeakReference (@object);
			return @object;
		}
		
		internal bool CheckIfContainerIsOutOfDate (string id, uint updateId)
		{
			var result = false;
			if (container_update_ids.ContainsKey (id) && container_update_ids [id] != updateId) {
				result = true;
			}
			container_update_ids [id] = updateId;
			return result;
		}
		
		internal bool CheckIfObjectIsOutOfDate (Object @object)
		{
			return container_update_ids.ContainsKey (@object.ParentId) &&
				container_update_ids[@object.ParentId] != @object.ParentUpdateId;
		}
		
		internal T GetObject<T> (string id) where T : Object
		{
			uint returned, total, update_id;
			var xml = controller.Browse (id, BrowseFlag.BrowseMetadata, "*", 0, 1, "",
				out returned, out total, out update_id);
			
			CheckIfContainerIsOutOfDate (id, update_id);
			foreach (var result in Deserialize<T> ("*", xml)) {
				return result;
			}
			
			return null;
		}
		
		public static T GetUpdatedObject<T> (T @object) where T : Object
		{
			if (@object == null) throw new ArgumentNullException ("object");
			
			return @object.ContentDirectory.GetObject<T> (@object.Id);
		}
	}
}
