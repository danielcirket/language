using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Parsing
{
    internal class CompilationUnit
    {
        public IEnumerable<ImportStatement> Imports { get; }
        public IEnumerable<ModuleDeclaration> Modules { get; }

        public CompilationUnit(IEnumerable<ImportStatement> imports, IEnumerable<ModuleDeclaration> modules)
        {
            if (imports == null)
                throw new ArgumentNullException(nameof(imports));

            if (modules == null)
                throw new ArgumentNullException(nameof(modules));

            Imports = imports;
            Modules = modules;
        }
    }
}
