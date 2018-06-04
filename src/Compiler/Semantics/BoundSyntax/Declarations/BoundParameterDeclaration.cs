using System;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;
using Compiler.Semantics.Symbols;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal class BoundParameterDeclaration : BoundDeclaration
    {
        public BoundParameterDeclaration(
            ParameterDeclaration parameter,
            Scope scope
        )
            : base(parameter, scope)
        {
        }

        public BoundParameterDeclaration(
            ParameterDeclaration parameter,
            BoundTypeExpression type,
            Scope scope
        )
            : base(parameter, type, scope)
        {
           
        }
    }
}
