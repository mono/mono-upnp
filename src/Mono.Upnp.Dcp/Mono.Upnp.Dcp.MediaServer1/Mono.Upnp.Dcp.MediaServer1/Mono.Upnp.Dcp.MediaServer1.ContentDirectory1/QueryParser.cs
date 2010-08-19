// 
// QueryParser.cs
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

using System.Text;

namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1
{
    using Expression = System.Action<QueryVisitor>;
    
    // Refer to ContentDirectory1 Service Template 1.0.1, Section 2.5.5.1: Search Criteria String Syntax
    public abstract class QueryParser
    {
        // Look Mom, no 3.5 dependency!
        delegate TResult Func<T, TResult> (T argument);
        delegate TResult Func<T1, T2, TResult> (T1 argument1, T2 argument2);

        protected abstract QueryParser OnCharacter (char character);

        protected abstract Expression OnDone ();

        protected static bool IsWhiteSpace (char character)
        {
            switch (character) {
            case ' ': return true;
            case '\t': return true;
            case '\n': return true;
            case '\v': return true;
            case '\f': return true;
            case '\r': return true;
            default: return false;
            }
        }

        class RootQueryParser : QueryParser
        {
            const string wild_card_error_message = "The wildcard must be used alone.";

            int parentheses;

            public RootQueryParser ()
            {
            }

            protected override QueryParser OnCharacter (char character)
            {
                if (IsWhiteSpace (character)) {
                    return this;
                } else if (character == '*') {
                    if (parentheses == -1) {
                        throw new QueryParsingException (wild_card_error_message);
                    } else {
                        parentheses = -1;
                        return this;
                    }
                } else if (character == '(') {
                    if (parentheses == -1) {
                        throw new QueryParsingException (wild_card_error_message);
                    } else {
                        parentheses++;
                        return this;
                    }
                } else if (character == ')') {
                    if (parentheses > 0) {
                        throw new QueryParsingException ("Empty expressions are not allowed.");
                    } else {
                        throw new QueryParsingException ("The parentheses are unbalanced.");
                    }
                } else {
                    if (parentheses == -1) {
                        throw new QueryParsingException (wild_card_error_message);
                    } else {
                        return new PropertyParser (token => new RootPropertyOperatorParser (token,
                            expression => new ExpressionParser (expression, parentheses))).OnCharacter (character);
                    }
                }
            }

            protected override Expression OnDone ()
            {
                if (parentheses == -1) {
                    return visitor => visitor.VisitAllResults ();
                } else {
                    throw new QueryParsingException ("The query is empty.");
                }
            }
        }

        class ExpressionParser : QueryParser
        {
            const int disjunction_priority = 1;
            const int conjunction_priority = 2;
            const int parenthetical_priority = 3;

            protected readonly Expression Expression;
            int parentheses;

            public ExpressionParser (Expression expression, int parentheses)
            {
                this.Expression = expression;
                this.parentheses = parentheses;
            }

            protected override QueryParser OnCharacter (char character)
            {
                if (IsWhiteSpace (character)) {
                    return this;
                } else if (character == '(') {
                    parentheses++;
                    return this;
                } else if (character == ')') {
                    parentheses--;
                    if (parentheses < 0) {
                        throw new QueryParsingException ("The parentheses are unbalanced.");
                    } else {
                        return this;
                    }
                } else if (character == 'a') {
                    var priority = GetPriority (conjunction_priority);
                    return new OperatorParser ("and",
                        new JunctionParser ("conjunction", parentheses, priority, MakeHandler (priority,
                            (leftOperand, rightOperand) => visitor => visitor.VisitAnd (leftOperand, rightOperand))))
                        .OnCharacter ('a');
                } else if (character == 'o') {
                    var priority = GetPriority (disjunction_priority);
                    return new OperatorParser ("or",
                        new JunctionParser ("disjunction", parentheses, priority, MakeHandler (priority,
                            (leftOperand, rightOperand) => visitor => visitor.VisitOr (leftOperand, rightOperand))))
                        .OnCharacter ('o');
                } else {
                    throw new QueryParsingException (string.Format ("Unexpected operator begining: {0}.", character));
                }
            }

            int GetPriority (int priority)
            {
                if (parentheses > 0) {
                    return parenthetical_priority + parentheses + priority;
                } else {
                    return priority;
                }
            }

            protected virtual Func<int, Func<Expression, Expression>, Func<Expression, Expression>> MakeHandler (int priority,
                                                                                                                 Func<Expression, Expression, Expression> binaryOperator)
            {
                return (priorPriority, priorOperator) => {
                    if (priorPriority < priority) {
                        return priorOperand => priorOperator (binaryOperator (Expression, priorOperand));
                    } else {
                        return priorOperand => binaryOperator (Expression, priorOperator (priorOperand));
                    }
                };
            }

            protected override Expression OnDone ()
            {
                if (parentheses == 0) {
                    return Expression;
                } else {
                    throw new QueryParsingException ("The parentheses are unbalanced.");
                }
            }
        }

        class JoinedExpressionParser : ExpressionParser
        {
            readonly Func<int, Func<Expression, Expression>, Func<Expression, Expression>> previous_handler;
            readonly int priority;

            public JoinedExpressionParser (Expression expression,
                                           int parentheses,
                                           int priority,
                                           Func<int, Func<Expression, Expression>, Func<Expression, Expression>> previousHandler)
                : base (expression, parentheses)
            {
                this.previous_handler = previousHandler;
                this.priority = priority;
            }

            protected override Func<int, Func<Expression, Expression>, Func<Expression, Expression>> MakeHandler (int priority,
                                                                                                                  Func<Expression, Expression, Expression> binaryOperator)
            {
                // An unintelligable but very slick operator priority algorithm.
                if (this.priority < priority) {
                    return (priorPriority, priorOperator) => {
                        if (this.priority < priorPriority) {
                            return priorOperand => previous_handler (this.priority, operand => operand) (
                                priorOperator (binaryOperator (Expression, priorOperand)));
                        } else {
                            return priorOperand => previous_handler (
                                priorPriority, operand => priorOperator (operand)) (
                                binaryOperator (Expression, priorOperand));
                        }
                    };
                } else {
                    return (priorPriority, priorOperator) => priorOperand => priorOperator (previous_handler (
                        priorPriority, operand => binaryOperator (operand, priorOperand)) (Expression));
                }
            }

            protected override Expression OnDone ()
            {
                return previous_handler (priority, operand => operand) (Expression);
            }
        }

        class JunctionParser : QueryParser
        {
            readonly string junction;
            readonly Func<int, Func<Expression, Expression>, Func<Expression, Expression>> previous_handler;
            readonly int priority;
            int parentheses;

            public JunctionParser (string junction,
                                   int parentheses,
                                   int priority,
                                   Func<int, Func<Expression, Expression>, Func<Expression, Expression>> previousHandler)
            {
                this.junction = junction;
                this.parentheses = parentheses;
                this.priority = priority;
                this.previous_handler = previousHandler;
            }

            protected override QueryParser OnCharacter (char character)
            {
                if (IsWhiteSpace (character)) {
                    return this;
                } else if (character == '(') {
                    parentheses++;
                    return this;
                } else if (character == ')') {
                    return Fail<QueryParser> ();
                } else {
                    return new PropertyParser (token => new RootPropertyOperatorParser (
                        token, expression => new JoinedExpressionParser (
                            expression, parentheses, priority, previous_handler))).OnCharacter (character);
                }
            }

            protected override Expression OnDone ()
            {
                return Fail<Expression> ();
            }

            T Fail<T> ()
            {
                throw new QueryParsingException (string.Format (
                    "Expecting an expression after the {0}.", junction));
            }
        }

        class OperatorParser : QueryParser
        {
            readonly string @operator;
            readonly QueryParser next_parser;
            bool initialized;
            int position;

            public OperatorParser (string @operator, QueryParser nextParser)
            {
                this.@operator = @operator;
                this.next_parser = nextParser;
            }

            protected override QueryParser OnCharacter (char character)
            {
                if (initialized) {
                    if (IsWhiteSpace (character)) {
                        return this;
                    } else {
                        return next_parser.OnCharacter (character);
                    }
                } else if (position == @operator.Length) {
                    if (IsWhiteSpace (character)) {
                        initialized = true;
                        return this;
                    } else {
                        return OnFailure (@operator + character);
                    }
                } else if (character != @operator[position]) {
                    if (IsWhiteSpace (character)) {
                        throw new QueryParsingException (string.Format (
                            "Unexpected operator: {0}.", @operator.Substring (0, position)));
                    } else {
                        return OnFailure (@operator.Substring (0, position) + character);
                    }
                } else {
                    position++;
                    return this;
                }
            }

            protected virtual QueryParser OnFailure (string token)
            {
                throw new QueryParsingException (string.Format (
                    "Unexpected operator begining: {0}.", token));
            }

            protected override Expression OnDone ()
            {
                if (position == @operator.Length) {
                    throw new QueryParsingException (string.Format (
                        "There is no operand for the operator: {0}.", @operator));
                } else {
                    throw new QueryParsingException (string.Format (
                        "Unexpected operator: {0}.", @operator.Substring (0, position)));
                }
            }

            public QueryParser Or (QueryParser otherParser)
            {
                return new DisjoinedTokenParser (@operator, next_parser, otherParser);
            }

            class DisjoinedTokenParser : OperatorParser
            {
                readonly QueryParser alternative_parser;

                public DisjoinedTokenParser (string @operator, QueryParser nextParser, QueryParser alternativeParser)
                    : base (@operator, nextParser)
                {
                    alternative_parser = alternativeParser;
                }

                protected override QueryParser OnFailure (string @operator)
                {
                    var parser = alternative_parser;
                    foreach (var character in @operator) {
                        parser = parser.OnCharacter (character);
                    }
                    return parser;
                }
            }
        }

        class RootPropertyOperatorParser : QueryParser
        {
            readonly string property;
            readonly Func<Expression, QueryParser> consumer;

            public RootPropertyOperatorParser (string property, Func<Expression, QueryParser> consumer)
            {
                this.property = property;
                this.consumer = consumer;
            }

            protected override QueryParser OnCharacter (char character)
            {
                if (IsWhiteSpace (character)) {
                    return this;
                }

                QueryParser parser;
                switch (character) {
                case '=':
                    parser = Operator ("=", value => visitor => visitor.VisitEquals (property, value));
                    break;
                case '!':
                    parser = Operator ("!=", value => visitor => visitor.VisitDoesNotEqual (property, value));
                    break;
                case '<':
                    parser = Operator ("<", value => visitor => visitor.VisitLessThan (property, value)). Or (
                        Operator ("<=", value => visitor => visitor.VisitLessThanOrEqualTo (property, value)));
                    break;
                case '>':
                    parser = Operator (">", value => visitor => visitor.VisitGreaterThan (property, value)). Or (
                        Operator (">=", value => visitor => visitor.VisitGreaterThanOrEqualTo (property, value)));
                    break;
                case 'c':
                    parser = Operator ("contains", value => visitor => visitor.VisitContains (property, value));
                    break;
                case 'e':
                    parser = new OperatorParser ("exists",
                        new BooleanParser (value => consumer (visitor => visitor.VisitExists (property, value))));
                    break;
                case 'd':
                    parser = Operator ("derivedfrom", value => visitor => visitor.VisitDerivedFrom (property, value)).Or (
                        Operator ("doesNotContain", value => visitor => visitor.VisitDoesNotContain (property, value)));
                    break;
                default:
                    throw new QueryParsingException (string.Format ("Unexpected operator begining: {0}.", character));
                }

                return parser.OnCharacter (character);
            }

            protected override Expression OnDone ()
            {
                throw new QueryParsingException (string.Format (
                    "No operator is applied to the property identifier: {0}.", property));
            }

            OperatorParser Operator (string token, Func<string, Expression> @operator)
            {
                return new OperatorParser (token, new StringParser (token, value => consumer (@operator (value))));
            }
        }

        class PropertyParser : QueryParser
        {
            readonly Func<string, QueryParser> consumer;
            StringBuilder builder = new StringBuilder ();

            public PropertyParser (Func<string, QueryParser> consumer)
            {
                this.consumer = consumer;
            }

            protected override QueryParser OnCharacter (char character)
            {
                if (IsWhiteSpace (character)) {
                    return consumer (builder.ToString ());
                } else {
                    builder.Append (character);
                    return this;
                }
            }

            protected override Expression OnDone ()
            {
                throw new QueryParsingException (string.Format (
                    @"The property identifier is not a part of an expression: {0}.", builder.ToString ()));
            }
        }

        class StringParser : QueryParser
        {
            readonly string @operator;
            readonly Func<string, QueryParser> consumer;
            StringBuilder builder;
            bool escaped;

            public StringParser (string @operator, Func<string, QueryParser> consumer)
            {
                this.@operator = @operator;
                this.consumer = consumer;
            }

            protected override QueryParser OnCharacter (char character)
            {
                if (builder == null) {
                    if (character == '"') {
                        builder = new StringBuilder ();
                        return this;
                    } else {
                        throw new QueryParsingException (string.Format (
                            "Expecting double-quoted string operand with the operator: {0}.", @operator));
                    }
                } else if (escaped) {
                    if (character == '\\') {
                        builder.Append ('\\');
                    } else  if (character == '"') {
                        builder.Append ('"');
                    } else {
                        throw new QueryParsingException (string.Format (
                            "Unexpected escape sequence: \\{0}.", character));
                    }
                    escaped = false;
                    return this;
                } else if (character == '\\') {
                    escaped = true;
                    return this;
                } else if (character == '"') {
                    return consumer (builder.ToString ());
                } else {
                    builder.Append (character);
                    return this;
                }
            }

            protected override Expression OnDone ()
            {
                throw new QueryParsingException (string.Format (
                    @"The double-quoted string is not terminated: ""{0}"".", builder.ToString ()));
            }
        }

        class BooleanParser : QueryParser
        {
            readonly Func<bool, QueryParser> consumer;
            bool @true;
            int position;

            public BooleanParser (Func<bool, QueryParser> consumer)
            {
                this.consumer = consumer;
            }

            protected override QueryParser OnCharacter (char character)
            {
                if (position == 0) {
                    if (IsWhiteSpace (character)) {
                        return this;
                    } else if (character == 't') {
                        @true = true;
                        return Check ("true", character);
                    } else if (character == 'f') {
                        return Check ("false", character);
                    } else {
                        return Fail<QueryParser> ();
                    }
                } else if (@true) {
                    return Check ("true", character);
                } else {
                    return Check ("false", character);
                }
            }

            protected override Expression OnDone ()
            {
                if (@true) {
                    if (position == "true".Length) {
                        return consumer (true).OnDone ();
                    } else {
                        return Fail<Expression> ();
                    }
                } else {
                    if (position == "false".Length) {
                        return consumer (false).OnDone ();
                    } else {
                        return Fail<Expression> ();
                    }
                }
            }

            QueryParser Check (string expected, char character)
            {
                if (position == expected.Length) {
                    if (IsWhiteSpace (character)) {
                        return consumer (@true);
                    } else if (character == ')' || character == '(') {
                        return consumer (@true).OnCharacter (character);
                    } else {
                        return Fail<QueryParser> ();
                    }
                } else if (expected[position] == character) {
                    position++;
                    return this;
                } else {
                    return Fail<QueryParser> ();
                }
            }

            T Fail<T> ()
            {
                throw new QueryParsingException (@"Expecting either ""true"" or ""false"".");
            }
        }

        public static Expression Parse (string query)
        {
            QueryParser parser = new RootQueryParser ();

            foreach (var character in query) {
                parser = parser.OnCharacter (character);
            }

            return parser.OnDone ();
        }
    }
}
