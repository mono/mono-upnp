//
// ServiceAction.cs
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
using System.Collections.Generic;
using System.Reflection;

using Mono.Upnp.Server.Internal;
using Mono.Upnp.Server.Serialization;

namespace Mono.Upnp.Server
{
    [XmlType ("action")]
    public class ServiceAction
	{
        readonly string name;
        readonly Dictionary<string, Argument> arguments = new Dictionary<string, Argument> ();
        MethodInfo method_info;
        
        protected internal ServiceAction (string name)
        {
            if (name == null) throw new ArgumentNullException ("name");
            
            this.name = name;
        }
        
        [XmlElement ("name")]
        public string Name {
            get { return name; }
        }
        
        [XmlArray ("argumentList")]
        public IEnumerable<Argument> Arguments {
            get { return arguments.Values; }
        }
        
        public MethodInfo MethodInfo {
            get { return method_info; }
        }
        
        internal void Initialize (ServiceController serviceController, MethodInfo methodInfo)
        {
            method_info = methodInfo;
            InitializeCore (serviceController, methodInfo);
        }
        
        protected virtual void InitializeCore (ServiceController serviceController, MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException ("methodInfo");
            
            foreach (var parameter in methodInfo.GetParameters ()) {
                ProcessArgument (serviceController, parameter);
            }
            
            if (methodInfo.ReturnType != typeof (void)) {
                ProcessArgument (serviceController, methodInfo.ReturnParameter, true);
            }
        }
        
        protected virtual void ProcessArgument (ServiceController serviceController, ParameterInfo parameterInfo, bool isRetVal)
        {
            if (serviceController == null) throw new ArgumentNullException ("serviceController");
            if (parameterInfo == null) throw new ArgumentNullException ("parameterInfo");
            
            isRetVal |= parameterInfo.IsRetval;
            var name = parameterInfo.Name;
            object default_value = null;
            AllowedValueRange allowed_value_range = null;
            
            foreach (var custom_attribute in parameterInfo.GetCustomAttributes (true)) {
                var argument_attribute = custom_attribute as UpnpArgumentAttribute;
                if (argument_attribute != null) {
                    if (argument_attribute.Name != null) {
                        name = argument_attribute.Name;
                    }
                    default_value = argument_attribute.DefaultValue;
                    allowed_value_range = argument_attribute.AllowedValueRange;
                    break;
                }
            }

            if (name == null && isRetVal) {
                name = "ReturnValue";
            }
            
            var related_state_variable = serviceController.GetRelatedStateVariable (name, parameterInfo.ParameterType, default_value, allowed_value_range);
            var direction = parameterInfo.IsOut | isRetVal ? ArgumentDirection.Out : ArgumentDirection.In;
            
            AddArgument (CreateArgument (name, isRetVal, related_state_variable, direction));
        }
        
        protected void AddArgument (Argument argument)
        {
            if (argument != null) {
                if (arguments.ContainsKey (argument.Name))
                    throw new UpnpServerException (string.Format ("The action '{0}' has multiple arguments named '{1}'.", name, argument.Name));
                
                arguments.Add (argument.Name, argument);
            }
        }
        
        protected virtual Argument CreateArgument (string name, ArgumentDirection direction, bool isRetVal, StateVariable relatedStateVariable)
        {
            return new Argument (this, direction, isRetVal, relatedStateVariable);
        }
	}
}
