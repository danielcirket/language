using System;
using System.Collections.Generic;
using Compiler.Semantics;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class ClassDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.ClassDeclaration;
        public IEnumerable<FieldDeclaration> Fields { get; }
        public IEnumerable<PropertyDeclaration> Properties { get; }
        public IEnumerable<MethodDeclaration> Methods { get; }
        public IEnumerable<ConstructorDeclaration> Constructors { get; }

        public ClassDeclaration(SourceFilePart filePart,
           string name,
           IEnumerable<FieldDeclaration> fields,
           IEnumerable<PropertyDeclaration> properties,
           IEnumerable<MethodDeclaration> methods,
           IEnumerable<ConstructorDeclaration> constructors
           )
           : base(filePart, name)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            if (methods == null)
                throw new ArgumentNullException(nameof(methods));

            if (constructors == null)
                throw new ArgumentNullException(nameof(constructors));

            Fields = fields;
            Properties = properties;
            Methods = methods;
            Constructors = constructors;
        }
    }
}
