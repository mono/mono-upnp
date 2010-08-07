// 
// Container.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Mono.Upnp.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    [XmlType ("container", Schemas.DidlLiteSchema)]
    public class Container : Object, IXmlDeserializable
    {
        protected Container ()
        {
            SearchClasses = new List<ClassReference> ();
            CreateClasses = new List<ClassReference> ();
        }

        public Container (string id, string parentId, ContainerOptions options)
            : base (id, parentId, options)
        {
            ChildCount = options.ChildCount;
            IsSearchable = options.IsSearchable;
            SearchClasses = Helper.MakeReadOnlyCopy (options.SearchClasses);
            CreateClasses = Helper.MakeReadOnlyCopy (options.CreateClasses);
        }

        protected void CopyToOptions (ContainerOptions options)
        {
            base.CopyToOptions (options);

            options.ChildCount = ChildCount;
            options.IsSearchable = IsSearchable;
            options.SearchClasses = new List<ClassReference> (SearchClasses);
            options.CreateClasses = new List<ClassReference> (CreateClasses);
        }

        public new ContainerOptions GetOptions ()
        {
            var options = new ContainerOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlAttribute ("childCount", OmitIfNull = true)]
        public virtual int? ChildCount { get; protected set; }
        
        [XmlAttribute ("searchable", OmitIfNull = true)]
        protected virtual string Searchable {
            get { return IsSearchable ? "true" : null; }
            set { IsSearchable = value == "true"; }
        }
        
        public bool IsSearchable { get; protected set; }

        [XmlArrayItem ("createClass", Schemas.UpnpSchema)]
        public virtual IList<ClassReference> CreateClasses { get; private set; }

        [XmlArrayItem ("searchClass", Schemas.UpnpSchema)]
        public virtual IList<ClassReference> SearchClasses { get; private set; }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            base.Deserialize (context);

            SearchClasses = new ReadOnlyCollection<ClassReference> (SearchClasses);
            CreateClasses = new ReadOnlyCollection<ClassReference> (CreateClasses);
        }
        
        protected override void DeserializeAttribute (XmlDeserializationContext context)
        {
            context.AutoDeserializeAttribute (this);
        }

        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            context.AutoDeserializeElement (this);
        }
        
        protected override void SerializeSelfAndMembers (XmlSerializationContext context)
        {
            context.AutoSerializeObjectAndMembers (this);
        }
        
        protected override void SerializeMembersOnly (XmlSerializationContext context)
        {
            context.AutoSerializeMembersOnly (this);
        }
    }
}
