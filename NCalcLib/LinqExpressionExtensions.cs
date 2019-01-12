using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqExpression = System.Linq.Expressions.Expression;

namespace NCalcLib
{
    internal static class LinqExpressionExtensions
    {
        public static bool HasDecimalType(this LinqExpression expression) => expression.Type == typeof(decimal);
        public static bool HasBooleanType(this LinqExpression expression) => expression.Type == typeof(bool);
    }
}
