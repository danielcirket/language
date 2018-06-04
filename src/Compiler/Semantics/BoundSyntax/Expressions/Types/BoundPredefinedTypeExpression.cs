using Compiler.Parsing.Syntax.Expressions.Types;
using Compiler.Semantics.BoundSyntax.Declarations;
using Compiler.Semantics.Symbols;

namespace Compiler.Semantics.BoundSyntax.Expressions.Types
{
    internal class BoundPredefinedTypeExpression : BoundTypeExpression
    {
        public BoundPredefinedTypeExpression(PredefinedTypeExpression expression, Symbol symbol, Scope scope) : 
            base(expression.Name, expression, symbol, scope)
        {

        }
    }
}
