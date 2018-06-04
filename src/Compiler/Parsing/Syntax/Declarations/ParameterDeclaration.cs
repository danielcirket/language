using System;
using Compiler.Parsing.Syntax.Expressions.Types;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class ParameterDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.ParameterDeclaration;
        public TypeExpression Type { get; }

        public ParameterDeclaration(SourceFilePart filePart, string name, TypeExpression type)
            : base(filePart, name)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Type = type;
        }
    }
}
