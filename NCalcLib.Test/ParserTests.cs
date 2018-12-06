using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            var tokens = Lexer.LexSubmission(text);
            var expression = Parser.ParseSubmission(tokens);

            var expected = NumberLiteralExpression(Token(0, "5", TokenType.NumberLiteral));

            Assert.Equal(expected, actual: expression);
        }

        [Fact]
        public void ParseNumberLiteral()
        {
            var text = "5.4";
            var token = Lexer.LexNumberLiteral(text);
            var parseResult = Parser.ParseNumberLiteral(ImmutableArray.Create(token));

            var expectedExpression = NumberLiteralExpression(Token(0, "5.4", TokenType.NumberLiteral));
            var expectedNextTokenIndex = 1;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        private static void AssertStartAndLength(int expectedStart, int expectedLength, Whitespace whitespace)
        {
            Assert.Equal(expected: expectedStart, actual: whitespace.Start);
            Assert.Equal(expected: expectedLength, actual: whitespace.Length);
        }

        [Fact]
        public void NegationExpression_Negative()
        {
            var tokens = ImmutableArray.Create(
                Token(0, "-", TokenType.Minus),
                Token(1, "123", TokenType.NumberLiteral));
            var parseResult = Parser.ParseNegationExpression(tokens);

            var expectedExpression =
                NegationExpression(
                    tokens[0],
                    NumberLiteralExpression(
                        tokens[1]));
            var expectedNextToken = 2;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextToken, parseResult.NextTokenIndex);
        }

        [Fact]
        public void NegationExpression_NotNegative()
        {
            var tokens = ImmutableArray.Create(Token(0, "123", TokenType.NumberLiteral));
            var parseResult = Parser.ParseNegationExpression(tokens);

            var expectedExpression = NumberLiteralExpression(tokens[0]);
            var expectedNextTokenIndex = 1;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void MultiplicationAndDivision_Single()
        {
            var tokens = ImmutableArray.Create(
                Token(0, "4", TokenType.NumberLiteral),
                Token(1, "*", TokenType.Asterisk),
                Token(2, "5", TokenType.NumberLiteral));

            var parseResult = Parser.ParseMultiplicationAndDivision(tokens);

            var expectedExpression =
                BinaryExpression(
                    NumberLiteralExpression(tokens[0]),
                    tokens[1],
                    NumberLiteralExpression(tokens[2]));
            var expectedNextTokenIndex = 3;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void AdditionAndSubtraction_Single()
        {
            var tokens = ImmutableArray.Create(
                Token(0, "2", TokenType.NumberLiteral),
                Token(1, "+", TokenType.Plus),
                Token(2, "3", TokenType.NumberLiteral));

            var parseResult = Parser.ParseAdditionAndSubtraction(tokens);

            var expectedExpression =
                BinaryExpression(
                    NumberLiteralExpression(tokens[0]),
                    tokens[1],
                    NumberLiteralExpression(tokens[2]));
            var expectedNextTokenIndex = 3;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void MultiplicationAndDivision_Multiple()
        {
            var tokens = ImmutableArray.Create(
                Token(0, "4", TokenType.NumberLiteral),
                Token(1, "*", TokenType.Asterisk),
                Token(2, "5", TokenType.NumberLiteral),
                Token(3, "/", TokenType.Slash),
                Token(4, "10", TokenType.NumberLiteral));

            var parseResult = Parser.ParseMultiplicationAndDivision(tokens);

            var expectedExpression =
                BinaryExpression(
                    BinaryExpression(
                        NumberLiteralExpression(tokens[0]),
                        tokens[1],
                        NumberLiteralExpression(tokens[2])),
                    tokens[3],
                    NumberLiteralExpression(tokens[4]));
            var expectedNextTokenIndex = 5;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void AdditionAndSubtraction_Multiple()
        {
            var tokens = ImmutableArray.Create(
                Token(0, "3", TokenType.NumberLiteral),
                Token(1, "-", TokenType.Minus),
                Token(2, "4", TokenType.NumberLiteral),
                Token(3, "+", TokenType.Plus),
                Token(4, "2", TokenType.NumberLiteral));

            var parseResult = Parser.ParseAdditionAndSubtraction(tokens);

            var expectedExpression =
                BinaryExpression(
                    BinaryExpression(
                        NumberLiteralExpression(tokens[0]),
                        tokens[1],
                        NumberLiteralExpression(tokens[2])),
                    tokens[3],
                    NumberLiteralExpression(tokens[4]));
            var expectedNextTokenIndex = 5;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void MultiplicationAndDivision_SingleExpression()
        {
            var tokens = ImmutableArray.Create(Token(0, "4", TokenType.NumberLiteral));
            var parseResult = Parser.ParseMultiplicationAndDivision(tokens);

            var expectedExpression = NumberLiteralExpression(tokens[0]);
            var expectedNextTokenIndex = 1;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void AdditionAndSubtraction_SingleExpression()
        {
            var tokens = ImmutableArray.Create(Token(0, "4", TokenType.NumberLiteral));
            var parseResult = Parser.ParseAdditionAndSubtraction(tokens);

            var expectedExpression = NumberLiteralExpression(tokens[0]);
            var expectedNextTokenIndex = 1;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void AdditionAndSubtraction_WithMultiplicationAndDivision()
        {
            var tokens = ImmutableArray.Create(
                Token(0, "1", TokenType.NumberLiteral),
                Token(1, "+", TokenType.Plus),
                Token(2, "2", TokenType.NumberLiteral),
                Token(3, "*", TokenType.Asterisk),
                Token(4, "3", TokenType.NumberLiteral),
                Token(5, "-", TokenType.Minus),
                Token(6, "4", TokenType.NumberLiteral),
                Token(7, "/", TokenType.Slash),
                Token(8, "5", TokenType.NumberLiteral));
            var parseResult = Parser.ParseAdditionAndSubtraction(tokens);

            var expectedExpression =
                BinaryExpression(
                    BinaryExpression(
                        NumberLiteralExpression(tokens[0]),
                        tokens[1],
                        BinaryExpression(
                            NumberLiteralExpression(tokens[2]),
                            tokens[3],
                            NumberLiteralExpression(tokens[4]))),
                    tokens[5],
                    BinaryExpression(
                        NumberLiteralExpression(tokens[6]),
                        tokens[7],
                        NumberLiteralExpression(tokens[8])));
            var expectedNextTokenIndex = 9;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void MultiplicationAndDivision_NegationSubExpression()
        {
            var tokens = Lexer.LexSubmission("2 * -3");
            var parseResult = Parser.ParseMultiplicationAndDivision(tokens);

            var expectedExpression =
                BinaryExpression(
                    NumberLiteralExpression(tokens[0]),
                    tokens[1],
                    NegationExpression(tokens[2],
                        NumberLiteralExpression(tokens[3])));
            var expectedNextTokenIndex = 4;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void ParenthesizedExpression_NumberLiteral()
        {
            var text = "(5)";
            var tokens = Lexer.LexSubmission(text);
            var parseResult = Parser.ParseParenthensizedExpression(tokens);

            var expectedExpression =
                ParenthesizedExpression(
                    tokens[0],
                    NumberLiteralExpression(tokens[1]),
                    tokens[2]);
            var expectedNextTokenIndex = 3;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void ParenthesizedExpression_BinaryExpression()
        {
            var text = "(1 + 2)";
            var tokens = Lexer.LexSubmission(text);
            var parseResult = Parser.ParseParenthensizedExpression(tokens);

            var expectedExpression =
                ParenthesizedExpression(
                    tokens[0],
                    BinaryExpression(
                        NumberLiteralExpression(tokens[1]),
                        tokens[2],
                        NumberLiteralExpression(tokens[3])),
                    tokens[4]);
            var expectedNextTokenIndex = 5;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void Identifier_SingleLetter()
        {
            var text = "a";
            var tokens = Lexer.LexSubmission(text);
            var parseResult = Parser.ParseIdentifier(tokens);

            var expectedExpression = IdentifierExpression(tokens[0]);
            var expectedNextTokenIndex = 1;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void Assignment_Single()
        {
            var text = "a = 5";
            var tokens = Lexer.LexSubmission(text);
            var parseResult = Parser.ParseAssignment(tokens);

            var expectedExpression =
                BinaryExpression(
                    IdentifierExpression(tokens[0]),
                    tokens[1],
                    NumberLiteralExpression(tokens[2]));
            var expectedNextTokenIndex = 3;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void Assignment_Double()
        {
            var text = "a = b = c";
            var tokens = Lexer.LexSubmission(text);
            var parseResult = Parser.ParseAssignment(tokens);

            var expectedExpression =
                BinaryExpression(
                    IdentifierExpression(tokens[0]),
                    tokens[1],
                    BinaryExpression(
                        IdentifierExpression(tokens[2]),
                        tokens[3],
                        IdentifierExpression(tokens[4])));
            var expectedNextTokenIndex = 5;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void BooleanLiteral_True()
        {
            var text = "true";
            var tokens = Lexer.LexSubmission(text);
            var parseResult = Parser.ParseBooleanLiteral(tokens);

            var expectedExpression = BooleanLiteralExpression(tokens[0]);
            var expectedNextTokenIndex = 1;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void Relational_LessThan()
        {
            var text = "5 < 4";
            var tokens = Lexer.LexSubmission(text);
            var parseResult = Parser.ParseRelational(tokens);

            var expectedExpression =
                BinaryExpression(
                    NumberLiteralExpression(tokens[0]),
                    tokens[1],
                    NumberLiteralExpression(tokens[2]));
            var expectedNextTokenIndex = 3;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }

        [Fact]
        public void Assignment_Parenthesized()
        {
            var text = "a = (b = 2)";
            var tokens = Lexer.LexSubmission(text);
            var parseResult =  Parser.ParseAssignment(tokens);

            var expectedExpression =
                BinaryExpression(
                    IdentifierExpression(tokens[0]),
                    tokens[1],
                    ParenthesizedExpression(
                        tokens[2],
                        BinaryExpression(
                            IdentifierExpression(tokens[3]),
                            tokens[4],
                            NumberLiteralExpression(tokens[5])),
                        tokens[6]));
            var expectedNextTokenIndex = 7;

            Assert.Equal(expectedExpression, parseResult.Expression);
            Assert.Equal(expectedNextTokenIndex, parseResult.NextTokenIndex);
        }
    }
}
