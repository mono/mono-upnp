//
// Log.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2008 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Diagnostics;

namespace Mono.Upnp.Internal
{
    internal static class Log
    {
        static bool? enabled;
        static bool Enabled {
            get {
                if (enabled == null) {
                    enabled = Environment.GetEnvironmentVariable ("MONO_UPNP_DEBUG") != null;
                }

                return enabled.Value;
            }
        }

        public static void Exception (Exception e)
        {
            Error (e.ToString ());
        }

        public static void Exception (string message, Exception e)
        {
            Error (string.Format ("{0}, {1}", message, e));
        }
        
        public static void Error (string message)
        {
            if (Enabled) {
                Console.Error.WriteLine ("Mono.Upnp Error: {0}", message);
            }
        }
        
        [Conditional ("DEBUG")]
        public static void Warning (string message)
        {
            if (Enabled) {
                Console.WriteLine ("Mono.Upnp Warning: {0}", message);
            }
        }
        
        [Conditional ("DEBUG")]
        public static void Information (string message)
        {
            if (Enabled) {
                Console.WriteLine ("Mono.Upnp Message: {0}", message);
            }
        }
    }
}
