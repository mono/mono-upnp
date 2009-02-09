// 
// Deserializer.cs
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
using System.Reflection;
using System.Text;
using System.Xml.XPath;

namespace Mono.Upnp.DidlLite.Internal
{
	class Deserializer
	{
		static readonly Dictionary<string, Type> types = new Dictionary<string, Type> ();
		readonly Dictionary<string, WeakReference> objects = new Dictionary<string, WeakReference>();
		
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
			return GetObjectFromCache (navigator) ?? CreateObject (navigator);
		}
		
		Object GetObjectFromCache (XPathNavigator navigator)
		{
			if (navigator.MoveToAttribute ("id", Protocol.DidlLiteSchema)) {
				var id = navigator.Value;
				if (objects.ContainsKey (id)) {
					var weak_reference = objects[id];
					if (weak_reference.IsAlive) {
						return (Object)weak_reference.Target;
					}
				}
			}
			return null;
		}
		
		Object CreateObject (XPathNavigator navigator)
		{
			navigator.MoveToChild ("class", Protocol.UpnpSchema);
			var @class = navigator.Value;
			navigator.MoveToParent ();
			while (!types.ContainsKey (@class)) {
				@class = @class.Substring (0, @class.LastIndexOf ('.'));
			}
			var @object = (Object)Activator.CreateInstance (types[@class], true);
			@object.Deserialize (navigator.ReadSubtree ());
			objects[@object.Id] = new WeakReference (@object);
			return @object;
		}
	}
}
