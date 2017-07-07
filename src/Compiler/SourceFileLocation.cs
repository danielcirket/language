using System;

namespace Compiler
{
    internal class SourceFileLocation
    {
        public int Column { get; }
        public int Index { get; }
        public int LineNumber { get; }

        public SourceFileLocation(int column, int index, int lineNo)
        {
            if (column < 0)
                throw new ArgumentOutOfRangeException(nameof(column));

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (lineNo < 0)
                throw new ArgumentOutOfRangeException(nameof(lineNo));

            Column = column;
            Index = index;
            LineNumber = lineNo;
        }
    }
}
