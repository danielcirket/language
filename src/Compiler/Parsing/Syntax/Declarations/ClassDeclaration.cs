using System;
using System.Collections.Generic;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class ClassDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.ClassDeclaration;
        public SyntaxModifier Modifier { get; }
        public IEnumerable<FieldDeclaration> Fields { get; }
        public IEnumerable<PropertyDeclaration> Properties { get; }
        public IEnumerable<MethodDeclaration> Methods { get; }
        public IEnumerable<ConstructorDeclaration> Constructors { get; }

        public ClassDeclaration(SourceFilePart filePart,
           string name,
           SyntaxModifier modifier,
           IEnumerable<FieldDeclaration> fields,
           IEnumerable<PropertyDeclaration> properties,
           IEnumerable<MethodDeclaration> methods,
           IEnumerable<ConstructorDeclaration> constructors,
           IEnumerable<AttributeSyntax> attributes
           )
           : base(filePart, name, attributes)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            if (methods == null)
                throw new ArgumentNullException(nameof(methods));

            if (constructors == null)
                throw new ArgumentNullException(nameof(constructors));

            Modifier = modifier;
            Fields = fields;
            Properties = properties;
            Methods = methods;
            Constructors = constructors;
        }
    }
}
