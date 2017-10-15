using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax;
using Compiler.Semantics;

namespace Compiler.Parsing
{
    internal class CompilationUnit
    {
        public IEnumerable<SyntaxNode> Children { get; }
        public Scope Scope { get; }

        public CompilationUnit(IEnumerable<SyntaxNode> children)
        {
            if (children == null)
                throw new ArgumentNullException(nameof(children));

            Children = children;
        }
        public CompilationUnit(CompilationUnit compilationUnit, IEnumerable<SyntaxNode> children, Scope scope)
            : this(children)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));

            Scope = scope;
        }
    }
}
