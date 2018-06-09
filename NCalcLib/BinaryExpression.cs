using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public class BinaryExpression : Expression
    {
        public BinaryExpression(Expression left, Token op, Expression right)
            :base(left.Start, left.Length + op.Length + right.Length)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public Expression Left { get; }
        public Token Operator { get; }
        public Expression Right { get; }

        public override bool Equals(object obj) => Equals(obj as Expression);
        public override bool Equals(Expression other) =>
            other is BinaryExpression expression
            && Left.Equals(expression.Left)
            && Operator.Equals(expression.Operator)
            && Right.Equals(expression.Right);

        public override int GetHashCode() =>
            Hash(
                Left.GetHashCode(),
                Operator.GetHashCode(),
                Right.GetHashCode());

        public BinaryExpression WithLeft(Expression newLeft)
        {
            if (Left.Equals(newLeft))
            {
                return this;
            }

            return new BinaryExpression(newLeft, Operator, Right);
        }

        public BinaryExpression WithOperator(Token newOperator)
        {
            if (Operator.Equals(newOperator))
            {
                return this;
            }

            return new BinaryExpression(Left, newOperator, Right);
        }

        public BinaryExpression WithRight(Expression newRight)
        {
            if (Right.Equals(newRight))
            {
                return this;
            }

            return new BinaryExpression(Left, Operator, newRight);
        }
    }
}