using System;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Parsing.Syntax.Expressions.Types;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal class BoundTypeDeclaration : BoundDeclaration
    {
        public string Name => SyntaxNode<TypeExpression>().Name;
        public BoundDeclaration Source { get; }

        public BoundTypeDeclaration(
            TypeExpression declaration,
            BoundDeclaration source) 
            : base(declaration, source.Scope)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Source = source;
        }
    }
}
