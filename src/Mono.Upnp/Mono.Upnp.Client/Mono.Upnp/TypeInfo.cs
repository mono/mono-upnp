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

using Mono.Upnp.Internal;

namespace Mono.Upnp
{
	public abstract class TypeInfo
	{
        internal TypeInfo (string typeDescription)
        {
            try {
                string[] sections = typeDescription.Split (':');
                Version version;
                string[] versions = sections[4].Split ('.');
                if (versions.Length == 1) {
                    version = new Version (Int32.Parse (versions[0]), 0);
                } else {
                    version = new Version (Int32.Parse (versions[0]), Int32.Parse (versions[1]));
                }
                domain_name = sections[1];
                type = sections[3];
                this.version = version;
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deseriailizing a type description.", e);
            }
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

        #region Equality

        public override string ToString ()
        {
            string version = this.version.Minor == 0
                ? this.version.Major.ToString ()
                : String.Format ("{0}.{1}", this.version.Major, this.version.Minor);
            return String.Format ("urn:{0}:{1}:{2}:{3}", domain_name, Kind, type, version);
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

        #endregion

    }
}
