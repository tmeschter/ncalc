﻿using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public sealed class BooleanLiteralExpression : Expression
    {
        public BooleanLiteralExpression(Token token)
        {
            Token = token;
        }

        public Token Token { get; }
        public bool Value => bool.Parse(Token.Text);

        public override bool Equals(object obj) => Equals(obj as Expression);
        public override bool Equals(Expression other) =>
            other is BooleanLiteralExpression expression
            && Token.Equals(expression.Token);

        public override int GetHashCode() => Token.GetHashCode();

        public override int Length() => Token.Length;
        public override int LengthWithWhitespace() => Token.LengthWithWhitespace;
        public override int Start() => Token.Start;
        public override int StartWithWhitespace() => Token.StartWithWhitespace;

        public override string ToString() => Value.ToString();
    }
}
