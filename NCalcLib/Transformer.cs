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
                    ImmutableList<string> errors;
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
                var error = $"Variable '{identifier.Id}' has not been declared.";
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

        private static TransformResult TransformBinop(BindingContext bindingContext, BinaryExpression binop)
        {
            TransformResult transformStandardBinop(Func<LinqExpression, LinqExpression, LinqExpression> createBinop, Func<LinqExpression, LinqExpression, ImmutableList<string>> typeCheck)
            {
                LinqExpression left;
                LinqExpression right;
                ImmutableList<string> leftErrors;
                ImmutableList<string> rightErrors;

                (bindingContext, left, leftErrors) = Transform(bindingContext, binop.Left);
                (bindingContext, right, rightErrors) = Transform(bindingContext, binop.Right);
                var typeCheckErrors = typeCheck(left, right);

                var linqBinop = typeCheckErrors.Count == 0
                    ? createBinop(left, right)
                    : LinqExpression.Default(left.Type);

                return new TransformResult(bindingContext, linqBinop, leftErrors.AddRange(rightErrors).AddRange(typeCheckErrors));
            }

            (BindingContext, ImmutableList<string> errors) handleDeclaration(DeclarationExpression declaration)
            {
                var id = declaration.Identifier.Text;
                Type type;
                var errors = ImmutableList<string>.Empty;
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
                        errors = errors.Add($"Unknown type '{declaration.Type.Text}'.");
                        break;

                }
                return (bindingContext.SetVariableType(id, type), errors);
            }

            TransformResult transformAssignment()
            {
                string variableName = string.Empty;
                var leftErrors = ImmutableList<string>.Empty;
                if (binop.Left is DeclarationExpression declaration)
                {
                    (bindingContext, leftErrors) = handleDeclaration(declaration);
                    variableName = declaration.Identifier.Text;
                }
                else if (binop.Left is IdentifierExpression identifier)
                {
                    variableName = identifier.Id;
                }

                if (!bindingContext.TryGetVariableType(variableName, out var _))
                {
                    leftErrors = leftErrors.Add($"Variable '{variableName}' has not been declared.");
                }

                LinqExpression right;
                ImmutableList<string> rightErrors;
                (bindingContext, right, rightErrors) = Transform(bindingContext, binop.Right);

                return new TransformResult(bindingContext, SetGlobalVariableExpression(variableName, right), leftErrors.AddRange(rightErrors));
            }

            ImmutableList<string> verifyNumericTypes(LinqExpression left, LinqExpression right)
            {
                var errors = ImmutableList<string>.Empty;
                var errorString = "Expected type '{0}' but found '{1}' instead.";

                if (!left.HasDecimalType())
                {
                    errors = errors.Add(string.Format(errorString, typeof(decimal), left.Type));
                }

                if (!right.HasDecimalType())
                {
                    errors = errors.Add(string.Format(errorString, typeof(decimal), right.Type));
                }

                return errors;
            }

            ImmutableList<string> verifyBooleanTypes(LinqExpression left, LinqExpression right)
            {
                var errors = ImmutableList<string>.Empty;
                var errorString = "Expected type '{0}' but found '{1}' instead.";

                if (!left.HasBooleanType())
                {
                    errors = errors.Add(string.Format(errorString, typeof(bool), left.Type));
                }

                if (!right.HasBooleanType())
                {
                    errors = errors.Add(string.Format(errorString, typeof(bool), right.Type));
                }

                return errors;
            }

            ImmutableList<string> verifyMatchingBooleanOrNumbericTypes(LinqExpression left, LinqExpression right)
            {
                var errors = ImmutableList<string>.Empty;
                var errorString = "Expected type '{0}' but found '{1}' instead.";

                if (left.HasBooleanType() || left.HasDecimalType())
                {
                    if (right.Type != left.Type)
                    {
                        errors = errors.Add(string.Format(errorString, left.Type, right.Type));
                    }
                }
                else
                {
                    errors = errors.Add($"Expected type '{typeof(decimal)}' or '{typeof(bool)}' but found '{left.Type}' instead.");
                }

                return errors;
            }

            switch (binop.Operator.Text)
            {
                case "+": return transformStandardBinop(LinqExpression.Add, verifyNumericTypes);
                case "-": return transformStandardBinop(LinqExpression.Subtract, verifyNumericTypes);
                case "*": return transformStandardBinop(LinqExpression.Multiply, verifyNumericTypes);
                case "/": return transformStandardBinop(LinqExpression.Divide, verifyNumericTypes);
                case "<": return transformStandardBinop(LinqExpression.LessThan, verifyNumericTypes);
                case "<=": return transformStandardBinop(LinqExpression.LessThanOrEqual, verifyNumericTypes);
                case ">": return transformStandardBinop(LinqExpression.GreaterThan, verifyNumericTypes);
                case ">=": return transformStandardBinop(LinqExpression.GreaterThanOrEqual, verifyNumericTypes);
                case "!=": return transformStandardBinop(LinqExpression.NotEqual, verifyMatchingBooleanOrNumbericTypes);
                case "==": return transformStandardBinop(LinqExpression.Equal, verifyMatchingBooleanOrNumbericTypes);
                case "=": return transformAssignment();
                default: throw new InvalidOperationException();
            }
        }
    }
}
