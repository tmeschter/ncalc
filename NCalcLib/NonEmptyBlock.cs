using System.Collections.Immutable;
using System.Linq;
using static NCalcLib.Utilities.HashCodes;

namespace NCalcLib
{
    public sealed class NonEmptyBlock : Block
    {
        public override ImmutableArray<Statement> Statements { get; }

        public NonEmptyBlock(ImmutableArray<Statement> statements)
        {
            Statements = statements;
        }

        public override int Start() => Statements[0].Start();

        public override int StartWithWhitespace() => Statements[0].StartWithWhitespace();

        public override int Length()
            => Statements[0].Length() + Statements.Skip(1).Aggregate(0, (a, s) => a + s.LengthWithWhitespace());

        public override int LengthWithWhitespace()
            => Statements.Aggregate(0, (a, s) => a + s.LengthWithWhitespace());

        public override bool Equals(Block other)
            => other is NonEmptyBlock block
                && Statements.Length == block.Statements.Length
                && Statements.Zip(block.Statements, (left, right) => left.Equals(right)).All(v => v);

        public override bool Equals(object obj) => Equals(obj as Block);

        public override int GetHashCode() => Hash(Statements.Select(s => s.GetHashCode()));
    }
}
