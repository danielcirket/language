using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class MethodDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.MethodDeclaration;
        public SyntaxModifier Modifier { get; }
        public BlockStatement Body { get; }
        public IEnumerable<ParameterDeclaration> Parameters { get; }
        public TypeDeclaration ReturnType { get; }

        public MethodDeclaration(
            SourceFilePart filePart, 
            SyntaxModifier modifier,
            string name, 
            TypeDeclaration returnType, 
            IEnumerable<ParameterDeclaration> parameters, 
            BlockStatement body)
            : base(filePart, name)
        {
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Modifier = modifier;
            ReturnType = returnType;
            Parameters = parameters;
            Body = body;
        }
        public MethodDeclaration(
            SourceFilePart filePart, 
            SyntaxModifier modifier,
            string name,
            TypeDeclaration returnType, 
            IEnumerable<ParameterDeclaration> parameters, 
            BlockStatement body, 
            IEnumerable<AttributeSyntax> attributes)
            : base(filePart, name, attributes)
        {
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Modifier = modifier;
            ReturnType = returnType;
            Parameters = parameters;
            Body = body;
        }
    }
}
