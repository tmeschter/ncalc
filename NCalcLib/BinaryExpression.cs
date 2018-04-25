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
    }
}