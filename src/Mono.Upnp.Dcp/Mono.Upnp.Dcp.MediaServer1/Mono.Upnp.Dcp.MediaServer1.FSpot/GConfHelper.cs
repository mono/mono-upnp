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
using GConf;
using System.Collections.Generic;

namespace Mono.Upnp.Dcp.MediaServer1.FSpot
{
    internal static class GConfHelper
    {
        internal const string GCONF_APP_PATH = "/apps/f-spot/upnp";
        internal const string LOOK_FOR_LIBRARIES_KEY = GCONF_APP_PATH + "/look_for_libraries";
        internal const string SHARE_LIBRARY_KEY = GCONF_APP_PATH + "/share_library";
        internal const string LIBRARY_NAME_KEY = GCONF_APP_PATH + "/library_name";
        internal const string SHARE_ALL_CATEGORIES_KEY = GCONF_APP_PATH + "/share_all_categories";
        internal const string SHARED_CATEGORIES_KEY = GCONF_APP_PATH + "/shared_categories";

        static GConfHelper ()
        {
            Client = new GConf.Client ();
        }

        public static GConf.Client Client { get; private set; }

        public static bool LookForLibraries
        {
            get
            {
                try {
                    return (bool)Client.Get (LOOK_FOR_LIBRARIES_KEY);
                } catch (NoSuchKeyException) {
                    Client.Set (LOOK_FOR_LIBRARIES_KEY, true);
                    return true;
                }
            }
            set
            {
                Client.Set (LOOK_FOR_LIBRARIES_KEY, value);
            }
        }

        public static bool ShareLibrary
        {
            get
            {
                try {
                    return (bool)Client.Get (SHARE_LIBRARY_KEY);
                } catch (NoSuchKeyException) {
                    Client.Set (SHARE_LIBRARY_KEY, false);
                    return false;
                }
            }
            set
            {
                Client.Set (SHARE_LIBRARY_KEY, value);
            }
        }

        public static string LibraryName
        {
            get
            {
                try {
                    return (string)Client.Get (LIBRARY_NAME_KEY);
                } catch (NoSuchKeyException) {
                    var default_value = string.Format ("{0} | F-Spot Photo Sharing", Environment.UserName);
                    Client.Set (LIBRARY_NAME_KEY, default_value);
                    return default_value;
                }
            }
            set
            {
                Client.Set (LIBRARY_NAME_KEY, value);
            }
        }

        public static bool ShareAllCategories
        {
            get
            {
                try {
                    return (bool)Client.Get (SHARE_ALL_CATEGORIES_KEY);
                } catch (NoSuchKeyException) {
                    Client.Set (SHARE_ALL_CATEGORIES_KEY, true);
                    return true;
                }
            }
            set
            {
                Client.Set (SHARE_ALL_CATEGORIES_KEY, value);
            }
        }

        public static IEnumerable<uint> SharedCategories
        {
            get
            {
                try {
                    var ret = new List<uint> ();
                    var list = Client.Get (GConfHelper.SHARED_CATEGORIES_KEY);
                    if (list != null) {
                        foreach (var item in (IEnumerable<int>)list)
                        {
                            ret.Add (Convert.ToUInt32 (item));
                        }
                    }

                    return ret.ToArray ();
                } catch (NoSuchKeyException) {
                    Client.Set (SHARE_ALL_CATEGORIES_KEY, new uint[0]);
                    return new uint[0];
                }
            }
            set
            {
                Client.Set (SHARED_CATEGORIES_KEY, UIntArrayToIntArray (value));
            }
        }

        private static IEnumerable<int> UIntArrayToIntArray (IEnumerable<uint> uintList)
        {
            var list = new List<int> ();
            foreach (var id in uintList) {
                list.Add (unchecked((int)id));
            }

            return list.ToArray ();
        }
    }
}

