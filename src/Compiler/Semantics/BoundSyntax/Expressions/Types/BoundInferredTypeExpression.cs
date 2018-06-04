using Compiler.Parsing.Syntax.Expressions.Types;
using Compiler.Semantics.BoundSyntax.Declarations;

namespace Compiler.Semantics.BoundSyntax.Expressions.Types
{
    internal class BoundInferredTypeExpression : BoundTypeExpression
    {
        public BoundInferredTypeExpression(InferredTypeExpression expression, Scope scope) : 
            base(expression.Name, expression, null, scope)
        {

        }
    }
}
