// 
// QueryParserTests.cs
//  
// Author:
//       Scott Thomas <lunchtimemama@gmail.com>
// 
// Copyright (c) 2010 Scott Thomas
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
using System.Text;

using NUnit.Framework;

using Mono.Upnp.Dcp.MediaServer1.ContentDirectory1;

namespace Mono.Upnp.Dcp.MediaServer1.Tests
{
    [TestFixture]
    public class QueryParserTests
    {
        #pragma warning disable 0659
        class Property
        {
            string name;

            public Property (string name)
            {
                this.name = name;
            }

            public Query Contains (string value)
            {
                return visitor => visitor.VisitContains (name, value);
            }

            public Query Exists (bool value)
            {
                return visitor => visitor.VisitExists (name, value);
            }

            public static Query operator ==(Property property, string value)
            {
                return visitor => visitor.VisitEquals (property.name, value);
            }

            public static Query operator !=(Property property, string value)
            {
                return visitor => visitor.VisitDoesNotEqual (property.name, value);
            }

            public static Query operator <(Property property, string value)
            {
                return visitor => visitor.VisitLessThan (property.name, value);
            }

            public static Query operator <=(Property property, string value)
            {
                return visitor => visitor.VisitLessThanOrEqualTo (property.name, value);
            }

            public static Query operator >(Property property, string value)
            {
                return visitor => visitor.VisitGreaterThan (property.name, value);
            }

            public static Query operator >=(Property property, string value)
            {
                return visitor => visitor.VisitGreaterThanOrEqualTo (property.name, value);
            }
        }
        #pragma warning restore 0659

        [Test]
        public void EqualityOperator ()
        {
            AssertEquality (new Property ("foo") == "bar", @"foo = ""bar""");
        }

        [Test]
        public void InequalityOperator ()
        {
            AssertEquality (new Property ("foo") != "bar", @"foo != ""bar""");
        }

        [Test]
        public void LessThanOperator ()
        {
            AssertEquality (new Property ("foo") < "5", @"foo < ""5""");
        }

        [Test]
        public void LessThanOrEqualOperator ()
        {
            AssertEquality (new Property ("foo") <= "5", @"foo <= ""5""");
        }

        [Test]
        public void GreaterThanOperator ()
        {
            AssertEquality (new Property ("foo") > "5", @"foo > ""5""");
        }

        [Test]
        public void GreaterThanOrEqualOperator ()
        {
            AssertEquality (new Property ("foo") >= "5", @"foo >= ""5""");
        }

        [Test]
        public void ContainsOperator ()
        {
            AssertEquality (new Property ("foo").Contains ("bar"), @"foo contains ""bar""");
        }

        [Test]
        public void ExistsTrue ()
        {
            AssertEquality (new Property ("foo").Exists (true), "foo exists true");
        }

        [Test]
        public void ExistsFalse ()
        {
            AssertEquality (new Property ("foo").Exists (false), "foo exists false");
        }

        [Test]
        public void ExistsTrueWithTrailingWhiteSpace ()
        {
            AssertEquality (new Property ("foo").Exists (true), "foo exists true ");
        }

        [Test]
        public void ExistsFalseWithTrailingWhiteSpace ()
        {
            AssertEquality (new Property ("foo").Exists (false), "foo exists false ");
        }

        [Test]
        public void ExistsTrueWithLeadingWhiteSpace ()
        {
            AssertEquality (new Property ("foo").Exists (true), "foo exists \t\rtrue");
        }

        [Test]
        public void ExistsFalseWithLeadingWhiteSpace ()
        {
            AssertEquality (new Property ("foo").Exists (false), "foo exists \t\rfalse");
        }

        [Test]
        public void WhiteSpaceAroundOperator ()
        {
            var expected = new Property ("foo") == "bar";
            AssertEquality (expected, @" foo = ""bar""");
            AssertEquality (expected, @"foo  = ""bar""");
            AssertEquality (expected, @"foo =  ""bar""");
            AssertEquality (expected, @"foo = ""bar"" ");
            AssertEquality (expected, @" foo  =  ""bar"" ");
        }

        [Test, ExpectedException (typeof (QueryParsingException), ExpectedMessage = "Incomplete operator: !=.")]
        public void IncompleteInequalityOperator ()
        {
            QueryParser.Parse (@"foo ! ""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = @"The property identifier is not a part of an expression: foo=""bar"".")]
        public void NoWhiteSpaceAroundOperator ()
        {
            QueryParser.Parse (@"foo=""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Whitespace is required around the operator: =.")]
        public void NoTrailingWhiteSpaceAroundOperator ()
        {
            QueryParser.Parse (@"foo =""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Whitespace is required around the operator: =.")]
        public void DoubleEqualityOperator ()
        {
            QueryParser.Parse (@"foo == ""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Whitespace is required around the operator: <.")]
        public void NoTrailingWhiteSpaceAroundLessThan ()
        {
            QueryParser.Parse (@"foo <""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Whitespace is required around the operator: <=.")]
        public void NoTrailingWhiteSpaceAroundLessThanOrEqualTo ()
        {
            QueryParser.Parse (@"foo <=""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Whitespace is required around the operator: >.")]
        public void NoTrailingWhiteSpaceAroundGreaterThan ()
        {
            QueryParser.Parse (@"foo >""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Whitespace is required around the operator: >=.")]
        public void NoTrailingWhiteSpaceAroundGreaterThanOrEqualTo ()
        {
            QueryParser.Parse (@"foo >=""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Expecting double-quoted string operand with the operator: =.")]
        public void UnquotedOperand ()
        {
            QueryParser.Parse ("foo = bar");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unrecognized operator begining: /.")]
        public void UnrecognizedOperator ()
        {
            QueryParser.Parse (@"foo / ""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = @"The double-quoted string is not terminated: ""bar"".")]
        public void UnterminatedDoubleQuotedString ()
        {
            QueryParser.Parse (@"foo = ""bar");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "There is no operand for the operator: =.")]
        public void MissingOperandWithNoSpace ()
        {
            QueryParser.Parse ("foo =");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "There is no operand for the operator: =.")]
        public void MissingOperandWithSpace ()
        {
            QueryParser.Parse ("foo = ");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException), ExpectedMessage = "Unrecognized operator: contain.")]
        public void IncompleteContainsOperator ()
        {
            QueryParser.Parse (@"foo contain ""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Whitespace is required around the operator: contains.")]
        public void OverlongContainsOperator ()
        {
            QueryParser.Parse (@"foo containsing ""bar""");
        }

        const string boolean_error_message = @"Expecting either ""true"" or ""false"".";

        [Test, ExpectedException (typeof (QueryParsingException), ExpectedMessage = boolean_error_message)]
        public void IllegalBooleanLiteral ()
        {
            QueryParser.Parse ("foo exists bar");
        }

        [Test, ExpectedException (typeof (QueryParsingException), ExpectedMessage = boolean_error_message)]
        public void IllegalTrueLiteral ()
        {
            QueryParser.Parse ("foo exists troo");
        }

        [Test, ExpectedException (typeof (QueryParsingException), ExpectedMessage = boolean_error_message)]
        public void IllegalFalseLiteral ()
        {
            QueryParser.Parse ("foo exists falze");
        }

        [Test, ExpectedException (typeof (QueryParsingException), ExpectedMessage = boolean_error_message)]
        public void IllegallyShortTrueLiteral ()
        {
            QueryParser.Parse ("foo exists tru");
        }

        [Test, ExpectedException (typeof (QueryParsingException), ExpectedMessage = boolean_error_message)]
        public void IllegallyShortTrueLiteralWithTrailingWhitespace ()
        {
            QueryParser.Parse ("foo exists tru ");
        }

        [Test, ExpectedException (typeof (QueryParsingException), ExpectedMessage = boolean_error_message)]
        public void IllegallyShortFalseLiteral ()
        {
            QueryParser.Parse ("foo exists fals");
        }

        [Test, ExpectedException (typeof (QueryParsingException), ExpectedMessage = boolean_error_message)]
        public void IllegallyShortFalseLiteralWithTrailingWhitespace ()
        {
            QueryParser.Parse ("foo exists fals ");
        }

        [Test, ExpectedException (typeof (QueryParsingException), ExpectedMessage = boolean_error_message)]
        public void IllegallyLongTrueLiteral ()
        {
            QueryParser.Parse ("foo exists truely");
        }

        [Test, ExpectedException (typeof (QueryParsingException), ExpectedMessage = boolean_error_message)]
        public void IllegallyLongFalseLiteral ()
        {
            QueryParser.Parse ("foo exists falserize");
        }

        void AssertEquality (Query expectedQuery, string actualQuery)
        {
            var expected_builder = new StringBuilder ();
            var actual_builder = new StringBuilder ();
            expectedQuery (new QueryStringifier (expected_builder));
            QueryParser.Parse (actualQuery) (new QueryStringifier (actual_builder));
            Assert.AreEqual (expected_builder.ToString (), actual_builder.ToString ());
        }
    }
}

