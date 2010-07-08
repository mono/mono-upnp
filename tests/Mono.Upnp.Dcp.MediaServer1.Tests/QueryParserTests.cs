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

            public Query DoesNotContain (string value)
            {
                return visitor => visitor.VisitDoesNotContain (name, value);
            }

            public Query DerivedFrom (string value)
            {
                return visitor => visitor.VisitDerivedFrom (name, value);
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

        static Query Conjoin (Query leftOperand, Query rightOperand)
        {
            return visitor => visitor.VisitAnd (leftOperand, rightOperand);
        }

        static Query Disjoin (Query leftOperand, Query rightOperand)
        {
            return visitor => visitor.VisitOr (leftOperand, rightOperand);
        }

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
        public void DerivedFromOperator ()
        {
            AssertEquality (new Property ("foo").DerivedFrom ("object.item"), @"foo derivedFrom ""object.item""");
        }

        [Test]
        public void DoesNotContainOperator ()
        {
            AssertEquality (new Property ("foo").DoesNotContain ("bar"), @"foo doesNotContain ""bar""");
        }

        [Test]
        public void AndOperator ()
        {
            AssertEquality (
                Conjoin (new Property ("foo") == "bar", new Property ("bat") == "baz"),
                @"foo = ""bar"" and bat = ""baz""");
        }

        [Test]
        public void OrOperator ()
        {
            AssertEquality (
                Disjoin (new Property ("foo") == "bar", new Property ("bat") == "baz"),
                @"foo = ""bar"" or bat = ""baz""");
        }

        [Test]
        public void OperatorPriority ()
        {
            AssertEquality (
                Disjoin (
                    Disjoin (
                        Conjoin (new Property ("foo") == "bar", new Property ("bat") == "baz"),
                        new Property ("name").Contains ("john")),
                    Conjoin (new Property ("eyes") == "green", new Property ("age") >= "21")),
                @"foo = ""bar"" and bat = ""baz"" or name contains ""john"" or eyes = ""green"" and age >= ""21""");
        }

        [Test]
        public void ParentheticalPriority1 ()
        {
            AssertEquality (
                Disjoin (
                    Conjoin (new Property ("foo") == "bar", new Property ("bat") == "baz"),
                    Conjoin (
                        Disjoin (new Property ("name").Contains ("john"), new Property ("eyes") == "green"),
                        new Property ("age") >= "21")),
                @"foo = ""bar"" and bat = ""baz"" or (name contains ""john"" or eyes = ""green"") and age >= ""21""");
        }

        [Test]
        public void ParentheticalPriority2 ()
        {
            AssertEquality (
                Conjoin (
                    Conjoin (
                        new Property ("foo") == "bar",
                        Disjoin (
                            Disjoin (new Property ("bat") == "baz", new Property ("name").Contains ("john")),
                            new Property ("eyes") == "green")),
                    new Property ("age") >= "21"),
                @"foo = ""bar"" and ((bat = ""baz"" or name contains ""john"") or eyes = ""green"") and age >= ""21""");
        }

        [Test]
        public void ParentheticalPriority3 ()
        {
            AssertEquality (
                Disjoin (
                    Conjoin (
                        new Property ("foo") == "bar",
                        Disjoin (new Property ("bat") == "baz", new Property ("name").Contains ("john"))),
                    Conjoin (new Property ("eyes") == "green", new Property ("age") >= "21")),
                @"foo = ""bar"" and (bat = ""baz"" or name contains ""john"") or eyes = ""green"" and age >= ""21""");
        }

        [Test]
        public void ParentheticalPriority4 ()
        {
            AssertEquality (
                Conjoin (
                    Conjoin (
                        Disjoin (new Property ("foo") == "bar", new Property ("bat") == "baz"),
                        Disjoin (new Property ("name").Contains ("john"), new Property ("eyes") == "green")),
                    new Property ("age") >= "21"),
                @"(foo = ""bar"" or bat = ""baz"") and (name contains ""john"" or eyes = ""green"") and age >= ""21""");
        }

        [Test]
        public void ParentheticalPriority5 ()
        {
            AssertEquality (
                Conjoin (
                    Disjoin (new Property ("foo") == "bar", new Property ("bat").Exists (true)),
                    Conjoin (
                        Disjoin (new Property ("name").Contains ("john"), new Property ("eyes").Exists (false)),
                        new Property ("age") >= "21")),
                @"(foo = ""bar"" or bat exists true) and ((name contains ""john"" or eyes exists false) and age >= ""21"")");
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

        [Test, ExpectedException (typeof (QueryParsingException), ExpectedMessage = "The query is empty.")]
        public void EmptyQuery ()
        {
            QueryParser.Parse ("");
        }

        [Test, ExpectedException (typeof (QueryParsingException), ExpectedMessage = "Incomplete operator: !=.")]
        public void IncompleteInequalityOperator ()
        {
            QueryParser.Parse (@"foo ! ""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "The property identifier is not a part of an expression: foo.")]
        public void NoOperator ()
        {
            QueryParser.Parse ("foo");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = @"No operator is applied to the property identifier: foo.")]
        public void NoOperatorAndTrailingWhiteSpace ()
        {
            QueryParser.Parse ("foo ");
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
            ExpectedMessage = "Unexpected operator begining: /.")]
        public void UnexpectedOperator ()
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
        [ExpectedException (typeof (QueryParsingException), ExpectedMessage = "Unexpected operator: contain.")]
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

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator begining: du.")]
        public void NeitherDerivedFromNorDoesNotContain ()
        {
            QueryParser.Parse (@"foo dumbo ""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator begining: d.")]
        public void IllegallyShortDerivedFromOrDoesNotContain ()
        {
            QueryParser.Parse ("foo d");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator: d.")]
        public void IllegallyShortDerivedFromOrDoesNotContainWithWhiteSpace ()
        {
            QueryParser.Parse (@"foo d ""bar""");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator begining: az.")]
        public void IllegalAndOperator1 ()
        {
            QueryParser.Parse (@"foo = ""bar"" az");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator begining: anz.")]
        public void IllegalAndOperator2 ()
        {
            QueryParser.Parse (@"foo = ""bar"" anz");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator begining: andz.")]
        public void IllegalAndOperator3 ()
        {
            QueryParser.Parse (@"foo = ""bar"" andz");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator: a.")]
        public void IllegallyShortAndOperator1 ()
        {
            QueryParser.Parse (@"foo = ""bar"" a");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator: a.")]
        public void IllegallyShortAndOperatorWithTrailingWhiteSpace1 ()
        {
            QueryParser.Parse ("foo = \"bar\" a\t");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator: an.")]
        public void IllegallyShortAndOperator2 ()
        {
            QueryParser.Parse (@"foo = ""bar"" an");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator: an.")]
        public void IllegallyShortAndOperatorWithTrailingWhiteSpace2 ()
        {
            QueryParser.Parse ("foo = \"bar\" an\t");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Expecting an expression after the conjunction.")]
        public void IncompleteConjuction ()
        {
            QueryParser.Parse (@"foo = ""bar"" and");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Expecting an expression after the conjunction.")]
        public void IncompleteConjuctionWithTrailingWhiteSpace ()
        {
            QueryParser.Parse ("foo = \"bar\" and \t\n");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator begining: oz.")]
        public void IllegalOrOperator1 ()
        {
            QueryParser.Parse (@"foo = ""bar"" oz");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator begining: orz.")]
        public void IllegalOrOperator2 ()
        {
            QueryParser.Parse (@"foo = ""bar"" orz");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator: o.")]
        public void IllegallyShortOrOperator ()
        {
            QueryParser.Parse (@"foo = ""bar"" o");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Unexpected operator: o.")]
        public void IllegallyShortOrOperatorWithTrailingWhiteSpace ()
        {
            QueryParser.Parse (@"foo = ""bar"" o ");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Expecting an expression after the disjunction.")]
        public void IncompleteDisjunction ()
        {
            QueryParser.Parse (@"foo = ""bar"" or");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Expecting an expression after the disjunction.")]
        public void IncompleteDisjunctionWithTrailingWhiteSpace ()
        {
            QueryParser.Parse (@"foo = ""bar"" or  ");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException), ExpectedMessage = "The parentheses are unbalanced.")]
        public void UnbalancedOpeningParenthesis ()
        {
            QueryParser.Parse (")");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException), ExpectedMessage = "Empty expressions are not allowed.")]
        public void OpeningEmptyParentheses ()
        {
            QueryParser.Parse ("()");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Expecting an expression after the conjunction.")]
        public void ConjoinedEmptyParentheses ()
        {
            QueryParser.Parse (@"foo = ""bar"" and ()");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Expecting an expression after the disjunction.")]
        public void DisjoinedEmptyParentheses ()
        {
            QueryParser.Parse (@"foo = ""bar"" or ()");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException), ExpectedMessage = "The parentheses are unbalanced.")]
        public void UnbalancedEquasionStatement ()
        {
            QueryParser.Parse (@"foo = ""bar"")");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException), ExpectedMessage = "The parentheses are unbalanced.")]
        public void UnbalancedExistsStatement ()
        {
            QueryParser.Parse (@"foo exists true )");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException), ExpectedMessage = "The parentheses are unbalanced.")]
        public void UnbalancedConjunction ()
        {
            QueryParser.Parse (@"foo exists true and bar = ""bat"" )");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException), ExpectedMessage = "The parentheses are unbalanced.")]
        public void UnbalancedDisjunction ()
        {
            QueryParser.Parse (@"foo exists true or bar = ""bat"" )");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Expecting an expression after the conjunction.")]
        public void IllegalParenthesisWithConjunction ()
        {
            QueryParser.Parse (@"foo exists true and )");
        }

        [Test]
        [ExpectedException (typeof (QueryParsingException),
            ExpectedMessage = "Expecting an expression after the disjunction.")]
        public void IllegalParenthesisWithDisjunction ()
        {
            QueryParser.Parse (@"foo exists true or )");
        }

        void AssertEquality (Query expectedQuery, string actualQuery)
        {
            var expected_builder = new StringBuilder ();
            var actual_builder = new StringBuilder ();
            expectedQuery (new QueryStringifier (expected_builder));
            QueryParser.Parse (actualQuery) (new QueryStringifier (actual_builder));
            Console.WriteLine (actual_builder.ToString ());
            Assert.AreEqual (expected_builder.ToString (), actual_builder.ToString ());
        }
    }
}

