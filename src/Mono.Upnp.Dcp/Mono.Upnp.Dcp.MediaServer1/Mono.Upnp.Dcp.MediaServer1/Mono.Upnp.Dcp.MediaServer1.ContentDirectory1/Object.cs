// 
// Object.cs
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

using Mono.Upnp.Dcp.MediaServer1.Internal;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public class Object : XmlAutomatable
    {
        protected Object ()
        {
            Resources = new List<Resource> ();
        }
        
        public Object (string id, ObjectOptions options)
        {
            if (id == null) {
                throw new ArgumentNullException ("id");
            } else if (options == null) {
                throw new ArgumentNullException ("options");
            }

            Id = id;
            Title = options.Title;
            Creator = options.Creator;
            WriteStatus = options.WriteStatus;
            IsRestricted = options.IsRestricted;
            Resources = Helper.MakeReadOnlyCopy (options.Resources);
        }

        protected void CopyToOptions (ObjectOptions options)
        {
            if (options == null) {
                throw new ArgumentNullException ("options");
            }

            options.Title = Title;
            options.Creator = Creator;
            options.WriteStatus = WriteStatus;
            options.IsRestricted = IsRestricted;
            options.Resources = new List<Resource> (Resources);
        }

        public ObjectOptions GetOptions ()
        {
            var options = new ObjectOptions ();
            CopyToOptions (options);
            return options;
        }
        
        [XmlElement ("class", Schemas.UpnpSchema)]
        public virtual Class Class { get; protected set; }
        
        [XmlElement ("title", Schemas.DublinCoreSchema)]
        public virtual string Title { get; protected set; }
        
        [XmlElement ("creator", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Creator { get; protected set; }
        
        [XmlElement ("writeStatus", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual WriteStatus? WriteStatus { get; protected set; }

        [XmlAttribute ("id")]
        public virtual string Id { get; protected set; }
        
        [XmlAttribute ("parentID")]
        public virtual string ParentId { get; protected set; }
        
        [XmlAttribute ("restricted")]
        protected virtual string Restricted {
            get { return IsRestricted ? "true" : "false"; }
            set { IsRestricted = value == "true"; }
        }
        
        public bool IsRestricted { get; protected set; }
        
        [XmlArrayItem]
        public virtual IList<Resource> Resources { get; private set; }
        
        public override string ToString ()
        {
            return string.Format("{0} ({1})", Id, Class.FullClassName);
        }

        protected override void Deserialize (XmlDeserializationContext context)
        {
            base.Deserialize (context);

            Resources = new ReadOnlyCollection<Resource> (Resources);
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
