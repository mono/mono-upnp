// 
// Resolution.cs
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

namespace Mono.Upnp.DidlLite
{
	public struct Resolution : IEquatable<Resolution>
	{
		readonly uint x;
		readonly uint y;
		
		Resolution (uint x, uint y)
		{
			this.x = x;
			this.y = y;
		}
		
		public uint X { get { return x; } }
		public uint Y { get { return y; } }
		
		public override string ToString ()
		{
			return string.Format("{0}x{1}", x, y);
		}
		
		public override int GetHashCode ()
		{
			return x.GetHashCode () ^ y.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			return (obj is Resolution) && Equals ((Resolution)obj);
		}
		
		public bool Equals (Resolution resolution)
		{
			return this == resolution;
		}

		public static bool operator == (Resolution resolution1, Resolution resolution2)
		{
			return resolution1.x == resolution2.x && resolution1.y == resolution2.y;
		}
		
		public static bool operator != (Resolution resolution1, Resolution resolution2)
		{
			return resolution1.x != resolution2.x || resolution1.y != resolution2.y;
		}
		
		internal static bool TryParse (string s, out Resolution result)
		{
			var index = s.IndexOf ('x');
			if (index > -1) {
				uint x, y;
				if (uint.TryParse (s.Substring (0, index), out x) && uint.TryParse (s.Substring (index + 1), out y)) {
					result = new Resolution (x, y);
					return true;
				}
			}
			result = new Resolution (0, 0);
			return false;
		}
	}
}
