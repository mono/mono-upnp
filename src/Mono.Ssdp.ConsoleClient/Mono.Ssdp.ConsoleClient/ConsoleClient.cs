//
// ConsoleClient.cs
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
using System.Collections.Generic;

using Mono.Ssdp;

namespace Mono.Ssdp.ConsoleClient
{
    public static class ConsoleClient
    {
        private static List<string> search_targets = new List<string> ();
        private static bool verbose = false;
        
        public static int Main (string [] args)
        {
            bool show_help = false;
        
            for (int i = 0; i < args.Length; i++) {
                if (args[i][0] != '-') {
                    continue;
                }
            
                switch (args[i]) {
                    case "-t":
                    case "--target":
                        search_targets.Add (args[++i]);
                        break;
                    case "-n":
                    case "--no-strict":
                        Client.StrictProtocol = false;
                        break;
                    case "-h":
                    case "--help":
                        show_help = true;
                        break;
                    case "-v":
                    case "--verbose":
                        verbose = true;
                        break;
                }
            }
            
            if (show_help) {
                Console.WriteLine ("Usage: mssdp-client [-t <USN-1> -t <USN-2> ... -t <USN-N>]");
                Console.WriteLine ();
                Console.WriteLine ("    -h|--help       shows this help");
                Console.WriteLine ("    -v|--verbose    print verbose details of what's happening");
                Console.WriteLine ("    -t|--target     search for specific target");
                Console.WriteLine ("    -n|--no-strict  turn off strict protocol handling");
                Console.WriteLine ();
                Console.WriteLine ("The default search target is ssdp:all to match any UPnP device");
                Console.WriteLine ();
                return 1;
            }
            
            if (!Client.StrictProtocol) {
                Console.WriteLine ("Strict protocol handling has been disabled. Mono.Ssdp will be more");
                Console.WriteLine ("relaxed when parsing SSDP responses and notifications.");
                Console.WriteLine ();
            }
            
            Console.WriteLine ("Hit ^C when you're bored waiting for responses.");
            Console.WriteLine ();
            
            Client client = new Client ();
            client.ServiceAdded += OnServiceOperation;
            client.ServiceRemoved += OnServiceOperation;
            client.ServiceUpdated += OnServiceOperation;
            
            if (search_targets.Count == 0) {
                search_targets.Add ("ssdp:all");
            }
            
            if (verbose) {
                Console.WriteLine ("Searching for the following targets:");
            }
            
            foreach (string target in search_targets) {
                if (verbose) {
                    Console.WriteLine ("    {0}", target);
                }
                
                client.Browse (target);
            }
            
            if (verbose) {
                Console.WriteLine ();
            }
            
            while (true) {
                System.Threading.Thread.Sleep (1000);
            }
        }
        
        private static void OnServiceOperation (object o, ServiceArgs args)
        {
            string action = null;
            switch (args.Operation) {
                case ServiceOperation.Added:   action = "ADDED  "; break;
                case ServiceOperation.Updated: action = "UPDATED"; break;
                case ServiceOperation.Removed: action = "REMOVED"; break;
            }
            
            Console.WriteLine ("{0}: [{1}] {2}", action, DateTime.Now, args.Usn);
            
            if (verbose && args.Operation != ServiceOperation.Removed) {
                Console.WriteLine ("    Search Type     : {0}", args.Service.ServiceType);
                Console.WriteLine ("    Expiration      : {0}", args.Service.Expiration);
                Console.WriteLine ("    Locations ({0}) :", args.Service.LocationCount);
                foreach (string location in args.Service.Locations) {
                    Console.WriteLine ("        {0}", location);
                }
                Console.WriteLine ();
            }
        }
    }
}