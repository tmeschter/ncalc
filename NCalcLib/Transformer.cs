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

                default:
                    throw new InvalidOperationException();
            }
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

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
