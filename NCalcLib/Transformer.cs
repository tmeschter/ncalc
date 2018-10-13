using System;
using LinqExpression = System.Linq.Expressions.Expression;

namespace NCalcLib
{
    public class Transformer
    {
        public static (BindingContext, LinqExpression) Transform(BindingContext bindingContext, Expression e)
        {
            switch (e)
            {
                case BinaryExpression binop:
                    return TransformBinop(bindingContext, binop);

                case BooleanLiteralExpression boolean:
                    return (bindingContext, LinqExpression.Constant(boolean.Value));

                case IdentifierExpression identifier:
                    return TransformIdentifier(bindingContext, identifier);

                case NegationExpression negation:
                    LinqExpression subexpression;
                    (bindingContext, subexpression) = Transform(bindingContext, negation.SubExpression);
                    return (bindingContext, LinqExpression.Negate(subexpression));

                case NumberLiteralExpression number:
                    return (bindingContext, LinqExpression.Constant(number.Value));

                case ParenthesizedExpression paren:
                    return Transform(bindingContext, paren.SubExpression);

                default:
                    throw new InvalidOperationException();
            }
        }

        private static (BindingContext, LinqExpression) TransformIdentifier(BindingContext bindingContext, IdentifierExpression identifier)
        {
            var type = bindingContext.GetVariableType(identifier.Id);
            var expression = LinqExpression.Convert(
                LinqExpression.Call(
                    LinqExpression.Property(
                        expression: null,
                        property: typeof(Globals).GetProperty(nameof(Globals.Singleton))),
                    typeof(Globals).GetMethod(nameof(Globals.GetVariable)),
                    LinqExpression.Constant(identifier.Id)),
                type);

            return (bindingContext, expression);
        }

        private static (BindingContext, LinqExpression) TransformBinop(BindingContext bindingContext, BinaryExpression binop)
        {
            (BindingContext, LinqExpression) transformStandardBinop(Func<LinqExpression, LinqExpression, LinqExpression> createBinop)
            {
                LinqExpression left;
                LinqExpression right;

                (bindingContext, left) = Transform(bindingContext, binop.Left);
                (bindingContext, right) = Transform(bindingContext, binop.Right);
                return (bindingContext, createBinop(left, right));
            }

            (BindingContext, LinqExpression) transformAssignment()
            {
                var variableName = ((IdentifierExpression)binop.Left).Id;
                LinqExpression right;
                (bindingContext, right) = Transform(bindingContext, binop.Right);
                bindingContext = bindingContext.SetVariableType(variableName, right.Type);

                return (bindingContext,
                     LinqExpression.Call(
                        LinqExpression.Property(
                            expression: null,
                            property: typeof(Globals).GetProperty(nameof(Globals.Singleton))),
                        typeof(Globals).GetMethod(nameof(Globals.SetVariable)),
                    LinqExpression.Constant(variableName),
                    LinqExpression.Convert(right, typeof(object))));
            }

            switch (binop.Operator.Text)
            {
                case "+": return transformStandardBinop(LinqExpression.Add);
                case "-": return transformStandardBinop(LinqExpression.Subtract);
                case "*": return transformStandardBinop(LinqExpression.Multiply);
                case "/": return transformStandardBinop(LinqExpression.Divide);
                case "<": return transformStandardBinop(LinqExpression.LessThan);
                case "<=": return transformStandardBinop(LinqExpression.LessThanOrEqual);
                case ">": return transformStandardBinop(LinqExpression.GreaterThan);
                case ">=": return transformStandardBinop(LinqExpression.GreaterThanOrEqual);
                case "!=": return transformStandardBinop(LinqExpression.NotEqual);
                case "==": return transformStandardBinop(LinqExpression.Equal);
                case "=": return transformAssignment();
                default: throw new InvalidOperationException();
            }
        }
    }
}
