using System.Collections.Immutable;
using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public sealed class EmptyBlock : Block
    {
        private readonly int _start;

        public override ImmutableArray<Statement> Statements => ImmutableArray<Statement>.Empty;

        public EmptyBlock(int start)
        {
            _start = start;
        }

        public override bool Equals(Block other) 
            => other is EmptyBlock block
                && _start == block._start;

        public override bool Equals(object obj) => Equals(obj as Block);

        public override int GetHashCode() => Hash(_start);

        public override int Length() => 0;

        public override int LengthWithWhitespace() => 0;

        public override int Start() => _start;

        public override int StartWithWhitespace() => _start;
    }
}
