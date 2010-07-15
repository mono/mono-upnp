// 
// GConfConstants.cs
//  
// Author:
//       Yavor Georgiev <fealebenpae@gmail.com>
// 
// Copyright (c) 2010 Yavor Georgiev
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
namespace Mono.Upnp.Dcp.MediaServer1.FSpot
{
    internal static class GConfConstants
    {
        internal const string GCONF_APP_PATH = "/apps/f-spot-upnp";
        internal const string LOOK_FOR_LIBRARIES_KEY = GCONF_APP_PATH + "/look_for_libraries";
        internal const string SHARE_LIBRARY_KEY = GCONF_APP_PATH + "/share_library";
        internal const string SHARE_ALL_CATEGORIES_KEY = GCONF_APP_PATH + "/share_all_categories";
        internal const string SHARED_CATEGORIES_KEY = GCONF_APP_PATH + "/shared_categories";
    }
}

