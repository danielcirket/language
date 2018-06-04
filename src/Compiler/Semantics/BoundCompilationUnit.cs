using System;
using System.Collections.Generic;
using Compiler.Parsing;
using Compiler.Semantics.BoundSyntax.Declarations;
using Compiler.Semantics.BoundSyntax.Statements;

namespace Compiler.Semantics
{
    internal class BoundCompilationUnit 
    {
        private readonly CompilationUnit _compilationUnit;

        public IEnumerable<BoundImportStatement> Imports { get; }
        public IEnumerable<BoundModuleDeclaration> Modules { get; }
        public Scope Scope { get; }

        public CompilationUnit CompilationSyntax() => _compilationUnit;

        public BoundCompilationUnit(CompilationUnit compilationUnit, IEnumerable<BoundImportStatement> imports, IEnumerable<BoundModuleDeclaration> modules, Scope scope)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));
            if (imports == null)
                throw new ArgumentNullException(nameof(imports));
            if (modules == null)
                throw new ArgumentNullException(nameof(modules));
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));

            _compilationUnit = compilationUnit;
            Imports = imports;
            Modules = modules;
            Scope = scope;
        }
    }
}
