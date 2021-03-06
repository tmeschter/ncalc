﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public static StringLiteralExpression StringLiteralExpression(Token token) => new StringLiteralExpression(token);

        public static DeclarationStatement Declaration(Token identifierToken, Token asToken, Token typeToken, Token equalsToken, Expression initializationExpression) => new DeclarationStatement(identifierToken, asToken, typeToken, equalsToken, initializationExpression);
        public static ExpressionStatement ExpressionStatement(Expression expression) => new ExpressionStatement(expression);
        public static IfStatement IfStatement(Token ifToken, Expression condition, Block trueBlock, Token endToken) => new IfStatement(ifToken, condition, trueBlock, endToken);
        public static IfElseStatement IfElseStatement(Token ifToken, Expression condition, Block trueBlock, Token elseToken, Block falseBlock, Token endToken) => new IfElseStatement(ifToken, condition, trueBlock, elseToken, falseBlock, endToken);
        public static WhileStatement WhileStatement(Token whileToken, Expression condition, Block bodyBody, Token endToken) => new WhileStatement(whileToken, condition, bodyBody, endToken);

        public static EmptyBlock EmptyBlock(int start) => new EmptyBlock(start);
        public static Block Block(params Statement[] statements) => new NonEmptyBlock(ImmutableArray.Create(statements));

    }
}
