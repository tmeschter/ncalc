using System;
using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public struct Whitespace : IEquatable<Whitespace>
    {
        public Whitespace(int start, string value)
        {
            Start = start;
            Value = value;
        }

        public int Start { get; }
        public int Length => Value.Length;
        public string Value { get; }
        public bool IsEmpty() => Length == 0;

        public bool Equals(Whitespace other) =>
            Start == other.Start
            && Value.Equals(other.Value);

        public override int GetHashCode() =>
            Hash(
                Start.GetHashCode(),
                Value.GetHashCode());
    }
}
