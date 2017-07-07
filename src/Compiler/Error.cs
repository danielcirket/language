using System;
using System.Collections.Generic;

namespace Compiler
{
    internal class Error
    {
        public IEnumerable<string> Lines { get; }
        public string Message { get; }
        public Severity Severity { get; }
        public SourceFilePart FilePart { get; }

        public Error(string message, IEnumerable<string> contents, Severity severity, SourceFilePart filePart)
        {
            message = Message;
            Lines = contents;
            Severity = severity;
            FilePart = filePart;
        }
    }
}
