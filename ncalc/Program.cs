using System;
using System.Collections.Generic;
using System.Linq;
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
                OutputErrors(input, errors);

                return (true, bindingContext);
            }

            var lambda = LinqExpression.Lambda<Action>(expression);
            var compiledLambda = lambda.Compile();
            compiledLambda();

            return (true, (GlobalBindingContext)newBindingContext);
        }

        private static void OutputErrors(string input, System.Collections.Immutable.ImmutableList<Diagnostic> errors)
        {
            var lineMap = new LineMap(input);

            foreach (var error in errors)
            {
                int firstPosition = error.Start;
                int lastPosition = error.Start + error.Length - 1;

                (int firstLine, int firstColumn) = lineMap.MapPositionToLineAndColumn(firstPosition);
                (int lastLine, int lastColumn) = lineMap.MapPositionToLineAndColumn(lastPosition);

                if (firstLine == lastLine)
                {
                    Console.WriteLine($"Line {firstLine + 1}: {error.Message}");
                    string lineText = lineMap.GetLineText(firstLine);
                    Console.WriteLine($"  {lineText}");
                    Console.WriteLine($"  {new string(' ', firstColumn)}{new string('^', lastColumn - firstColumn + 1)}");
                }
                else
                {
                    Console.WriteLine($"Lines {firstLine + 1}-{lastLine + 1}: {error.Message}");

                    // First line
                    string lineText = lineMap.GetLineText(firstLine);
                    Console.WriteLine($"  {lineText}");
                    Console.WriteLine($"  {new string(' ', firstColumn)}{new string('^', lineText.Length - firstColumn)}");

                    // Middle lines
                    for (int index = firstLine + 1; index < lastLine; index++)
                    {
                        lineText = lineMap.GetLineText(index);
                        Console.WriteLine($"  {lineText}");
                        Console.WriteLine($"  {new string('^', lineText.Length)}");
                    }

                    // End line
                    if (lastLine != firstLine)
                    {
                        lineText = lineMap.GetLineText(lastLine);
                        Console.WriteLine($"  {lineText}");
                        Console.WriteLine($"  {new string('^', lastColumn + 1)}");
                    }
                }
            }
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
                OutputErrors(input, errors);

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
