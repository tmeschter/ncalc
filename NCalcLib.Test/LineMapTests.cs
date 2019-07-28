using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NCalcLib.Test
{
    public class LineMapTests
    {
        [Theory]
        [InlineData("", 1)]
        [InlineData("\r\n", 2)]
        [InlineData("\r\n\r\n", 3)]
        [InlineData("alpha\r\n", 2)]
        [InlineData("\r\nbeta", 2)]
        [InlineData("alpha\r\nbeta", 2)]
        [InlineData("alpha\r\nbeta\r\b", 3)]
        [InlineData("alpha\r\nbeta\r\bgamma", 3)]
        public void LineCounts(string text, int lineCount)
        {
            var map = new LineMap(text);

            Assert.Equal(expected: lineCount, actual: map.LineCount);
        }

        [Theory]
        [InlineData("", 0, 0, 0)]
        [InlineData("\r\n", 0, 0, 0)]
        [InlineData("\r\n", 1, 0, 1)]
        [InlineData("\r\n", 2, 1, 0)]
        [InlineData("alpha", 0, 0, 0)]
        [InlineData("alpha", 4, 0, 4)]
        [InlineData("alpha", 5, 0, 5)]
        [InlineData("alpha\r\nbeta", 7, 1, 0)]
        [InlineData("alpha\r\nbeta", 9, 1, 2)]
        [InlineData("alpha\r\nbeta\r\bgamma", 14, 2, 2)]
        public void MapPositionToLineAndColumn(string text, int position, int expectedLine, int expectedColumn)
        {
            var map = new LineMap(text);

            var lineAndColumn = map.MapPositionToLineAndColumn(position);

            Assert.Equal(expected: new LineAndColumn(expectedLine, expectedColumn), actual: lineAndColumn);
        }

        [Fact]
        public void Constructor_NullTextThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var map = new LineMap(null);
            });
        }

        [Fact]
        public void MapPositionToLineAndColumn_InvalidPositionThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var map = new LineMap("");

                var lineAndColumn = map.MapPositionToLineAndColumn(-1);
            });
        }

        [Theory]
        [InlineData("", 0, "")]
        [InlineData("\r\n", 0, "")]
        [InlineData("\r\n", 1, "")]
        [InlineData("alpha\r\nbeta", 0, "alpha")]
        [InlineData("alpha\r\nbeta", 1, "beta")]
        [InlineData("alpha\r\nbeta\r\ngamma", 1, "beta")]
        public void GetLineText(string text, int line, string expectedLineText)
        {
            var map = new LineMap(text);
            var lineText = map.GetLineText(line);

            Assert.Equal(expected: expectedLineText, actual: lineText);
        }
    }
}
