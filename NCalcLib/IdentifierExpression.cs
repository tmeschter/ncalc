using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public class IdentifierExpression : Expression
    {
        public IdentifierExpression(Token token)
        {
            Token = token;
        }

        public Token Token { get; }
        public string Id => Token.Text;

        public override bool Equals(object obj) => Equals(obj as Expression);
        public override bool Equals(Expression other) =>
            other is IdentifierExpression expression
            && Token.Equals(expression.Token);

        public override int GetHashCode() => Token.GetHashCode();

        public override int Length() => Token.Length;
        public override int LengthWithWhitespace() => Token.LengthWithWhitespace;
        public override int Start() => Token.Start;
        public override int StartWithWhitespace() => Token.StartWithWhitespace;

        public override string ToString() => Id;
    }
}