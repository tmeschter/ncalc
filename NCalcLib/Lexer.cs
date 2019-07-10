using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NCalcLib
{
    public sealed class Lexer
    {
        public static Token LexSimpleToken(string text, int start, string tokenText, TokenType tokenType)
        {
            var whitespace = LexWhitespace(text, start);
            var tokenStart = start + whitespace.Length;

            if (tokenStart + tokenText.Length > text.Length)
            {
                return null;
            }

            for (int i = 0; i < tokenText.Length; i++)
            {
                if (text[tokenStart + i] != tokenText[i])
                {
                    return null;
                }
            }

            return new Token(whitespace, tokenText, tokenType);
        }

        public static Whitespace LexWhitespace(string text, int start = 0)
        {
            int index = start;

            while (index < text.Length
                && char.IsWhiteSpace(text[index]))
            {
                index++;
            }

            return new Whitespace(start, text.Substring(start, index - start));
        }

        public static Token LexEndOfInput(string text, int start)
        {
            var whitespace = LexWhitespace(text, start);

            if (start + whitespace.Length != text.Length)
            {
                return null;
            }

            return new Token(whitespace, string.Empty, TokenType.EndOfInput);
        }

        public static Token LexIdentifier(string text, int start = 0)
        {
            var whitespace = LexWhitespace(text, start);
            var tokenStart = start + whitespace.Length;

            int length = IdentifierLength(text, tokenStart);
            if (length == 0)
            {
                return null;
            }

            return new Token(whitespace, text.Substring(tokenStart, length), TokenType.Identifier);
        }

        private static int IdentifierLength(string text, int start)
        {
            int index = start;
            if (index < text.Length
                && char.IsLetter(text[index]))
            {
                index++;
            }
            else
            {
                return 0;
            }

            while (index < text.Length
                && char.IsLetterOrDigit(text[index]))
            {
                index++;
            }

            return index - start;
        }

        public static Token LexIdentifierOrKeyword(string text, int start)
        {
            var identifierToken = LexIdentifier(text, start);
            if (identifierToken == null)
            {
                return null;
            }

            switch (identifierToken.Text)
            {
                case "true": return identifierToken.WithType(TokenType.TrueLiteral);
                case "false": return identifierToken.WithType(TokenType.FalseLiteral);
                case "as": return identifierToken.WithType(TokenType.AsKeyword);
                case "boolean": return identifierToken.WithType(TokenType.BooleanKeyword);
                case "number": return identifierToken.WithType(TokenType.NumberKeyword);
                case "if": return identifierToken.WithType(TokenType.IfKeyword);
                case "else": return identifierToken.WithType(TokenType.ElseKeyword);
                case "end": return identifierToken.WithType(TokenType.EndKeyword);
                case "while": return identifierToken.WithType(TokenType.WhileKeyword);
                case "and": return identifierToken.WithType(TokenType.AndKeyword);
                case "or": return identifierToken.WithType(TokenType.OrKeyword);
                default: return identifierToken;
            }
        }

        public static Token LexNumberLiteral(string text, int start = 0)
        {
            var whitespace = LexWhitespace(text, start);
            start = start + whitespace.Length;

            int index = start;
            while (index < text.Length
                && char.IsDigit(text[index]))
            {
                index++;
            }

            if (index < text.Length
                && text[index] == '.')
            {
                index++;

                while (index < text.Length
                    && char.IsDigit(text[index]))
                {
                    index++;
                }

                if (text[index - 1] == '.')
                {
                    return null;
                }
            }

            int length = index - start;
            if (length == 0)
            {
                return null;
            }

            return new Token(whitespace, text.Substring(start, length), TokenType.NumberLiteral);
        }

        public static Token LexNextToken(string text, int start = 0)
        {
            return LexSimpleToken(text, start, "==", TokenType.EqualEqual)
                ?? LexSimpleToken(text, start, "=", TokenType.Equal)
                ?? LexSimpleToken(text, start, "!=", TokenType.BangEqual)
                ?? LexSimpleToken(text, start, ">=", TokenType.GreaterThanEqual)
                ?? LexSimpleToken(text, start, ">", TokenType.GreaterThan)
                ?? LexSimpleToken(text, start, "<=", TokenType.LessThanEqual)
                ?? LexSimpleToken(text, start, "<", TokenType.LessThan)
                ?? LexSimpleToken(text, start, "+", TokenType.Plus)
                ?? LexSimpleToken(text, start, "-", TokenType.Minus)
                ?? LexSimpleToken(text, start, "*", TokenType.Asterisk)
                ?? LexSimpleToken(text, start, "/", TokenType.Slash)
                ?? LexSimpleToken(text, start, "(", TokenType.LeftParen)
                ?? LexSimpleToken(text, start, ")", TokenType.RightParen)
                ?? LexNumberLiteral(text, start)
                ?? LexIdentifierOrKeyword(text, start)
                ?? LexEndOfInput(text, start);
        }

        public static ImmutableArray<Token> LexSubmission(string submission)
        {
            var tokens = ImmutableArray.CreateBuilder<Token>();

            int index = 0;
            Token token;
            bool done = false;
            while ((token = LexNextToken(submission, index)) != null
                && !done)
            {
                if (token.Type == TokenType.EndOfInput)
                {
                    done = true;
                }

                tokens.Add(token);
                index = index + token.LengthWithWhitespace;
            }

            return tokens.ToImmutable();
        }
    }
}
