using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions.Types;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class PropertyDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.PropertyDeclaration;
        public SyntaxFlags Flags { get; }
        public SyntaxModifier Modifier { get; }
        public MethodDeclaration Getter { get; }
        public MethodDeclaration Setter { get; }
        public TypeExpression Type { get; }

        public PropertyDeclaration(
            SyntaxFlags flags,
            SyntaxModifier modifier,
            string name,
            TypeExpression type, 
            MethodDeclaration getter, 
            MethodDeclaration setter,
            IEnumerable<AttributeSyntax> attributes,
            SourceFilePart filePart)
            : base(filePart, name, attributes)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (attributes == null)
                throw new ArgumentNullException(nameof(attributes));

            Flags = flags;
            Modifier = modifier;
            Type = type;
            Getter = getter;
            Setter = setter;
        }
    }
}
