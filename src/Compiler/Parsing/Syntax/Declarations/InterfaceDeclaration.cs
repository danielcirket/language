using System;
using System.Collections.Generic;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class InterfaceDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.InterfaceDeclaration;
        public SyntaxModifier Modifier { get; }
        public IEnumerable<PropertyDeclaration> Properties { get; }
        public IEnumerable<MethodDeclaration> Methods { get; }

        public InterfaceDeclaration(
            SourceFilePart filePart, 
            SyntaxModifier modifier,
            string name, 
            IEnumerable<PropertyDeclaration> properties,
            IEnumerable<MethodDeclaration> methods,
            IEnumerable<AttributeSyntax> attributes) 
            : base(filePart, name, attributes)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            Modifier = modifier;
            Properties = properties;
            Methods = methods;
        }
    }
}
