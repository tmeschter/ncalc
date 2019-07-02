using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public class WhileStatement : Statement
    {
        public WhileStatement(Token whileToken, Expression condition, Block bodyBlock, Token endToken)
        {
            this.WhileToken = whileToken;
            this.Condition = condition;
            this.BodyBlock = bodyBlock;
            this.EndToken = endToken;
        }

        public Token WhileToken { get; }
        public Expression Condition { get; }
        public Block BodyBlock { get; }
        public Token EndToken { get; }

        public override bool Equals(Statement other) =>
            other is WhileStatement statement
            && WhileToken.Equals(statement.WhileToken)
            && Condition.Equals(statement.Condition)
            && BodyBlock.Equals(statement.BodyBlock)
            && EndToken.Equals(statement.EndToken);

        public override bool Equals(object obj) => Equals(obj as Statement);

        public override int GetHashCode() =>
            Hash(
                WhileToken.GetHashCode(),
                Condition.GetHashCode(),
                BodyBlock.GetHashCode(),
                EndToken.GetHashCode());

        public override int Length() =>
            WhileToken.Length
            + Condition.LengthWithWhitespace()
            + BodyBlock.LengthWithWhitespace()
            + EndToken.LengthWithWhitespace;

        public override int LengthWithWhitespace() => 
            WhileToken.LengthWithWhitespace
            + Condition.LengthWithWhitespace()
            + BodyBlock.LengthWithWhitespace()
            + EndToken.LengthWithWhitespace;

        public override int Start() => WhileToken.Start;

        public override int StartWithWhitespace() => WhileToken.StartWithWhitespace;
    }
}