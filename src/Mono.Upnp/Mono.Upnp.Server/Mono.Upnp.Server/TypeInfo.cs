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
using System.Xml;

using Mono.Upnp.Server.Internal;

namespace Mono.Upnp.Server
{
	public abstract class TypeInfo
	{
        internal TypeInfo (string type, Version version)
            : this ("schemas-upnp-org", type, version)
        {
        }

        internal TypeInfo (string domainName, string type, Version version)
        {
            if (domainName == null) {
                throw new ArgumentNullException ("domainName");
            }
            if (type == null) {
                throw new ArgumentNullException ("type");
            }
            if (version == null) {
                throw new ArgumentNullException ("version");
            }

            this.domain_name = domainName;
            this.type = type;
            this.version = version;
        }

        protected abstract string Kind { get; }

        private string domain_name;
        public string DomainName {
            get { return domain_name; }
        }

        private string type;
        public string Type {
            get { return type; }
        }

        private Version version;
        public Version Version {
            get { return version; }
        }

        private string to_string;
        public override string ToString ()
        {
            if (to_string == null) {
                string version = this.version.Minor == 0
                    ? this.version.Major.ToString ()
                    : String.Format ("{0}.{1}", this.version.Major, this.version.Minor);
                to_string = String.Format ("urn:{0}:{1}:{2}:{3}", domain_name, Kind, type, version);
            }
            return to_string;
        }

        private string to_url_string;
        public string ToUrlString ()
        {
            if (to_url_string == null) {
                string version = this.version.Minor == 0
                    ? this.version.Major.ToString ()
                    : String.Format ("{0}.{1}", this.version.Major, this.version.Minor);
                to_url_string = String.Format ("{0}/{1}/{2}", domain_name, type, version);
            }
            return to_url_string;
        }

        public override bool Equals (object obj)
        {
            TypeInfo type_info = obj as TypeInfo;
            return type_info != null && this == type_info;
        }

        public override int GetHashCode ()
        {
            return DomainName.GetHashCode () ^ Type.GetHashCode () ^ Version.GetHashCode ();
        }

        public static bool operator == (TypeInfo type1, TypeInfo type2)
        {
            if (Object.ReferenceEquals (type1, null) && Object.ReferenceEquals (type2, null)) {
                return true;
            } else if (Object.ReferenceEquals (type1, null) || Object.ReferenceEquals (type2, null)) {
                return false;
            }
            return type1.DomainName == type2.DomainName &&
                type1.Type == type2.Type &&
                type1.Version == type2.Version &&
                type1.Kind == type2.Kind;
        }

        public static bool operator != (TypeInfo type1, TypeInfo type2)
        {
            if (Object.ReferenceEquals (type1, null) && Object.ReferenceEquals (type2, null)) {
                return false;
            } else if (Object.ReferenceEquals (type1, null) || Object.ReferenceEquals (type2, null)) {
                return true;
            }
            return type1.DomainName != type2.DomainName ||
                type1.Type != type2.Type ||
                type1.Version != type2.Version ||
                type1.Kind != type2.Kind;
        }

    }
}
