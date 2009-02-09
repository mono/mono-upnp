// 
// Ratings.cs
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

namespace Mono.Upnp.DidlLite.Av
{
	public enum MpaaRating
	{
		None = 0,
		G = 1,
		Pg = 2,
		Pg13 = 3,
		R = 4,
		Nc17 = 5,
		Nr = 6
	}
	
	public enum RiaaRating
	{
		None = 0,
		PaEc = 8,
	}
	
	public enum EsrbRating
	{
		None = 0,
		Ec = 16,
		E = 32,
		E10 = 48,
		T = 64,
		M = 80,
		Ao = 96,
		Rp = 112,
	}
	
	public enum TvGuidelinesRating
	{
		None = 0,
		TvY = 128,
		TvY7 = 256,
		TvY7fv = 384,
		TvG = 512,
		TvPg = 640,
		Tv14 = 768,
		TvMa = 896
	}
	
	public struct Ratings : IEquatable<Ratings>
	{
		const int MpaaMask = 7;
		const int RiaaMask = 8;
		const int EsrbMask = 112;
		const int TvGuidelinesMask = 896;
		
		readonly int ratings;
		
		public Ratings (MpaaRating mpaaRating,
		                RiaaRating riaaRating,
		                EsrbRating esrbRating,
		                TvGuidelinesRating tvGuilelinesRating)
		{
			ratings = (int)mpaaRating | (int)riaaRating | (int)esrbRating | (int)tvGuilelinesRating;
		}
		
		public MpaaRating MpaaRating {
			get { return (MpaaRating)(ratings & MpaaMask); }
		}
		
		public RiaaRating RiaaRating {
			get { return (RiaaRating)(ratings & RiaaMask); }
		}
		
		public EsrbRating EsrbRating {
			get { return (EsrbRating)(ratings & EsrbMask); }
		}
		
		public TvGuidelinesRating TvGuidelinesRating {
			get { return (TvGuidelinesRating)(ratings & TvGuidelinesMask); }
		}
		
		public override int GetHashCode ()
		{
			return ratings.GetHashCode ();
		}
		
		public override bool Equals (object obj)
		{
			return (obj is Ratings) && Equals ((Ratings)obj);
		}
		
		public bool Equals (Ratings ratings)
		{
			return this == ratings;
		}
		
		public static bool operator == (Ratings ratings1, Ratings ratings2)
		{
			return ratings1.ratings == ratings2.ratings;
		}
		
		public static bool operator != (Ratings ratings1, Ratings ratings2)
		{
			return ratings1.ratings != ratings2.ratings;
		}
	}
}
