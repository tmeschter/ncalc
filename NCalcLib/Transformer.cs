using System;
using LinqExpression = System.Linq.Expressions.Expression;

namespace NCalcLib
{
    public class Transformer
    {
        public static LinqExpression Transform(Expression e)
        {
            switch (e)
            {
                case BinaryExpression binop:
                    return TransformBinop(binop);

                case NegationExpression negation:
                    var subexpression = Transform(negation.SubExpression);
                    return LinqExpression.Negate(subexpression);

                case NumberLiteralExpression number:
                    return LinqExpression.Constant(number.Value);

                case ParenthesizedExpression paren:
                    return Transform(paren.SubExpression);

                case IdentifierExpression identifier:
                    return TransformIdentifier(identifier);

                default:
                    throw new InvalidOperationException();
            }
        }

        private static LinqExpression TransformIdentifier(IdentifierExpression identifier)
        {
            return LinqExpression.Call(
                LinqExpression.Property(
                    expression: null,
                    property: typeof(Globals).GetProperty(nameof(Globals.Singleton))),
                typeof(Globals).GetMethod(nameof(Globals.GetVariable)),
                LinqExpression.Constant(identifier.Id));
        }

        private static LinqExpression TransformBinop(BinaryExpression binop)
        {
            var left = Transform(binop.Left);
            var right = Transform(binop.Right);

            switch (binop.Operator.Text)
            {
                case "+":
                    return LinqExpression.Add(left, right);

                case "-":
                    return LinqExpression.Subtract(left, right);

                case "*":
                    return LinqExpression.Multiply(left, right);

                case "/":
                    return LinqExpression.Divide(left, right);

                case "=":
                    return LinqExpression.Call(
                        LinqExpression.Property(
                            expression: null,
                            property: typeof(Globals).GetProperty(nameof(Globals.Singleton))),
                        typeof(Globals).GetMethod(nameof(Globals.SetVariable)),
                        LinqExpression.Constant(((IdentifierExpression)binop.Left).Id),
                        right);

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
