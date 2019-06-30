using System.Collections.Generic;
using System.Linq;

namespace NCalcLib.Utilities
{
    internal static class HashCodes
    {
        public static int Hash(int input)
        {
            int hash = 17;
            hash = Mutate(hash, input);
            return hash;
        }

        public static int Hash(int input1, int input2)
        {
            int hash = 17;
            hash = Mutate(hash, input1);
            hash = Mutate(hash, input2);
            return hash;
        }

        public static int Hash(int input1, int input2, int input3)
        {
            int hash = 17;
            hash = Mutate(hash, input1);
            hash = Mutate(hash, input2);
            hash = Mutate(hash, input3);
            return hash;
        }

        public static int Hash(int input1, int input2, int input3, int input4)
        {
            int hash = 17;
            hash = Mutate(hash, input1);
            hash = Mutate(hash, input2);
            hash = Mutate(hash, input3);
            hash = Mutate(hash, input4);
            return hash;
        }

        public static int Hash(int input1, int input2, int input3, int input4, int input5)
        {
            int hash = 17;
            hash = Mutate(hash, input1);
            hash = Mutate(hash, input2);
            hash = Mutate(hash, input3);
            hash = Mutate(hash, input4);
            hash = Mutate(hash, input5);
            return hash;
        }

        public static int Hash(IEnumerable<int> inputs)
        {
            return inputs.Aggregate(17, Mutate); 
        }

        public static int Hash(params int[] inputs)
        {
            return Hash((IEnumerable<int>)inputs);
        }

        private static int Mutate(int seed, int input)
        {
            return seed * 31 + input;
        }
    }
}
