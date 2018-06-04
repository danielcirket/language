using Compiler.Parsing.Syntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal class BoundEnumMemberDeclaration : BoundDeclaration
    {
        public BoundExpression Value { get; }

        public BoundEnumMemberDeclaration(
            EnumMemberDeclaration member,
            BoundExpression value,
            Scope scope
        ) 
            : base(member, scope)
        {
            // NOTE(Dan): Value can be null to allow enum X { One, Two, Three }
            Value = value;
        }
    }
}
