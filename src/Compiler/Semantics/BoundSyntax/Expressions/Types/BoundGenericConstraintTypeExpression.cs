using Compiler.Parsing.Syntax.Expressions.Types;
using Compiler.Semantics.BoundSyntax.Declarations;
using Compiler.Semantics.Symbols;

namespace Compiler.Semantics.BoundSyntax.Expressions.Types
{
    internal class BoundGenericConstraintTypeExpression : BoundTypeExpression
    {
        public BoundGenericConstraintTypeExpression(GenericConstraintTypeExpression expression, Symbol declaration, Scope scope) : 
            base(expression.Name, expression, declaration, scope)
        {

        }
    }
}
