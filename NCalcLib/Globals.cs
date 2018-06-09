using System.Collections.Generic;

namespace NCalcLib
{
    public sealed class Globals
    {
        public static Globals Singleton { get; } = new Globals();

        private Dictionary<string, decimal> _variables;

        public Globals()
        {
            _variables = new Dictionary<string, decimal>();
        }

        public decimal GetVariable(string variable)
        {
            if (_variables.TryGetValue(variable, out decimal value))
            {
                return value;
            }
            else
            {
                return 0m;
            }
        }

        public decimal SetVariable(string variable, decimal value)
        {
            _variables[variable] = value;
            return value;
        }
    }
}
