using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal class BoundEnumDeclaration : BoundDeclaration
    {
        public SyntaxModifier Modifier => SyntaxNode<EnumDeclaration>().Modifier;
        public IEnumerable<BoundEnumMemberDeclaration> Members { get; }

        public BoundEnumDeclaration(
            EnumDeclaration declaration,
            IEnumerable<BoundEnumMemberDeclaration> members,
            Scope scope
        ) 
            : base(declaration, scope)
        {
            if (members == null)
                throw new ArgumentNullException(nameof(members));

            Members = members;
        }
    }
}
