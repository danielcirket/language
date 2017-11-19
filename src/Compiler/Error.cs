using System;
using System.Collections.Generic;
using Compiler.Lexing;

namespace Compiler
{
    internal class Error
    {
        public Token Token { get; }
        public IEnumerable<string> Lines { get; }
        public string Message { get; }
        public Severity Severity { get; }
        public SourceFilePart FilePart { get; }

        public Error(string message, Token token, IEnumerable<string> contents, Severity severity, SourceFilePart filePart)
        {
            Token = token;
            Message = message;
            Lines = contents;
            Severity = severity;
            FilePart = filePart;
        }
    }
}
