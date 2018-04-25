using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private static int Mutate(int seed, int input)
        {
            return seed * 31 + input;
        }
    }
}
