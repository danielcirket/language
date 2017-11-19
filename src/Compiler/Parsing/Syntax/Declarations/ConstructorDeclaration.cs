using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Statements;
//using Compiler.Semantics;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class ConstructorDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.ConstructorDeclaration;
        public SyntaxModifier Modifier { get; }
        public BlockStatement Body { get; }
        public IEnumerable<ParameterDeclaration> Parameters { get; }

        public ConstructorDeclaration(
            SourceFilePart filePart, 
            SyntaxModifier modifier,
            string name, 
            IEnumerable<ParameterDeclaration> parameters, 
            BlockStatement body,
            IEnumerable<AttributeSyntax> attributes)
            : base(filePart, name, attributes)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (body == null)
                throw new ArgumentNullException(nameof(body));

            Modifier = modifier;
            Parameters = parameters;
            Body = body;
        }
    }
}
