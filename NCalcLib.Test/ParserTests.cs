using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static NCalcLib.SyntaxFactory;

namespace NCalcLib.Test
{
    public class ParserTests
    {
        [Fact]
        public void Submission_NumberLiteralExpression()
        {
            var text = "5";
            var expression = Parser.ParseSubmission(text);

            Assert.Equal(expected: 0, actual: expression.Start);
            Assert.Equal(expected: 1, actual: expression.Length);
            Assert.True(expression is NumberLiteralExpression);
            Assert.Equal(expected: "5", actual: ((NumberLiteralExpression)expression).Token.Text);
            Assert.Equal(expected: 5, actual: ((NumberLiteralExpression)expression).Value);
        }

        [Fact]
        public void NumberLiteralExpression_SingleDigit()
        {
            var text = "5";
            var expression = Parser.ParseNumberLiteral(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 1, node: expression);
            Assert.Equal(expected: "5", actual: expression.Token.Text);
            Assert.Equal(expected: 5, actual: expression.Value);
        }

        [Fact]
        public void NumberLiteralExpression_DigitWithDecimalPoint()
        {
            var text = "5.";
            var expression = Parser.ParseNumberLiteral(text);

            Assert.Null(expression);
        }

        [Fact]
        public void NumberLiteralExpression_DecimalDigits()
        {
            var text = "5.4";
            var expression = Parser.ParseNumberLiteral(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 3, node: expression);
            Assert.Equal(expected: "5.4", actual: expression.Token.Text);
            Assert.Equal(expected: 5.4m, actual: expression.Value);
        }

        private static void AssertStartAndLength(int expectedStart, int expectedLength, Whitespace whitespace)
        {
            Assert.Equal(expected: expectedStart, actual: whitespace.Start);
            Assert.Equal(expected: expectedLength, actual: whitespace.Length);
        }

        private static void AssertStartAndLength(int expectedStart, int expectedLength, Node node)
        {
            Assert.Equal(expected: expectedStart, actual: node.Start);
            Assert.Equal(expected: expectedLength, actual: node.Length);
        }

        [Fact]
        public void NumberLiteralExpression_MultiDigit()
        {
            var text = "25";
            var expression = Parser.ParseNumberLiteral(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 2, node: expression);
            Assert.Equal(expected: 25, actual: expression.Value);
            Assert.Equal(expected: "25", actual: expression.Token.Text);
        }

        [Fact]
        public void NegationExpression()
        {
            var text = "-123";
            var expression = (NegationExpression)Parser.ParseNegationExpression(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 4, node: expression);
            var subExpression = (NumberLiteralExpression)expression.SubExpression;
            Assert.Equal(expected: 123, actual: subExpression.Value);
        }

        [Fact]
        public void NegationExpression_NotNegative()
        {
            var text = "123";
            var expression = (NumberLiteralExpression)Parser.ParseNegationExpression(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 3, node: expression);
            Assert.Equal(expected: 123, actual: expression.Value);
            Assert.Equal(expected: "123", actual: expression.Token.Text);
        }

        [Fact]
        public void NumberLiteralExpression_LeadingWhitespace()
        {
            var text = "  456";
            var expression = Parser.ParseNumberLiteral(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 5, node: expression);
            Assert.Equal(expected: 456, actual: expression.Value);
            Assert.Equal(expected: "456", actual: expression.Token.Text);
        }

        [Fact]
        public void Whitespace_None()
        {
            var text = "";
            var whitespace = Parser.ParseWhitespace(text);

            Assert.Equal(expected: 0, actual: whitespace.Start);
            Assert.Equal(expected: 0, actual: whitespace.Length);
            Assert.Equal(expected: "", actual: whitespace.Value);
        }

        [Fact]
        public void Whitespace_SingleSpace()
        {
            var text = " ";
            var whitespace = Parser.ParseWhitespace(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 1, whitespace: whitespace);
        }

        [Fact]
        public void Whitespace_MultipleSpaces()
        {
            var text = "   ";
            var whitespace = Parser.ParseWhitespace(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 3, whitespace: whitespace);
        }

        [Fact]
        public void Addition()
        {
            var text = "1 + 2";
            var expression = Parser.ParseAdditionAndSubtraction(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 5, node: expression);
        }

        [Fact]
        public void Token_Basic()
        {
            var text = "+";
            var token = Parser.ParseToken(text, "+");

            AssertStartAndLength(expectedStart: 0, expectedLength: 1, node: token);
            Assert.True(token.Whitespace.IsEmpty());
        }

        [Fact]
        public void Token_NoMatch()
        {
            var text = "-";
            var token = Parser.ParseToken(text, "+");

            Assert.Null(token);
        }

        [Fact]
        public void Token_WithWhitespace()
        {
            var text = "  +";
            var token = Parser.ParseToken(text, "+");

            AssertStartAndLength(expectedStart: 0, expectedLength: 3, node: token);
            AssertStartAndLength(expectedStart: 0, expectedLength: 2, whitespace: token.Whitespace);
            Assert.Equal(expected: 2, actual: token.TokenStart);
            Assert.Equal(expected: 1, actual: token.TokenLength);
        }

        [Fact]
        public void MultiplicationAndDivision_Single()
        {
            var text = "4 * 5";
            var expression = (BinaryExpression)Parser.ParseMultiplicationAndDivision(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 5, node: expression);
            var left = (NumberLiteralExpression)expression.Left;
            Assert.Equal(expected: 4, actual: left.Value);
            var right = (NumberLiteralExpression)expression.Right;
            Assert.Equal(expected: 5, actual: right.Value);
            AssertStartAndLength(expectedStart: 1, expectedLength: 2, node: expression.Operator);
        }

        [Fact]
        public void AdditionAndSubtraction_Single()
        {
            var text = "2 + 3";
            var expression = (BinaryExpression)Parser.ParseAdditionAndSubtraction(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 5, node: expression);
            var left = (NumberLiteralExpression)expression.Left;
            Assert.Equal(expected: 2, actual: left.Value);
            var right = (NumberLiteralExpression)expression.Right;
            Assert.Equal(expected: 3, actual: right.Value);
            AssertStartAndLength(expectedStart: 1, expectedLength: 2, node: expression.Operator);
        }

        [Fact]
        public void MultiplicationAndDivision_Multiple()
        {
            var text = " 4 * 5 / 10";
            var expression = (BinaryExpression)Parser.ParseMultiplicationAndDivision(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 11, node: expression);
        }

        [Fact]
        public void AdditionAndSubtraction_Multiple()
        {
            var text = " 3 * 4 / 2";
            var expression = (BinaryExpression)Parser.ParseAdditionAndSubtraction(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 10, node: expression);
        }

        [Fact]
        public void MultiplicationAndDivision_SingleExpression()
        {
            var text = "4";
            var expression = (NumberLiteralExpression)Parser.ParseMultiplicationAndDivision(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 1, node: expression);
            Assert.Equal(expected: 4, actual: expression.Value);
        }

        [Fact]
        public void AdditionAndSubtraction_SingleExpression()
        {
            var text = "4";
            var expression = (NumberLiteralExpression)Parser.ParseAdditionAndSubtraction(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 1, node: expression);
            Assert.Equal(expected: 4, actual: expression.Value);
        }

        [Fact]
        public void AdditionAndSubtraction_WithMultiplicationAndDivision()
        {
            var text = "1 + 2 * 3 - 4 / 5";
            var expression = (BinaryExpression)Parser.ParseAdditionAndSubtraction(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 17, node: expression);
        }

        [Fact]
        public void MultiplicationAndDivision_NegationSubExpression()
        {
            var text = "2 * -3";
            var expression = (BinaryExpression)Parser.ParseMultiplicationAndDivision(text);

            AssertStartAndLength(expectedStart: 0, expectedLength: 6, node: expression);
            var right = (NegationExpression)expression.Right;
            AssertStartAndLength(expectedStart: 3, expectedLength: 3, node: right);
        }

        [Fact]
        public void ParenthesizedExpression_NumberLiteral()
        {
            var text = "(5)";
            var expression = (ParenthesizedExpression)Parser.ParseParenthensizedExpression(text);

            var expected = ParenthesizedExpression(
                Token(0, "("),
                NumberLiteralExpression(Token(1, "5")),
                Token(2, ")"));

            Assert.Equal(expected, actual: expression);
        }

        [Fact]
        public void ParenthesizedExpression_BinaryExpression()
        {
            var text = "(1 + 2)";
            var expression = (ParenthesizedExpression)Parser.ParseParenthensizedExpression(text);

            var expected = ParenthesizedExpression(
                Token(0, "("),
                BinaryExpression(
                    NumberLiteralExpression(Token(1, "1")),
                    Token(Whitespace(2, " "), 3, "+"),
                    NumberLiteralExpression(Token(Whitespace(4, " "), 5, "2"))),
                Token(6, ")"));

            Assert.Equal(expected, actual: expression);
        }

        [Fact]
        public void Identifier_SingleLetter()
        {
            var text = "a";
            var expression = Parser.ParseIdentifier(text);

            var expected = IdentifierExpression(Token(0, "a"));

            Assert.Equal(expected, actual: expression);
        }

        [Fact]
        public void Identifier_MultipleLetters()
        {
            var text = "abc";
            var expression = Parser.ParseIdentifier(text);

            var expected = IdentifierExpression(Token(0, "abc"));

            Assert.Equal(expected, actual: expression);
        }

        [Fact]
        public void Identifier_LetterAndNumbers()
        {
            var text = "a123";
            var expression = Parser.ParseIdentifier(text);

            var expected = IdentifierExpression(Token(0, "a123"));

            Assert.Equal(expected, actual: expression);
        }

        [Fact]
        public void Identifier_StartsWithNumber()
        {
            var text = "1abc";
            var expression = Parser.ParseIdentifier(text);

            Assert.Null(expression);
        }

        [Fact]
        public void Assignment_Single()
        {
            var text = "a = 5";
            var expression = Parser.ParseAssignment(text);

            var expected = BinaryExpression(
                IdentifierExpression(Token(0, "a")),
                Token(Whitespace(1, " "), 2, "="),
                NumberLiteralExpression(Token(Whitespace(3, " "), 4, "5")));

            Assert.Equal(expected, actual: expression);
        }

        [Fact]
        public void Assignment_Double()
        {
            var text = "a = b = c";
            var expression = Parser.ParseAssignment(text);

            var expected = BinaryExpression(
                IdentifierExpression(Token(0, "a")),
                Token(Whitespace(1, " "), 2, "="),
                BinaryExpression(
                    IdentifierExpression(Token(Whitespace(3, " "), 4, "b")),
                    Token(Whitespace(5, " "), 6, "="),
                    IdentifierExpression(Token(Whitespace(7, " "), 8, "c"))));

            Assert.Equal(expected, actual: expression);
        }
    }
}
