using System;
using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public sealed class DeclarationExpression : Expression
    {
        public DeclarationExpression(Token identifierToken, Token asToken, Token typeToken)
        {
            Identifier = identifierToken;
            AsKeyword = asToken;
            Type = typeToken;
        }

        public Token Identifier { get; }
        public Token AsKeyword { get; }
        public Token Type { get; }

        public override bool Equals(object obj) => Equals(obj as DeclarationExpression);
        public override bool Equals(Expression other) =>
            other is DeclarationExpression expression
            && Identifier.Equals(expression.Identifier)
            && AsKeyword.Equals(expression.AsKeyword)
            && Type.Equals(expression.Type);

        public override int GetHashCode() =>
            Hash(
                Identifier.GetHashCode(),
                AsKeyword.GetHashCode(),
                Type.GetHashCode());
    }
}
