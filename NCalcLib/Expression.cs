using System;

namespace NCalcLib
{
    public abstract class Expression : IEquatable<Expression>
    {
        public abstract int Start();
        public abstract int StartWithWhitespace();
        public abstract int Length();
        public abstract int LengthWithWhitespace();

        public abstract bool Equals(Expression other);
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
    }
}