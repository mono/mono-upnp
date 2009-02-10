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

        public AllowedValueRange (Type type, XmlReader reader)
        {
            DeserializeRootElement (reader);
            VerifyDeserialization (type);
        }
        
        public string Minimum { get; private set; }
        public string Maximum { get; private set; }
        public string Step { get; private set; }
        internal IComparable Min { get; set; }
        internal IComparable Max { get; set; }

        void DeserializeRootElement (XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException ("reader");

            try {
                while (Helper.ReadToNextElement (reader)) {
					var property_reader = reader.ReadSubtree ();
					property_reader.Read ();
                    try {
                    	DeserializePropertyElement (property_reader);
                    } catch (Exception e) {
                        Log.Exception (
                            "There was a problem deserializing one of the allowed value range description elements.", e);
                    } finally {
						property_reader.Close ();
					}
                }
            } catch (Exception e) {
                throw new UpnpDeserializationException (
                    string.Format ("There was a problem deserializing {0}.", ToString ()), e);
            }
        }

        void DeserializePropertyElement (XmlReader reader)
        {
            switch (reader.Name) {
            case "maximum":
                Maximum = reader.ReadString ();
                break;
            case "minimum":
                Minimum = reader.ReadString ();
                break;
            case "step":
                Step = reader.ReadString ();
                break;
            default: // This is a workaround for Mono bug 334752
                reader.Skip ();
                break;
            }
        }

        void VerifyDeserialization (Type type)
        {
            if (Maximum == null) {
                Maximum = GetProperty (type, "MaxValue");
                if (Maximum == null) {
                    throw new UpnpDeserializationException (
                        "The allowed value range has no maximum value and one cannot be infered.");
                } else {
                    Log.Exception (new UpnpDeserializationException (
                        string.Format ("The allowed value range has no maximum value, using {0}.MaxValue.", type.Name)));
                }
            }
            if (Minimum == null) {
                Minimum = GetProperty (type, "MinValue");
                if (Minimum == null) {
                    throw new UpnpDeserializationException (
                        "The allowed value range has no minimum value and one cannot be infered.");
                } else {
                    Log.Exception (new UpnpDeserializationException (
                        string.Format ("The allowed value range has no minimum value, using {0}.MinValue.", type.Name)));
                }
            }
        }

        static string GetProperty (Type type, string name)
        {
            if (type != null) {
                var property = type.GetProperty (name, BindingFlags.Public | BindingFlags.Static);
                if (property != null) {
                    return property.GetGetMethod ().Invoke (null, null).ToString ();
                }
            }
            return null;
        }

        public override string ToString ()
        {
            return String.Format ("AllowedValueRange {{ {0}-{1}, {2} }}", Minimum, Maximum, Step);
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
                range1.Minimum == range2.Minimum &&
                range1.Maximum == range2.Maximum &&
                range1.Step == range2.Step;
        }

        public static bool operator != (AllowedValueRange range1, AllowedValueRange range2)
        {
            return !(range1 == range2);
        }

        public override int GetHashCode ()
        {
            return Minimum.GetHashCode () ^ Maximum.GetHashCode () ^ (Step == null ? 0 : Step.GetHashCode ());
        }
    }
}
