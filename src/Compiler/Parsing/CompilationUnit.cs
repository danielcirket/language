using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax;

namespace Compiler.Parsing
{
    internal class CompilationUnit
    {
        public IEnumerable<SyntaxNode> Children { get; }
        
        public CompilationUnit(IEnumerable<SyntaxNode> children)
        {
            if (children == null)
                throw new ArgumentNullException(nameof(children));

            Children = children;
        }
    }
}
