using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

using Mono.Upnp.Server.Internal;

namespace Mono.Upnp.Server
{
    public class Action
	{
        private readonly Service service;
        private readonly string name;
        private readonly Dictionary<string, Argument> arguments = new Dictionary<string, Argument> ();
        private Argument return_argument;
        private MethodInfo method;

        public IDictionary<string, Argument> Arguments  {
            get { return arguments; }
        }

        protected internal Action (Service service, string name)
        {
            if (service == null) {
                throw new ArgumentNullException ("service");
            }
            if (name == null) {
                throw new ArgumentNullException ("name");
            }

            this.service = service;
            this.name = name;
        }

        protected internal virtual void Initialize (MethodInfo method)
        {
            this.method = method;
            foreach (ParameterInfo parameter in method.GetParameters ()) {
                ProcessArgument (parameter);
            }
            if (method.ReturnType != typeof (void)) {
                ProcessArgument (method.ReturnParameter, true);
            }
        }

        protected internal virtual void Execute ()
        {
            object[] parameters = new object[arguments.Count];
            int i = 0;
            foreach (Argument argument in arguments.Values) {
                parameters[i++] = argument.Value;
            }
            object retval = method.Invoke (service, parameters);
            if (return_argument != null) {
                return_argument.Value = retval;
            }
            i = 0;
            foreach (Argument argument in arguments.Values) {
                if (argument.Direction == ArgumentDirection.Out) {
                    argument.Value = parameters[i];
                }
                i++;
            }
        }

        private void ProcessArgument (ParameterInfo parameter)
        {
            ProcessArgument (parameter, false);
        }

        protected virtual void ProcessArgument (ParameterInfo parameter, bool isRetVal)
        {
            isRetVal |= parameter.IsRetval;
            string name = parameter.Name;
            object default_value = null;
            AllowedValueRange allowed_value_range = null;
            foreach (object attribute in parameter.GetCustomAttributes (true)) {
                UpnpArgumentAttribute argument_attribute = attribute as UpnpArgumentAttribute;
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
                name = name + "ReturnValue";
            }
            StateVariable related_state_variable = CreateRelatedStateVariable (name, parameter.ParameterType, default_value, allowed_value_range);
            ArgumentDirection direction = parameter.IsOut | isRetVal ? ArgumentDirection.Out : ArgumentDirection.In;
            Argument argument = new Argument (this, name, isRetVal, related_state_variable, direction);
            if (parameter.IsRetval) {
                SetReturnArgument (argument);
            } else {
                AddParameterArgument (argument);
            }
            if (!service.StateVariables.ContainsKey (related_state_variable.Name)) {
                service.StateVariables.Add (related_state_variable.Name, related_state_variable);
            }
        }

        protected virtual StateVariable CreateRelatedStateVariable (string argumentName, Type dataType, object defaultValue, AllowedValueRange allowedValueRange)
        {
            string name = "A_ARG_TYPE_" + argumentName;
            if (service.StateVariables.ContainsKey (name)) {
                int count = 1;
                while (StateVariableNameConflict (name, dataType)) {
                    name = String.Format ("A_ARG_TYPE_{0}_{1}", argumentName, count++);
                }
                return service.StateVariables.ContainsKey (name)
                    ? service.StateVariables[name]
                    : new StateVariable (service, name, dataType, defaultValue, allowedValueRange, false);
            } else {
                return new StateVariable (service, name, dataType, defaultValue, allowedValueRange, false);
            }
        }

        protected bool StateVariableNameConflict (string name, Type dataType)
        {
            if (!service.StateVariables.ContainsKey (name)) {
                return false;
            }
            StateVariable variable = service.StateVariables[name];
            return variable.DataType != dataType && variable.SendEvents == true;
        }

        protected void AddParameterArgument (Argument argument)
        {
            if (arguments.ContainsKey (argument.Name)) {
                throw new UpnpException (String.Format ("The action '{0}' has multiple arguments named '{1}'.", name, argument.Name));
            }
            arguments.Add (argument.Name, argument);
        }

        protected void SetReturnArgument (Argument argument)
        {
            if (return_argument != null) {
                throw new UpnpException ("A return value has already been set.");
            }
            return_argument = argument;
        }

        public string Name {
            get { return name; }
        }

        public Service Service {
            get { return service; }
        }

        protected internal void Serialize (XmlWriter writer)
        {
            writer.WriteStartElement ("action");
            writer.WriteStartElement ("name");
            writer.WriteValue (name);
            writer.WriteEndElement ();
            writer.WriteStartElement ("argumentList");
            foreach (Argument argument in arguments.Values) {
                argument.Serialize (writer);
            }
            writer.WriteEndElement ();
            writer.WriteEndElement ();
        }
	}
}
