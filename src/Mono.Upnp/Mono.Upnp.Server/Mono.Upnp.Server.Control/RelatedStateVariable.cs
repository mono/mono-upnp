// 
// RelatedServiceVariable.cs
//  
// Author:
//       Scott Peterson <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Peterson
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

namespace Mono.Upnp.Server.Control
{
    public class RelatedStateVariable : ServiceVariable
    {
        readonly object default_value;
        readonly AllowedValueRange allowed_value_range;
        
        [XmlElement ("defaultValue", OmitIfNull = true)]
        public object DefaultValue {
            get { return default_value; }
        }
        
        [XmlElement ("allowedValueRange", OmitIfNull = true)]
        public AllowedValueRange AllowedValueRange {
            get { return allowed_value_range; }
        }
        
        [XmlArray ("allowedValueList", OmitIfNull = true)]
        [XmlArrayItem ("allowedValue")]
        public IEnumerable<string> AllowedValueList {
            get { return Type.IsEnum ? GetValues (Type) : null; }
        }
        
        protected internal RelatedStateVariablename, Type dataType, object defaultValue, AllowedValueRange allowedValueRange)
            : base (name, dataType, false)
        {
            // TODO check that allowedValueRange is only used with numeric types
            
            this.default_value = defaultValue;
            this.allowed_value_range = allowedValueRange;
        }
        
        static IEnumerable<string> GetValues (Type enumType)
        {
            foreach (var field in enumType.GetFields ()) {
                var value = field.Name;
                foreach (var custom_attribute in field.GetCustomAttributes ()) {
                    var allowed_value = custom_attribute as AllowedValueAttribute;
                    if (allowed_value != null && allowed_value.Value != null) {
                        value = allowed_value.Value;
                        break;
                    }
                }
                yield return value;
            }
        }
    }
}
