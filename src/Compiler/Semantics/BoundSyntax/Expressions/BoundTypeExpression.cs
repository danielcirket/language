using Compiler.Parsing.Syntax.Expressions;
using Compiler.Semantics.Symbols;

namespace Compiler.Semantics.BoundSyntax.Expressions
{
    internal class BoundTypeExpression : BoundExpression
    {
        public string Name { get; }
        public Symbol Symbol { get; }
        public override BoundTypeExpression Type => this;

        public BoundTypeExpression(string name, Expression expression, Symbol symbol, Scope scope) 
            : base(expression, scope)
        {
            Name = name;
            Symbol = symbol;
        }
    }
}
