// 
// UpnpRelatedStateVariableAttribute.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2009 Scott Thomas
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

namespace Mono.Upnp.Control
{
    [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class UpnpRelatedStateVariableAttribute : Attribute
    {
        readonly string minimum_value;
        readonly string maximum_value;
        
        public UpnpRelatedStateVariableAttribute ()
        {
        }
        
        public UpnpRelatedStateVariableAttribute (string name)
            : this (name, null, null)
        {
        }
        
        public UpnpRelatedStateVariableAttribute (string minimumValue, string maximumValue)
            : this (null, minimumValue, maximumValue)
        {
        }
        
        public UpnpRelatedStateVariableAttribute (string name, string minimumValue, string maximumValue)
        {
            Name = name;
            this.minimum_value = minimumValue;
            this.maximum_value = maximumValue;
        }
        
        public string Name { get; set; }
        
        public string DataType { get; set; }
        
        public string DefaultValue { get; set; }
        
        public string MinimumValue {
            get { return minimum_value; }
        }
        
        public string MaximumValue {
            get { return maximum_value; }
        }
        
        public string StepValue { get; set; }
    }
}
