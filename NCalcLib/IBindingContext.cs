using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using LinqExpression = System.Linq.Expressions.Expression;

namespace NCalcLib
{
    public interface IBindingContext
    {
        LinqExpression CreateGetVariableExpression(string variableName);
        LinqExpression CreateSetVariableExpression(string variableName, LinqExpression expression);
        Type GetVariableType(string variable);
        IBindingContext SetVariableType(string variable, Type type);
        bool TryGetVariableType(string variable, out Type variableType);
        IEnumerable<ParameterExpression> LocalVariables { get; }
    }
}