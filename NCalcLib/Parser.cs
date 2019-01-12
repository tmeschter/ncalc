using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCalcLib
{
    public class ParseResult
    {
        public Expression Expression { get; }
        public int NextTokenIndex { get; }

        public ParseResult(Expression expression, int nextTokenIndex)
        {
            Expression = expression;
            NextTokenIndex = nextTokenIndex;
        }
    }

    public class Parser
    {
        public static Expression ParseSubmission(ImmutableArray<Token> tokens)
        {
            var start = 0;
            var result = ParseAssignment(tokens, start);

            if (result == null)
            {
                return null;
            }

            if (result != null
                && result.NextTokenIndex == tokens.Length - 1
                && tokens[result.NextTokenIndex].Type == TokenType.EndOfInput)
            {
                return result.Expression;
            }

            return null;
        }

        public static ParseResult ParseNumberLiteral(ImmutableArray<Token> tokens, int start = 0) =>
            start < tokens.Length && tokens[start].Type == TokenType.NumberLiteral
            ? new ParseResult(new NumberLiteralExpression(tokens[start]), start + 1)
            : null;

        public static ParseResult ParseNegationExpression(ImmutableArray<Token> tokens, int start = 0)
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
                    return new ParseResult(new NegationExpression(operatorToken, subExpressionParseResult.Expression), subExpressionParseResult.NextTokenIndex);
                }

                return subExpressionParseResult;
            }

            return null;
        }

        public static ParseResult ParseOperandExpression(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseNumberLiteral(tokens, start)
                ?? ParseParenthensizedExpression(tokens, start)
                ?? ParseBooleanLiteral(tokens, start)
                ?? ParseIdentifier(tokens, start);
        }

        public static ParseResult ParseParenthensizedExpression(ImmutableArray<Token> tokens, int start = 0)
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

            return new ParseResult(new ParenthesizedExpression(leftParenToken, subExpressionParseResult.Expression, rightParenToken), index);
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

        public static ParseResult ParseAdditionAndSubtraction(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseLeftAssociativeBinop(tokens, start, new[] { TokenType.Plus, TokenType.Minus }, ParseMultiplicationAndDivision);
        }

        public static ParseResult ParseEquality(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseLeftAssociativeBinop(tokens, start, new[] { TokenType.EqualEqual, TokenType.BangEqual }, ParseRelational);
        }

        private static ParseResult ParseLeftAssociativeBinop(ImmutableArray<Token> tokens, int start, TokenType[] opTokenTypes, Func<ImmutableArray<Token>, int, ParseResult> subExpressionParser)
        {
            var index = start;
            var expressionParseResult = subExpressionParser(tokens, index);
            if (expressionParseResult == null)
            {
                return null;
            }

            var expression = expressionParseResult.Expression;
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
                expression = new BinaryExpression(expression, op, rightHandExpressionResult.Expression);

                op = GetNextToken(tokens, index, opTokenTypes);
            }

            return new ParseResult(expression, index);
        }

        public static ParseResult ParseRelational(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseLeftAssociativeBinop(tokens, start, new[] { TokenType.LessThanEqual, TokenType.GreaterThanEqual, TokenType.LessThan, TokenType.GreaterThan }, ParseAdditionAndSubtraction);
        }

        public static ParseResult ParseMultiplicationAndDivision(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseLeftAssociativeBinop(tokens, start, new[] { TokenType.Asterisk, TokenType.Slash }, ParseNegationExpression);
        }

        public static ParseResult ParseIdentifier(ImmutableArray<Token> tokens, int start = 0) =>
            start < tokens.Length && tokens[start].Type == TokenType.Identifier
            ? new ParseResult(new IdentifierExpression(tokens[start]), start + 1)
            : null;

        public static ParseResult ParseAssignment(ImmutableArray<Token> tokens, int start = 0)
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
                        var expression = new BinaryExpression(leftHandSideResult.Expression, equalSign, subExpressionParseResult.Expression);
                        return new ParseResult(expression, subExpressionParseResult.NextTokenIndex);
                    }

                    return null;
                }
            }

            return ParseEquality(tokens, start);
        }

        public static ParseResult ParseExpression(ImmutableArray<Token> tokens, int start = 0)
        {
            return ParseAssignment(tokens, start);
        }

        public static ParseResult ParseBooleanLiteral(ImmutableArray<Token> tokens, int start = 0)
        {
            if (start >= tokens.Length)
            {
                return null;
            }

            switch (tokens[start].Type)
            {
                case TokenType.FalseLiteral:
                case TokenType.TrueLiteral:
                    return new ParseResult(new BooleanLiteralExpression(tokens[start]), start + 1);

                default:
                    return null;
            }
        }

        public static ParseResult ParseDeclarationExpression(ImmutableArray<Token> tokens, int start = 0)
        {
            if (start + 2 < tokens.Length
                && tokens[start].Type == TokenType.Identifier
                && tokens[start + 1].Type == TokenType.AsKeyword
                && IsTypeToken(tokens[start + 2]))
            {
                return new ParseResult(new DeclarationExpression(tokens[start], tokens[start + 1], tokens[start + 2]), start + 3);
            }

            return null;
        }

        private static bool IsTypeToken(Token token)
        {
            return token.Type == TokenType.BooleanKeyword
                || token.Type == TokenType.NumberKeyword;
        }
    }
}
