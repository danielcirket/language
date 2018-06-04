using System;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;
using Compiler.Semantics.Symbols;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal class BoundVariableDeclaration : BoundDeclaration
    {
        public VariableMutabilityType MutabilityType => SyntaxNode<VariableDeclaration>().MutabilityType;
        public BoundExpression Value { get; }

        public BoundVariableDeclaration(
            VariableDeclaration declaration,
            BoundTypeExpression type, 
            BoundExpression value,
            Scope scope
        )
            : base(declaration, type, scope)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }
    }
}
