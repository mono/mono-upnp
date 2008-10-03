using System.IO;
using System.Xml;

namespace Mono.Upnp.Dcp.Sharpener
{
	public class RunnerContext
	{
        private readonly string type;
        private readonly string class_name;
        private readonly string @namespace;
        private readonly XmlReader reader;

        public RunnerContext (string type, string class_name, string @namespace, string path)
            : this (type, class_name, @namespace, XmlReader.Create (new StreamReader (path)))
        {
        }

        public RunnerContext (string type, string className, string @namespace, XmlReader reader)
        {
            this.type = type;
            this.class_name = className;
            this.@namespace = @namespace;
            this.reader = reader;
        }

        public string Type {
            get { return type; }
        }

        public string ClassName {
            get { return class_name; }
        }

        public string Namespace {
            get { return @namespace; }
        }

        public XmlReader Reader {
            get { return reader; }
        }
	}
}
