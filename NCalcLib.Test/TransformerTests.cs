using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using LinqExprs = System.Linq.Expressions;
using static NCalcLib.SyntaxFactory;
using System.Collections.Immutable;

namespace NCalcLib.Test
{
    public class TransformerTests
    {
        [Fact]
        public void NumberLiteral()
        {
            var syntax = NumberLiteralExpression(Token(0, "5"));

            (var newContext, var expression) = Transformer.Transform(BindingContext.Empty, syntax);

            Assert.Equal(expected: 5m, actual: ((LinqExprs.ConstantExpression)expression).Value);
        }
    }
}
