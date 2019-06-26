using System;
using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public sealed class DeclarationStatement : Statement
    {
        public DeclarationStatement(Token identifierToken, Token asToken, Token typeToken, Token equalsToken, Expression initializationExpression)
        {
            Identifier = identifierToken;
            AsKeyword = asToken;
            Type = typeToken;
            EqualsToken = equalsToken;
            InitializationExpression = initializationExpression;
        }

        public Token Identifier { get; }
        public Token AsKeyword { get; }
        public Token Type { get; }
        public Token EqualsToken { get; }
        public Expression InitializationExpression { get; }

        public override bool Equals(object obj) => Equals(obj as DeclarationStatement);
        public override bool Equals(Statement other) =>
            other is DeclarationStatement statement
            && Identifier.Equals(statement.Identifier)
            && AsKeyword.Equals(statement.AsKeyword)
            && Type.Equals(statement.Type)
            && EqualsToken.Equals(statement.EqualsToken)
            && InitializationExpression.Equals(statement.InitializationExpression);

        public override int GetHashCode() =>
            Hash(
                Identifier.GetHashCode(),
                AsKeyword.GetHashCode(),
                Type.GetHashCode(),
                EqualsToken.GetHashCode(),
                InitializationExpression.GetHashCode());

        public override int Length() => Identifier.Length + AsKeyword.LengthWithWhitespace + Type.LengthWithWhitespace + EqualsToken.LengthWithWhitespace + InitializationExpression.LengthWithWhitespace();
        public override int LengthWithWhitespace() => Identifier.LengthWithWhitespace + AsKeyword.LengthWithWhitespace + Type.LengthWithWhitespace + EqualsToken.LengthWithWhitespace + InitializationExpression.LengthWithWhitespace();
        public override int Start() => Identifier.Start;
        public override int StartWithWhitespace() => Identifier.StartWithWhitespace;
    }
}
