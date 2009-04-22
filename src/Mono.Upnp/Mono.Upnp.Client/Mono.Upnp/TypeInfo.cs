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

namespace Mono.Upnp
{
    public class TypeInfo
    {
		readonly string domain_name;
		readonly string type;
		readonly Version version;
        readonly string kind;
		
        internal TypeInfo (string typeDescription, string kind)
        {
            try {
                var sections = typeDescription.Trim ().Split (':');
                var versions = sections[4].Split ('.');
                if (versions.Length == 1) {
                    version = new Version (int.Parse (versions[0]), 0);
                } else {
                    version = new Version (int.Parse (versions[0]), int.Parse (versions[1]));
                }
                domain_name = sections[1];
                type = sections[3];
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deseriailizing a type description.", e);
            }
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
            var version = this.version.Minor == 0
                ? this.version.Major.ToString ()
                : string.Format ("{0}.{1}", this.version.Major, this.version.Minor);
            return string.Format ("urn:{0}:{1}:{2}:{3}", domain_name, kind, type, version);
        }

        public override bool Equals (object obj)
        {
            var type_info = obj as TypeInfo;
            return type_info == this;
        }

        public override int GetHashCode ()
        {
            return domain_name.GetHashCode () ^ type.GetHashCode () ^ version.GetHashCode () ^ kind.GetHashCode ();
        }

        public static bool operator == (TypeInfo type1, TypeInfo type2)
        {
            if (ReferenceEquals (type1, null)) {
                return ReferenceEquals (type2, null);
            }
            return !ReferenceEquals (type2, null) &&
                type1.domain_name == type2.domain_name &&
                type1.type == type2.type &&
                type1.version == type2.version &&
                type1.kind == type2.kind;
        }

        public static bool operator != (TypeInfo type1, TypeInfo type2)
        {
            return !(type1 == type2);
        }
    }
}
