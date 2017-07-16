using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Parsing.Syntax.Expressions
{
    internal class LambdaExpression : Expression
    {
        public override SyntaxKind Kind => SyntaxKind.LambdaExpression;
        public BlockStatement Body { get; }
        public IEnumerable<ParameterDeclaration> Parameters { get; }

        public LambdaExpression(SourceFilePart filePart, IEnumerable<ParameterDeclaration> parameters, BlockStatement body) 
            : base(filePart)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (body == null)
                throw new ArgumentNullException(nameof(body));

            Parameters = parameters;
            Body = body;
        }
    }
}
