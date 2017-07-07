using System;
using System.Collections.Generic;
using System.IO;

namespace Compiler
{
    internal class SourceFile
    {
        public string Name { get; }
        public string FullPath { get; }
        public string Contents { get; }
        public IEnumerable<string> Lines { get; }

        public SourceFile(string path, string source)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            FullPath = path;
            Name = Path.GetFileName(path);
            Contents = source;
            Lines = Contents.Split(new[] { "\n", "\r\n" }, options: StringSplitOptions.None);
        }
    }
}
