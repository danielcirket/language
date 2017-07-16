using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class ConstructorDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.ConstructorDeclaration;
        public BlockStatement Body { get; }
        public IEnumerable<ParameterDeclaration> Parameters { get; }

        public ConstructorDeclaration(SourceFilePart filePart, string name, IEnumerable<ParameterDeclaration> parameters, BlockStatement body) 
            : base(filePart, name)
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
