using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public class NegationExpression : Expression
    {
        public NegationExpression(Token operatorToken, Expression subExpression)
            : base(operatorToken.Start, operatorToken.Length + subExpression.Length)
        {
            OperatorToken = operatorToken;
            SubExpression = subExpression;
        }

        public Token OperatorToken { get; }
        public Expression SubExpression { get; }

        public override bool Equals(object obj) => Equals(obj as Expression);
        public override bool Equals(Expression other) =>
            other is NegationExpression expression
            && OperatorToken.Equals(expression.OperatorToken)
            && SubExpression.Equals(expression.SubExpression);

        public override int GetHashCode() =>
            Hash(OperatorToken.GetHashCode(), SubExpression.GetHashCode());

        public NegationExpression WithOperator(Token newOperator)
        {
            if (OperatorToken.Equals(newOperator))
            {
                return this;
            }

            return new NegationExpression(newOperator, SubExpression);
        }

        public NegationExpression WithSubExpression(Expression newSubExpression)
        {
            if (SubExpression.Equals(newSubExpression))
            {
                return this;
            }

            return new NegationExpression(OperatorToken, newSubExpression);
        }
    }
}