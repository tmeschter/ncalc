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
        EndOfInput,
        IfKeyword,
        ElseKeyword,
        EndKeyword,
        WhileKeyword
    }

    public sealed class Token : IEquatable<Token>
    {
        public Token(int start, string text, TokenType type)
            : this(new Whitespace(start, ""), text, type)
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
        public int Start => Whitespace.Start + Whitespace.Length;
        public int Length => Text.Length;

        public int StartWithWhitespace => Whitespace.Start;
        public int LengthWithWhitespace => Whitespace.Length + Text.Length;

        public bool Equals(Token other) =>
            other != null
            && Whitespace.Equals(other.Whitespace)
            && Start == other.Start
            && Text == other.Text
            && Type == other.Type;

        public override bool Equals(object obj) => Equals(obj as Token);

        public override int GetHashCode() =>
            Hash(
                Whitespace.GetHashCode(),
                Start.GetHashCode(),
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