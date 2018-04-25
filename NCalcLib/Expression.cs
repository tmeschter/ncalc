using System;

namespace NCalcLib
{
    public abstract class Expression : Node, IEquatable<Expression>
    {
        public Expression(int start, int length)
            : base(start, length)
        {
        }

        public abstract bool Equals(Expression other);
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
    }
}