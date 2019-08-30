using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NCalcLib.Test
{
    public class LexerTests
    {
        [Fact]
        public void Submission_NumberLiteralToken()
        {
            var text = "5";
            var tokens = Lexer.LexSubmission(text);

            Assert.Equal(expected: 2, actual: tokens.Length);
            Assert.Equal(expected: TokenType.NumberLiteral, actual: tokens[0].Type);
            Assert.Equal(expected: TokenType.EndOfInput, actual: tokens[1].Type);
        }

        [Theory]
        [InlineData("5", 0, 1, 0, 1)]
        [InlineData("5.4", 0, 3, 0, 3)]
        [InlineData("25", 0, 2, 0, 2)]
        [InlineData("  456", 0, 5, 2, 3)]
        public void NumberLiteral_Lexes(string text, int expectedStartWithWhitespace, int expectedLengthWithWhitespace, int expectedStart, int expectedLength)
        {
            var token = Lexer.LexNumberLiteral(text);

            AssertStartLengthAndType(token, TokenType.NumberLiteral, expectedStartWithWhitespace, expectedLengthWithWhitespace, expectedStart, expectedLength);
        }

        [Theory]
        [InlineData("5.")]
        public void NumberLiteral_DoesNotLex(string text)
        {
            var token = Lexer.LexNumberLiteral(text);

            Assert.Null(token);
        }

        [Theory]
        [InlineData("", 0, 0)]
        [InlineData(" ", 0, 1)]
        [InlineData("   ", 0, 3)]
        public void Whitespace_Lexes(string text, int expectedStart, int expectedLength)
        {
            var whitespace = Lexer.LexWhitespace(text);

            AssertStartAndLength(whitespace, expectedStart, expectedLength);
        }

        [Theory]
        [InlineData("1foo")]
        public void Identifier_DoesNotLex(string text)
        {
            var identifierToken = Lexer.LexIdentifier(text);

            Assert.Null(identifierToken);
        }

        [Theory]
        [InlineData("+", "+", TokenType.Plus, 0, 1, 0, 1)]
        [InlineData(" +", "+", TokenType.Plus, 0, 2, 1, 1)]
        [InlineData(" + ", "+", TokenType.Plus, 0, 2, 1, 1)]
        public void SimpleToken_Lexes(string text, string tokenText, TokenType expectedType, int expectedStartWithWhitespace, int expectedLengthWithWhitespace, int expectedStart, int expectedLength)
        {
            var token = Lexer.LexSimpleToken(text, 0, tokenText, expectedType);

            AssertStartLengthAndType(token, expectedType, expectedStartWithWhitespace, expectedLengthWithWhitespace, expectedStart, expectedLength);
        }

        [Theory]
        [InlineData("+", "-", TokenType.Plus)]
        public void SimpleToken_DoesNotLex(string text, string tokenText, TokenType tokenType)
        {
            var token = Lexer.LexSimpleToken(text, 0, tokenText, tokenType);

            Assert.Null(token);
        }

        [Theory]
        [InlineData("and", TokenType.AndKeyword)]
        [InlineData("andfoo", TokenType.Identifier)]
        [InlineData("as", TokenType.AsKeyword)]
        [InlineData("asfoo", TokenType.Identifier)]
        [InlineData("boolean", TokenType.BooleanKeyword)]
        [InlineData("booleanfoo", TokenType.Identifier)]
        [InlineData("else", TokenType.ElseKeyword)]
        [InlineData("elsefoo", TokenType.Identifier)]
        [InlineData("false", TokenType.FalseLiteral)]
        [InlineData("falsefoo", TokenType.Identifier)]
        [InlineData("foo", TokenType.Identifier)]
        [InlineData("foo1bar", TokenType.Identifier)]
        [InlineData("if", TokenType.IfKeyword)]
        [InlineData("iffoo", TokenType.Identifier)]
        [InlineData("number", TokenType.NumberKeyword)]
        [InlineData("numberfoo", TokenType.Identifier)]
        [InlineData("end", TokenType.EndKeyword)]
        [InlineData("endfoo", TokenType.Identifier)]
        [InlineData("or", TokenType.OrKeyword)]
        [InlineData("orfoo", TokenType.Identifier)]
        [InlineData("string", TokenType.StringKeyword)]
        [InlineData("stringfoo", TokenType.Identifier)]
        [InlineData("true", TokenType.TrueLiteral)]
        [InlineData("truefoo", TokenType.Identifier)]
        [InlineData("while", TokenType.WhileKeyword)]
        [InlineData("whilefoo", TokenType.Identifier)]
        public void IdentifierOrKeyword_Lexes(string text, TokenType expectedType)
        {
            for (var precedingWhitespaceLength = 0; precedingWhitespaceLength <= 2; precedingWhitespaceLength++)
            {
                var textWithWhitespace = new string(' ', precedingWhitespaceLength) + text;

                var expectedStartWithWhitespace = 0;
                var expectedLengthWithWhitespace = textWithWhitespace.Length;
                var expectedStart = precedingWhitespaceLength;
                var expectedLength = text.Length;

                var token = Lexer.LexIdentifierOrKeyword(textWithWhitespace, 0);

                AssertStartLengthAndType(token, expectedType, expectedStartWithWhitespace, expectedLengthWithWhitespace, expectedStart, expectedLength);
            }
        }

        [Theory]
        [InlineData("\"foo\"")]
        [InlineData("\"alpha beta\"")]
        [InlineData("\"alpha\r\nbeta\"")]
        public void StringLiteral_Lexes(string text)
        {
            for (var precedingWhitespaceLength = 0; precedingWhitespaceLength <= 2; precedingWhitespaceLength++)
            {
                var textWithWhitespace = new string(' ', precedingWhitespaceLength) + text;
                var token = Lexer.LexStringLiteral(textWithWhitespace);

                var expectedStartWithWhitespace = 0;
                var expectedLengthWithWhitespace = textWithWhitespace.Length;
                var expectedStart = precedingWhitespaceLength;
                var expectedLength = text.Length;

                Assert.Equal(expected: text, actual: token.Text);
                AssertStartLengthAndType(token, TokenType.StringLiteral, expectedStartWithWhitespace, expectedLengthWithWhitespace, expectedStart, expectedLength);
            }
        }

        [Theory]
        [InlineData("\"foo")]
        [InlineData("foo\"")]
        public void StringLiteral_DoesNotLex(string text)
        {
            var token = Lexer.LexStringLiteral(text);

            Assert.Null(token);
        }

        private static void AssertStartLengthAndType(Token token, TokenType expectedType, int expectedStartWithWhitespace, int expectedLengthWithWhitespace, int expectedStart, int expectedLength)
        {
            Assert.Equal(expectedType, actual: token.Type);
            Assert.Equal(expectedStartWithWhitespace, actual: token.StartWithWhitespace);
            Assert.Equal(expectedLengthWithWhitespace, actual: token.LengthWithWhitespace);
            Assert.Equal(expectedStart, actual: token.Start);
            Assert.Equal(expectedLength, actual: token.Length);
        }

        private static void AssertStartAndLength(Whitespace whitespace, int expectedStart, int expectedLength)
        {
            Assert.Equal(expected: expectedStart, actual: whitespace.Start);
            Assert.Equal(expected: expectedLength, actual: whitespace.Length);
        }

        private static void AssertStartAndLength(Token token, int expectedStartWithWhitepsace, int expectedLengthWithWhitespace)
        {
            Assert.Equal(expected: expectedStartWithWhitepsace, actual: token.StartWithWhitespace);
            Assert.Equal(expected: expectedLengthWithWhitespace, actual: token.LengthWithWhitespace);
        }

        private static void AssertTokenStartAndLength(Token token, int expectedStart, int expectedLength)
        {
            Assert.Equal(expectedStart, actual: token.Start);
            Assert.Equal(expectedLength, actual: token.Length);
        }
    }
}
