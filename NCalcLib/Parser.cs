﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCalcLib
{
    public class Parser
    {
        public static Expression ParseSubmission(string text)
        {
            var start = 0;
            var expression = ParseAssignment(text, start);

            if (expression != null)
            {
                start = expression.Length;
            }

            var whitespace = ParseWhitespace(text, start);

            if (whitespace.Start + whitespace.Length != text.Length)
            {
                return null;
            }

            return expression;
        }

        public static NumberLiteralExpression ParseNumberLiteral(string text, int start = 0)
        {
            var whitespace = ParseWhitespace(text, start);
            start = start + whitespace.Length;

            int index = start;
            while (index < text.Length
                && char.IsDigit(text[index]))
            {
                index++;
            }

            if (index < text.Length
                && text[index] == '.')
            {
                index++;

                while (index < text.Length
                    && char.IsDigit(text[index]))
                {
                    index++;
                }

                if (text[index - 1] == '.')
                {
                    return null;
                }
            }

            int length = index - start;
            if (length == 0)
            {
                return null;
            }

            var numberToken = new Token(whitespace, text.Substring(start, length));

            return new NumberLiteralExpression(numberToken);
        }

        public static Expression ParseNegationExpression(string text, int start = 0)
        {
            int index = start;

            var operatorToken = ParseToken(text, "-", index);

            if (operatorToken != null)
            {
                index = index + operatorToken.Length;
            }

            var subExpression = ParseOperandExpression(text, index);

            if (subExpression != null)
            {
                if (operatorToken != null)
                {
                    return new NegationExpression(operatorToken, subExpression);
                }

                return subExpression;
            }

            return null;
        }

        public static Expression ParseOperandExpression(string text, int start = 0)
        {
            return ParseNumberLiteral(text, start)
                ?? ParseParenthensizedExpression(text, start)
                ?? ParseBooleanLiteral(text, start)
                ?? (Expression)ParseIdentifier(text, start);
        }

        public static Expression ParseParenthensizedExpression(string text, int start = 0)
        {
            int index = start;

            var leftParen = ParseToken(text, "(", index);
            if (leftParen == null)
            {
                return null;
            }

            index = index + leftParen.Length;

            var subExpression = ParseExpression(text, index);
            if (subExpression == null)
            {
                return null;
            }

            index = index + subExpression.Length;

            var rightParen = ParseToken(text, ")", index);
            if (rightParen == null)
            {
                return null;
            }

            return new ParenthesizedExpression(leftParen, subExpression, rightParen);
        }

        public static Whitespace ParseWhitespace(string text, int start = 0)
        {
            int index = start;

            while (index < text.Length
                && char.IsWhiteSpace(text[index]))
            {
                index++;
            }

            return new Whitespace(start, text.Substring(start, index - start));
        }

        public static Expression ParseAdditionAndSubtraction(string text, int start = 0)
        {
            var index = start;

            var expression = ParseMultiplicationAndDivision(text, index);
            if (expression == null)
            {
                return null;
            }

            index = index + expression.Length;

            var op = ParseToken(text, "+", index)
                    ?? ParseToken(text, "-", index);
            while (op != null)
            {
                index = index + op.Length;
                var rightHandExpression = ParseMultiplicationAndDivision(text, index);
                if (rightHandExpression == null)
                {
                    return null;
                }

                index = index + rightHandExpression.Length;
                expression = new BinaryExpression(expression, op, rightHandExpression);

                op = ParseToken(text, "+", index)
                    ?? ParseToken(text, "-", index);
            }

            return expression;
        }

        public static Expression ParseEquality(string text, int start = 0)
        {
            var index = start;
            var expression = ParseRelational(text, index);
            if (expression == null)
            {
                return null;
            }

            index = index + expression.Length;

            var op = ParseToken(text, "==", index)
                ?? ParseToken(text, "!=", index);
            if (op != null)
            {
                index = index + op.Length;
                var rightHandExpression = ParseRelational(text, index);
                if (rightHandExpression == null)
                {
                    return null;
                }

                index = index + rightHandExpression.Length;
                expression = new BinaryExpression(expression, op, rightHandExpression);
            }

            return expression;
        }

        public static Expression ParseRelational(string text, int start = 0)
        {
            var index = start;
            var expression = ParseAdditionAndSubtraction(text, index);
            if (expression == null)
            {
                return null;
            }

            index = index + expression.Length;

            var op = ParseToken(text, "<=", index)
                ?? ParseToken(text, ">=", index)
                ?? ParseToken(text, "<", index)
                ?? ParseToken(text, ">", index);
            if (op != null)
            {
                index = index + op.Length;
                var rightHandExpression = ParseAdditionAndSubtraction(text, index);
                if (rightHandExpression == null)
                {
                    return null;
                }

                index = index + rightHandExpression.Length;
                expression = new BinaryExpression(expression, op, rightHandExpression);
            }

            return expression;
        }

        public static Token ParseToken(string text, string expected, int start = 0)
        {
            int index = start;

            var whitespaceLength = 0;
            var whitespace = ParseWhitespace(text, start);

            whitespaceLength = whitespace.Length;
            index = index + whitespaceLength;

            int tokenStart = index;
            while (index < text.Length
                && (index - tokenStart) < expected.Length
                && text[index] == expected[index - tokenStart])
            {
                index++;
            }

            int tokenLength = index - tokenStart;
            if (tokenLength == 0)
            {
                return null;
            }

            return new Token(whitespace, text.Substring(tokenStart, tokenLength));
        }

        public static Expression ParseMultiplicationAndDivision(string text, int start = 0)
        {
            var index = start;

            var expression = ParseNegationExpression(text, index);
            if (expression == null)
            {
                return null;
            }

            index = index + expression.Length;

            var op = ParseToken(text, "*", index)
                    ?? ParseToken(text, "/", index);
            while (op != null)
            {
                index = index + op.Length;
                var rightHandExpression = ParseNegationExpression(text, index);
                if (rightHandExpression == null)
                {
                    return null;
                }

                index = index + rightHandExpression.Length;
                expression = new BinaryExpression(expression, op, rightHandExpression);

                op = ParseToken(text, "*", index)
                    ?? ParseToken(text, "/", index);
            }

            return expression;
        }

        public static IdentifierExpression ParseIdentifier(string text, int start = 0)
        {
            var whitespace = ParseWhitespace(text, start);
            start = start + whitespace.Length;

            int length = IdentifierLength(text, start);
            if (length == 0)
            {
                return null;
            }

            var identifierToken = new Token(whitespace, text.Substring(start, length));

            return new IdentifierExpression(identifierToken);
        }

        private static int IdentifierLength(string text, int start)
        {
            int index = start;
            if (char.IsLetter(text[index]))
            {
                index++;
            }
            else
            {
                return 0;
            }

            while (index < text.Length
                && char.IsLetterOrDigit(text[index]))
            {
                index++;
            }

            return index - start;
        }

        public static Expression ParseAssignment(string text, int start = 0)
        {
            int index = start;
            var identifier = ParseIdentifier(text, index);
            if (identifier != null)
            {
                index = index + identifier.Length;
                var equalSign = ParseToken(text, "=", index);
                // Make sure we didn't parse a '=' when we should have parsed a '=='.
                if (equalSign != null
                    && text[index + equalSign.Length] != '=')
                {
                    index = index + equalSign.Length;

                    var subExpression = ParseExpression(text, index);
                    if (subExpression != null)
                    {
                        return new BinaryExpression(identifier, equalSign, subExpression);
                    }

                    return null;
                }
            }

            return ParseEquality(text, start);
        }

        public static Expression ParseExpression(string text, int start = 0)
        {
            return ParseAssignment(text, start);
        }

        public static BooleanLiteralExpression ParseBooleanLiteral(string text, int start = 0)
        {
            var whitespace = ParseWhitespace(text, start);
            start = start + whitespace.Length;

            int length = IdentifierLength(text, start);
            if (length == 0)
            {
                return null;
            }

            if (string.Compare(text, start, "true", 0, length) == 0
                || string.Compare(text, start, "false", 0, length) == 0)
            {
                return new BooleanLiteralExpression(new Token(whitespace, text.Substring(start, length)));
            }

            return null;
        }
    }
}