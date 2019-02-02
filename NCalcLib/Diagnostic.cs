using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCalcLib
{
    public sealed class Diagnostic
    {
        public Diagnostic(int start, int length, string message)
        {
            Start = start;
            Length = length;
            Message = message;
        }

        public int Start { get; }
        public int Length { get; }
        public string Message { get; }
    }
}
