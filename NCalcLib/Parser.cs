using System;
using System.Collections.Immutable;

namespace NCalcLib
{
    public class Parser
    {
        public static Expression ParseExpressionSubmission(ImmutableArray<Token> tokens)
        {
            var start = 0;
            var result = ParseExpression(tokens, start);

            if (result != null
                && result.NextTokenIndex == tokens.Length - 1
                && tokens[result.NextTokenIndex].Type == TokenType.EndOfInput)
            {
                return result.Node;
            }

            return null;
        }

        public static Statement ParseStatementSubmission(ImmutableArray<Token> tokens)
        {
            var start = 0;
            var result = ParseStatement(tokens, start);

            if (result != null
                && result.NextTokenIndex == tokens.Length - 1
                && tokens[result.NextTokenIndex].Type == TokenType.EndOfInput)
            {
                return result.Node;
            }

            return null;
        }

        public static ParseResult<Expression> ParseNumberLiteral(ImmutableArray<Token> tokens, int start = 0) =>
            start < tokens.Length && tokens[start].Type == TokenType.NumberLiteral
            ? new ParseResult<Expression>(new NumberLiteralExpression(tokens[start]), start + 1)
            : null;

        public static ParseResult<Expression> ParseNegationExpression(ImmutableArray<Token> tokens, int start = 0)
        {
            Token operatorToken = GetNextToken(tokens, start, TokenType.Minus);
            if (operatorToken != null)
            {
                start = start + 1;
            }

            var subExpressionParseResult = ParseOperandExpression(tokens, start);
            if (subExpressionParseResult != null)
            {
                if (operatorToken != null)
                {
                    return new ParseResult<Expression>(new NegationExpression(operatorToken, subExpressionParseResult.Node), subExpressionParseResult.NextTokenIndex);
                }

                return subExpressionParseResult;
            }

            return null;
        }

        public static ParseResult<Expression> ParseOperandExpression(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseNumberLiteral(tokens, start)
                ?? ParseParenthensizedExpression(tokens, start)
                ?? ParseBooleanLiteral(tokens, start)
                ?? ParseIdentifier(tokens, start);
        }

        public static ParseResult<Expression> ParseParenthensizedExpression(ImmutableArray<Token> tokens, int start = 0)
        {
            int index = start;

            Token leftParenToken = GetNextToken(tokens, index, TokenType.LeftParen);
            if (leftParenToken == null)
            {
                return null;
            }

            index = index + 1;

            var subExpressionParseResult = ParseExpression(tokens, index);
            if (subExpressionParseResult == null)
            {
                return null;
            }

            index = subExpressionParseResult.NextTokenIndex;

            Token rightParenToken = GetNextToken(tokens, index, TokenType.RightParen);
            if (rightParenToken == null)
            {
                return null;
            }

            index = index + 1;

            return new ParseResult<Expression>(new ParenthesizedExpression(leftParenToken, subExpressionParseResult.Node, rightParenToken), index);
        }

        private static Token GetNextToken(ImmutableArray<Token> tokens, int index, TokenType tokenType) =>
            index < tokens.Length && tokens[index].Type == tokenType
                ? tokens[index]
                : null;

        private static Token GetNextToken(ImmutableArray<Token> tokens, int index, params TokenType[] tokenTypes)
        {
            foreach (var tokenType in tokenTypes)
            {
                var token = GetNextToken(tokens, index, tokenType);
                if (token != null)
                {
                    return token;
                }
            }

            return null;
        }

        public static ParseResult<Expression> ParseAdditionAndSubtraction(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseLeftAssociativeBinop(tokens, start, new[] { TokenType.Plus, TokenType.Minus }, ParseMultiplicationAndDivision);
        }

        public static ParseResult<Expression> ParseEquality(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseLeftAssociativeBinop(tokens, start, new[] { TokenType.EqualEqual, TokenType.BangEqual }, ParseRelational);
        }

        private static ParseResult<Expression> ParseLeftAssociativeBinop(ImmutableArray<Token> tokens, int start, TokenType[] opTokenTypes, Func<ImmutableArray<Token>, int, ParseResult<Expression>> subExpressionParser)
        {
            var index = start;
            var expressionParseResult = subExpressionParser(tokens, index);
            if (expressionParseResult == null)
            {
                return null;
            }

            var expression = expressionParseResult.Node;
            index = expressionParseResult.NextTokenIndex;

            var op = GetNextToken(tokens, index, opTokenTypes);
            while (op != null)
            {
                index = index + 1;
                var rightHandExpressionResult = subExpressionParser(tokens, index);
                if (rightHandExpressionResult == null)
                {
                    return null;
                }

                index = rightHandExpressionResult.NextTokenIndex;
                expression = new BinaryExpression(expression, op, rightHandExpressionResult.Node);

                op = GetNextToken(tokens, index, opTokenTypes);
            }

            return new ParseResult<Expression>(expression, index);
        }

        public static ParseResult<Expression> ParseRelational(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseLeftAssociativeBinop(tokens, start, new[] { TokenType.LessThanEqual, TokenType.GreaterThanEqual, TokenType.LessThan, TokenType.GreaterThan }, ParseAdditionAndSubtraction);
        }

        public static ParseResult<Expression> ParseMultiplicationAndDivision(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseLeftAssociativeBinop(tokens, start, new[] { TokenType.Asterisk, TokenType.Slash }, ParseNegationExpression);
        }

        public static ParseResult<Expression> ParseIdentifier(ImmutableArray<Token> tokens, int start = 0) =>
            start < tokens.Length && tokens[start].Type == TokenType.Identifier
            ? new ParseResult<Expression>(new IdentifierExpression(tokens[start]), start + 1)
            : null;

        public static ParseResult<Expression> ParseAssignment(ImmutableArray<Token> tokens, int start = 0)
        {
            int index = start;
            var leftHandSideResult = ParseDeclarationExpression(tokens, index) ?? ParseIdentifier(tokens, index);
            if (leftHandSideResult != null)
            {
                index = leftHandSideResult.NextTokenIndex;
                var equalSign = GetNextToken(tokens, leftHandSideResult.NextTokenIndex, TokenType.Equal);
                if (equalSign != null)
                {
                    index = index + 1;

                    var subExpressionParseResult = ParseExpression(tokens, index);
                    if (subExpressionParseResult != null)
                    {
                        var expression = new BinaryExpression(leftHandSideResult.Node, equalSign, subExpressionParseResult.Node);
                        return new ParseResult<Expression>(expression, subExpressionParseResult.NextTokenIndex);
                    }

                    return null;
                }
            }

            return ParseEquality(tokens, start);
        }

        public static ParseResult<Expression> ParseExpression(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseAssignment(tokens, start);
        }

        public static ParseResult<Expression> ParseBooleanLiteral(ImmutableArray<Token> tokens, int start = 0)
        {
            if (start >= tokens.Length)
            {
                return null;
            }

            switch (tokens[start].Type)
            {
                case TokenType.FalseLiteral:
                case TokenType.TrueLiteral:
                    return new ParseResult<Expression>(new BooleanLiteralExpression(tokens[start]), start + 1);

                default:
                    return null;
            }
        }

        public static ParseResult<Expression> ParseDeclarationExpression(ImmutableArray<Token> tokens, int start = 0)
        {
            if (start + 2 < tokens.Length
                && tokens[start].Type == TokenType.Identifier
                && tokens[start + 1].Type == TokenType.AsKeyword
                && IsTypeToken(tokens[start + 2]))
            {
                return new ParseResult<Expression>(new DeclarationExpression(tokens[start], tokens[start + 1], tokens[start + 2]), start + 3);
            }

            return null;
        }

        private static bool IsTypeToken(Token token)
        {
            return token.Type == TokenType.BooleanKeyword
                || token.Type == TokenType.NumberKeyword;
        }

        public static ParseResult<Statement> ParseStatement(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseIf(tokens, start)
                ?? ParseExpressionStatement(tokens, start);
        }

        public static ParseResult<Statement> ParseIf(ImmutableArray<Token> tokens, int start = 0)
        {
            if (start < tokens.Length
                && tokens[start].Type == TokenType.IfKeyword)
            {
                var ifToken = tokens[start];
                start = start + 1;

                var expressionParseResult = ParseExpression(tokens, start);
                if (expressionParseResult != null)
                {
                    var blockParseResult = ParseBlock(tokens, expressionParseResult.NextTokenIndex);
                    if (blockParseResult.NextTokenIndex < tokens.Length
                        && tokens[blockParseResult.NextTokenIndex].Type == TokenType.EndKeyword)
                    {
                        var endToken = tokens[blockParseResult.NextTokenIndex];
                        return new ParseResult<Statement>(
                            new IfStatement(ifToken, expressionParseResult.Node, blockParseResult.Node, endToken),
                            blockParseResult.NextTokenIndex + 1);
                    }
                }
            }

            return null;
        }

        public static ParseResult<Statement> ParseExpressionStatement(ImmutableArray<Token> tokens, int start = 0)
        {
            var expressionParseResult = ParseExpression(tokens, start);
            if (expressionParseResult != null)
            {
                return new ParseResult<Statement>(new ExpressionStatement(expressionParseResult.Node), expressionParseResult.NextTokenIndex);
            }

            return null;
        }

        public static ParseResult<Block> ParseBlock(ImmutableArray<Token> tokens, int start = 0)
        {
            var builder = ImmutableArray.CreateBuilder<Statement>();
            var result = ParseStatement(tokens, start);
            while (result != null)
            {
                builder.Add(result.Node);
                start = result.NextTokenIndex;
                result = ParseStatement(tokens, start);
            }

            return builder.Count == 0
                ? new ParseResult<Block>(new EmptyBlock(start), start)
                : new ParseResult<Block>(new NonEmptyBlock(builder.ToImmutable()), start);
        }
    }
}
