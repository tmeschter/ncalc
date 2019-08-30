using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCalcLib
{
    public class StringLiteralExpression : Expression
    {
        public StringLiteralExpression(Token token)
        {
            Token = token;
        }

        public Token Token { get; }

        public string Value => Token.Text.Substring(1, Token.Text.Length - 2);

        public override bool Equals(object obj) => Equals(obj as Expression);
        public override bool Equals(Expression other) =>
            other is StringLiteralExpression expression
            && Token.Equals(expression.Token);

        public override int GetHashCode() => Token.GetHashCode();

        public override int Length() => Token.Length;
        public override int LengthWithWhitespace() => Token.LengthWithWhitespace;
        public override int Start() => Token.Start;
        public override int StartWithWhitespace() => Token.StartWithWhitespace;

        public override string ToString() => Token.Text;
    }
}
