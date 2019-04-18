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

                var tokens = Lexer.LexSubmission(input);
                var expressionSyntax = Parser.ParseSubmission(tokens);
                if (expressionSyntax == null)
                {
                    continue;
                }

                (var newBindingContext, var expression, var errors) = Transformer.Transform(globalBindingContext, expressionSyntax);
                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error);
                    }

                    continue;
                }

                if (expression == null)
                {
                    continue;
                }

                expression = LinqExpression.Convert(expression, typeof(object));

                globalBindingContext = newBindingContext;

                var lambda = LinqExpression.Lambda<Func<object>>(expression);
                var compiledLambda = lambda.Compile();
                var result = compiledLambda();
                Console.WriteLine(result);
            }
        }
    }
}
