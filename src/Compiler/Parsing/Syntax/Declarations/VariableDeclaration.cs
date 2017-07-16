using System;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class VariableDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;
        public TypeDeclaration Type { get; }
        public Expression Value { get; }

        public VariableDeclaration(SourceFilePart filePart, string name, TypeDeclaration type, Expression value) 
            : base(filePart, name)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Type = type;
            Value = value;
        }
    }
}
