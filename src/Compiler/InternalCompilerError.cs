using System;

namespace Compiler
{
    internal class InternalCompilerError : Exception
    {
        public InternalCompilerError(string message) : base(message) { }
        public InternalCompilerError(string message, Exception innerException) : base(message, innerException) { }

        public InternalCompilerError() { }
    }
}
