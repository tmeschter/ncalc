using System.Collections.Immutable;
using LinqExpression = System.Linq.Expressions.Expression;

namespace NCalcLib
{
    public sealed class TransformResult
    {
        public TransformResult(BindingContext bindingContext, LinqExpression expression, ImmutableList<string> errors)
        {
            BindingContext = bindingContext;
            Expression = expression;
            Errors = errors;
        }

        public TransformResult(BindingContext bindingContext, LinqExpression expression)
            : this(bindingContext, expression, ImmutableList<string>.Empty)
        {
        }

        public BindingContext BindingContext { get; }
        public LinqExpression Expression { get; }
        public ImmutableList<string> Errors { get; }

        public void Deconstruct(out BindingContext bindingContext, out LinqExpression expression, out ImmutableList<string> errors)
        {
            bindingContext = BindingContext;
            expression = Expression;
            errors = Errors;
        }
    }
}
