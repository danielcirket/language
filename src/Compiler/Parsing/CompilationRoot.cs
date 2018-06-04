using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax;
//using Compiler.Semantics;

namespace Compiler.Parsing
{
    internal class CompilationRoot
    {
        public IEnumerable<CompilationUnit> CompilationUnits { get; }

        public CompilationRoot(IEnumerable<CompilationUnit> compilationUnits)
        {
            if (compilationUnits == null)
                throw new ArgumentNullException(nameof(compilationUnits));

            CompilationUnits = compilationUnits;
        }
    }
}
