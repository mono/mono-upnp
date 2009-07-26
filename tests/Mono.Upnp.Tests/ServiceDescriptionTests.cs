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
        
        static ServiceController CreateServiceController ()
        {
            return new ServiceController (
                new[] {
                    new DummyServiceAction (
                        "Browse",
                        new[] {
                            new Argument ("browseFlag", "A_ARG_TYPE_BrowseFlag", ArgumentDirection.In),
                            new Argument ("offset", "A_ARG_TYPE_Offset", ArgumentDirection.In),
                            new Argument ("requestCount", "A_ARG_TYPE_RequestCount", ArgumentDirection.In),
                            new Argument ("resultCount", "A_ARG_TYPE_ResultCount", ArgumentDirection.Out),
                            new Argument ("result", "A_ARG_TYPE_Result", ArgumentDirection.Out, true)
                        }
                    ),
                    new DummyServiceAction (
                        "Search",
                        new[] {
                            new Argument ("searchFlag", "A_ARG_TYPE_SearchFlag", ArgumentDirection.In),
                            new Argument ("offset", "A_ARG_TYPE_Offset", ArgumentDirection.In),
                            new Argument ("requestCount", "A_ARG_TYPE_RequestCount", ArgumentDirection.In),
                            new Argument ("resultCount", "A_ARG_TYPE_ResultCount", ArgumentDirection.Out),
                            new Argument ("result", "A_ARG_TYPE_Result", ArgumentDirection.Out, true)
                        }
                    ),
                },
                new[] {
                    new StateVariable ("A_ARG_TYPE_BrowseFlag", new[] { "BrowseMetadata", "BrowseObjects" }),
                    new StateVariable ("A_ARG_TYPE_SearchFlag", new[] { "SearchMetadata", "SearchObjects" }),
                    new StateVariable ("A_ARG_TYPE_Offset", "ui4"),
                    new StateVariable ("A_ARG_TYPE_RequestCount", "ui4", new AllowedValueRange ("1", "100"), new StateVariableOptions { DefaultValue = "50" }),
                    new StateVariable ("A_ARG_TYPE_ResultCount", "ui4"),
                    new StateVariable ("A_ARG_TYPE_Result", "string"),
                    new DummyStateVariable ("SystemId", "ui4")
                }
            );
        }
        
        public static void AssertEquality (ServiceController sourceController, ServiceController targetController)
        {
            Assert.IsNotNull (targetController);
            Assert.AreEqual (sourceController.SpecVersion.Major, targetController.SpecVersion.Major);
            Assert.AreEqual (sourceController.SpecVersion.Minor, targetController.SpecVersion.Minor);
            var source_actions = sourceController.Actions.Values.GetEnumerator ();
            var target_actions = targetController.Actions.Values.GetEnumerator ();
            while (source_actions.MoveNext ()) {
                Assert.IsTrue (target_actions.MoveNext ());
                AssertEquality (source_actions.Current, target_actions.Current);
            }
            Assert.IsFalse (target_actions.MoveNext ());
            var source_state_variables = sourceController.StateVariables.Values.GetEnumerator ();
            var target_state_variables = targetController.StateVariables.Values.GetEnumerator ();
            while (source_state_variables.MoveNext ()) {
                Assert.IsTrue (target_state_variables.MoveNext ());
                AssertEquality (source_state_variables.Current, target_state_variables.Current);
            }
            Assert.IsFalse (target_state_variables.MoveNext ());
        }
        
        public static void AssertEquality (ServiceAction sourceAction, ServiceAction targetAction)
        {
            Assert.AreEqual (sourceAction.Name, targetAction.Name);
            var source_arguments = sourceAction.Arguments.Values.GetEnumerator ();
            var target_arguments = targetAction.Arguments.Values.GetEnumerator ();
            while (source_arguments.MoveNext ()) {
                Assert.IsTrue (target_arguments.MoveNext ());
                AssertEquality (source_arguments.Current, target_arguments.Current);
            }
            Assert.IsFalse (target_arguments.MoveNext ());
        }
        
        public static void AssertEquality (Argument sourceArgument, Argument targetArgument)
        {
            Assert.AreEqual (sourceArgument.Name, targetArgument.Name);
            Assert.AreEqual (sourceArgument.Direction, targetArgument.Direction);
            Assert.AreEqual (sourceArgument.IsReturnValue, targetArgument.IsReturnValue);
            Assert.AreEqual (sourceArgument.RelatedStateVariable, targetArgument.RelatedStateVariable);
        }
        
        public static void AssertEquality (StateVariable sourceStateVariable, StateVariable targetStateVariable)
        {
            Assert.AreEqual (sourceStateVariable.Name, targetStateVariable.Name);
            Assert.AreEqual (sourceStateVariable.DataType, targetStateVariable.DataType);
            Assert.AreEqual (sourceStateVariable.SendsEvents, targetStateVariable.SendsEvents);
            Assert.AreEqual (sourceStateVariable.IsMulticast, targetStateVariable.IsMulticast);
            Assert.AreEqual (sourceStateVariable.DefaultValue, targetStateVariable.DefaultValue);
            if (sourceStateVariable.AllowedValues != null) {
                var source_values = sourceStateVariable.AllowedValues.GetEnumerator ();
                var target_values = targetStateVariable.AllowedValues.GetEnumerator ();
                while (source_values.MoveNext ()) {
                    Assert.IsTrue (target_values.MoveNext ());
                    Assert.AreEqual (source_values.Current, target_values.Current);
                }
                Assert.IsFalse (target_values.MoveNext ());
            }
            if (sourceStateVariable.AllowedValueRange != null) {
                Assert.AreEqual (sourceStateVariable.AllowedValueRange.Minimum, targetStateVariable.AllowedValueRange.Minimum);
                Assert.AreEqual (sourceStateVariable.AllowedValueRange.Maximum, targetStateVariable.AllowedValueRange.Maximum);
                Assert.AreEqual (sourceStateVariable.AllowedValueRange.Step, targetStateVariable.AllowedValueRange.Step);
            }
        }
        
        [Test]
        public void OfflineFullScpdTest ()
        {
            var source_controller = CreateServiceController ();
            var target_controller = deserializer.DeserializeServiceController (serializer.GetString (source_controller));
            AssertEquality (source_controller, target_controller);
        }
        
        [Test]
        public void FullScpdDeserializationTest ()
        {
            var controller = deserializer.DeserializeServiceController (Mono.Upnp.Tests.Xml.FullScpd);
            Assert.AreEqual (1, controller.SpecVersion.Major);
            Assert.AreEqual (1, controller.SpecVersion.Minor);
            Assert.AreEqual (2, controller.Actions.Count);
            var action = controller.Actions["Browse"];
            Assert.AreEqual ("Browse", action.Name);
            Assert.AreEqual (5, action.Arguments.Count);
            var argument = action.Arguments["browseFlag"];
            Assert.AreEqual ("browseFlag", argument.Name);
            Assert.AreEqual (ArgumentDirection.In, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_BrowseFlag", argument.RelatedStateVariable);
            argument = action.Arguments["offset"];
            Assert.AreEqual ("offset", argument.Name);
            Assert.AreEqual (ArgumentDirection.In, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_Offset", argument.RelatedStateVariable);
            argument = action.Arguments["requestCount"];
            Assert.AreEqual ("requestCount", argument.Name);
            Assert.AreEqual (ArgumentDirection.In, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_RequestCount", argument.RelatedStateVariable);
            argument = action.Arguments["resultCount"];
            Assert.AreEqual ("resultCount", argument.Name);
            Assert.AreEqual (ArgumentDirection.Out, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_ResultCount", argument.RelatedStateVariable);
            argument = action.Arguments["result"];
            Assert.AreEqual ("result", argument.Name);
            Assert.AreEqual (ArgumentDirection.Out, argument.Direction);
            Assert.IsTrue (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_Result", argument.RelatedStateVariable);
            action = controller.Actions["Search"];
            Assert.AreEqual ("Search", action.Name);
            Assert.AreEqual (5, action.Arguments.Count);
            argument = action.Arguments["searchFlag"];
            Assert.AreEqual ("searchFlag", argument.Name);
            Assert.AreEqual (ArgumentDirection.In, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_SearchFlag", argument.RelatedStateVariable);
            argument = action.Arguments["offset"];
            Assert.AreEqual ("offset", argument.Name);
            Assert.AreEqual (ArgumentDirection.In, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_Offset", argument.RelatedStateVariable);
            argument = action.Arguments["requestCount"];
            Assert.AreEqual ("requestCount", argument.Name);
            Assert.AreEqual (ArgumentDirection.In, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_RequestCount", argument.RelatedStateVariable);
            argument = action.Arguments["resultCount"];
            Assert.AreEqual ("resultCount", argument.Name);
            Assert.AreEqual (ArgumentDirection.Out, argument.Direction);
            Assert.IsFalse (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_ResultCount", argument.RelatedStateVariable);
            argument = action.Arguments["result"];
            Assert.AreEqual ("result", argument.Name);
            Assert.AreEqual (ArgumentDirection.Out, argument.Direction);
            Assert.IsTrue (argument.IsReturnValue);
            Assert.AreEqual ("A_ARG_TYPE_Result", argument.RelatedStateVariable);
            Assert.AreEqual (7, controller.StateVariables.Count);
            var state_variable = controller.StateVariables["A_ARG_TYPE_BrowseFlag"];
            Assert.AreEqual ("A_ARG_TYPE_BrowseFlag", state_variable.Name);
            Assert.AreEqual ("string", state_variable.DataType);
            Assert.IsNull (state_variable.AllowedValueRange);
            Assert.IsFalse (state_variable.SendsEvents);
            Assert.IsFalse (state_variable.IsMulticast);
            Assert.IsNull (state_variable.DefaultValue);
            var allowed_values = state_variable.AllowedValues.GetEnumerator ();
            Assert.IsTrue (allowed_values.MoveNext ());
            Assert.AreEqual ("BrowseMetadata", allowed_values.Current);
            Assert.IsTrue (allowed_values.MoveNext ());
            Assert.AreEqual ("BrowseObjects", allowed_values.Current);
            Assert.IsFalse (allowed_values.MoveNext ());
            state_variable = controller.StateVariables["A_ARG_TYPE_SearchFlag"];
            Assert.AreEqual ("A_ARG_TYPE_SearchFlag", state_variable.Name);
            Assert.AreEqual ("string", state_variable.DataType);
            Assert.IsNull (state_variable.AllowedValueRange);
            Assert.IsFalse (state_variable.SendsEvents);
            Assert.IsFalse (state_variable.IsMulticast);
            Assert.IsNull (state_variable.DefaultValue);
            allowed_values = state_variable.AllowedValues.GetEnumerator ();
            Assert.IsTrue (allowed_values.MoveNext ());
            Assert.AreEqual ("SearchMetadata", allowed_values.Current);
            Assert.IsTrue (allowed_values.MoveNext ());
            Assert.AreEqual ("SearchObjects", allowed_values.Current);
            Assert.IsFalse (allowed_values.MoveNext ());
            state_variable = controller.StateVariables["A_ARG_TYPE_Offset"];
            Assert.AreEqual ("A_ARG_TYPE_Offset", state_variable.Name);
            Assert.AreEqual ("ui4", state_variable.DataType);
            Assert.IsNull (state_variable.AllowedValues);
            Assert.IsNull (state_variable.AllowedValueRange);
            Assert.IsFalse (state_variable.SendsEvents);
            Assert.IsFalse (state_variable.IsMulticast);
            Assert.IsNull (state_variable.DefaultValue);
            state_variable = controller.StateVariables["A_ARG_TYPE_RequestCount"];
            Assert.AreEqual ("A_ARG_TYPE_RequestCount", state_variable.Name);
            Assert.AreEqual ("ui4", state_variable.DataType);
            Assert.AreEqual ("50", state_variable.DefaultValue);
            Assert.AreEqual ("1", state_variable.AllowedValueRange.Minimum);
            Assert.AreEqual ("100", state_variable.AllowedValueRange.Maximum);
            Assert.IsNull (state_variable.AllowedValues);
            Assert.IsFalse (state_variable.SendsEvents);
            Assert.IsFalse (state_variable.IsMulticast);
            state_variable = controller.StateVariables["A_ARG_TYPE_ResultCount"];
            Assert.AreEqual ("A_ARG_TYPE_ResultCount", state_variable.Name);
            Assert.AreEqual ("ui4", state_variable.DataType);
            Assert.IsNull (state_variable.AllowedValues);
            Assert.IsNull (state_variable.AllowedValueRange);
            Assert.IsFalse (state_variable.SendsEvents);
            Assert.IsFalse (state_variable.IsMulticast);
            Assert.IsNull (state_variable.DefaultValue);
            state_variable = controller.StateVariables["A_ARG_TYPE_Result"];
            Assert.AreEqual ("A_ARG_TYPE_Result", state_variable.Name);
            Assert.AreEqual ("string", state_variable.DataType);
            Assert.IsNull (state_variable.AllowedValues);
            Assert.IsNull (state_variable.AllowedValueRange);
            Assert.IsFalse (state_variable.SendsEvents);
            Assert.IsFalse (state_variable.IsMulticast);
            Assert.IsNull (state_variable.DefaultValue);
            state_variable = controller.StateVariables["SystemId"];
            Assert.AreEqual ("SystemId", state_variable.Name);
            Assert.IsNull (state_variable.AllowedValues);
            Assert.IsNull (state_variable.AllowedValueRange);
            Assert.IsFalse (state_variable.IsMulticast);
            Assert.IsNull (state_variable.DefaultValue);
            Assert.IsTrue (state_variable.SendsEvents);
            Assert.AreEqual ("ui4", state_variable.DataType);
        }
        
        [Test]
        public void FullScpdSerializationTest ()
        {
            var controller = CreateServiceController ();
            Assert.AreEqual (Mono.Upnp.Tests.Xml.FullScpd, serializer.GetString (controller));
        }
    }
}
