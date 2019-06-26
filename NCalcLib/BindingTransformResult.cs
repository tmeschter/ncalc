using System.Collections.Immutable;
using LinqExpression = System.Linq.Expressions.Expression;

namespace NCalcLib
{
    public sealed class BindingTransformResult
    {
        public BindingTransformResult(LinqExpression expression, ImmutableList<Diagnostic> errors)
        {
            Expression = expression;
            Errors = errors;
        }

        public BindingTransformResult(LinqExpression expression)
            : this(expression, ImmutableList<Diagnostic>.Empty)
        {
        }

        public LinqExpression Expression { get; }
        public ImmutableList<Diagnostic> Errors { get; }

        public void Deconstruct(out LinqExpression expression, out ImmutableList<Diagnostic> errors)
        {
            expression = Expression;
            errors = Errors;
        }
    }
}
