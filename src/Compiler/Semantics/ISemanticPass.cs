using Compiler.Parsing;

namespace Compiler.Semantics
{
    internal interface ISemanticPass
    {
        bool ShouldContinue { get; }
        void Run(ErrorSink errorSink, ref CompilationUnit compilationUnit);
    }
}
