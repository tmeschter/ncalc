using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCalcLib
{
    public sealed class BindingContext
    {
        public static BindingContext Empty = new BindingContext(ImmutableDictionary<string, Type>.Empty);

        private readonly ImmutableDictionary<string, Type> _map;

        private BindingContext(ImmutableDictionary<string, Type> map)
        {
            _map = map;
        }

        public Type GetVariableType(string variable)
        {
            return _map[variable];
        }

        public BindingContext SetVariableType(string variable, Type type)
        {
            return new BindingContext(_map.SetItem(variable, type));
        }
    }
}
