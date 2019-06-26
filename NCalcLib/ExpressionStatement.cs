namespace NCalcLib
{
    public class ExpressionStatement : Statement
    {
        public ExpressionStatement(Expression expression)
        {
            Expression = expression;
        }

        public Expression Expression { get; }

        public override bool Equals(object obj) => Equals(obj as Statement);
        public override bool Equals(Statement other) =>
            other is ExpressionStatement expressionStatement
            && Expression.Equals(expressionStatement.Expression);


        public override int GetHashCode() => Expression.GetHashCode();

        public override int Length() => Expression.Length();
        public override int LengthWithWhitespace() => Expression.LengthWithWhitespace();
        public override int Start() => Expression.Start();
        public override int StartWithWhitespace() => Expression.StartWithWhitespace();

        public override string ToString() => Expression.ToString();
    }
}