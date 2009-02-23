// 
// Results.cs
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

using System.Collections;
using System.Collections.Generic;

namespace Mono.Upnp.ContentDirectory
{
	public abstract class Results<T> : IEnumerable<T> where T : Object
	{
		readonly ContentDirectory content_directory;
		readonly string object_id;
		readonly string sort_criteria;
		readonly string filter;
		readonly uint request_count;
		readonly uint offset;
		readonly List<T> results = new List<T> ();
		
		internal Results (ContentDirectory contentDirectory, string objectId, string sortCriteria,
		                  string filter, uint requestCount, uint offset)
		{
			this.content_directory = contentDirectory;
			this.object_id = objectId;
			this.sort_criteria = sortCriteria ?? "";
			this.filter = string.IsNullOrEmpty (filter) ? "*" : filter;
			this.request_count = requestCount;
			this.offset = offset;
		}
		
		protected ContentDirectory ContentDirectory { get { return content_directory; } }
		protected string ObjectId { get { return object_id; } }
		protected string SortCriteria { get { return sort_criteria; } }
		protected string Filter { get { return filter; } }
		protected uint RequestCount { get { return request_count; } }
		public uint Offset { get { return offset; } }
		public uint ReturnedCount { get; private set; }
		public uint TotalCount { get; private set; }
		public bool IsOutOfDate { get; private set; }
		public bool HasMoreResults {
			get { return Offset + ReturnedCount < TotalCount; }
		}
		
		internal void FetchResults ()
		{
			uint returned_count, total_count, update_id;
			var xml = FetchXml (out returned_count, out total_count, out update_id);
			ReturnedCount = returned_count;
			TotalCount = total_count;
			if (content_directory.CheckIfContainerIsOutOfDate (object_id, update_id) && offset != 0) {
				IsOutOfDate = true;
			} else {
				foreach (var result in content_directory.Deserialize<T> (filter, xml)) {
					results.Add (result);
				}
			}
		}
		
		protected abstract string FetchXml (out uint returnedCount, out uint totalCount, out uint updateId);
		
		public Results<T> GetMoreResults ()
		{
			if (!HasMoreResults) return null;
			
			var results = CreateMoreResults ();
			results.FetchResults ();
			return results;
		}
		
		protected abstract Results<T> CreateMoreResults ();
		
		public IEnumerator<T> GetEnumerator ()
		{
			return results.GetEnumerator ();
		}
		
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}
}
