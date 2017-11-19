using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class FieldDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.FieldDeclaration;
        public SyntaxModifier Modifier { get; }
        public Expression DefaultValue { get; }
        public TypeDeclaration Type { get; }

        public FieldDeclaration(
            SourceFilePart filePart,
            SyntaxModifier modifier,
            string name, 
            TypeDeclaration type, 
            Expression value,
            IEnumerable<AttributeSyntax> attributes)
           : base(filePart, name, attributes)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // NOTE(Dan): Allow null default values, e.g. int _field; rather than int _field = 1; 
            Modifier = modifier;
            Type = type;
            DefaultValue = value;
        }
    }
}
