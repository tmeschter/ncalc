using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public class BinaryExpression : Expression
    {
        public BinaryExpression(Expression left, Token op, Expression right)
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

        public override int Length() => Left.Length() + Operator.LengthWithWhitespace + Right.LengthWithWhitespace();
        public override int LengthWithWhitespace() => Left.LengthWithWhitespace() + Operator.LengthWithWhitespace + Right.LengthWithWhitespace();
        public override int Start() => Left.Start();
        public override int StartWithWhitespace() => Left.StartWithWhitespace();

        public override string ToString() => $"[{Left}{Operator}{Right}]";

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