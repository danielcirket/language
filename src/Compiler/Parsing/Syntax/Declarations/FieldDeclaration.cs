using System;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Semantics;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class FieldDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.FieldDeclaration;
        public Expression DefaultValue { get; }
        public TypeDeclaration Type { get; }

        public FieldDeclaration(SourceFilePart filePart, string name, TypeDeclaration type, Expression value)
           : base(filePart, name)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // NOTE(Dan): Allow null default values, e.g. int _field; rather than int _field = 1; 
            Type = type;
            DefaultValue = value;
        }
    }
}
