// 
// ServiceDescriptionTests.cs
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
using NUnit.Framework;

using Mono.Upnp.Tests;
using Mono.Upnp.Xml;

namespace Mono.Upnp.Control.Tests
{
    [TestFixture]
    public class ServiceDescriptionTests
    {
        readonly XmlSerializer serializer = new XmlSerializer ();
        readonly DummyDeserializer deserializer = new DummyDeserializer ();
        
        [Test]
        public void FullScpdDeserializationTest ()
        {
            var controller = deserializer.DeserializeServiceController (Mono.Upnp.Tests.Xml.FullScpd);
            //Assert.AreEqual (1, controller.SpecVersion.Major);
            //Assert.AreEqual (1, controller.SpecVersion.Minor);
            Assert.AreEqual (2, controller.Actions.Count);
            var action = controller.Actions["Browse"];
            Assert.AreEqual ("Browse", action.Name);
            Assert.AreEqual (5, action.Arguments.Count);
            var argument = action.Arguments["browseFlag"];
            Assert.AreEqual ("browseFlag", argument.Name);
            Assert.AreEqual (ArgumentDirection.In, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_BrowseFlag", argument.RelatedStateVariableName);
            argument = action.Arguments["offset"];
            Assert.AreEqual ("offset", argument.Name);
            Assert.AreEqual (ArgumentDirection.In, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_Offset", argument.RelatedStateVariableName);
            argument = action.Arguments["requestCount"];
            Assert.AreEqual ("requestCount", argument.Name);
            Assert.AreEqual (ArgumentDirection.In, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_RequestCount", argument.RelatedStateVariableName);
            argument = action.Arguments["resultCount"];
            Assert.AreEqual ("resultCount", argument.Name);
            Assert.AreEqual (ArgumentDirection.Out, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_ResultCount", argument.RelatedStateVariableName);
            argument = action.Arguments["result"];
            Assert.AreEqual ("result", argument.Name);
            Assert.AreEqual (ArgumentDirection.Out, argument.Direction);
            Assert.IsTrue (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_Result", argument.RelatedStateVariableName);
            action = controller.Actions["Search"];
            Assert.AreEqual ("Search", action.Name);
            Assert.AreEqual (5, action.Arguments.Count);
            argument = action.Arguments["searchFlag"];
            Assert.AreEqual ("searchFlag", argument.Name);
            Assert.AreEqual (ArgumentDirection.In, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_SearchFlag", argument.RelatedStateVariableName);
            argument = action.Arguments["offset"];
            Assert.AreEqual ("offset", argument.Name);
            Assert.AreEqual (ArgumentDirection.In, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_Offset", argument.RelatedStateVariableName);
            argument = action.Arguments["requestCount"];
            Assert.AreEqual ("requestCount", argument.Name);
            Assert.AreEqual (ArgumentDirection.In, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_RequestCount", argument.RelatedStateVariableName);
            argument = action.Arguments["resultCount"];
            Assert.AreEqual ("resultCount", argument.Name);
            Assert.AreEqual (ArgumentDirection.Out, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_ResultCount", argument.RelatedStateVariableName);
            argument = action.Arguments["result"];
            Assert.AreEqual ("result", argument.Name);
            Assert.AreEqual (ArgumentDirection.Out, argument.Direction);
            Assert.IsTrue (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_Result", argument.RelatedStateVariableName);
            Assert.AreEqual (7, controller.StateVariables.Count);
            var state_variable = controller.StateVariables["A_ARG_TYPE_BrowseFlag"];
            Assert.AreEqual ("A_ARG_TYPE_BrowseFlag", state_variable.Name);
            Assert.AreEqual ("string", state_variable.DataType);
            var allowed_values = state_variable.AllowedValues.GetEnumerator ();
            Assert.IsTrue (allowed_values.MoveNext ());
            Assert.AreEqual ("BrowseMetadata", allowed_values.Current);
            Assert.IsTrue (allowed_values.MoveNext ());
            Assert.AreEqual ("BrowseObjects", allowed_values.Current);
            Assert.IsFalse (allowed_values.MoveNext ());
            state_variable = controller.StateVariables["A_ARG_TYPE_SearchFlag"];
            Assert.AreEqual ("A_ARG_TYPE_SearchFlag", state_variable.Name);
            Assert.AreEqual ("string", state_variable.DataType);
            allowed_values = state_variable.AllowedValues.GetEnumerator ();
            Assert.IsTrue (allowed_values.MoveNext ());
            Assert.AreEqual ("SearchMetadata", allowed_values.Current);
            Assert.IsTrue (allowed_values.MoveNext ());
            Assert.AreEqual ("SearchObjects", allowed_values.Current);
            Assert.IsFalse (allowed_values.MoveNext ());
            state_variable = controller.StateVariables["A_ARG_TYPE_Offset"];
            Assert.AreEqual ("A_ARG_TYPE_Offset", state_variable.Name);
            Assert.AreEqual ("ui4", state_variable.DataType);
            state_variable = controller.StateVariables["A_ARG_TYPE_RequestCount"];
            Assert.AreEqual ("A_ARG_TYPE_RequestCount", state_variable.Name);
            Assert.AreEqual ("ui4", state_variable.DataType);
            Assert.AreEqual ("50", state_variable.DefaultValue);
            Assert.AreEqual ("1", state_variable.AllowedValueRange.Minimum);
            Assert.AreEqual ("100", state_variable.AllowedValueRange.Maximum);
            state_variable = controller.StateVariables["A_ARG_TYPE_ResultCount"];
            Assert.AreEqual ("A_ARG_TYPE_ResultCount", state_variable.Name);
            Assert.AreEqual ("ui4", state_variable.DataType);
            state_variable = controller.StateVariables["A_ARG_TYPE_Result"];
            Assert.AreEqual ("A_ARG_TYPE_Result", state_variable.Name);
            Assert.AreEqual ("string", state_variable.DataType);
            state_variable = controller.StateVariables["SystemId"];
            Assert.AreEqual ("SystemId", state_variable.Name);
            Assert.IsTrue (state_variable.SendsEvents);
            Assert.AreEqual ("ui4", state_variable.DataType);
        }
    }
}
