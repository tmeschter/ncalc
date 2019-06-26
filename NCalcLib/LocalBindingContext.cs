using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using LinqExpression = System.Linq.Expressions.Expression;

namespace NCalcLib
{
    public sealed class LocalBindingContext : IBindingContext
    {
        private readonly IBindingContext _parentContext;
        private readonly ImmutableDictionary<string, ParameterExpression> _map = ImmutableDictionary<string, ParameterExpression>.Empty;

        public LocalBindingContext(IBindingContext parentContext, ImmutableDictionary<string, ParameterExpression> map)
        {
            _parentContext = parentContext;
            _map = map;
        }

        public LocalBindingContext(IBindingContext parentContext)
            : this(parentContext, ImmutableDictionary<string, ParameterExpression>.Empty)
        {
        }

        public LinqExpression CreateGetVariableExpression(string variableName) =>
            _map.TryGetValue(variableName, out var expression) ? expression : _parentContext.CreateGetVariableExpression(variableName);

        public LinqExpression CreateSetVariableExpression(string variableName, LinqExpression valueExpression) =>
            _map.TryGetValue(variableName, out var parameterExpression)
            ? LinqExpression.Assign(parameterExpression, valueExpression)
            : _parentContext.CreateSetVariableExpression(variableName, valueExpression);

        public Type GetVariableType(string variable) =>
            _map.TryGetValue(variable, out var expression) ? expression.Type : _parentContext.GetVariableType(variable);

        public IBindingContext SetVariableType(string variable, Type type) =>
            new LocalBindingContext(_parentContext, _map.SetItem(variable, LinqExpression.Parameter(type, variable)));

        public bool TryGetVariableType(string variable, out Type variableType)
        {
            if (_map.TryGetValue(variable, out var expression))
            {
                variableType = expression.Type;
                return true;
            }
            else
            {
                return _parentContext.TryGetVariableType(variable, out variableType);
            }
        }

        public IEnumerable<ParameterExpression> LocalVariables => _map.Values;
    }
}
