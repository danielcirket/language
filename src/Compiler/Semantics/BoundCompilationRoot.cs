using System;
using System.Collections.Generic;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics
{
    internal class BoundCompilationRoot
    {
        public IEnumerable<BoundCompilationUnit> CompilationUnits { get; }
        public Scope Scope { get; }
        public Dictionary<string, BoundTypeExpression> PredefinedTypeMap { get; }

        public BoundCompilationRoot(
            IEnumerable<BoundCompilationUnit> compilationUnits,
            Dictionary<string, BoundTypeExpression> predefinedTypeMap,
            Scope scope
            )
        {
            if (compilationUnits == null)
                throw new ArgumentNullException(nameof(compilationUnits));
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            if (predefinedTypeMap == null)
                throw new ArgumentNullException(nameof(predefinedTypeMap));

            CompilationUnits = compilationUnits;
            Scope = scope;
            PredefinedTypeMap = predefinedTypeMap;
        }
    }
}
