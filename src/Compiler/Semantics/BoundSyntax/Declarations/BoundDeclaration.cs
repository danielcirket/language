using System;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;
using Compiler.Semantics.Symbols;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal abstract class BoundDeclaration : BoundSyntaxNode
    {
        public Scope Scope { get; }
        public string Name => SyntaxNode<Declaration>().Name;
        public SourceFilePart FilePart => SyntaxNode<Declaration>().FilePart;
        public BoundTypeExpression Type { get; }

        protected BoundDeclaration(Declaration declaration, Scope scope)
            : base(declaration)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));

            Scope = scope;
        }
        protected BoundDeclaration(Declaration declaration, BoundTypeExpression type, Scope scope)
           : this(declaration, scope)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Type = type;
        }
    }
}
