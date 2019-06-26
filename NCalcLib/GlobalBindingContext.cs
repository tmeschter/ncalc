using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using LinqExpression = System.Linq.Expressions.Expression;

namespace NCalcLib
{
    public sealed class GlobalBindingContext : IBindingContext
    {
        public static GlobalBindingContext Empty = new GlobalBindingContext(ImmutableDictionary<string, Type>.Empty);

        private readonly ImmutableDictionary<string, Type> _map;

        public IEnumerable<ParameterExpression> LocalVariables => Enumerable.Empty<ParameterExpression>();

        private GlobalBindingContext(ImmutableDictionary<string, Type> map) => _map = map;

        public Type GetVariableType(string variable) => _map[variable];

        public bool TryGetVariableType(string variable, out Type variableType) => _map.TryGetValue(variable, out variableType);

        public IBindingContext SetVariableType(string variable, Type type) => new GlobalBindingContext(_map.SetItem(variable, type));

        public LinqExpression CreateGetVariableExpression(string variableName)
        {
            if (!TryGetVariableType(variableName, out var variableType))
            {
                return null;
            }

            return GetGlobalVariableExpression(variableName, variableType);
        }

        public LinqExpression CreateSetVariableExpression(string variableName, LinqExpression expression)
        {
            if (!TryGetVariableType(variableName, out var variableType))
            {
                return null;
            }

            return SetGlobalVariableExpression(variableName, expression);
        }

        private static LinqExpression GetGlobalVariableExpression(string variableName, Type variableType)
        {
            return LinqExpression.Convert(
                LinqExpression.Call(
                    LinqExpression.Property(
                        expression: null,
                        property: typeof(Globals).GetProperty(nameof(Globals.Singleton))),
                    typeof(Globals).GetMethod(nameof(Globals.GetVariable)),
                    LinqExpression.Constant(variableName)),
                variableType);
        }

        private static LinqExpression SetGlobalVariableExpression(string variableName, LinqExpression expression)
        {
            return LinqExpression.Call(
                    LinqExpression.Property(
                        expression: null,
                        property: typeof(Globals).GetProperty(nameof(Globals.Singleton))),
                    typeof(Globals).GetMethod(nameof(Globals.SetVariable)),
                    LinqExpression.Constant(variableName),
                    LinqExpression.Convert(expression, typeof(object)));
        }
    }
}
