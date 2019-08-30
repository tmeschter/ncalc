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
            GetNextToken(tokens, start, TokenType.NumberLiteral) is Token token
            ? new ParseResult<Expression>(new NumberLiteralExpression(token), start + 1)
            : null;

        public static ParseResult<Expression> ParseStringLiteral(ImmutableArray<Token> tokens, int start = 0) =>
            GetNextToken(tokens, start, TokenType.StringLiteral) is Token token
            ? new ParseResult<Expression>(new StringLiteralExpression(token), start + 1)
            : null;

        public static ParseResult<Expression> ParseNegationExpression(ImmutableArray<Token> tokens, int start = 0)
        {
            Token operatorToken = GetNextToken(tokens, start, TokenType.Minus);
            if (operatorToken != null)
            {
                start = start + 1;
            }

            if (ParseOperandExpression(tokens, start) is ParseResult<Expression> subExpressionParseResult)
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
                ?? ParseStringLiteral(tokens, start)
                ?? ParseParenthensizedExpression(tokens, start)
                ?? ParseBooleanLiteral(tokens, start)
                ?? ParseIdentifier(tokens, start);
        }

        public static ParseResult<Expression> ParseParenthensizedExpression(ImmutableArray<Token> tokens, int start = 0)
        {
            if (GetNextToken(tokens, start, TokenType.LeftParen) is Token leftParenToken
                && ParseExpression(tokens, start + 1) is ParseResult<Expression> subExpressionParseResult
                && GetNextToken(tokens, subExpressionParseResult.NextTokenIndex, TokenType.RightParen) is Token rightParenToken)
            {
                return new ParseResult<Expression>(
                    new ParenthesizedExpression(leftParenToken, subExpressionParseResult.Node, rightParenToken),
                    subExpressionParseResult.NextTokenIndex + 1);
            }

            return null;
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
            var nextTokenIndex = start;

            if (!(subExpressionParser(tokens, nextTokenIndex) is ParseResult<Expression> expressionParseResult))
            {
                return null;
            }

            var expression = expressionParseResult.Node;
            nextTokenIndex = expressionParseResult.NextTokenIndex;

            while (GetNextToken(tokens, nextTokenIndex, opTokenTypes) is Token op)
            {
                nextTokenIndex = nextTokenIndex + 1;
                if (!(subExpressionParser(tokens, nextTokenIndex) is ParseResult<Expression> rightHandExpressionResult))
                {
                    return null;
                }

                nextTokenIndex = rightHandExpressionResult.NextTokenIndex;
                expression = new BinaryExpression(expression, op, rightHandExpressionResult.Node);
            }

            return new ParseResult<Expression>(expression, nextTokenIndex);
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
            GetNextToken(tokens, start, TokenType.Identifier) is Token identifierToken
            ? new ParseResult<Expression>(new IdentifierExpression(identifierToken), start + 1)
            : null;

        public static ParseResult<Expression> ParseAssignment(ImmutableArray<Token> tokens, int start = 0)
        {
            int index = start;
            var leftHandSideResult = ParseIdentifier(tokens, index);
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

            return ParseConditionalOr(tokens, start);
        }

        public static ParseResult<Expression> ParseExpression(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseAssignment(tokens, start);
        }

        public static ParseResult<Expression> ParseBooleanLiteral(ImmutableArray<Token> tokens, int start = 0)
        {
            return GetNextToken(tokens, start, TokenType.FalseLiteral, TokenType.TrueLiteral) is Token booleanLiteralToken
                ? new ParseResult<Expression>(new BooleanLiteralExpression(booleanLiteralToken), start + 1)
                : null;
        }

        public static ParseResult<Statement> ParseStatement(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseIfElse(tokens, start)
                ?? ParseIf(tokens, start)
                ?? ParseDeclaration(tokens, start)
                ?? ParseExpressionStatement(tokens, start)
                ?? ParseWhile(tokens, start);
        }

        public static ParseResult<Statement> ParseWhile(ImmutableArray<Token> tokens, int start = 0)
        {
            if (GetNextToken(tokens, start, TokenType.WhileKeyword) is Token whileToken
                && ParseExpression(tokens, start + 1) is ParseResult<Expression> expressionParseResult
                && ParseBlock(tokens, expressionParseResult.NextTokenIndex) is ParseResult<Block> bodyBlockParseResult
                && GetNextToken(tokens, bodyBlockParseResult.NextTokenIndex, TokenType.EndKeyword) is Token endToken)
            {
                return new ParseResult<Statement>(
                    new WhileStatement(whileToken, expressionParseResult.Node, bodyBlockParseResult.Node, endToken),
                    bodyBlockParseResult.NextTokenIndex + 1);
            }

            return null;
        }

        public static ParseResult<Statement> ParseIfElse(ImmutableArray<Token> tokens, int start = 0)
        {
            if (GetNextToken(tokens, start, TokenType.IfKeyword) is Token ifToken
                && ParseExpression(tokens, start + 1) is ParseResult<Expression> expressionParseResult
                && ParseBlock(tokens, expressionParseResult.NextTokenIndex) is ParseResult<Block> trueBlockParseResult
                && GetNextToken(tokens, trueBlockParseResult.NextTokenIndex, TokenType.ElseKeyword) is Token elseToken
                && ParseBlock(tokens, trueBlockParseResult.NextTokenIndex + 1) is ParseResult<Block> falseBlockParseResult
                && GetNextToken(tokens, falseBlockParseResult.NextTokenIndex, TokenType.EndKeyword) is Token endToken)
            {
                return new ParseResult<Statement>(
                    new IfElseStatement(ifToken, expressionParseResult.Node, trueBlockParseResult.Node, elseToken, falseBlockParseResult.Node, endToken),
                    falseBlockParseResult.NextTokenIndex + 1);
            }

            return null;
        }

        public static ParseResult<Statement> ParseIf(ImmutableArray<Token> tokens, int start = 0)
        {
            if (GetNextToken(tokens, start, TokenType.IfKeyword) is Token ifToken
                && ParseExpression(tokens, start + 1) is ParseResult<Expression> expressionParseResult
                && ParseBlock(tokens, expressionParseResult.NextTokenIndex) is ParseResult<Block> blockParseResult
                && GetNextToken(tokens, blockParseResult.NextTokenIndex, TokenType.EndKeyword) is Token endToken)
            {
                return new ParseResult<Statement>(
                    new IfStatement(ifToken, expressionParseResult.Node, blockParseResult.Node, endToken),
                     blockParseResult.NextTokenIndex + 1);
            }

            return null;
        }

        public static ParseResult<Statement> ParseDeclaration(ImmutableArray<Token> tokens, int start = 0)
        {
            if (GetNextToken(tokens, start, TokenType.Identifier) is Token identifierToken
                && GetNextToken(tokens, start + 1, TokenType.AsKeyword) is Token asToken
                && GetNextToken(tokens, start + 2, TokenType.BooleanKeyword, TokenType.NumberKeyword, TokenType.StringKeyword) is Token typeToken
                && GetNextToken(tokens, start + 3, TokenType.Equal) is Token equalToken
                && ParseExpression(tokens, start + 4) is ParseResult<Expression> expressionParseResult)
            {
                return new ParseResult<Statement>(
                    new DeclarationStatement(identifierToken, asToken, typeToken, equalToken, expressionParseResult.Node),
                    expressionParseResult.NextTokenIndex);
            }

            return null;
        }

        public static ParseResult<Statement> ParseExpressionStatement(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseExpression(tokens, start) is ParseResult<Expression> expressionParseResult
                ? new ParseResult<Statement>(new ExpressionStatement(expressionParseResult.Node), expressionParseResult.NextTokenIndex)
                : null;
        }

        public static ParseResult<Block> ParseBlock(ImmutableArray<Token> tokens, int start = 0)
        {
            var builder = ImmutableArray.CreateBuilder<Statement>();

            while (ParseStatement(tokens, start) is ParseResult<Statement> result)
            {
                builder.Add(result.Node);
                start = result.NextTokenIndex;
            }

            return builder.Count == 0
                ? new ParseResult<Block>(new EmptyBlock(tokens[start].StartWithWhitespace), start)
                : new ParseResult<Block>(new NonEmptyBlock(builder.ToImmutable()), start);
        }

        public static ParseResult<Expression> ParseConditionalOr(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseLeftAssociativeBinop(tokens, start, new[] { TokenType.OrKeyword }, ParseConditionalAnd);
        }

        public static ParseResult<Expression> ParseConditionalAnd(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseLeftAssociativeBinop(tokens, start, new[] { TokenType.AndKeyword }, ParseEquality);
        }

    }
}
