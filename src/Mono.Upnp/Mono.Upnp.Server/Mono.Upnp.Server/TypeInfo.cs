//
// TypeInfo.cs
//
// Author:
//   Scott Peterson <lunchtimemama@gmail.com>
//
// Copyright (C) 2008 S&S Black Ltd.
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

using Mono.Upnp.Server.Internal;

namespace Mono.Upnp.Server
{
	public class TypeInfo
	{
        readonly string type;
        readonly Version version;
        readonly string domain_name;
        readonly string kind;
        string to_string;
        string to_url_string;
        
        internal TypeInfo (string type, Version version, string domainName, string kind)
        {
            if (type == null) {
                throw new ArgumentNullException ("type");
            }
            if (version == null) {
                throw new ArgumentNullException ("version");
            }

            this.type = type;
            this.version = version;
            this.domain_name = domainName ?? "schemas-upnp-org";
            this.kind = kind;
        }

        public string DomainName {
            get { return domain_name; }
        }

        public string Type {
            get { return type; }
        }

        public Version Version {
            get { return version; }
        }

        public override string ToString ()
        {
            if (to_string == null) {
                var version = this.version.Minor == 0
                    ? this.version.Major.ToString ()
                    : string.Format ("{0}.{1}", this.version.Major, this.version.Minor);
                to_string = string.Format ("urn:{0}:{1}:{2}:{3}", domain_name, kind, type, version);
            }
            return to_string;
        }

        public string ToUrlString ()
        {
            if (to_url_string == null) {
                var version = this.version.Minor == 0
                    ? this.version.Major.ToString ()
                    : string.Format ("{0}.{1}", this.version.Major, this.version.Minor);
                to_url_string = string.Format ("{0}/{1}/{2}", domain_name, type, version);
            }
            return to_url_string;
        }

        public override bool Equals (object obj)
        {
            return this == obj as TypeInfo;
        }

        public override int GetHashCode ()
        {
            return DomainName.GetHashCode () ^ Type.GetHashCode () ^ Version.GetHashCode ();
        }

        public static bool operator == (TypeInfo type1, TypeInfo type2)
        {
            if (ReferenceEquals (type1, null)) {
                return ReferenceEquals (type2, null);
            }
            return !ReferenceEquals (type2, null) &&
                type1.DomainName == type2.DomainName &&
                type1.Type == type2.Type &&
                type1.Version == type2.Version &&
                type1.kind == type2.kind;
        }

        public static bool operator != (TypeInfo type1, TypeInfo type2)
        {
            if (ReferenceEquals (type1, null)) {
                return !ReferenceEquals (type2, null);
            }
            return ReferenceEquals (type2, null) ||
                type1.DomainName != type2.DomainName ||
                type1.Type != type2.Type ||
                type1.Version != type2.Version ||
                type1.kind != type2.kind;
        }

    }
}
