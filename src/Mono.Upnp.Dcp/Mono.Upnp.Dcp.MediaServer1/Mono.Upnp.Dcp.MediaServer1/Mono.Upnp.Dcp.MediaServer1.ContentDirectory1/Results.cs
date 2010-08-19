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

using System;
using System.Collections;
using System.Collections.Generic;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public abstract class Results<T> : IEnumerable<T>
    {
        public Results (Container container,
                        string sortCriteria,
                        string filter,
                        uint requestCount,
                        uint offset,
                        uint totalCount,
                        IList<T> results)
        {
            if (container == null) {
                throw new ArgumentNullException ("container");
            } else if (results == null) {
                throw new ArgumentNullException ("results");
            }

            this.Container = container;
            this.SortCriteria = sortCriteria;
            this.Filter = filter;
            this.RequestCount = requestCount;
            this.Offset = offset;
            this.TotalCount = totalCount;
            this.ResultsList = results;
        }
        
        protected Container Container { get; private set; }
        
        protected string SortCriteria { get; private set; }
        
        protected string Filter { get; private set; }
        
        protected uint RequestCount { get; private set; }
        
        public uint Offset { get; private set; }
        
        public uint TotalCount { get; private set; }

        public uint Count {
            get { return (uint)ResultsList.Count; }
        }

        public IList<T> ResultsList { get; private set; }
        
        public bool HasMoreResults {
            get { return Offset + ResultsList.Count < TotalCount; }
        }

        public Results<T> GetMoreResults (RemoteContentDirectory contentDirectory)
        {
            return GetMoreResults (contentDirectory, new ResultsSettings {
                SortCriteria = SortCriteria,
                Filter = Filter,
                RequestCount = System.Math.Min (RequestCount, TotalCount - (Offset + Count)),
                Offset = Offset + Count
            });
        }
        
        protected abstract Results<T> GetMoreResults (RemoteContentDirectory contentDirectory,
                                                      ResultsSettings settings);
        
        public IEnumerator<T> GetEnumerator ()
        {
            return ResultsList.GetEnumerator ();
        }
        
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }
    }
}
