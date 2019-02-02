using System.Collections.Immutable;
using LinqExpression = System.Linq.Expressions.Expression;

namespace NCalcLib
{
    public sealed class TransformResult
    {
        public TransformResult(BindingContext bindingContext, LinqExpression expression, ImmutableList<Diagnostic> errors)
        {
            BindingContext = bindingContext;
            Expression = expression;
            Errors = errors;
        }

        public TransformResult(BindingContext bindingContext, LinqExpression expression)
            : this(bindingContext, expression, ImmutableList<Diagnostic>.Empty)
        {
        }

        public BindingContext BindingContext { get; }
        public LinqExpression Expression { get; }
        public ImmutableList<Diagnostic> Errors { get; }

        public void Deconstruct(out BindingContext bindingContext, out LinqExpression expression, out ImmutableList<Diagnostic> errors)
        {
            bindingContext = BindingContext;
            expression = Expression;
            errors = Errors;
        }
    }
}
