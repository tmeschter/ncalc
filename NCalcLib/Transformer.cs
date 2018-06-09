using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqExprs = System.Linq.Expressions;

namespace NCalcLib
{
    public class Transformer
    {
        public static LinqExprs.Expression Transform(Expression e)
        {
            switch (e)
            {
                case BinaryExpression binop:
                    return TransformBinop(binop);

                case NegationExpression negation:
                    var subexpression = Transform(negation.SubExpression);
                    return LinqExprs.Expression.Negate(subexpression);

                case NumberLiteralExpression number:
                    return LinqExprs.Expression.Constant(number.Value);

                case ParenthesizedExpression paren:
                    return Transform(paren.SubExpression);

                case IdentifierExpression identifier:
                    return TransformIdentifier(identifier);

                default:
                    throw new InvalidOperationException();
            }
        }

        private static LinqExprs.Expression TransformIdentifier(IdentifierExpression identifier)
        {
            return LinqExprs.Expression.Call(
                LinqExprs.Expression.Property(
                    expression: null,
                    property: typeof(Globals).GetProperty(nameof(Globals.Singleton))),
                typeof(Globals).GetMethod(nameof(Globals.GetVariable)),
                LinqExprs.Expression.Constant(identifier.Id));
        }

        private static LinqExprs.Expression TransformBinop(BinaryExpression binop)
        {
            var left = Transform(binop.Left);
            var right = Transform(binop.Right);

            switch (binop.Operator.Text)
            {
                case "+":
                    return LinqExprs.Expression.Add(left, right);

                case "-":
                    return LinqExprs.Expression.Subtract(left, right);

                case "*":
                    return LinqExprs.Expression.Multiply(left, right);

                case "/":
                    return LinqExprs.Expression.Divide(left, right);

                case "=":
                    return LinqExprs.Expression.Call(
                        LinqExprs.Expression.Property(
                            expression: null,
                            property: typeof(Globals).GetProperty(nameof(Globals.Singleton))),
                        typeof(Globals).GetMethod(nameof(Globals.SetVariable)),
                        LinqExprs.Expression.Constant(((IdentifierExpression)binop.Left).Id),
                        right);

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
