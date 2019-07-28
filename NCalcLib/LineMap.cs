using System;
using System.Collections.Generic;

namespace NCalcLib
{
    public struct LineAndColumn : IEquatable<LineAndColumn>
    {
        public LineAndColumn(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; }
        public int Column { get; }

        public void Deconstruct(out int line, out int column)
        {
            line = Line;
            column = Column;
        }

        public bool Equals(LineAndColumn other)
        {
            return Line == other.Line
                && Column == other.Column;
        }

        public override bool Equals(object obj)
        {
            return obj is LineAndColumn other && Equals(other);
        }

        public override string ToString()
        {
            return $"({Line}, {Column})";
        }
    }

    public class LineMap
    {
        private string text;
        private List<string> lines;

        public LineMap(string text)
        {
            this.text = text ?? throw new ArgumentNullException(nameof(text));

            int lineStart = 0;

            List<string> lines = new List<string>();

            var endOfLineChars = new[] { '\r', '\n' };

            int endOfLineCharIndex;
            while ((endOfLineCharIndex = text.IndexOfAny(endOfLineChars, lineStart)) >= 0)
            {
                if (text[endOfLineCharIndex] == '\r'
                    && endOfLineCharIndex + 1 < text.Length
                    && text[endOfLineCharIndex + 1] == '\n')
                {
                    endOfLineCharIndex++;
                }

                var lineLength = endOfLineCharIndex - lineStart + 1;

                lines.Add(text.Substring(lineStart, lineLength));

                lineStart = lineStart + lineLength;
            }

            lines.Add(text.Substring(lineStart));

            this.lines = lines;
        }

        public int LineCount => lines.Count;

        public LineAndColumn MapPositionToLineAndColumn(int position)
        {
            if (position < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            int lineStart = 0;

            for (int i = 0; i < lines.Count; i++)
            {
                if (position >= lineStart && position < lineStart + lines[i].Length)
                {
                    return new LineAndColumn(i, position - lineStart);
                }

                lineStart = lineStart + lines[i].Length;
            }

            return new LineAndColumn(lines.Count - 1, lines[lines.Count - 1].Length);
        }

        public string GetLineText(int line) => lines[line].TrimEnd(new[] { '\r', '\n' });
    }
}