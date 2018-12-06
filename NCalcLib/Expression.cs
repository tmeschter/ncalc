using System;

namespace NCalcLib
{
    public abstract class Expression : IEquatable<Expression>
    {
        public abstract bool Equals(Expression other);
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
    }
}