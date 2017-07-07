using System.Collections.Generic;

namespace Compiler
{
    internal class SourceFilePart
    {
        public string FilePath { get; }
        public SourceFileLocation Start { get; }
        public SourceFileLocation End { get; }
        public IEnumerable<string> Lines { get; }

        public SourceFilePart(string path, SourceFileLocation start, SourceFileLocation end, IEnumerable<string> lines)
        {
            FilePath = path;
            Start = start;
            End = end;
            Lines = lines;
        }
    }
}
