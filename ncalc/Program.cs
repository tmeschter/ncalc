using System;
using NCalcLib;

namespace ncalc
{
    class Program
    {
        static void Main(string[] args)
        {
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

                var expression = Transformer.Transform(expressionSyntax);
                if (expression == null)
                {
                    continue;
                }

                var lambda = System.Linq.Expressions.Expression.Lambda<Func<decimal>>(expression);
                var compiledLambda = lambda.Compile();
                var result = compiledLambda();
                Console.WriteLine(result);

            }
        }
    }
}
