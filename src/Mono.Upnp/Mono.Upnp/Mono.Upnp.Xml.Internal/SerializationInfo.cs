// 
// SerializationInfo.cs
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

namespace Mono.Upnp.Xml.Internal
{
    delegate void Serializer<TContext> (object obj, XmlSerializationContext<TContext> context);
    
    class SerializationInfo<TContext>
    {
        readonly SerializationCompiler<TContext> compiler;
        
        Serializer<TContext> type_serializer;
        Serializer<TContext> type_auto_serializer;
        Serializer<TContext> member_serializer;
        Serializer<TContext> member_auto_serializer;
        
        public SerializationInfo (XmlSerializer<TContext> xmlSerializer, Type type)
        {
            compiler = new SerializationCompiler<TContext> (xmlSerializer, this, type);
        }
        
        public Serializer<TContext> TypeSerializer {
            get {
                if (this.type_serializer == null) {
                    Serializer<TContext> type_serializer = null;
                    this.type_serializer = (obj, context) => type_serializer (obj, context);
                    type_serializer = compiler.CreateTypeSerializer ();
                    this.type_serializer = type_serializer;
                }
                return this.type_serializer;
            }
        }
        
        public Serializer<TContext> TypeAutoSerializer {
            get {
                if (type_auto_serializer == null) {
                    type_auto_serializer = compiler.CreateTypeAutoSerializer ();
                }
                return type_auto_serializer;
            }
        }
        
        public Serializer<TContext> MemberSerializer {
            get {
                if (this.member_serializer == null) {
                    Serializer<TContext> member_serializer = null;
                    this.member_serializer = (obj, context) => member_serializer (obj, context);
                    member_serializer = compiler.CreateMemberSerializer ();
                    this.member_serializer = member_serializer;
                }
                return this.member_serializer;
            }
        }
        
        public Serializer<TContext> MemberAutoSerializer {
            get {
                if (member_auto_serializer == null) {
                    member_auto_serializer = compiler.CreateMemberAutoSerializer ();
                }
                return member_auto_serializer;
            }
        }
    }
}
