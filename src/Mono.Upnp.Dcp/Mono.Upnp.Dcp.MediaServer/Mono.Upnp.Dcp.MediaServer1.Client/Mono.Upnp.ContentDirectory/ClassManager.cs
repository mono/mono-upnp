// 
// ClassManager.cs
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
using System.Text;

using Mono.Upnp.ContentDirectory.Av;

namespace Mono.Upnp.ContentDirectory
{
	public static class ClassManager
	{
		readonly static Dictionary<string, Type> types;
		
		static ClassManager ()
		{
			types = new Dictionary<string, Type> ();
			RegisterType (typeof (Object));
			RegisterType (typeof (Item));
			RegisterType (typeof (Container));
			RegisterType (typeof (Album));
			RegisterType (typeof (AudioBook));
			RegisterType (typeof (AudioBroadcast));
			RegisterType (typeof (AudioItem));
			RegisterType (typeof (Genre));
			RegisterType (typeof (ImageItem));
			RegisterType (typeof (Movie));
			RegisterType (typeof (MovieGenre));
			RegisterType (typeof (MusicAlbum));
			RegisterType (typeof (MusicArtist));
			RegisterType (typeof (MusicGenre));
			RegisterType (typeof (MusicTrack));
			RegisterType (typeof (MusicVideoClip));
			RegisterType (typeof (Person));
			RegisterType (typeof (Photo));
			RegisterType (typeof (PhotoAlbum));
			RegisterType (typeof (PlaylistContainer));
			RegisterType (typeof (PlaylistItem));
			RegisterType (typeof (StorageFolder));
			RegisterType (typeof (StorageSystem));
			RegisterType (typeof (StorageVolume));
			RegisterType (typeof (TextItem));
			RegisterType (typeof (VideoBroadcast));
			RegisterType (typeof (VideoItem));
		}
		
		public static void RegisterType (Type type)
		{
			if (type != typeof (Object) && !type.IsSubclassOf (typeof (Object))) {
				throw new ArgumentException (
					"The type is not a subclass of Mono.Upnp.ContentDirectory.Metadata.Object");
			}
			
			var builder = new StringBuilder ();
			BuildClassName (type, builder);
			types[builder.ToString ()] = type;
		}
		
		static string GetClassName (Type type)
		{
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
			
			foreach (var attribute in type.GetCustomAttributes (false)) {
				var class_name = attribute as ClassNameAttribute;
				if (class_name != null) {
					builder.Append (class_name.ClassName);
					return;
				}
			}
			
			builder.Append (char.ToLower (type.Name [0]));
			for (var i = 1; i < type.Name.Length; i++) {
				builder.Append (type.Name[i]);
			}
		}
		
		public static Type GetTypeFromClass (string @class)
		{
			while (!types.ContainsKey (@class)) {
				var dot = @class.LastIndexOf ('.');
				if (dot == -1) {
					return null;
				}
				@class = @class.Substring (0, dot);
			}
			return types[@class];
		}
	}
}
