// ContentDirectory1.cs auto-generated at 1/18/2009 9:31:12 PM by Sharpener

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;

using Mono.Upnp.Description;
using Mono.Upnp.Control;

namespace Mono.Upnp.ContentDirectory
{
    public class ContentDirectory
    {
        public static readonly ServiceType ServiceType = new ServiceType (
			"urn:schemas-upnp-org:service:ContentDirectory:1");
		
		readonly Dictionary<string, Dictionary<string, WeakReference>> object_cache =
			new Dictionary<string, WeakReference> ();
		readonly Dictionary<string, uint> container_update_ids = new Dictionary<string, uint> ();
		
        readonly ServiceController controller;
		
        public ContentDirectory (ServiceAnnouncement announcement)
        {
            if (announcement == null) throw new ArgumentNullException ("announcement");
			
            var description = announcement.GetDescription ();
			if (description == null) {
				throw new UpnpDeserializationException (string.Format (
					"Could not get a description for {0}.", announcement));
			}
            controller = description.GetController ();
            if (controller == null) {
				throw new UpnpDeserializationException (string.Format (
					"Could not get a controller for {0}.", description));
			}
			
            Verify ();
        }

        public string GetSearchCapabilities ()
        {
            ActionResult action_result = controller.Actions["GetSearchCapabilities"].Invoke ();
            return action_result.OutValues["SearchCaps"];
        }

        public string GetSortCapabilities ()
        {
            ActionResult action_result = controller.Actions["GetSortCapabilities"].Invoke ();
            return action_result.OutValues["SortCaps"];
        }

        public string GetSystemUpdateId ()
        {
            ActionResult action_result = controller.Actions["GetSystemUpdateID"].Invoke ();
            return action_result.OutValues["Id"];
        }

        internal string Browse (string objectId, BrowseFlag browseFlag, string filter, uint startingIndex, uint requestedCount, string sortCriteria, out uint numberReturned, out uint totalMatches, out uint updateId)
        {
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (6);
            in_arguments.Add ("ObjectID", objectId);
            in_arguments.Add ("BrowseFlag", browseFlag.ToString ());
            in_arguments.Add ("Filter", filter);
            in_arguments.Add ("StartingIndex", startingIndex.ToString ());
            in_arguments.Add ("RequestedCount", requestedCount.ToString ());
            in_arguments.Add ("SortCriteria", sortCriteria);
            ActionResult action_result = controller.Actions["Browse"].Invoke (in_arguments);
            numberReturned = uint.Parse (action_result.OutValues["NumberReturned"]);
            totalMatches = uint.Parse (action_result.OutValues["TotalMatches"]);
            updateId = uint.Parse (action_result.OutValues["UpdateID"]);
			return action_result.OutValues["Result"];
        }

        public bool CanSearch { get { return controller.Actions.ContainsKey ("Search"); } }
        internal string Search (string containerId, string searchCriteria, string filter, uint startingIndex, uint requestedCount, string sortCriteria, out uint numberReturned, out uint totalMatches, out uint updateId)
        {
            if (!CanSearch) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (6);
            in_arguments.Add ("ContainerID", containerId);
            in_arguments.Add ("SearchCriteria", searchCriteria);
            in_arguments.Add ("Filter", filter);
            in_arguments.Add ("StartingIndex", startingIndex.ToString ());
            in_arguments.Add ("RequestedCount", requestedCount.ToString ());
            in_arguments.Add ("SortCriteria", sortCriteria);
            ActionResult action_result = controller.Actions["Search"].Invoke (in_arguments);
            numberReturned = uint.Parse (action_result.OutValues["NumberReturned"]);
            totalMatches = uint.Parse (action_result.OutValues["TotalMatches"]);
            updateId = uint.Parse (action_result.OutValues["UpdateID"]);
			return action_result.OutValues["Result"];
        }

        public bool CanCreateObject { get { return controller.Actions.ContainsKey ("CreateObject"); } }
        public void CreateObject (string containerId, string elements, out string objectId, out string result)
        {
            if (!CanCreateObject) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (2);
            in_arguments.Add ("ContainerID", containerId);
            in_arguments.Add ("Elements", elements);
            ActionResult action_result = controller.Actions["CreateObject"].Invoke (in_arguments);
            objectId = action_result.OutValues["ObjectID"];
            result = action_result.OutValues["Result"];
        }

        public bool CanDestroyObject { get { return controller.Actions.ContainsKey ("DestroyObject"); } }
        public void DestroyObject (string objectId)
        {
            if (!CanDestroyObject) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("ObjectID", objectId);
            ActionResult action_result = controller.Actions["DestroyObject"].Invoke (in_arguments);
        }

        public bool CanUpdateObject { get { return controller.Actions.ContainsKey ("UpdateObject"); } }
        public void UpdateObject (string objectId, string currentTagValue, string newTagValue)
        {
            if (!CanUpdateObject) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (3);
            in_arguments.Add ("ObjectID", objectId);
            in_arguments.Add ("CurrentTagValue", currentTagValue);
            in_arguments.Add ("NewTagValue", newTagValue);
            ActionResult action_result = controller.Actions["UpdateObject"].Invoke (in_arguments);
        }

        public bool CanImportResource { get { return controller.Actions.ContainsKey ("ImportResource"); } }
        public string ImportResource (Uri sourceUri, Uri destinationUri)
        {
            if (!CanImportResource) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (2);
            in_arguments.Add ("SourceURI", sourceUri.ToString ());
            in_arguments.Add ("DestinationURI", destinationUri.ToString ());
            ActionResult action_result = controller.Actions["ImportResource"].Invoke (in_arguments);
            return action_result.OutValues["TransferID"];
        }

        public bool CanExportResource { get { return controller.Actions.ContainsKey ("ExportResource"); } }
        public string ExportResource (Uri sourceUri, Uri destinationUri)
        {
            if (!CanExportResource) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (2);
            in_arguments.Add ("SourceURI", sourceUri.ToString ());
            in_arguments.Add ("DestinationURI", destinationUri.ToString ());
            ActionResult action_result = controller.Actions["ExportResource"].Invoke (in_arguments);
            return action_result.OutValues["TransferID"];
        }

        public bool CanStopTransferResource { get { return controller.Actions.ContainsKey ("StopTransferResource"); } }
        public void StopTransferResource (uint transferId)
        {
            if (!CanStopTransferResource) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("TransferID", transferId.ToString ());
            ActionResult action_result = controller.Actions["StopTransferResource"].Invoke (in_arguments);
        }

        public bool CanGetTransferProgress { get { return controller.Actions.ContainsKey ("GetTransferProgress"); } }
        public void GetTransferProgress (uint transferId, out string transferStatus, out string transferLength, out string transferTotal)
        {
            if (!CanGetTransferProgress) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("TransferID", transferId.ToString ());
            ActionResult action_result = controller.Actions["GetTransferProgress"].Invoke (in_arguments);
            transferStatus = action_result.OutValues["TransferStatus"];
            transferLength = action_result.OutValues["TransferLength"];
            transferTotal = action_result.OutValues["TransferTotal"];
        }

        public bool CanDeleteResource { get { return controller.Actions.ContainsKey ("DeleteResource"); } }
        public void DeleteResource (Uri resourceUri)
        {
            if (!CanDeleteResource) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (1);
            in_arguments.Add ("ResourceURI", resourceUri.ToString ());
            ActionResult action_result = controller.Actions["DeleteResource"].Invoke (in_arguments);
        }

        public bool CanCreateReference { get { return controller.Actions.ContainsKey ("CreateReference"); } }
        public string CreateReference (string containerId, string objectId)
        {
            if (!CanCreateReference) throw new NotImplementedException ();
            Dictionary<string, string> in_arguments = new Dictionary<string, string> (2);
            in_arguments.Add ("ContainerID", containerId);
            in_arguments.Add ("ObjectID", objectId);
            ActionResult action_result = controller.Actions["CreateReference"].Invoke (in_arguments);
            return action_result.OutValues["NewID"];
        }

        public bool HasTransferIds { get { return controller.StateVariables.ContainsKey ("TransferIDs"); } }
        public event EventHandler<StateVariableChangedArgs<string>> TransferIdsChanged {
            add {
                if (!HasTransferIds) return;
                controller.StateVariables["TransferIDs"].Changed += value;
            }
            remove {
                if (!HasTransferIds) return;
                controller.StateVariables["TransferIDs"].Changed -= value;
            }
        }

        public event EventHandler<StateVariableChangedArgs<string>> SystemUpdateIdChanged {
            add { controller.StateVariables["SystemUpdateID"].Changed += value; }
            remove { controller.StateVariables["SystemUpdateID"].Changed -= value; }
        }

        public bool HasContainerUpdateIds { get { return controller.StateVariables.ContainsKey ("ContainerUpdateIDs"); } }
        public event EventHandler<StateVariableChangedArgs<string>> ContainerUpdateIdsChanged {
            add {
                if (!HasContainerUpdateIds) return;
                controller.StateVariables["ContainerUpdateIDs"].Changed += value;
            }
            remove {
                if (!HasContainerUpdateIds) return;
                controller.StateVariables["ContainerUpdateIDs"].Changed -= value;
            }
        }

        void Verify ()
        {
            if (!controller.Actions.ContainsKey ("GetSearchCapabilities"))
				throw new UpnpDeserializationException (string.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required action GetSearchCapabilities.", controller.Description.Id));
            if (!controller.Actions.ContainsKey ("GetSortCapabilities"))
				throw new UpnpDeserializationException (string.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required action GetSortCapabilities.", controller.Description.Id));
            if (!controller.Actions.ContainsKey ("GetSystemUpdateID"))
				throw new UpnpDeserializationException (string.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required action GetSystemUpdateID.", controller.Description.Id));
            if (!controller.Actions.ContainsKey ("Browse"))
				throw new UpnpDeserializationException (string.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required action Browse.", controller.Description.Id));
            if (!controller.StateVariables.ContainsKey ("SystemUpdateID"))
				throw new UpnpDeserializationException (string.Format ("The service {0} claims to be of type urn:schemas-upnp-org:service:ContentDirectory:1 but it does not have the required state variable SystemUpdateID.", controller.Description.Id));
        }
		
		internal IEnumerable<T> Deserialize<T> (string filter, string xml) where T : Object
		{
			if (!object_cache.ContainsKey (filter)) {
				object_cache[filter] = new Dictionary<string, WeakReference> ();
			}
			
			using (var reader = new StringReader (xml)) {
				var navigator = new XPathDocument (reader).CreateNavigator ();
				if (navigator.MoveToNext ("DIDL-Lite", Protocol.DidlLiteSchema) && navigator.MoveToFirstChild ()) {
					do {
						yield return DerserializeObject (filter, navigator);
					} while (navigator.MoveToNext ());
				}
			}
		}
		
		T DerserializeObject<T> (string filter, XPathNavigator navigator) where T : Object
		{
			return GetObjectFromCache (filter, navigator) ?? CreateObject (filter, navigator);
		}
		
		T GetObjectFromCache<T> (string filter, XPathNavigator navigator) where T : Object
		{
			if (navigator.MoveToAttribute ("id", Protocol.DidlLiteSchema)) {
				var id = navigator.Value;
				if (object_cache.ContainsKey (filter) && object_cache[filter].ContainsKey (id)) {
					var weak_reference = object_cache[filter][id];
					if (weak_reference.IsAlive) {
						var @object = (Object)weak_reference.Target;
						if (!CheckIfObjectIsOutOfDate (@object)) {
							return @object;
						}
					}
				}
			}
			return null;
		}
		
		T CreateObject<T> (string filter, XPathNavigator navigator) where T : Object
		{
			navigator.MoveToChild ("class", Protocol.UpnpSchema);
			var @class = navigator.Value;
			navigator.MoveToParent ();
			
			while (!types.ContainsKey (@class)) {
				@class = @class.Substring (0, @class.LastIndexOf ('.'));
			}
			
			@object = (Object)Activator.CreateInstance (types[@class], true);
			@object.Deserialize (navigator.ReadSubtree ());
			if (container_update_ids.ContainsKey (@object.ParentId)) {
				@object.ParentUpdateId = container_update_ids[@object.ParentId];
			}
			
			if (!object_hierarchy.ContainsKey (@object.ParentId)) {
				object_hierarchy[@object.ParentId] = new Dictionary<string, string> ();
			}
			object_hierarchy[@object.ParentId][@object.Id] = null;
			object_cache[filter][@object.Id] = new WeakReference (@object);
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
    }
}