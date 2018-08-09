using Compiler.Parsing.Syntax.Expressions;
using Compiler.Semantics.BoundSyntax.Declarations;
using Compiler.Semantics.Symbols;

namespace Compiler.Semantics.BoundSyntax.Expressions
{
    internal class BoundIdentifierExpression : BoundExpression
    {
        public string Name => SyntaxNode<IdentifierExpression>().Name;
        public Symbol Symbol { get; }
        public override BoundTypeExpression Type => Symbol?.Declaration?.Type;

        public BoundIdentifierExpression(
            IdentifierExpression expression,
            Scope scope)
            : base(expression, scope)
        {
        }
        public BoundIdentifierExpression(
            IdentifierExpression expression,
            Symbol symbol,
            Scope scope)
            : base(expression, scope)
        {
            Symbol = symbol;
        }
        public BoundIdentifierExpression(
            IdentifierExpression expression,
            Symbol declaration,
            BoundTypeExpression type,
            Scope scope)
            : base(expression, type, scope)
        {
            Symbol = declaration;
        }
    }
}
