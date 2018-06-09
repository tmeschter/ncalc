using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public class IdentifierExpression : Expression
    {
        public IdentifierExpression(Token token)
            : base(token.Start, token.Length)
        {
            Token = token;
        }

        public Token Token { get; }
        public string Id => Token.Text;

        public override bool Equals(object obj) => Equals(obj as Expression);
        public override bool Equals(Expression other) =>
            other is IdentifierExpression expression
            && Token.Equals(expression.Token);

        public override int GetHashCode() =>
            Hash(Token.GetHashCode());
    }
}