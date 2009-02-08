
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.XPath;

namespace Mono.Upnp.DidlLite
{
	public class Deserializer
	{
		static readonly Dictionary<string, Type> types = new Dictionary<string, Type> ();
		
		static Deserializer ()
		{
			foreach (var type in Assembly.GetExecutingAssembly ().GetTypes ()) {
				if (type.IsSubclassOf (typeof (Object))) {
					types[GetClassName (type)] = type;
				}
			}
		}
		
		static string GetClassName (Type type)
		{
			foreach (var attribute in type.GetCustomAttributes (false)) {
				var class_name = attribute as ClassNameAttribute;
				if (class_name != null) {
					return class_name.FullyQualifiedClassName;
				}
			}
			
			var builder = new StringBuilder ();
			BuildClassName (type, builder);
			return builder.ToString ();
		}
		
		static void BuildClassName (Type type, StringBuilder builder)
		{
			if (type != typeof (Object)) {
				BuildClassName (type.BaseType, builder);
				builder.Append ('.');
			}
			
			builder.Append (char.ToLower (type.Name [0]));
			for (var i = 1; i < type.Name.Length; i++) {
				builder.Append (type.Name[i]);
			}
		}
		
		public IEnumerable<Object> Deserialize (IXPathNavigable navigable)
		{
			var navigator = navigable.CreateNavigator ();
			if (!navigator.MoveToNext ("DIDL-Lite", Protocol.DidlLiteSchema) || !navigator.MoveToFirstChild ()) {
				yield break;
			}
			
			do {
				yield return DerserializeObject (navigator);
			} while (navigator.MoveToNext ());
		}
		
		Object DerserializeObject (XPathNavigator navigator)
		{
			navigator.MoveToChild ("class", Protocol.UpnpSchema);
			var @class = navigator.Value;
			navigator.MoveToParent ();
			while (!types.ContainsKey (@class)) {
				var dot = @class.LastIndexOf ('.');
				@class = @class.Substring (0, dot);
			}
			Object @object = (Object)Activator.CreateInstance (types[@class], true);
			@object.Deserialize (navigator.ReadSubtree ());
			return @object;
		}
	}
}
