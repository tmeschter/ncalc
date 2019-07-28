using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LinqExpression = System.Linq.Expressions.Expression;

namespace NCalcLib
{
    public class Transformer
    {
        public static TransformResult Transform(IBindingContext bindingContext, Node e)
        {
            switch (e)
            {
                case BinaryExpression binop:
                    return TransformBinop(bindingContext, binop);

                case BooleanLiteralExpression boolean:
                    return new TransformResult(bindingContext, LinqExpression.Constant(boolean.Value));

                case IdentifierExpression identifier:
                    return TransformIdentifier(bindingContext, identifier);

                case NegationExpression negation:
                    LinqExpression subexpression;
                    ImmutableList<Diagnostic> errors;
                    (bindingContext, subexpression, errors) = Transform(bindingContext, negation.SubExpression);
                    return new TransformResult(bindingContext, LinqExpression.Negate(subexpression), errors);

                case NumberLiteralExpression number:
                    return new TransformResult(bindingContext, LinqExpression.Constant(number.Value));

                case ParenthesizedExpression paren:
                    return Transform(bindingContext, paren.SubExpression);

                case DeclarationStatement statement:
                    return TransformDeclaration(bindingContext, statement);

                case ExpressionStatement statement:
                    return Transform(bindingContext, statement.Expression);

                case IfStatement statement:
                    return TransformIf(bindingContext, statement);

                case IfElseStatement statement:
                    return TransformIfElse(bindingContext, statement);

                case Block block:
                    return TransformBlock(bindingContext, block);

                case WhileStatement statement:
                    return TransformWhile(bindingContext, statement);

                default:
                    throw new InvalidOperationException();
            }
        }

        private static TransformResult TransformBlock(IBindingContext bindingContext, Block block)
        {
            var originalBindingContext = bindingContext;

            bindingContext = new LocalBindingContext(bindingContext);
            var allDiagnostics = ImmutableList.CreateBuilder<Diagnostic>();
            var expressions = new List<LinqExpression>();

            foreach (var statement in block.Statements)
            {
                LinqExpression expression;
                ImmutableList<Diagnostic> diagnostics;
                (bindingContext, expression, diagnostics) = Transform(bindingContext, statement);
                expressions.Add(expression);
                allDiagnostics.AddRange(diagnostics);
            }

            return new TransformResult(
                originalBindingContext,
                LinqExpression.Block(bindingContext.LocalVariables, expressions),
                allDiagnostics.ToImmutable());
        }

        private static TransformResult TransformIf(IBindingContext bindingContext, IfStatement statement)
        {
            var (newBindingContext, conditionalExpression, conditionalErrors) = Transform(bindingContext, statement.Condition);
            var (newBodyBindingContext, bodyExpression, bodyErrors) = Transform(newBindingContext, statement.TrueBlock);

            System.Linq.Expressions.ConditionalExpression ifExpression;
            if (conditionalExpression.Type == typeof(bool))
            {
                ifExpression = LinqExpression.IfThen(conditionalExpression, bodyExpression);
            }
            else
            {
                conditionalErrors = conditionalErrors.Add(
                    new Diagnostic(
                        statement.Condition.Start(),
                        statement.Condition.Length(),
                        $"Expected type '{typeof(bool)}' but found type '{conditionalExpression.Type}' instead."));
                ifExpression = LinqExpression.IfThen(LinqExpression.Constant(false), bodyExpression);
            }

            return new TransformResult(bindingContext, ifExpression, conditionalErrors.AddRange(bodyErrors));
        }

        private static TransformResult TransformIfElse(IBindingContext bindingContext, IfElseStatement statement)
        {
            var (newBindingContext, conditionalExpression, conditionalErrors) = Transform(bindingContext, statement.Condition);
            var (trueBodyBindingContext, trueExpression, trueBodyErrors) = Transform(newBindingContext, statement.TrueBlock);
            var (falseBodyBindingContext, falseExpression, falseBodyErrors) = Transform(newBindingContext, statement.FalseBlock);

            System.Linq.Expressions.ConditionalExpression ifElseExpression;
            if (conditionalExpression.Type == typeof(bool))
            {
                ifElseExpression = LinqExpression.IfThenElse(conditionalExpression, trueExpression, falseExpression);
            }
            else
            {
                conditionalErrors = conditionalErrors.Add(
                    new Diagnostic(
                        statement.Condition.Start(),
                        statement.Condition.Length(),
                        $"Expected type '{typeof(bool)}' but found type '{conditionalExpression.Type}' instead."));
                ifElseExpression = LinqExpression.IfThenElse(LinqExpression.Constant(false), trueExpression, falseExpression);
            }

            return new TransformResult(bindingContext, ifElseExpression, conditionalErrors.AddRange(trueBodyErrors).AddRange(falseBodyErrors));
        }

        private static TransformResult TransformWhile(IBindingContext bindingContext, WhileStatement statement)
        {
            var (newBindingContext, conditionalExpression, conditionalErrors) = Transform(bindingContext, statement.Condition);
            var (newBodyBindingContext, bodyExpression, bodyErrors) = Transform(newBindingContext, statement.BodyBlock);

            var breakLabel = LinqExpression.Label("LoopBreak");

            var loopExpression = 
                LinqExpression.Loop(
                    LinqExpression.IfThenElse(
                        conditionalExpression,
                        bodyExpression,
                        LinqExpression.Break(breakLabel)),
                    breakLabel);

            return new TransformResult(bindingContext, loopExpression, conditionalErrors.AddRange(bodyErrors));
        }

        private static TransformResult TransformIdentifier(IBindingContext bindingContext, IdentifierExpression identifier)
        {
            var expression = bindingContext.CreateGetVariableExpression(identifier.Id);
            if (expression == null)
            {
                var error = new Diagnostic(identifier.Start(), identifier.Length(), $"Variable '{identifier.Id}' has not been declared.");
                return new TransformResult(bindingContext, LinqExpression.Constant(0m), ImmutableList.Create(error));
            }
            else
            {
                return new TransformResult(bindingContext, expression);
            }
        }

        private static TransformResult TransformBinopCore(
            IBindingContext bindingContext,
            BinaryExpression binop,
            Func<LinqExpression, LinqExpression, LinqExpression> createLinqBinop,
            Func<BinaryExpression, LinqExpression, LinqExpression, ImmutableList<Diagnostic>> typeCheckBinop,
            Type errorType)
        {
            LinqExpression left;
            LinqExpression right;
            ImmutableList<Diagnostic> leftErrors;
            ImmutableList<Diagnostic> rightErrors;

            (bindingContext, left, leftErrors) = Transform(bindingContext, binop.Left);
            (bindingContext, right, rightErrors) = Transform(bindingContext, binop.Right);
            ImmutableList<Diagnostic> typeCheckErrors = typeCheckBinop(binop, left, right);

            var linqBinop = typeCheckErrors.Count == 0
                ? createLinqBinop(left, right)
                : LinqExpression.Default(errorType);

            return new TransformResult(bindingContext, linqBinop, leftErrors.AddRange(rightErrors).AddRange(typeCheckErrors));
        }

        private static ImmutableList<Diagnostic> TypeCheckNumericBinop(BinaryExpression binop, LinqExpression left, LinqExpression right)
        {
            return TypeCheckBinopCore(binop, left, right, typeof(decimal));
        }

        private static ImmutableList<Diagnostic> TypeCheckBooleanBinop(BinaryExpression binop, LinqExpression left, LinqExpression right)
        {
            return TypeCheckBinopCore(binop, left, right, typeof(bool));
        }

        private static ImmutableList<Diagnostic> TypeCheckBinopCore(BinaryExpression binop, LinqExpression left, LinqExpression right, Type expectedType)
        {
            var typeCheckErrors = ImmutableList<Diagnostic>.Empty;
            var errorString = "Expected type '{0}' but found '{1}' instead.";

            if (left.Type != expectedType)
            {
                typeCheckErrors = typeCheckErrors.Add(new Diagnostic(binop.Left.Start(), binop.Left.Length(), string.Format(errorString, expectedType, left.Type)));
            }

            if (right.Type != expectedType)
            {
                typeCheckErrors = typeCheckErrors.Add(new Diagnostic(binop.Right.Start(), binop.Right.Length(), string.Format(errorString, expectedType, right.Type)));
            }

            return typeCheckErrors;
        }

        private static ImmutableList<Diagnostic> TypeCheckMatchingNumericOrBooleanTypes(BinaryExpression binop, LinqExpression left, LinqExpression right)
        {
            var typeCheckErrors = ImmutableList<Diagnostic>.Empty;
            var errorString = "Expected type '{0}' but found '{1}' instead.";

            if (left.HasBooleanType() || left.HasDecimalType())
            {
                if (right.Type != left.Type)
                {
                    typeCheckErrors = typeCheckErrors.Add(new Diagnostic(binop.Right.Start(), binop.Right.Length(), string.Format(errorString, left.Type, right.Type)));
                }
            }
            else
            {
                typeCheckErrors = typeCheckErrors.Add(new Diagnostic(binop.Left.Start(), binop.Left.Length(), $"Expected type '{typeof(decimal)}' or '{typeof(bool)}' but found '{left.Type}' instead."));
            }

            return typeCheckErrors;
        }

        private static TransformResult TransformDeclaration(IBindingContext bindingContext, DeclarationStatement declaration)
        {
            var variableName = declaration.Identifier;
            Type type;
            var leftErrors = ImmutableList<Diagnostic>.Empty;
            switch (declaration.Type.Type)
            {
                case TokenType.BooleanKeyword:
                    type = typeof(bool);
                    break;
                case TokenType.NumberKeyword:
                    type = typeof(decimal);
                    break;
                default:
                    type = typeof(decimal);
                    leftErrors = leftErrors.Add(new Diagnostic(declaration.Type.Start, declaration.Type.Length, $"Unknown type '{declaration.Type.Text}'."));
                    break;

            }

            LinqExpression initExpression;
            ImmutableList<Diagnostic> initExpressionErrors;
            (bindingContext, initExpression, initExpressionErrors) = Transform(bindingContext, declaration.InitializationExpression);

            // TODO check if the variable already exists.

            bindingContext = bindingContext.SetVariableType(variableName.Text, type);

            var expression = bindingContext.CreateSetVariableExpression(variableName.Text, initExpression);
            if (expression == null)
            {
                leftErrors = leftErrors.Add(new Diagnostic(variableName.Start, variableName.Length, $"Variable '{variableName.Text}' has not been declared."));
                expression = LinqExpression.Constant(0m);
            }

            return new TransformResult(bindingContext, expression, leftErrors.AddRange(initExpressionErrors));
        }

        private static TransformResult TransformBinop(IBindingContext bindingContext, BinaryExpression binop)
        {
            TransformResult transformAssignment()
            {
                Token variableNameToken = null;
                var leftErrors = ImmutableList<Diagnostic>.Empty;
                if (binop.Left is IdentifierExpression identifier)
                {
                    variableNameToken = identifier.Token;
                }
                else
                {
                    // TODO: emit an error if the left side of the assignment is anything other than an identifier
                }

                LinqExpression right;
                ImmutableList<Diagnostic> rightErrors;
                (bindingContext, right, rightErrors) = Transform(bindingContext, binop.Right);

                var expression = bindingContext.CreateSetVariableExpression(variableNameToken.Text, right);
                if (expression == null)
                {
                    leftErrors = leftErrors.Add(new Diagnostic(variableNameToken.Start, variableNameToken.Length, $"Variable '{variableNameToken.Text}' has not been declared."));
                    expression = LinqExpression.Constant(0m);
                }

                return new TransformResult(bindingContext, expression, leftErrors.AddRange(rightErrors));
            }

            switch (binop.Operator.Text)
            {
                case "+": return TransformBinopCore(bindingContext, binop, LinqExpression.Add, TypeCheckNumericBinop, typeof(decimal));
                case "-": return TransformBinopCore(bindingContext, binop, LinqExpression.Subtract, TypeCheckNumericBinop, typeof(decimal));
                case "*": return TransformBinopCore(bindingContext, binop, LinqExpression.Multiply, TypeCheckNumericBinop, typeof(decimal));
                case "/": return TransformBinopCore(bindingContext, binop, LinqExpression.Divide, TypeCheckNumericBinop, typeof(decimal));
                case "<": return TransformBinopCore(bindingContext, binop, LinqExpression.LessThan, TypeCheckNumericBinop, typeof(decimal));
                case "<=": return TransformBinopCore(bindingContext, binop, LinqExpression.LessThanOrEqual, TypeCheckNumericBinop, typeof(decimal));
                case ">": return TransformBinopCore(bindingContext, binop, LinqExpression.GreaterThan, TypeCheckNumericBinop, typeof(decimal));
                case ">=": return TransformBinopCore(bindingContext, binop, LinqExpression.GreaterThanOrEqual, TypeCheckNumericBinop, typeof(decimal));
                case "!=": return TransformBinopCore(bindingContext, binop, LinqExpression.NotEqual, TypeCheckMatchingNumericOrBooleanTypes, typeof(bool));
                case "==": return TransformBinopCore(bindingContext, binop, LinqExpression.Equal, TypeCheckMatchingNumericOrBooleanTypes, typeof(bool));
                case "or": return TransformBinopCore(bindingContext, binop, LinqExpression.OrElse, TypeCheckBooleanBinop, typeof(bool));
                case "and": return TransformBinopCore(bindingContext, binop, LinqExpression.AndAlso, TypeCheckBooleanBinop, typeof(bool));
                case "=": return transformAssignment();
                default: throw new InvalidOperationException();
            }
        }
    }
}
