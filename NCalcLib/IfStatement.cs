using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public class IfStatement : Statement
    {
        public IfStatement(Token ifToken, Expression condition, Block trueBlock, Token endToken)
        {
            IfToken = ifToken;
            Condition = condition;
            TrueBlock = trueBlock;
            EndToken = endToken;
        }

        public Token IfToken { get; }
        public Expression Condition { get; }
        public Block TrueBlock { get; }
        public Token EndToken { get; }

        public override bool Equals(Statement other) =>
            other is IfStatement statement
            && IfToken.Equals(statement.IfToken)
            && Condition.Equals(statement.Condition)
            && TrueBlock.Equals(statement.TrueBlock)
            && EndToken.Equals(statement.EndToken);
        public override bool Equals(object obj) => Equals(obj as Statement);

        public override int GetHashCode() =>
            Hash(
                IfToken.GetHashCode(),
                Condition.GetHashCode(),
                TrueBlock.GetHashCode(),
                EndToken.GetHashCode());

        public override int Length() => IfToken.Length + Condition.LengthWithWhitespace() + TrueBlock.LengthWithWhitespace() + EndToken.LengthWithWhitespace;
        public override int LengthWithWhitespace() => IfToken.LengthWithWhitespace + Condition.LengthWithWhitespace() + TrueBlock.LengthWithWhitespace() + EndToken.LengthWithWhitespace;
        public override int Start() => IfToken.Start;
        public override int StartWithWhitespace() => IfToken.StartWithWhitespace;
    }
}