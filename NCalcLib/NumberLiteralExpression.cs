using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public class NumberLiteralExpression : Expression
    {
        public NumberLiteralExpression(Token token)
            : base(token.Start, token.Length)
        {
            Token = token;
        }

        public Token Token { get; }
        public int Value => int.Parse(Token.Text);

        public override bool Equals(object obj) => Equals(obj as Expression);
        public override bool Equals(Expression other) =>
            other is NumberLiteralExpression expression
            && Token.Equals(expression.Token);

        public override int GetHashCode() =>
            Hash(Token.GetHashCode());
    }
}