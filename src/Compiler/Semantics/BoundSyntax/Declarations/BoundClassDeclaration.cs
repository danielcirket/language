using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal class BoundClassDeclaration : BoundDeclaration
    {
        public SyntaxModifier Modifier => SyntaxNode<ClassDeclaration>().Modifier;
        public int Arity => SyntaxNode<ClassDeclaration>().GenericTypeParameters.Count();
        public IEnumerable<BoundTypeExpression> GenericParameters { get; }
        public IEnumerable<BoundFieldDeclaration> Fields { get; }
        public IEnumerable<BoundPropertyDeclaration> Properties { get; }
        public IEnumerable<BoundMethodDeclaration> Methods { get; }
        public IEnumerable<BoundConstructorDeclaration> Constructors { get; }

        public BoundClassDeclaration(
            ClassDeclaration classDeclaration,
            IEnumerable<BoundTypeExpression> genericParameters,
            IEnumerable<BoundFieldDeclaration> fields,
            IEnumerable<BoundPropertyDeclaration> properties,
            IEnumerable<BoundMethodDeclaration> methods,
            IEnumerable<BoundConstructorDeclaration> constructors,
            Scope scope
        )
           : base(classDeclaration, scope)
        {
            if (genericParameters == null)
                throw new ArgumentNullException(nameof(genericParameters));
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            if (methods == null)
                throw new ArgumentNullException(nameof(methods));
            if (constructors == null)
                throw new ArgumentNullException(nameof(constructors));

            GenericParameters = genericParameters;
            Fields = fields;
            Properties = properties;
            Methods = methods;
            Constructors = constructors;
        }
    }
}
