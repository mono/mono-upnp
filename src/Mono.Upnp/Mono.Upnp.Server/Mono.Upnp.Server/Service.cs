using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

using Mono.Upnp.Server.Internal;

namespace Mono.Upnp.Server
{
	public abstract class Service : IDisposable
	{
        private readonly object mutex = new object ();
        private readonly string id;
        private ActionServer action_server;
        private EventServer event_server;
        private DescriptionServer description_server;
        private bool started;

        protected Service (ServiceType type, string id)
        {
            if (type == null) {
                throw new ArgumentNullException ("type");
            }
            if (String.IsNullOrEmpty (id)) {
                throw new ArgumentNullException ("id");
            }

            this.type = type;
            this.id = id;
        }

        private Dictionary<string, Action> actions = new Dictionary<string, Action> ();
        internal IDictionary<string, Action> Actions {
            get { return actions; }
        }

        private Dictionary<string, StateVariable> state_variables = new Dictionary<string, StateVariable> ();
        internal IDictionary<string, StateVariable> StateVariables {
            get { return state_variables; }
        }

        private readonly ServiceType type;
        public ServiceType Type {
            get { return type; }
        }

        protected internal virtual void Initialize (Uri baseUrl)
        {
            if (description_server != null) {
                throw new InvalidOperationException ("The service has already been initialized. Services may only be used with one device.");
            }

            Uri url = new Uri (baseUrl, String.Format ("{0}/{1}/", type.ToUrlString (), id));
            description_server = new DescriptionServer (SerializeForServiceDescription, url);
            action_server = new ActionServer (this, new Uri (url, "actions/"));
            event_server = new EventServer (this, new Uri (url, "events/"));
            ProcessActions ();
            ProcessStateVariables ();
        }

        public virtual void Start ()
        {
            lock (mutex) {
                if (started) {
                    throw new InvalidOperationException ("The service is already started.");
                }
                if (description_server == null) {
                    throw new InvalidOperationException ("The service has not been initialized or has been disposed.");
                }
                action_server.Start ();
                event_server.Start ();
                description_server.Start ();
                started = true;
            }
        }

        public virtual void Stop ()
        {
            lock (mutex) {
                if (!started) {
                    return;
                }
                action_server.Stop ();
                event_server.Stop ();
                description_server.Stop ();
                started = false;
            }
        }

        protected virtual void ProcessActions ()
        {
            foreach (MethodInfo method in GetType ().GetMethods ()) {
                ProcessAction (method);
            }
            foreach (MethodInfo method in GetType ().GetMethods (BindingFlags.Instance | BindingFlags.NonPublic)) {
                ProcessAction (method);
            }
        }

        protected virtual void ProcessAction (MethodInfo method)
        {
            string name = null;
            foreach (object attribute in method.GetCustomAttributes (true)) {
                UpnpActionAttribute action_attribute = attribute as UpnpActionAttribute;
                if (action_attribute != null) {
                    name = String.IsNullOrEmpty (action_attribute.Name) ? method.Name : action_attribute.Name;
                    break;
                }
            }
            if (name == null) {
                return;
            }

            Action action = new Action (this, name);
            AddAction (action);
            action.Initialize (method);
        }

        protected virtual void ProcessStateVariables ()
        {
            foreach (EventInfo @event in GetType ().GetEvents ()) {
                ProcessStateVariable (@event);
            }
            foreach (EventInfo @event in GetType ().GetEvents (BindingFlags.Instance | BindingFlags.NonPublic)) {
                ProcessStateVariable (@event);
            }
        }

        protected virtual void ProcessStateVariable (EventInfo @event)
        {
            string name = null;
            foreach (object attribute in @event.GetCustomAttributes (true)) {
                UpnpStateVariableAttribute state_variable_attribute = attribute as UpnpStateVariableAttribute;
                if (state_variable_attribute != null) {
                    name = state_variable_attribute.Name;
                }
            }
            if (name == null) {
                return;
            }

            StateVariable state_variable = new StateVariable (this, name);
            AddStateVariable (state_variable);
            state_variable.Initialize (@event);
        }

        protected void AddAction (Action action)
        {
            if (actions.ContainsKey (action.Name)) {
                // TODO add service type name
                throw new UpnpException (String.Format ("The service already contains an action named '{0}'.", action.Name));
            }
            actions.Add (action.Name, action);
        }

        protected void AddStateVariable (StateVariable stateVariable)
        {
            if (state_variables.ContainsKey (stateVariable.Name)) {
                // TODO add service type name
                throw new UpnpException (String.Format ("The service already contains an state variable named '{0}'.", stateVariable.Name));
            }
            state_variables.Add (stateVariable.Name, stateVariable);
        }

        internal void PublishStateVariableChange ()
        {
            event_server.PublishUpdates ();
        }

        protected internal void SerializeForDeviceDescription (XmlWriter writer)
        {
            writer.WriteStartElement ("service");
            writer.WriteStartElement ("serviceType");
            writer.WriteValue (Type.ToString ());
            writer.WriteEndElement ();
            writer.WriteStartElement ("serviceId");
            writer.WriteValue (String.Format ("urn:{0}:serviceId:{1}", Type.DomainName, id));
            writer.WriteEndElement ();
            writer.WriteStartElement ("SCPDURL");
            writer.WriteValue (description_server.Url.ToString ());
            writer.WriteEndElement ();
            writer.WriteStartElement ("controlURL");
            writer.WriteValue (action_server.Url.ToString ());
            writer.WriteEndElement ();
            writer.WriteStartElement ("eventSubURL");
            writer.WriteValue (event_server.Url);
            writer.WriteEndElement ();
            writer.WriteEndElement ();
        }

        protected internal void SerializeForServiceDescription (XmlWriter writer)
        {
            writer.WriteStartElement ("scpd", "urn:schemas-upnp-org:service-1-0");
            Helper.WriteSpecVersion (writer);

            writer.WriteStartElement ("actionList");
            foreach (Action action in actions.Values) {
                action.Serialize (writer);
            }
            writer.WriteEndElement ();
            writer.WriteStartElement ("serviceStateTable");
            foreach (StateVariable variable in state_variables.Values) {
                variable.Serialize (writer);
            }
            writer.WriteEndElement ();
            writer.WriteEndElement ();
        }

        public void Dispose ()
        {
            lock (mutex) {
                Dispose (true);
                GC.SuppressFinalize (this);
            }
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposing) {
                Stop ();
                if (action_server != null) {
                    action_server.Dispose ();
                    event_server.Dispose ();
                    description_server.Dispose ();
                }
            }
        }
    }
}
