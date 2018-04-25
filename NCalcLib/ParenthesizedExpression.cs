using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public class ParenthesizedExpression : Expression
    {
        public ParenthesizedExpression(Token leftParen, Expression subExpression, Token rightParen)
            : base(leftParen.Start, leftParen.Length + subExpression.Length + rightParen.Length)
        {
            LeftParen = leftParen;
            SubExpression = subExpression;
            RightParen = rightParen;
        }

        public Token LeftParen { get; }
        public Expression SubExpression { get; }
        public Token RightParen { get; }

        public override bool Equals(object obj) => Equals(obj as Expression);
        public override bool Equals(Expression other) =>
            other is ParenthesizedExpression expression
            && LeftParen.Equals(expression.LeftParen)
            && SubExpression.Equals(expression.SubExpression)
            && RightParen.Equals(expression.RightParen);

        public override int GetHashCode() =>
            Hash(
                LeftParen.GetHashCode(),
                SubExpression.GetHashCode(),
                RightParen.GetHashCode());
    }
}
