using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using LinqExpression = System.Linq.Expressions.Expression;

namespace NCalcLib
{
    public class Transformer
    {
        public static TransformResult Transform(BindingContext bindingContext, Expression e)
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

                default:
                    throw new InvalidOperationException();
            }
        }

        private static TransformResult TransformIdentifier(BindingContext bindingContext, IdentifierExpression identifier)
        {
            if (!bindingContext.TryGetVariableType(identifier.Id, out var variableType))
            {
                var error = new Diagnostic(identifier.Start(), identifier.Length(), $"Variable '{identifier.Id}' has not been declared.");
                return new TransformResult(bindingContext, LinqExpression.Constant(0m), ImmutableList.Create(error));
            }
            else
            {
                var expression = GetGlobalVariableExpression(identifier.Id, variableType);

                return new TransformResult(bindingContext, expression);
            }
        }

        private static LinqExpression SetGlobalVariableExpression(string variableName, LinqExpression expression)
        {
            return LinqExpression.Call(
                    LinqExpression.Property(
                        expression: null,
                        property: typeof(Globals).GetProperty(nameof(Globals.Singleton))),
                    typeof(Globals).GetMethod(nameof(Globals.SetVariable)),
                    LinqExpression.Constant(variableName),
                    LinqExpression.Convert(expression, typeof(object)));
        }

        private static LinqExpression GetGlobalVariableExpression(string variableName, Type variableType)
        {
            return LinqExpression.Convert(
                LinqExpression.Call(
                    LinqExpression.Property(
                        expression: null,
                        property: typeof(Globals).GetProperty(nameof(Globals.Singleton))),
                    typeof(Globals).GetMethod(nameof(Globals.GetVariable)),
                    LinqExpression.Constant(variableName)),
                variableType);
        }

        private static TransformResult TransformNumericBinop(
            BindingContext bindingContext,
            BinaryExpression binop,
            Func<LinqExpression, LinqExpression, LinqExpression> createLinqBinop,
            Func<BinaryExpression, LinqExpression, LinqExpression, ImmutableList<Diagnostic>> typeCheckBinop)
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
                : LinqExpression.Default(typeof(decimal));

            return new TransformResult(bindingContext, linqBinop, leftErrors.AddRange(rightErrors).AddRange(typeCheckErrors));
        }

        private static ImmutableList<Diagnostic> TypeCheckNumericBinop(BinaryExpression binop, LinqExpression left, LinqExpression right)
        {
            var typeCheckErrors = ImmutableList<Diagnostic>.Empty;
            var errorString = "Expected type '{0}' but found '{1}' instead.";

            if (!left.HasDecimalType())
            {
                typeCheckErrors = typeCheckErrors.Add(new Diagnostic(binop.Left.Start(), binop.Left.Length(), string.Format(errorString, typeof(decimal), left.Type)));
            }

            if (!right.HasDecimalType())
            {
                typeCheckErrors = typeCheckErrors.Add(new Diagnostic(binop.Right.Start(), binop.Right.Length(), string.Format(errorString, typeof(decimal), right.Type)));
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

        private static TransformResult TransformBinop(BindingContext bindingContext, BinaryExpression binop)
        {
            (BindingContext, ImmutableList<Diagnostic> errors) handleDeclaration(DeclarationExpression declaration)
            {
                var id = declaration.Identifier.Text;
                Type type;
                var errors = ImmutableList<Diagnostic>.Empty;
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
                        errors = errors.Add(new Diagnostic(declaration.Type.Start, declaration.Type.Length, $"Unknown type '{declaration.Type.Text}'."));
                        break;

                }
                return (bindingContext.SetVariableType(id, type), errors);
            }

            TransformResult transformAssignment()
            {
                Token variableNameToken = null;
                var leftErrors = ImmutableList<Diagnostic>.Empty;
                if (binop.Left is DeclarationExpression declaration)
                {
                    (bindingContext, leftErrors) = handleDeclaration(declaration);
                    variableNameToken = declaration.Identifier;
                }
                else if (binop.Left is IdentifierExpression identifier)
                {
                    variableNameToken = identifier.Token;
                }

                if (!bindingContext.TryGetVariableType(variableNameToken.Text, out var _))
                {
                    leftErrors = leftErrors.Add(new Diagnostic(variableNameToken.Start, variableNameToken.Length, $"Variable '{variableNameToken.Text}' has not been declared."));
                }

                LinqExpression right;
                ImmutableList<Diagnostic> rightErrors;
                (bindingContext, right, rightErrors) = Transform(bindingContext, binop.Right);

                return new TransformResult(bindingContext, SetGlobalVariableExpression(variableNameToken.Text, right), leftErrors.AddRange(rightErrors));
            }

            switch (binop.Operator.Text)
            {
                case "+": return TransformNumericBinop(bindingContext, binop, LinqExpression.Add, TypeCheckNumericBinop);
                case "-": return TransformNumericBinop(bindingContext, binop, LinqExpression.Subtract, TypeCheckNumericBinop);
                case "*": return TransformNumericBinop(bindingContext, binop, LinqExpression.Multiply, TypeCheckNumericBinop);
                case "/": return TransformNumericBinop(bindingContext, binop, LinqExpression.Divide, TypeCheckNumericBinop);
                case "<": return TransformNumericBinop(bindingContext, binop, LinqExpression.LessThan, TypeCheckNumericBinop);
                case "<=": return TransformNumericBinop(bindingContext, binop, LinqExpression.LessThanOrEqual, TypeCheckNumericBinop);
                case ">": return TransformNumericBinop(bindingContext, binop, LinqExpression.GreaterThan, TypeCheckNumericBinop);
                case ">=": return TransformNumericBinop(bindingContext, binop, LinqExpression.GreaterThanOrEqual, TypeCheckNumericBinop);
                case "!=": return TransformNumericBinop(bindingContext, binop, LinqExpression.NotEqual, TypeCheckMatchingNumericOrBooleanTypes);
                case "==": return TransformNumericBinop(bindingContext, binop, LinqExpression.Equal, TypeCheckMatchingNumericOrBooleanTypes);
                case "=": return transformAssignment();
                default: throw new InvalidOperationException();
            }
        }
    }
}
