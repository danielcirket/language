using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions.Types;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class InterfaceDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.InterfaceDeclaration;
        public SyntaxModifier Modifier { get; }
        public IEnumerable<PropertyDeclaration> Properties { get; }
        public IEnumerable<MethodDeclaration> Methods { get; }
        public IEnumerable<TypeExpression> GenericTypeParameters { get; }

        public InterfaceDeclaration(
            SourceFilePart filePart, 
            SyntaxModifier modifier,
            string name,
            IEnumerable<TypeExpression> genericParameters,
            IEnumerable<PropertyDeclaration> properties,
            IEnumerable<MethodDeclaration> methods,
            IEnumerable<AttributeSyntax> attributes) 
            : base(filePart, name, attributes)
        {
            if (genericParameters == null)
                throw new ArgumentNullException(nameof(genericParameters));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            Modifier = modifier;
            GenericTypeParameters = genericParameters;
            Properties = properties;
            Methods = methods;
        }
    }
}
