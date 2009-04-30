// 
// EventedStateVariable.cs
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
using System.Reflection;

namespace Mono.Upnp.Server.Control
{
    public class EventedStateVariable : StateVariable
    {
        readonly EventInfo event_info;
        
        protected internal EventedStateVariable (string name, EventInfo eventInfo)
            : base (name, GetEventType (eventInfo), true)
        {
            this.event_info = eventInfo;
        }
        
        public EventInfo EventInfo {
            get { return event_info; }
        }
        
        static Type GetEventType (EventInfo eventInfo)
        {
            if (eventInfo == null) throw new ArgumentNullException ("eventInfo");
            
            var type = eventInfo.EventHandlerType;
            if (!type.IsGenericType || type.GetGenericTypeDefinition () != typeof (EventHandler<>)) {
                Die ();
            }
            
            type = type.GetGenericArguments ()[0];
            if (!type.IsGenericType || type.GetGenericTypeDefinition () != typeof (StateVariableChangedArgs<>)) {
                Die ();
            }
            
            return type.GetGenericArguments ()[0];
        }
        
        static void Die ()
        {
            throw new UpnpServerException (string.Format (
                "The UPnP state variable {0} must be of the type EventHandler<StateVariableChangedArgs<>>.", name));
        }
    }
}
