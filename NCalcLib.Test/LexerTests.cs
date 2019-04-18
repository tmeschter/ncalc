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
        public void NumberLiteral_Lexes(string text, int expectedStart, int expectedLength, int expectedTokenStart, int expectedTokenLength)
        {
            var token = Lexer.LexNumberLiteral(text);

            AssertStartLengthAndType(token, TokenType.NumberLiteral, expectedStart, expectedLength, expectedTokenStart, expectedTokenLength);
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
        [InlineData("foo", 0, 3, 0, 3)]
        [InlineData("  foo", 0, 5, 2, 3)]
        [InlineData("foo1bar", 0, 7, 0, 7)]
        public void Identifier_Lexes(string text, int expectedStart, int expectedLength, int expectedTokenStart, int expectedTokenLength)
        {
            var identifierToken = Lexer.LexIdentifier(text);

            AssertStartLengthAndType(identifierToken, TokenType.Identifier, expectedStart, expectedLength, expectedTokenStart, expectedTokenLength);
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
        public void SimpleToken_Lexes(string text, string tokenText, TokenType expectedType, int expectedStart, int expectedLength, int expectedTokenStart, int expectedTokenLength)
        {
            var token = Lexer.LexSimpleToken(text, 0, tokenText, expectedType);

            AssertStartLengthAndType(token, expectedType, expectedStart, expectedLength, expectedTokenStart, expectedTokenLength);
        }

        [Theory]
        [InlineData("+", "-", TokenType.Plus)]
        public void SimpleToken_DoesNotLex(string text, string tokenText, TokenType tokenType)
        {
            var token = Lexer.LexSimpleToken(text, 0, tokenText, tokenType);

            Assert.Null(token);
        }

        [Theory]
        [InlineData("true", TokenType.TrueLiteral, 0, 4, 0, 4)]
        [InlineData("false", TokenType.FalseLiteral, 0, 5, 0, 5)]
        [InlineData("  true", TokenType.TrueLiteral, 0, 6, 2, 4)]
        public void BooleanLiteral_Lexes(string text, TokenType expectedType, int expectedStart, int expectedLength, int expectedTokenStart, int expectedTokenLength)
        {
            var token = Lexer.LexIdentifierOrKeyword(text, 0);

            AssertStartLengthAndType(token, expectedType, expectedStart, expectedLength, expectedTokenStart, expectedTokenLength);
        }

        [Theory]
        [InlineData("truefoo")]
        public void BooleanLiteral_DoesNotLex(string text)
        {
            var token = Lexer.LexIdentifierOrKeyword(text, 0);

            Assert.Equal(TokenType.Identifier, token.Type);
        }

        [Theory]
        [InlineData("as", 0, 2, 0, 2)]
        [InlineData("  as", 0, 4, 2, 2)]
        [InlineData(" as ", 0, 3, 1, 2)]
        public void AsKeyword_Lexes(string text, int expectedStart, int expectedLength, int expectedTokenStart, int expectedTokenLength)
        {
            var token = Lexer.LexIdentifierOrKeyword(text, 0);

            AssertStartLengthAndType(token, TokenType.AsKeyword, expectedStart, expectedLength, expectedTokenStart, expectedTokenLength);
        }

        [Theory]
        [InlineData("asb")]
        public void AsKeyword_DoesNotLex(string text)
        {
            var token = Lexer.LexIdentifierOrKeyword(text, 0);

            Assert.Equal(TokenType.Identifier, token.Type);
        }

        [Theory]
        [InlineData("boolean", 0, 7, 0, 7)]
        [InlineData("  boolean", 0, 9, 2, 7)]
        [InlineData(" boolean ", 0, 8, 1, 7)]
        public void BooleanKeyword_Lexes(string text, int expectedStart, int expectedLength, int expectedTokenStart, int expectedTokenLength)
        {
            var token = Lexer.LexIdentifierOrKeyword(text, 0);

            AssertStartLengthAndType(token, TokenType.BooleanKeyword, expectedStart, expectedLength, expectedTokenStart, expectedTokenLength);
        }

        [Theory]
        [InlineData("booleanb")]
        public void BooleanKeyword_DoesNotLex(string text)
        {
            var token = Lexer.LexIdentifierOrKeyword(text, 0);

            Assert.Equal(TokenType.Identifier, token.Type);
        }

        [Theory]
        [InlineData("number", 0, 6, 0, 6)]
        [InlineData("  number", 0, 8, 2, 6)]
        [InlineData(" number ", 0, 7, 1, 6)]
        public void NumberKeyword_Lexes(string text, int expectedStart, int expectedLength, int expectedTokenStart, int expectedTokenLength)
        {
            var token = Lexer.LexIdentifierOrKeyword(text, 0);

            AssertStartLengthAndType(token, TokenType.NumberKeyword, expectedStart, expectedLength, expectedTokenStart, expectedTokenLength);
        }

        [Theory]
        [InlineData("numbera")]
        public void NumberKeyword_DoesNotLex(string text)
        {
            var token = Lexer.LexIdentifierOrKeyword(text, 0);

            Assert.Equal(TokenType.Identifier, token.Type);
        }

        private static void AssertStartLengthAndType(Token token, TokenType expectedType, int expectedStart, int expectedLength, int expectedTokenStart, int expectedTokenLength)
        {
            Assert.Equal(expectedType, actual: token.Type);
            Assert.Equal(expectedStart, actual: token.Start);
            Assert.Equal(expectedLength, actual: token.Length);
            Assert.Equal(expectedTokenStart, actual: token.TokenStart);
            Assert.Equal(expectedTokenLength, actual: token.TokenLength);
        }

        private static void AssertStartAndLength(Whitespace whitespace, int expectedStart, int expectedLength)
        {
            Assert.Equal(expected: expectedStart, actual: whitespace.Start);
            Assert.Equal(expected: expectedLength, actual: whitespace.Length);
        }

        private static void AssertStartAndLength(Token token, int expectedStart, int expectedLength)
        {
            Assert.Equal(expected: expectedStart, actual: token.Start);
            Assert.Equal(expected: expectedLength, actual: token.Length);
        }

        private static void AssertTokenStartAndLength(Token token, int expectedStart, int expectedLength)
        {
            Assert.Equal(expectedStart, actual: token.TokenStart);
            Assert.Equal(expectedLength, actual: token.TokenLength);
        }
    }
}
