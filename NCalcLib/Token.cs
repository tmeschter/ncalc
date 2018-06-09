using System;
using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public sealed class Token : Node, IEquatable<Token>
    {
        public Token(int tokenStart, string text)
            : this(new Whitespace(tokenStart, ""), text)
        {
        }

        public Token(Whitespace whitespace, string text)
            : base(whitespace.Start, whitespace.Length + text.Length)
        {
            Whitespace = whitespace;
            Text = text;
        }

        public Whitespace Whitespace { get; }
        public string Text { get; }
        public int TokenStart => Whitespace.Start + Whitespace.Length;
        public int TokenLength => Text.Length;

        public bool Equals(Token other) =>
            other != null
            && Whitespace.Equals(other.Whitespace)
            && TokenStart == other.TokenStart
            && Text == other.Text;

        public override bool Equals(object obj) => Equals(obj as Token);

        public override int GetHashCode() =>
            Hash(
                Whitespace.GetHashCode(),
                TokenStart.GetHashCode(),
                Text.GetHashCode());

        public Token WithWhitespace(Whitespace newWhitespace)
        {
            if (Whitespace.Equals(newWhitespace))
            {
                return this;
            }

            return new Token(newWhitespace, Text);
        }

        public Token WithText(string newText)
        {
            if (Text.Equals(newText))
            {
                return this;
            }

            return new Token(Whitespace, newText);
        }
    }
}