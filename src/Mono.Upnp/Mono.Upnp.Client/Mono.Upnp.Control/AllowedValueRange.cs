//
// AllowedValueRange.cs
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

namespace Mono.Upnp.Control
{
	public class AllowedValueRange
	{
        internal AllowedValueRange (XmlReader reader)
        {
            Deserialize (reader);
        }

        private string minimum;
        public string Minimum {
            get { return minimum; }
        }

        private string maximum;
        public string Maximum {
            get { return maximum; }
        }

        private string step;
        public string Step {
            get { return step; }
        }

        #region Overrides

        public override string ToString ()
        {
            return String.Format ("AllowedValueRange {{ {0}-{1}, {2} }}", minimum, maximum, step);
        }

        public override bool Equals (object obj)
        {
            AllowedValueRange range = obj as AllowedValueRange;
            return range != null && range.minimum == minimum && range.maximum == maximum && range.step == step;
        }

        public override int GetHashCode ()
        {
            return minimum.GetHashCode () ^ maximum.GetHashCode () ^ (step == null ? 0 : step.GetHashCode ());
        }

        #endregion

        #region Deserialization

        private void Deserialize (XmlReader reader)
        {
            try {
                reader.Read ();
                while (Helper.ReadToNextElement (reader)) {
                    Deserialize (reader.ReadSubtree (), reader.Name);
                }
                reader.Close ();
            } catch (Exception e) {
                throw new UpnpDeserializationException ("There was a problem deserializing an allowed value range.", e);
            }
            VerifyDeserialization ();
        }

        private void Deserialize (XmlReader reader, string element)
        {
            reader.Read ();
            switch (element) {
            case "maximum":
                maximum = reader.ReadString ();
                break;
            case "minimum":
                minimum = reader.ReadString ();
                break;
            case "step":
                step = reader.ReadString ();
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
            reader.Close ();
        }

        private void VerifyDeserialization ()
        {
            if (maximum == null) {
                throw new UpnpDeserializationException ("The allowed value range does not specify a maximum value.");
            }
            if (minimum == null) {
                throw new UpnpDeserializationException ("The allowed value range does not specify a minimum value.");
            }
        }

        #endregion

    }
}
