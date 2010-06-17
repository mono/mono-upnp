// 
// ServiceTests.cs
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

using Mono.Upnp.Control;

namespace Mono.Upnp.Tests
{
    [TestFixture]
    public class ServiceTests
    {
        class ActionTestClass
        {
            [UpnpAction]
            public void Foo ()
            {
            }
        }
        
        [Test]
        public void ActionTest ()
        {
            var service = new DummyService<ActionTestClass> ();
            var controller = new ServiceController (new[] { new DummyServiceAction ("Foo") }, null);
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class ActionNameTestClass
        {
            [UpnpAction ("foo")]
            public void Foo ()
            {
            }
        }
        
        [Test]
        public void ActionNameTest ()
        {
            var service = new DummyService<ActionNameTestClass> ();
            var controller = new ServiceController (new[] { new DummyServiceAction ("foo") }, null);
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class ArgumentTestClass
        {
            [UpnpAction]
            public void Foo (string bar)
            {
            }
        }
        
        [Test]
        public void ArgumentTest ()
        {
            var service = new DummyService<ArgumentTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class OutArgumentTestClass
        {
            [UpnpAction]
            public void Foo (out string bar)
            {
                bar = string.Empty;
            }
        }
        
        [Test]
        public void OutArgumentTest ()
        {
            var service = new DummyService<OutArgumentTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.Out) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class RefArgumentTestClass
        {
            [UpnpAction]
            public void Foo (ref string bar)
            {
            }
        }
        
        [Test]
        public void RefArgumentTest ()
        {
            var service = new DummyService<RefArgumentTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.Out) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class ReturnArgumentTestClass
        {
            [UpnpAction]
            public string Foo ()
            {
                return string.Empty;
            }
        }
        
        [Test]
        public void ReturnArgumentTest ()
        {
            var service = new DummyService<ReturnArgumentTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("result", "A_ARG_result", ArgumentDirection.Out, true) })
                },
                new[] {
                    new StateVariable ("A_ARG_result", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class ArgumentNameTestClass
        {
            [UpnpAction]
            public void Foo ([UpnpArgument ("Bar")]string bar)
            {
            }
        }
        
        [Test]
        public void ArgumentNameTest ()
        {
            var service = new DummyService<ArgumentNameTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("Bar", "A_ARG_Bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_Bar", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class ReturnArgumentNameTestClass
        {
            [UpnpAction]
            [return: UpnpArgument ("foo")]
            public string Foo ()
            {
                return null;
            }
        }
        
        [Test]
        public void ReturnArgumentNameTest ()
        {
            var service = new DummyService<ReturnArgumentNameTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("foo", "A_ARG_foo", ArgumentDirection.Out, true) })
                },
                new[] {
                    new StateVariable ("A_ARG_foo", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class RelatedStateVariableNameTestClass
        {
            [UpnpAction]
            public void Foo ([UpnpRelatedStateVariable ("X_ARG_bar")]string bar)
            {
            }
        }
        
        [Test]
        public void RelatedStateVariableNameTest ()
        {
            var service = new DummyService<RelatedStateVariableNameTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "X_ARG_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("X_ARG_bar", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class ReturnArgumentRelatedStateVariableNameTestClass
        {
            [UpnpAction]
            [return: UpnpRelatedStateVariable ("X_ARG_foo")]
            public string Foo ()
            {
                return null;
            }
        }
        
        [Test]
        public void ReturnArgumentRelatedStateVariableNameTest ()
        {
            var service = new DummyService<ReturnArgumentRelatedStateVariableNameTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("result", "X_ARG_foo", ArgumentDirection.Out, true) })
                },
                new[] {
                    new StateVariable ("X_ARG_foo", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class RelatedStateVariableDataTypeTestClass
        {
            [UpnpAction]
            public void Foo ([UpnpRelatedStateVariable (DataType = "string.foo")]string bar)
            {
            }
        }
        
        [Test]
        public void RelatedStateVariableDataTypeTest ()
        {
            var service = new DummyService<RelatedStateVariableDataTypeTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", "string.foo")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class RelatedStateVariableDefaultValueTestClass
        {
            [UpnpAction]
            public void Foo ([UpnpRelatedStateVariable (DefaultValue = "hey")]string bar)
            {
            }
        }
        
        [Test]
        public void RelatedStateVariableDefaultValueTest ()
        {
            var service = new DummyService<RelatedStateVariableDefaultValueTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", "string", new StateVariableOptions { DefaultValue = "hey" })
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class RelatedStateVariableAllowedValueRangeTestClass
        {
            [UpnpAction]
            public void Foo ([UpnpRelatedStateVariable ("0", "100", StepValue = "2")]int bar)
            {
            }
        }
        
        [Test]
        public void RelatedStateVariableAllowedValueRangeTest ()
        {
            var service = new DummyService<RelatedStateVariableAllowedValueRangeTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", "i4", new AllowedValueRange ("0", "100", "2"))
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        enum EnumArgumentTestEnum
        {
            Foo,
            Bar
        }
        
        class EnumArgumentTestClass
        {
            [UpnpAction]
            public void Foo (EnumArgumentTestEnum bar)
            {
            }
        }
        
        [Test]
        public void EnumArgumentTest ()
        {
            var service = new DummyService<EnumArgumentTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", new[] { "Foo", "Bar" })
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        enum EnumArgumentNameTestEnum
        {
            [UpnpEnum ("foo")] Foo,
            [UpnpEnum ("bar")] Bar
        }
        
        class EnumArgumentNameTestClass
        {
            [UpnpAction]
            public void Foo (EnumArgumentNameTestEnum bar)
            {
            }
        }
        
        [Test]
        public void EnumArgumentNameTest ()
        {
            var service = new DummyService<EnumArgumentNameTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", new[] { "foo", "bar" })
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class ArgumentNameAgreementTestClass
        {
            [UpnpAction]
            public void Foo (string bar)
            {
            }
            
            [UpnpAction]
            public void Bar (string bar)
            {
            }
        }
        
        [Test]
        public void ArgumentNameAgreementTest ()
        {
            var service = new DummyService<ArgumentNameAgreementTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) }),
                    new DummyServiceAction ("Bar", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class ArgumentTypeConflictTestClass
        {
            [UpnpAction]
            public void Foo (string bar)
            {
            }
            
            [UpnpAction]
            public void Bar (int bar)
            {
            }
        }
        
        [Test]
        public void ArgumentTypeConflictTest ()
        {
            var service = new DummyService<ArgumentTypeConflictTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) }),
                    new DummyServiceAction ("Bar", new[] { new Argument ("bar", "A_ARG_Bar_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", "string"),
                    new StateVariable ("A_ARG_Bar_bar", "i4")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class RelatedStateVariableAllowedValueRangeConflictTest1Class
        {
            [UpnpAction]
            public void Foo (int bar)
            {
            }
            
            [UpnpAction]
            public void Bar ([UpnpRelatedStateVariable("0", "100")] int bar)
            {
            }
        }
        
        [Test]
        public void RelatedStateVariableAllowedValueRangeConflictTest1 ()
        {
            var service = new DummyService<RelatedStateVariableAllowedValueRangeConflictTest1Class> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) }),
                    new DummyServiceAction ("Bar", new[] { new Argument ("bar", "A_ARG_Bar_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", "i4"),
                    new StateVariable ("A_ARG_Bar_bar", "i4", new AllowedValueRange ("0", "100"))
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class RelatedStateVariableAllowedValueRangeConflictTest2Class
        {
            [UpnpAction]
            public void Foo ([UpnpRelatedStateVariable("0", "100")] int bar)
            {
            }
            
            [UpnpAction]
            public void Bar (int bar)
            {
            }
        }
        
        [Test]
        public void RelatedStateVariableAllowedValueRangeConflictTest2 ()
        {
            var service = new DummyService<RelatedStateVariableAllowedValueRangeConflictTest2Class> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) }),
                    new DummyServiceAction ("Bar", new[] { new Argument ("bar", "A_ARG_Bar_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", "i4", new AllowedValueRange ("0", "100")),
                    new StateVariable ("A_ARG_Bar_bar", "i4")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class RelatedStateVariableAllowedValueRangeConflictTest3Class
        {
            [UpnpAction]
            public void Foo ([UpnpRelatedStateVariable("0", "100")] int bar)
            {
            }
            
            [UpnpAction]
            public void Bar ([UpnpRelatedStateVariable("0", "101")] int bar)
            {
            }
        }
        
        [Test]
        public void RelatedStateVariableAllowedValueRangeConflictTest3 ()
        {
            var service = new DummyService<RelatedStateVariableAllowedValueRangeConflictTest3Class> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) }),
                    new DummyServiceAction ("Bar", new[] { new Argument ("bar", "A_ARG_Bar_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", "i4", new AllowedValueRange ("0", "100")),
                    new StateVariable ("A_ARG_Bar_bar", "i4", new AllowedValueRange ("0", "101"))
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class RelatedStateVariableAllowedValuesConflictTest1Class
        {
            [UpnpAction]
            public void Foo (string bar)
            {
            }
            
            [UpnpAction]
            public void Bar (EnumArgumentTestEnum bar)
            {
            }
        }
        
        [Test]
        public void RelatedStateVariableAllowedValuesConflictTest1 ()
        {
            var service = new DummyService<RelatedStateVariableAllowedValuesConflictTest1Class> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) }),
                    new DummyServiceAction ("Bar", new[] { new Argument ("bar", "A_ARG_Bar_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", "string"),
                    new StateVariable ("A_ARG_Bar_bar", new[] { "Foo", "Bar" })
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class RelatedStateVariableAllowedValuesConflictTest2Class
        {
            [UpnpAction]
            public void Foo (EnumArgumentTestEnum bar)
            {
            }
            
            [UpnpAction]
            public void Bar (string bar)
            {
            }
        }
        
        [Test]
        public void RelatedStateVariableAllowedValuesConflictTest2 ()
        {
            var service = new DummyService<RelatedStateVariableAllowedValuesConflictTest2Class> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) }),
                    new DummyServiceAction ("Bar", new[] { new Argument ("bar", "A_ARG_Bar_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", new[] { "Foo", "Bar" }),
                    new StateVariable ("A_ARG_Bar_bar", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class RelatedStateVariableAllowedValuesConflictTest3Class
        {
            [UpnpAction]
            public void Foo (EnumArgumentTestEnum bar)
            {
            }
            
            [UpnpAction]
            public void Bar (EnumArgumentNameTestEnum bar)
            {
            }
        }
        
        [Test]
        public void RelatedStateVariableAllowedValuesConflictTest3 ()
        {
            var service = new DummyService<RelatedStateVariableAllowedValuesConflictTest3Class> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) }),
                    new DummyServiceAction ("Bar", new[] { new Argument ("bar", "A_ARG_Bar_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_bar", new[] { "Foo", "Bar" }),
                    new StateVariable ("A_ARG_Bar_bar", new[] { "foo", "bar" })
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
                
        class ArgumentsTestClass
        {
            [UpnpAction]
            public void Foo (string stringArg, int intArg, byte byteArg, ushort ushortArg, uint uintArg, sbyte sbyteArg,
                             short shortArg, long longArg, float floatArg, double doubleArg, char charArg,
                             DateTime dateTimeArg, bool boolArg, byte[] byteArrayArg, Uri uriArg)
            {
            }
        }
        
        [Test]
        public void ArgumentsTest ()
        {
            var service = new DummyService<ArgumentsTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new[] {
                        new Argument ("stringArg", "A_ARG_stringArg", ArgumentDirection.In),
                        new Argument ("intArg", "A_ARG_intArg", ArgumentDirection.In),
                        new Argument ("byteArg", "A_ARG_byteArg", ArgumentDirection.In),
                        new Argument ("ushortArg", "A_ARG_ushortArg", ArgumentDirection.In),
                        new Argument ("uintArg", "A_ARG_uintArg", ArgumentDirection.In),
                        new Argument ("sbyteArg", "A_ARG_sbyteArg", ArgumentDirection.In),
                        new Argument ("shortArg", "A_ARG_shortArg", ArgumentDirection.In),
                        new Argument ("longArg", "A_ARG_longArg", ArgumentDirection.In),
                        new Argument ("floatArg", "A_ARG_floatArg", ArgumentDirection.In),
                        new Argument ("doubleArg", "A_ARG_doubleArg", ArgumentDirection.In),
                        new Argument ("charArg", "A_ARG_charArg", ArgumentDirection.In),
                        new Argument ("dateTimeArg", "A_ARG_dateTimeArg", ArgumentDirection.In),
                        new Argument ("boolArg", "A_ARG_boolArg", ArgumentDirection.In),
                        new Argument ("byteArrayArg", "A_ARG_byteArrayArg", ArgumentDirection.In),
                        new Argument ("uriArg", "A_ARG_uriArg", ArgumentDirection.In)
                    })
                },
                new[] {
                    new StateVariable ("A_ARG_stringArg", "string"),
                    new StateVariable ("A_ARG_intArg", "i4"),
                    new StateVariable ("A_ARG_byteArg", "ui1"),
                    new StateVariable ("A_ARG_ushortArg", "ui2"),
                    new StateVariable ("A_ARG_uintArg", "ui4"),
                    new StateVariable ("A_ARG_sbyteArg", "i1"),
                    new StateVariable ("A_ARG_shortArg", "i2"),
                    new StateVariable ("A_ARG_longArg", "int"),
                    new StateVariable ("A_ARG_floatArg", "r4"),
                    new StateVariable ("A_ARG_doubleArg", "r8"),
                    new StateVariable ("A_ARG_charArg", "char"),
                    new StateVariable ("A_ARG_dateTimeArg", "date"),
                    new StateVariable ("A_ARG_boolArg", "boolean"),
                    new StateVariable ("A_ARG_byteArrayArg", "bin"),
                    new StateVariable ("A_ARG_uriArg", "uri")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class StateVariableTestClass
        {
            [UpnpStateVariable]
            public event EventHandler<StateVariableChangedArgs<string>> FooChanged;
        }
        
        [Test]
        public void StateVariableTest ()
        {
            var service = new DummyService<StateVariableTestClass> ();
            var controller = new ServiceController (null,
                new[] {
                    new DummyStateVariable ("FooChanged", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class StateVariableNameTestClass
        {
            [UpnpStateVariable ("fooChanged")]
            public event EventHandler<StateVariableChangedArgs<string>> FooChanged;
        }
        
        [Test]
        public void StateVariableNameTest ()
        {
            var service = new DummyService<StateVariableNameTestClass> ();
            var controller = new ServiceController (null,
                new[] {
                    new DummyStateVariable ("fooChanged", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class StateVariableDataTypeTestClass
        {
            [UpnpStateVariable (DataType = "string.foo")]
            public event EventHandler<StateVariableChangedArgs<string>> FooChanged;
        }
        
        [Test]
        public void StateVariableDataTypeTest ()
        {
            var service = new DummyService<StateVariableDataTypeTestClass> ();
            var controller = new ServiceController (null,
                new[] {
                    new DummyStateVariable ("FooChanged", "string.foo")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class StateVariableIsMulticastTestClass
        {
            [UpnpStateVariable (IsMulticast = true)]
            public event EventHandler<StateVariableChangedArgs<string>> FooChanged;
        }
        
        [Test]
        public void StateVariableIsMulticastTest ()
        {
            var service = new DummyService<StateVariableIsMulticastTestClass> ();
            var controller = new ServiceController (null,
                new[] {
                    new DummyStateVariable ("FooChanged", "string", true)
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class StateVariablesTestClass
        {
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<string>> StringChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<int>> IntChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<byte>> ByteChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<ushort>> UshortChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<uint>> UintChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<sbyte>> SbyteChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<short>> ShortChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<long>> LongChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<float>> FloatChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<double>> DoubleChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<char>> CharChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<bool>> BoolChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<byte[]>> ByteArrayChanged;
            [UpnpStateVariable] public event EventHandler<StateVariableChangedArgs<Uri>> UriChanged;
        }
        
        [Test]
        public void StateVariablesTest ()
        {
            var service = new DummyService<StateVariablesTestClass> ();
            var controller = new ServiceController (null,
                new[] {
                    new DummyStateVariable ("StringChanged", "string"),
                    new DummyStateVariable ("IntChanged", "i4"),
                    new DummyStateVariable ("ByteChanged", "ui1"),
                    new DummyStateVariable ("UshortChanged", "ui2"),
                    new DummyStateVariable ("UintChanged", "ui4"),
                    new DummyStateVariable ("SbyteChanged", "i1"),
                    new DummyStateVariable ("ShortChanged", "i2"),
                    new DummyStateVariable ("LongChanged", "int"),
                    new DummyStateVariable ("FloatChanged", "r4"),
                    new DummyStateVariable ("DoubleChanged", "r8"),
                    new DummyStateVariable ("CharChanged", "char"),
                    new DummyStateVariable ("BoolChanged", "boolean"),
                    new DummyStateVariable ("ByteArrayChanged", "bin"),
                    new DummyStateVariable ("UriChanged", "uri")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class EventedRelatedStateVariableTestClass
        {
            [UpnpAction]
            public void GetFoo ([UpnpRelatedStateVariable ("Foo")] out string foo)
            {
                foo = null;
            }
            
            [UpnpStateVariable ("Foo")]
            public event EventHandler<StateVariableChangedArgs<string>> FooChanged;
        }
        
        [Test]
        public void EventedRelatedStateVariableTest ()
        {
            var service = new DummyService<EventedRelatedStateVariableTestClass> ();
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("GetFoo", new[] { new Argument ("foo", "Foo", ArgumentDirection.Out) })
                },
                new[] {
                    new DummyStateVariable ("Foo", "string")
                }
            );
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class OptionalActionClass
        {
            [UpnpAction (OmitUnless = "CanFoo")]
            public void Foo (string foo)
            {
            }
            
            [UpnpAction]
            public void Bar (string bar)
            {
            }
            
            public bool CanFoo { get; set; }
        }
        
        [Test]
        public void UnimplementedOptionalAction ()
        {
            var service = new DummyService<OptionalActionClass> ();
            var controller = new ServiceController (
                new[] { new DummyServiceAction ("Bar", new [] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) }) },
                new[] { new StateVariable ("A_ARG_bar", "string") });
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        [Test]
        public void ImplementedOptionalAction ()
        {
            var service = new DummyService<OptionalActionClass> (new OptionalActionClass { CanFoo = true });
            var controller = new ServiceController (
                new[] {
                    new DummyServiceAction ("Foo", new [] { new Argument ("foo", "A_ARG_foo", ArgumentDirection.In) }),
                    new DummyServiceAction ("Bar", new [] { new Argument ("bar", "A_ARG_bar", ArgumentDirection.In) })
                },
                new[] {
                    new StateVariable ("A_ARG_foo", "string"),
                    new StateVariable ("A_ARG_bar", "string") });
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class ErroneousOptionalActionClass
        {
            [UpnpAction (OmitUnless = "CanFoo")]
            public void Foo ()
            {
            }
        }
        
        [Test, ExpectedException (typeof (UpnpServiceDefinitionException))]
        public void ErroneousOptionalAction ()
        {
            new DummyService<ErroneousOptionalActionClass> ();
        }
        
        class OptionalStateVariablesClass
        {
            [UpnpStateVariable (OmitUnless = "HasFoo")]
            public event EventHandler<StateVariableChangedArgs<string>> FooChanged;
            
            [UpnpStateVariable]
            public event EventHandler<StateVariableChangedArgs<string>> BarChanged;
            
            public bool HasFoo { get; set; }
        }
        
        [Test]
        public void UnimplementedOptionalStateVariable ()
        {
            var service = new DummyService<OptionalStateVariablesClass> ();
            var controller = new ServiceController (null,
                new[] { new DummyStateVariable ("BarChanged", "string") });
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        [Test]
        public void ImplementedOptionalStateVariable ()
        {
            var service = new DummyService<OptionalStateVariablesClass> (new OptionalStateVariablesClass { HasFoo = true });
            var controller = new ServiceController (null,
                new[] {
                    new DummyStateVariable ("FooChanged", "string"),
                    new DummyStateVariable ("BarChanged", "string") });
            ServiceDescriptionTests.AssertEquality (controller, service.GetController ());
        }
        
        class ErroneousOptionalStateVariablesClass
        {
            [UpnpStateVariable (OmitUnless = "HasFoo")]
            public event EventHandler<StateVariableChangedArgs<string>> FooChanged;
        }
        
        [Test, ExpectedException (typeof (UpnpServiceDefinitionException))]
        public void ErroneousOptionalStateVariable ()
        {
            new DummyService<ErroneousOptionalStateVariablesClass> ();
        }
        
        class ErroneousArgumentTypeClass
        {
            [UpnpAction]
            public void Foo (Exception e)
            {
            }
        }
        
        [Test, ExpectedException (typeof (UpnpServiceDefinitionException))]
        public void ErroneousArgumentType ()
        {
            new DummyService<ErroneousArgumentTypeClass> ();
        }
    }
}
