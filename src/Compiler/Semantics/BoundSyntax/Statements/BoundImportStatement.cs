using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Parsing.Syntax.Statements;
using Compiler.Semantics.BoundSyntax.Declarations;

namespace Compiler.Semantics.BoundSyntax.Statements
{
    internal class BoundImportStatement : BoundStatement
    {
        //public override SyntaxKind Kind => throw new NotImplementedException();
        public IEnumerable<IdentifierExpression> Names => SyntaxNode<ImportStatement>().Names;
        public string Name => SyntaxNode<ImportStatement>().Name;
        public BoundModuleDeclaration Module { get; }

        public BoundImportStatement(
            ImportStatement statement,
            BoundModuleDeclaration module)
            : base(statement, module?.Scope ?? new Scope())
        {
            Module = module;
        }
    }
}
