using System;
using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public enum TokenType
    {
        NumberLiteral,
        TrueLiteral,
        FalseLiteral,
        Plus,
        Minus,
        Asterisk,
        Slash,
        LeftParen,
        RightParen,
        LessThanEqual,
        GreaterThanEqual,
        LessThan,
        GreaterThan,
        EqualEqual,
        Equal,
        BangEqual,
        Identifier,
        AsKeyword,
        BooleanKeyword,
        NumberKeyword,
        Unknown,
        EndOfInput
    }

    public sealed class Token : IEquatable<Token>
    {
        public Token(int tokenStart, string text, TokenType type)
            : this(new Whitespace(tokenStart, ""), text, type)
        {
        }

        public Token(Whitespace whitespace, string text, TokenType type)
        {
            Whitespace = whitespace;
            Text = text;
            Type = type;
        }

        public Whitespace Whitespace { get; }
        public string Text { get; }
        public TokenType Type { get; }
        public int TokenStart => Whitespace.Start + Whitespace.Length;
        public int TokenLength => Text.Length;

        public int Start => Whitespace.Start;
        public int Length => Whitespace.Length + Text.Length;

        public bool Equals(Token other) =>
            other != null
            && Whitespace.Equals(other.Whitespace)
            && TokenStart == other.TokenStart
            && Text == other.Text
            && Type == other.Type;

        public override bool Equals(object obj) => Equals(obj as Token);

        public override int GetHashCode() =>
            Hash(
                Whitespace.GetHashCode(),
                TokenStart.GetHashCode(),
                Text.GetHashCode(),
                Type.GetHashCode());

        public override string ToString() => Text;

        public Token WithWhitespace(Whitespace newWhitespace)
        {
            if (Whitespace.Equals(newWhitespace))
            {
                return this;
            }

            return new Token(newWhitespace, Text, Type);
        }

        public Token WithText(string newText)
        {
            if (Text.Equals(newText))
            {
                return this;
            }

            return new Token(Whitespace, newText, Type);
        }

        public Token WithType(TokenType newType)
        {
            if (Type == newType)
            {
                return this;
            }

            return new Token(Whitespace, Text, newType);
        }
    }
}