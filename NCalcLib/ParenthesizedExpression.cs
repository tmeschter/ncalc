using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public class ParenthesizedExpression : Expression
    {
        public ParenthesizedExpression(Token leftParen, Expression subExpression, Token rightParen)
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

        public override string ToString()
        {
            return $"{LeftParen}{SubExpression}{RightParen}";
        }

        public ParenthesizedExpression WithLeftParen(Token newLeftParen)
        {
            if (LeftParen.Equals(newLeftParen))
            {
                return this;
            }

            return new ParenthesizedExpression(newLeftParen, SubExpression, RightParen);
        }

        public ParenthesizedExpression WithSubExpression(Expression newSubExpression)
        {
            if (SubExpression.Equals(newSubExpression))
            {
                return this;
            }

            return new ParenthesizedExpression(LeftParen, newSubExpression, RightParen);
        }

        public ParenthesizedExpression WithRightParen(Token newRightParen)
        {
            if (RightParen.Equals(newRightParen))
            {
                return this;
            }

            return new ParenthesizedExpression(LeftParen, SubExpression, newRightParen);
        }

        public override int Start() => LeftParen.Start;
        public override int StartWithWhitespace() => LeftParen.StartWithWhitespace;
        public override int Length() => LeftParen.Length + SubExpression.LengthWithWhitespace() + RightParen.LengthWithWhitespace;
        public override int LengthWithWhitespace() => LeftParen.LengthWithWhitespace + SubExpression.LengthWithWhitespace() + RightParen.LengthWithWhitespace;
    }
}
