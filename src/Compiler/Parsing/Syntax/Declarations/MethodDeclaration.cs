using System;
using System.Collections.Generic;
using System.Text;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class MethodDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.MethodDeclaration;
        public BlockStatement Body { get; }
        public IEnumerable<ParameterDeclaration> Parameters { get; }
        public TypeDeclaration ReturnType { get; }

        public MethodDeclaration(SourceFilePart filePart, string name, TypeDeclaration returnType, IEnumerable<ParameterDeclaration> parameters, BlockStatement body) 
            : base(filePart, name)
        {
            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (body == null)
                throw new ArgumentNullException(nameof(body));

            ReturnType = returnType;
            Parameters = parameters;
            Body = body;
        }
    }
}
