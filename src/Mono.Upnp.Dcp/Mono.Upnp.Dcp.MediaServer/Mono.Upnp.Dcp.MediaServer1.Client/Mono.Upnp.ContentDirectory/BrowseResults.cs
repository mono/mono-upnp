// 
// BrowseResults.cs
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

using System.Collections.Generic;

namespace Mono.Upnp.ContentDirectory
{
	sealed class BrowseResults : Results<Object>
	{
		public BrowseResults(ContentDirectory contentDirectory, string objectId, string sortCriteria,
		                     string filter, uint requestCount, uint offset)
			: base (contentDirectory, objectId, sortCriteria, filter, requestCount, offset)
		{
		}
		
		protected override string FetchXml (out uint returnedCount, out uint totalCount, out uint updateId)
		{
			return ContentDirectory.Controller.Browse (ObjectId, BrowseFlag.BrowseDirectChildren, Filter, Offset, 
				RequestCount, SortCriteria, out returnedCount, out totalCount, out updateId);
		}
		
		protected override Results<Object> CreateMoreResults ()
		{
			return new BrowseResults (ContentDirectory, ObjectId, SortCriteria,
				 Filter, RequestCount, Offset + ReturnedCount);
		}
	}
}
