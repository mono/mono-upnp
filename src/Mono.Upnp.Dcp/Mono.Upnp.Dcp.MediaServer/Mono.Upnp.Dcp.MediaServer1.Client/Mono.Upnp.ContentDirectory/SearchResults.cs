// 
// SearchResults.cs
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

namespace Mono.Upnp.Dcp.MediaServer1.Client
{
	public sealed class SearchResults<T> : Results<T>
	{
		readonly string search_criteria;
		
		internal SearchResults(ContentDirectory contentDirectory, string objectId, string searchCriteria,
		                       uint requestCount, string sortCriteria, int offset)
			: base (contentDirectory, objectId, requestCount, sortCriteria, offset)
		{
			search_criteria = searchCriteria;
		}
		
		protected override string FetchXml (out uint returnedCount, out uint totalCount, out uint updateId)
		{
			return ContentDirectory.Search (ObjectId, search_criteria, "*", Offset, 
				RequestCount, SortCriteria, out returnedCount, out totalCount, out id, out updateId);
		}

		public BrowseResults<T> GetMoreResults ()
		{
			if (!HasMoreResults) return null;
			var search_results = new SearchResults (ContentDirectory, ObjectId, search_criteria, RequestCount,
				sortCriteria, Offset + ReturnedCount);
			return search_results.FetchResults () ? search_results : null;
		}
	}
}
