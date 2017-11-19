using System;
using System.Collections.Generic;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class PropertyDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.PropertyDeclaration;
        public SyntaxModifier Modifier { get; }
        public MethodDeclaration Getter { get; }
        public MethodDeclaration Setter { get; }
        public TypeDeclaration Type { get; }

        public PropertyDeclaration(
            SourceFilePart filePart, 
            SyntaxModifier modifier,
            string name, 
            TypeDeclaration type, 
            MethodDeclaration getter, 
            MethodDeclaration setter,
            IEnumerable<AttributeSyntax> attributes)
            : base(filePart, name, attributes)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (attributes == null)
                throw new ArgumentNullException(nameof(attributes));

            Modifier = modifier;
            Type = type;
            Getter = getter;
            Setter = setter;
        }
    }
}
