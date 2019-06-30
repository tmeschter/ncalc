using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public class IfElseStatement : Statement
    {
        public IfElseStatement(Token ifToken, Expression condition, Block trueBlock, Token elseToken, Block falseBlock, Token endToken)
        {
            this.IfToken = ifToken;
            this.Condition = condition;
            this.TrueBlock = trueBlock;
            this.ElseToken = elseToken;
            this.FalseBlock = falseBlock;
            this.EndToken = endToken;
        }

        public Token IfToken { get; }
        public Expression Condition { get; }
        public Block TrueBlock { get; }
        public Token ElseToken { get; }
        public Block FalseBlock { get; }
        public Token EndToken { get; }

        public override bool Equals(Statement other) =>
            other is IfElseStatement statement
            && IfToken.Equals(statement.IfToken)
            && Condition.Equals(statement.Condition)
            && TrueBlock.Equals(statement.TrueBlock)
            && ElseToken.Equals(statement.ElseToken)
            && FalseBlock.Equals(statement.FalseBlock)
            && EndToken.Equals(statement.EndToken);

        public override bool Equals(object obj) => Equals(obj as Statement);

        public override int GetHashCode() => 
            Hash(
                IfToken.GetHashCode(),
                Condition.GetHashCode(),
                TrueBlock.GetHashCode(),
                ElseToken.GetHashCode(),
                FalseBlock.GetHashCode(),
                EndToken.GetHashCode());

        public override int Length() => 
            IfToken.Length
            + Condition.LengthWithWhitespace()
            + TrueBlock.LengthWithWhitespace()
            + ElseToken.LengthWithWhitespace
            + FalseBlock.LengthWithWhitespace()
            + EndToken.LengthWithWhitespace;

        public override int LengthWithWhitespace() =>
            IfToken.LengthWithWhitespace
            + Condition.LengthWithWhitespace()
            + TrueBlock.LengthWithWhitespace()
            + ElseToken.LengthWithWhitespace
            + FalseBlock.LengthWithWhitespace()
            + EndToken.LengthWithWhitespace;

        public override int Start() => IfToken.Start;

        public override int StartWithWhitespace() => IfToken.StartWithWhitespace;
    }
}