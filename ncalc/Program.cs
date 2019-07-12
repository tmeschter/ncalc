using System;
using System.Text;
using NCalcLib;
using LinqExpression = System.Linq.Expressions.Expression;

namespace ncalc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var globalBindingContext = GlobalBindingContext.Empty;

            var inputBuilder = new StringBuilder();

            while (true)
            {
                if (inputBuilder.Length == 0)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write(". ");
                }

                var rawInput = Console.ReadLine();

                if (string.IsNullOrEmpty(rawInput))
                {
                    inputBuilder.Clear();
                    continue;
                }

                inputBuilder.AppendLine(rawInput);

                var input = inputBuilder.ToString();

                bool success;
                (success, globalBindingContext) = TryProcessInput(input, globalBindingContext);
                if (success)
                {
                    inputBuilder.Clear();
                }
            }
        }

        private static (bool success, GlobalBindingContext newBindingContext) TryProcessInput(string input, GlobalBindingContext bindingContext)
        {
            var (succeeded, newBindingContext) = HandleExpression(input, bindingContext);
            if (succeeded)
            {
                return (true, newBindingContext);
            }

            (succeeded, newBindingContext) = HandleStatement(input, bindingContext);
            if (succeeded)
            {
                return (true, newBindingContext);
            }

            return (false, bindingContext);
        }

        private static (bool, GlobalBindingContext) HandleStatement(string input, GlobalBindingContext bindingContext)
        {
            var tokens = Lexer.LexSubmission(input);
            var statementSyntax = Parser.ParseStatementSubmission(tokens);
            if (statementSyntax == null)
            {
                return (false, bindingContext);
            }

            var (newBindingContext, expression, errors) = Transformer.Transform(bindingContext, statementSyntax);
            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    Console.WriteLine(error.Message);
                    Console.WriteLine($"  {statementSyntax}");
                    Console.WriteLine($"  {new string(' ', error.Start)}{new string('^', error.Length)}");
                }

                return (true, bindingContext);
            }

            var lambda = LinqExpression.Lambda<Action>(expression);
            var compiledLambda = lambda.Compile();
            compiledLambda();

            return (true, (GlobalBindingContext)newBindingContext);
        }

        private static (bool, GlobalBindingContext) HandleExpression(string input, GlobalBindingContext bindingContext)
        {
            var tokens = Lexer.LexSubmission(input);
            var expressionSyntax = Parser.ParseExpressionSubmission(tokens);
            if (expressionSyntax == null)
            {
                return (false, bindingContext);
            }

            var (newBindingContext, expression, errors) = Transformer.Transform(bindingContext, expressionSyntax);
            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    Console.WriteLine(error.Message);
                    Console.WriteLine($"  {expressionSyntax}");
                    Console.WriteLine($"  {new string(' ', error.Start)}{new string('^', error.Length)}");
                }

                return (true, bindingContext);
            }

            expression = LinqExpression.Convert(expression, typeof(object));

            var lambda = LinqExpression.Lambda<Func<object>>(expression);
            var compiledLambda = lambda.Compile();
            var result = compiledLambda();
            Console.WriteLine(result);

            return (true, (GlobalBindingContext)newBindingContext);
        }
    }
}
