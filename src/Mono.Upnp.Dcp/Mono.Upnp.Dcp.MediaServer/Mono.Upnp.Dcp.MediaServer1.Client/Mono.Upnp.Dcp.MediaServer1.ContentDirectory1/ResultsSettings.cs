// 
// ResultsSettings.cs
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

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
	public struct ResultsSettings
	{
        const int sort_criteria_mask = 1;
        const int filter_mask = 2;
        const int request_count_mask = 4;
        
        int field_mask;
        string sort_criteria;
        string filter;
        uint request_count;
        
		public string SortCriteria {
            get { return (field_mask & sort_criteria_mask) == 0 ? "" : sort_criteria; }
            set {
                field_mask |= sort_criteria_mask;
                sort_criteria = value;
            }
        }
		public string Filter {
            get { return (field_mask & filter_mask) == 0 ? "*" : filter; }
            set {
                field_mask |= filter_mask;
                filter = value;
            }
        }
		public uint RequestCount {
            get { return (field_mask & request_count_mask) == 0 ? 200 : request_count; }
            set {
                field_mask |= request_count_mask;
                request_count = value;
            }
        }
		public uint Offset { get; set; }
	}
}
