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

        public override bool Equals(Statement other)
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }

        public override int Length()
        {
            throw new System.NotImplementedException();
        }

        public override int LengthWithWhitespace()
        {
            throw new System.NotImplementedException();
        }

        public override int Start()
        {
            throw new System.NotImplementedException();
        }

        public override int StartWithWhitespace()
        {
            throw new System.NotImplementedException();
        }
    }
}