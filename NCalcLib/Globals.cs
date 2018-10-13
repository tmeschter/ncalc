using System.Collections.Generic;

namespace NCalcLib
{
    public sealed class Globals
    {
        public static Globals Singleton { get; } = new Globals();

        private Dictionary<string, object> _variables;

        public Globals()
        {
            _variables = new Dictionary<string, object>();
        }

        public object GetVariable(string variable)
        {
            if (_variables.TryGetValue(variable, out object value))
            {
                return value;
            }
            else
            {
                return 0m;
            }
        }

        public object SetVariable(string variable, object value)
        {
            _variables[variable] = value;
            return value;
        }
    }
}
