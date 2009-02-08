
using System;

namespace Mono.Upnp.DidlLite
{
	[AttributeUsage (AttributeTargets.Class)]
	public sealed class ClassNameAttribute : Attribute
	{
		readonly string fully_qualified_class_name;
		
		public ClassNameAttribute(string fullyQualifiedClassName)
		{
			fully_qualified_class_name = fullyQualifiedClassName;
		}
		
		public string FullyQualifiedClassName { get { return fully_qualified_class_name; } }
	}
}
