namespace NCalcLib
{
    public class ParseResult<T> where T : Node
    {
        public T Node { get; }
        public int NextTokenIndex { get; }

        public ParseResult(T node, int nextTokenIndex)
        {
            Node = node;
            NextTokenIndex = nextTokenIndex;
        }
    }
}
