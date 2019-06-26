using System;
using System.Collections.Immutable;

namespace NCalcLib
{
    public abstract class Block : Node, IEquatable<Block>
    {
        public abstract ImmutableArray<Statement> Statements { get; }

        public abstract int Start();
        public abstract int StartWithWhitespace();
        public abstract int Length();
        public abstract int LengthWithWhitespace();

        public abstract bool Equals(Block other);
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
    }
}
