using Compiler.Parsing;

namespace Compiler.Semantics
{
    internal interface ISemanticPass
    {
        bool ShouldContinue { get; }
        void Run(ref BoundCompilationRoot compilationRoot);
    }

}
