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
using System.Reflection;
using System.Text;

using Mono.Upnp.ContentDirectory.Av;

namespace Mono.Upnp.ContentDirectory
{
	public static class ClassManager
	{
		readonly static Dictionary<string, Type> types;
		readonly static Dictionary<Type, string> names;
		
		static ClassManager ()
		{
			types = new Dictionary<string, Type> ();
			names = new Dictionary<Type, string>();
			RegisterType<Object> ();
			RegisterType<Item> ();
			RegisterType<Container> ();
			RegisterType<Album> ();
			RegisterType<AudioBook> ();
			RegisterType<AudioBroadcast> ();
			RegisterType<AudioItem> ();
			RegisterType<Genre> ();
			RegisterType<ImageItem> ();
			RegisterType<Movie> ();
			RegisterType<MovieGenre> ();
			RegisterType<MusicAlbum> ();
			RegisterType<MusicArtist> ();
			RegisterType<MusicGenre> ();
			RegisterType<MusicTrack> ();
			RegisterType<MusicVideoClip> ();
			RegisterType<Person> ();
			RegisterType<Photo> ();
			RegisterType<PhotoAlbum> ();
			RegisterType<PlaylistContainer> ();
			RegisterType<PlaylistItem> ();
			RegisterType<StorageFolder> ();
			RegisterType<StorageSystem> ();
			RegisterType<StorageVolume> ();
			RegisterType<TextItem> ();
			RegisterType<VideoBroadcast> ();
			RegisterType<VideoItem> ();
		}
		
		public static void RegisterType<T> () where T : Object
		{
			var type = typeof (T);
			var name = CreateClassName (type, typeof (Object));
			types[name] = type;
			names[type] = name;
		}
		
		public static string GetClassFromType<T> () where T : Object
		{
			string name;
			var type = typeof (T);
			if (names.TryGetValue (type, out name)) {
				return name;
			} else {
				throw new InvalidOperationException (string.Format (
					"The type {0} has not been registered for use with the ClassManager.", type));
			}
		}
		
		internal static string CreateClassName (Type type, Type rootType)
		{
			var builder = new StringBuilder ();
			BuildClassName (type, rootType, builder);
			return builder.ToString ();
		}
		
		static void BuildClassName (Type type, Type rootType, StringBuilder builder)
		{
			if (type != rootType) {
				BuildClassName (type.BaseType, rootType, builder);
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
