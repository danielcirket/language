using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;
using Compiler.Semantics.Symbols;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal class BoundPropertyDeclaration : BoundDeclaration
    {
        public SyntaxModifier Modifier => SyntaxNode<PropertyDeclaration>().Modifier;
        public Symbol Getter { get; }
        public Symbol Setter { get; }

        public BoundPropertyDeclaration(
            PropertyDeclaration property,
            Symbol getter,
            Symbol setter,
            Scope scope
        )
            : base(property, scope)
        {
            Getter = getter;
            Setter = setter;
        }
        public BoundPropertyDeclaration(
            PropertyDeclaration property,
            BoundTypeExpression type,
            Symbol getter,
            Symbol setter,
            Scope scope
        )
            : base(property, type, scope)
        {           
            Getter = getter;
            Setter = setter;
        }
    }
}
