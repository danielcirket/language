using System;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;
using Compiler.Semantics.Symbols;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal class BoundFieldDeclaration : BoundDeclaration
    {
        public SyntaxModifier Modifier => SyntaxNode<FieldDeclaration>().Modifier;
        public BoundExpression DefaultValue { get; }

        public BoundFieldDeclaration(
            FieldDeclaration field,
            Scope scope
        )
           : base(field, scope)
        {
        }
        public BoundFieldDeclaration(
            FieldDeclaration field,
            BoundTypeExpression type,
            Scope scope
        )
           : base(field, type, scope)
        {
            
        }
        public BoundFieldDeclaration(
            FieldDeclaration field,
            BoundTypeExpression type,
            BoundExpression defaultValue,
            Scope scope
        )
           : base(field, type, scope)
        {
            DefaultValue = defaultValue;
        }
    }
}
