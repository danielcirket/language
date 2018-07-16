using System;
using System.Collections.Generic;
using System.Text;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics.Types
{
    internal class Environment
    {
        private readonly IDictionary<string, BoundTypeExpression> _typeMap;

        public Environment()
        {
            _typeMap = new Dictionary<string, BoundTypeExpression>();
        }
    }
}
