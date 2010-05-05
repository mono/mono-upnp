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

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Control
{
    [XmlType ("action")]
    public class ServiceAction : Description, IMappable<string>, IXmlDeserializer<Argument>
    {
        readonly static Dictionary<string, string> emptyArguments = new Dictionary<string, string> ();
        
        readonly ServiceController controller;
        readonly CollectionMap<string, Argument> arguments;
        readonly ServiceActionExecutor executor;

        protected internal ServiceAction (Deserializer deserializer, ServiceController controller)
            : base (deserializer)
        {
            if (controller == null) throw new ArgumentNullException ("controller");

            this.controller = controller;
            arguments = new CollectionMap<string, Argument> ();
        }
        
        public ServiceAction (string name, IEnumerable<Argument> arguments, ServiceActionExecutor executor)
            : this (arguments, executor)
        {
            Name = name;
        }
        
        protected ServiceAction (IEnumerable<Argument> arguments, ServiceActionExecutor executor)
        {
            if (executor == null) throw new ArgumentNullException ("executor");
            
            this.arguments = Helper.MakeReadOnlyCopy<string, Argument> (arguments);
            this.executor = executor;
        }

        [XmlElement ("name")]
        public virtual string Name { get; protected set; }
        
        [XmlArray ("argumentList")]
        protected virtual ICollection<Argument> ArgumentList {
            get { return arguments; }
        }
        
        public IMap<string, Argument> Arguments {
            get { return arguments; }
        }
        
        public IMap<string, string> Invoke ()
        {
            return Invoke (0);
        }

        public IMap<string, string> Invoke (int retryAttempts)
        {
            return Invoke (emptyArguments, retryAttempts);
        }
        
        public IMap<string, string> Invoke (IDictionary<string, string> arguments)
        {
            return Invoke (arguments, 0);
        }

        public IMap<string, string> Invoke (IDictionary<string, string> arguments, int retryAttempts)
        {
            VerifyArguments (arguments);
            //CheckDisposed ();
            return InvokeCore (arguments, retryAttempts);
        }

        protected virtual IMap<string, string> InvokeCore (IDictionary<string, string> arguments, int retryAttempts)
        {
            if (arguments == null) throw new ArgumentNullException ("arguments");
            
            return controller.Invoke (this, arguments, retryAttempts);
        }
        
        protected internal virtual IDictionary<string, string> Execute (IDictionary<string, string> arguments)
        {
            if (executor == null) throw new InvalidOperationException ("This ServiceAction was create for deserialization and cannot be executed locally. Use the Invoke method to invoke the action across the network.");
            
            return executor (arguments);
        }

        void VerifyArguments (IDictionary<string, string> arguments)
        {
            foreach (var pair in arguments) {
                if (!Arguments.ContainsKey (pair.Key) || Arguments[pair.Key].Direction != ArgumentDirection.In) {
                    throw new ArgumentException ("This action does not have an in argument called {0}.", pair.Key);
                }
                //VerifyArgumentValue (Arguments[pair.Key], pair.Value);
            }
        }

//        void VerifyArgumentValue (Argument argument, string value)
//        {
//            if (argument.RelatedStateVariable == null) {
//                return;
//            }
//            
//            var type = argument.RelatedStateVariable.Type;
//            var values = argument.RelatedStateVariable.AllowedValues;
//            if (values != null && type == typeof (string) && !values.Contains (value)) {
//                throw new ArgumentException (
//                    string.Format ("The value {0} is not allowed for the argument {1}.", value, argument.Name));
//            }
//            
//            var range = argument.RelatedStateVariable.AllowedValueRange;
//            if (range != null && type is IComparable) {
//                var parse = type.GetMethod ("Parse", BindingFlags.Public | BindingFlags.Static);
//                var arg = parse.Invoke (null, new object[] { value });
//                if (range.Min == null) {
//                    range.Min = (IComparable)parse.Invoke (null, new object[] { range.Minimum });
//                    range.Max = (IComparable)parse.Invoke (null, new object[] { range.Maximum });
//                }
//                if (range.Min.CompareTo (arg) > 0) {
//                    throw new ArgumentOutOfRangeException (argument.Name, value, string.Format (
//                        "The value is less than {0}.", range.Minimum));
//                } else if (range.Max.CompareTo (arg) < 0) {
//                    throw new ArgumentOutOfRangeException (argument.Name, value, string.Format (
//                        "The value is greater than {0}.", range.Maximum));
//                }
//            }
//        }
        
        Argument IXmlDeserializer<Argument>.Deserialize (XmlDeserializationContext context)
        {
            return DeserializeArgument (context);
        }
        
        protected virtual Argument DeserializeArgument (XmlDeserializationContext context)
        {
            return Deserializer != null ? Deserializer.DeserializeArgument (context) : null;
        }
        
        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoDeserializeElement (this);
        }

        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoSerializeObjectAndMembers (this);
        }

        protected override void SerializeMembersOnly (Mono.Upnp.Xml.XmlSerializationContext context)
        {
            if (context == null) throw new ArgumentNullException ("context");
            
            context.AutoSerializeMembersOnly (this);
        }
        
        string IMappable<string>.Map ()
        {
            return Name;
        }
    }
}
