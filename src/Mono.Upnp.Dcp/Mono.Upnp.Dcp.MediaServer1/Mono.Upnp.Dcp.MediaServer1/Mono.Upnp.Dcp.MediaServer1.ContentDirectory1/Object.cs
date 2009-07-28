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

using Mono.Upnp.Xml;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    public abstract class Object : XmlAutomatable
    {
        readonly ContentDirectory content_directory;
        readonly List<Resource> resources = new List<Resource> ();
        bool is_restricted;
        string title;
        string creator;
        Class @class;
        WriteStatus? write_status;
        
        protected Object (ContentDirectory contentDirectory, Container parent)
        {
            if (contentDirectory == null) throw new ArgumentNullException ("contentDirectory");
            
            content_directory = contentDirectory;
            Id = contentDirectory.GetNewObjectId ();
            ParentId = parent == null ? "-1" : parent.Id;
            Class = new Class (ClassManager.GetClassNameFromType (GetType ()));
        }
        
        protected void OnUpdate ()
        {
            content_directory.OnSystemUpdate ();
        }
        
        [XmlElement ("class", Schemas.UpnpSchema)]
        public virtual Class Class {
            get { return @class; }
            set { @class = value; }
        }

        [XmlAttribute ("id")]
        public virtual string Id { get; protected set; }
        
        [XmlAttribute ("parentId")]
        public virtual string ParentId { get; protected set; }
        
        [XmlAttribute ("restricted")]
        protected virtual string IsRestrictedValue {
            get { return IsRestricted ? "true" : "false"; }
            set { IsRestricted = value == "true"; }
        }
        
        public bool IsRestricted {
            get { return is_restricted; }
            set { is_restricted = value; }
        }
        
        [XmlArrayItem]
        protected virtual ICollection<Resource> ResourceCollection {
            get { return resources; }
        }
        
        public IEnumerable<Resource> Resources {
            get { return resources; }
        }
        
        [XmlElement ("title", Schemas.DublinCoreSchema)]
        public virtual string Title {
            get { return title; }
            set { title = value; }
        }
        
        [XmlElement ("creator", Schemas.DublinCoreSchema, OmitIfNull = true)]
        public virtual string Creator {
            get { return creator; }
            set { creator = value; }
        }
        
        [XmlElement ("writeStatus", Schemas.UpnpSchema, OmitIfNull = true)]
        public virtual WriteStatus? WriteStatus {
            get { return write_status; }
            set { write_status = value; }
        }
        
        internal Deserializer Deserializer { get; private set; }
        
        internal uint ParentUpdateId { get; set; }
        
        public void AddResource (Resource resource)
        {
            resources.Add (resource);
        }
        
        public bool RemoveResource (Resource resource)
        {
            return resources.Remove (resource);
        }
        
//        public Container GetParent ()
//        {
//            return ParentId == "-1" ? null : ContentDirectory.GetObject<Container> (ParentId);
//        }
//        
//        public bool CanDestroy {
//            get { return !IsRestricted && ContentDirectory.Controller.CanDestroyObject; }
//        }
//        
//        public void Destroy ()
//        {
//            ContentDirectory.Controller.DestroyObject (Id);
//        }
//        
//        public bool IsOutOfDate {
//            get { return ContentDirectory.CheckIfObjectIsOutOfDate (this); }
//        }
        
        public override string ToString ()
        {
            return string.Format("{0} ({1})", Id, Class.FullClassName);
        }
        
        protected override void DeserializeAttribute (XmlDeserializationContext context)
        {
            context.AutoDeserializeAttribute (this);
        }

        protected override void DeserializeElement (XmlDeserializationContext context)
        {
            context.AutoDeserializeElement (this);
        }
    }
}