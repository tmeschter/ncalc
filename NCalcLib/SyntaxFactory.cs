using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCalcLib
{
    public static class SyntaxFactory
    {
        public static Whitespace Whitespace(int start, string value) => new Whitespace(start, value);

        public static Token Token(int tokenStart, string text, TokenType tokenType) => new Token(tokenStart, text, tokenType);
        public static Token Token(Whitespace whitespace, string text, TokenType tokenType) => new Token(whitespace, text, tokenType);

        public static BinaryExpression BinaryExpression(Expression left, Token op, Expression right) => new BinaryExpression(left, op, right);
        public static NegationExpression NegationExpression(Token operatorToken, Expression subExpression) => new NegationExpression(operatorToken, subExpression);
        public static NumberLiteralExpression NumberLiteralExpression(Token token) => new NumberLiteralExpression(token);
        public static BooleanLiteralExpression BooleanLiteralExpression(Token token) => new BooleanLiteralExpression(token);
        public static ParenthesizedExpression ParenthesizedExpression(Token leftParen, Expression subExpression, Token rightParen) => new ParenthesizedExpression(leftParen, subExpression, rightParen);
        public static IdentifierExpression IdentifierExpression(Token token) => new IdentifierExpression(token);
    }
}
