
using System;
using System.Collections.Generic;

namespace Mono.Upnp.Control
{
    public sealed class ActionResult
    {
        readonly string return_value;
        readonly IDictionary<string, string> out_arguments;

        public ActionResult (string returnValue, IDictionary<string, string> outArguments)
        {
            if (outArguments == null) throw new ArgumentNullException ();
            
            return_value = returnValue;
            out_arguments = new Dictionary<string, string> ();
        }

        public string ReturnValue {
            get { return return_value; }
        }

        public IDictionary<string, string> OutValues {
            get { return out_arguments; }
        }
    }
}
