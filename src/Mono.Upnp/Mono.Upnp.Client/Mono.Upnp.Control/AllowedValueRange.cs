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
using System.Reflection;
using System.Xml;

using Mono.Upnp.Internal;

namespace Mono.Upnp.Control
{
	public sealed class AllowedValueRange
	{
        string minimum;
        string maximum;
        string step;
        IComparable min;
        IComparable max;

        public AllowedValueRange (Type type, XmlReader reader)
        {
            Deserialize (reader);
            VerifyDeserialization (type);
        }
        
        public string Minimum {
            get { return minimum; }
        }

        internal IComparable Min {
            get { return min; }
            set { min = value; }
        }

        public string Maximum {
            get { return maximum; }
        }

        internal IComparable Max {
            get { return max; }
            set { max = value; }
        }

        public string Step {
            get { return step; }
        }

        void Deserialize (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                reader.Read ();
                while (Helper.ReadToNextElement (reader)) {
                    try {
                        Deserialize (reader.ReadSubtree (), reader.Name.ToLower ());
                    } catch (Exception e) {
                        Log.Exception ("There was a problem deserializing one of the allowed value range description elements.", e);
                    }
                }
                reader.Close ();
            } catch (Exception e) {
                throw new UpnpDeserializationException (string.Format ("There was a problem deserializing {0}.", ToString ()), e);
            }
        }

        void Deserialize (XmlReader reader, string element)
        {
            reader.Read ();
            switch (element.ToLower ()) {
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

        void VerifyDeserialization (Type type)
        {
            if (maximum == null) {
                maximum = GetProperty (type, "MaxValue");
                if (maximum == null) {
                    throw new UpnpDeserializationException ("The allowed value range has no maximum value and one cannot be infered.");
                } else {
                    Log.Exception (new UpnpDeserializationException (
                        string.Format ("The allowed value range has no maximum value, using {0}.MaxValue.", type.Name)));
                }
            }
            if (minimum == null) {
                minimum = GetProperty (type, "MinValue");
                if (minimum == null) {
                    throw new UpnpDeserializationException ("The allowed value range has no minimum value and one cannot be infered.");
                } else {
                    Log.Exception (new UpnpDeserializationException (
                        string.Format ("The allowed value range has no minimum value, using {0}.MinValue.", type.Name)));
                }
            }
        }

        static string GetProperty (Type type, string name)
        {
            if (type != null) {
                PropertyInfo property = type.GetProperty (name, BindingFlags.Public | BindingFlags.Static);
                if (property != null) {
                    return property.GetGetMethod ().Invoke (null, null).ToString ();
                }
            }
            return null;
        }

        public override string ToString ()
        {
            return String.Format ("AllowedValueRange {{ {0}-{1}, {2} }}", minimum, maximum, step);
        }

        public override bool Equals (object obj)
        {
            AllowedValueRange range = obj as AllowedValueRange;
            return range == this;
        }

        public static bool operator == (AllowedValueRange range1, AllowedValueRange range2)
        {
            if (ReferenceEquals (range1, null)) {
                return ReferenceEquals (range2, null);
            }
            return !ReferenceEquals (range2, null) &&
                range1.minimum == range2.minimum &&
                range1.maximum == range2.maximum &&
                range1.step == range2.step;
        }

        public static bool operator != (AllowedValueRange range1, AllowedValueRange range2)
        {
            return !(range1 == range2);
        }

        public override int GetHashCode ()
        {
            return minimum.GetHashCode () ^ maximum.GetHashCode () ^ (step == null ? 0 : step.GetHashCode ());
        }
    }
}
