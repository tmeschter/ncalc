using System;

namespace NCalcLib
{
    public abstract class Statement : Node, IEquatable<Statement>
    {
        public abstract int Start();
        public abstract int StartWithWhitespace();
        public abstract int Length();
        public abstract int LengthWithWhitespace();

        public abstract bool Equals(Statement other);
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
    }
}