using System;
using System.Collections.Immutable;
using NCalcLib;
using LinqExpression = System.Linq.Expressions.Expression;

namespace ncalc
{
    class Program
    {
        static void Main(string[] args)
        {
            var globalBindingContext = BindingContext.Empty;
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    break;
                }

                var expressionSyntax = Parser.ParseSubmission(input);
                if (expressionSyntax == null)
                {
                    continue;
                }

                (var newBindingContext, var expression) = Transformer.Transform(globalBindingContext, expressionSyntax);
                if (expression == null)
                {
                    continue;
                }

                globalBindingContext = newBindingContext;

                var lambda = LinqExpression.Lambda<Func<decimal>>(expression);
                var compiledLambda = lambda.Compile();
                var result = compiledLambda();
                Console.WriteLine(result);
            }
        }
    }
}
