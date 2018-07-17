using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Parsing.Syntax.Expressions.Types;

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
        public IEnumerable<IdentifierExpression> Inheritors { get; }
        public IEnumerable<TypeExpression> GenericTypeParameters { get; }

        public ClassDeclaration(SourceFilePart filePart,
           string name,
           SyntaxModifier modifier,
           IEnumerable<FieldDeclaration> fields,
           IEnumerable<PropertyDeclaration> properties,
           IEnumerable<MethodDeclaration> methods,
           IEnumerable<ConstructorDeclaration> constructors,
           IEnumerable<TypeExpression> genericParameters,
           IEnumerable<IdentifierExpression> inheritors,
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
            if (genericParameters == null)
                throw new ArgumentNullException(nameof(genericParameters));
            if (inheritors == null)
                throw new ArgumentNullException(nameof(inheritors));

            Modifier = modifier;
            Fields = fields;
            Properties = properties;
            Methods = methods;
            Constructors = constructors;
            GenericTypeParameters = genericParameters;
            Inheritors = inheritors;
        }
    }
}
