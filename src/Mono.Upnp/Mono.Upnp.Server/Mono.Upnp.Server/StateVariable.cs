using System;
using System.Reflection;
using System.Xml;

using Mono.Upnp.Server.Internal;

namespace Mono.Upnp.Server
{
    public class StateVariable
	{
        private static readonly MethodInfo on_event = typeof (StateVariable).GetMethod ("OnEvent", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly Service service;
        private readonly string name;
        private Type data_type;
        private readonly object default_value;
        private readonly AllowedValueRange allowed_value_range;
        private readonly bool send_events;

        internal StateVariable (Service service, string name)
            : this (service, name, null, null, null, true)
        {
        }

        protected internal StateVariable (Service service, string name, Type dataType, object defaultValue, AllowedValueRange allowedValueRange, bool sendEvents)
        {
            if (service == null) {
                throw new ArgumentNullException ("service");
            }
            if (name == null) {
                throw new ArgumentNullException ("name");
            }
            if (dataType == null) {
                throw new ArgumentNullException ("dataType");
            }

            // TODO check that allowedValueRange is only used with numeric types

            this.service = service;
            this.name = name;
            this.data_type = dataType.IsByRef ? dataType.GetElementType () : dataType;
            this.default_value = defaultValue;
            this.allowed_value_range = allowedValueRange;
            this.send_events = sendEvents;
            Helper.GetDataType (data_type);
        }

        protected internal virtual void Initialize (EventInfo @event)
        {
            if (@event.EventHandlerType.GetGenericTypeDefinition () != typeof (StateVariableChangedArgs<>)) {
                throw new UpnpException (String.Format (
                    "The UPnP state variable {0} must be of the type EventHandler<StateVariableChangedArgs<>>.", name));
            }
            data_type = @event.EventHandlerType.GetGenericArguments ()[0].GetGenericArguments ()[0];
            Delegate del = Delegate.CreateDelegate (
                typeof (EventHandler<>).MakeGenericType (typeof (StateVariableChangedArgs<>).MakeGenericType (data_type)),
                on_event);
            @event.AddEventHandler (service, del);
        }

        private void OnEvent<T> (object sender, StateVariableChangedArgs<T> args)
        {
            value = args.NewValue;
            service.PublishStateVariableChange ();
        }

        public string Name {
            get { return name; }
        }

        public Type DataType {
            get { return data_type; }
        }

        public bool SendEvents {
            get { return send_events; }
        }

        public object DefaultValue {
            get { return default_value; }
        }

        private object value;
        public object Value {
            get { return value; }
        }

        protected internal void Serialize (XmlWriter writer)
        {
            writer.WriteStartElement ("stateVariable");
            writer.WriteAttributeString ("sendEvents", send_events ? "yes" : "no");
            writer.WriteStartElement ("name");
            writer.WriteValue (name);
            writer.WriteEndElement ();
            writer.WriteStartElement ("dataType");
            writer.WriteValue (Helper.GetDataType (data_type));
            writer.WriteEndElement ();
            if (default_value != null) {
                writer.WriteStartElement ("defaultValue");
                writer.WriteValue (default_value);
                writer.WriteEndElement ();
            }
            if (data_type.IsEnum) {
                writer.WriteStartElement ("allowedValueList");
                foreach (string value in Enum.GetNames (data_type)) {
                    writer.WriteStartElement ("allowedValue");
                    writer.WriteValue (value);
                    writer.WriteEndElement ();
                }
                writer.WriteEndElement ();
            } else if (allowed_value_range != null) {
                writer.WriteStartElement ("allowedValueRange");
                writer.WriteStartElement ("minimum");
                writer.WriteValue (allowed_value_range.MinValue);
                writer.WriteEndElement ();
                writer.WriteStartElement ("maximum");
                writer.WriteValue (allowed_value_range.MaxValue);
                writer.WriteEndElement ();
                writer.WriteStartElement ("step");
                writer.WriteValue (allowed_value_range.Steps);
                writer.WriteEndElement ();
                writer.WriteEndElement ();
            }
            writer.WriteEndElement ();
        }

        public override bool Equals (object obj)
        {
            StateVariable variable = obj as StateVariable;
            return variable != null &&
                variable.name == name &&
                variable.data_type == data_type &&
                variable.send_events == send_events;
        }

        public override int GetHashCode ()
        {
            return name.GetHashCode () ^ data_type.GetHashCode () ^ send_events.GetHashCode ();
        }
	}
}
