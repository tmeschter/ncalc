using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using LinqExprs = System.Linq.Expressions;
using static NCalcLib.SyntaxFactory;

namespace NCalcLib.Test
{
    public class TransformerTests
    {
        [Fact]
        public void NumberLiteral()
        {
            var syntax = NumberLiteralExpression(Token(0, "5"));
            var expression = (LinqExprs.ConstantExpression)Transformer.Transform(syntax);

            Assert.Equal(expected: 5, actual: expression.Value);
        }
    }
}
