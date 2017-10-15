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

        public ClassDeclaration(SourceFilePart span, string name, IEnumerable<ConstructorDeclaration> constructors,
                                IEnumerable<FieldDeclaration> fields,
                                IEnumerable<MethodDeclaration> methods,
                                IEnumerable<PropertyDeclaration> properties)
            : base(span, name)
        {
            Constructors = constructors;
            Fields = fields;
            Methods = methods;
            Properties = properties;
        }
        public ClassDeclaration(SourceFilePart span, string name, IEnumerable<ConstructorDeclaration> constructors,
                                IEnumerable<FieldDeclaration> fields,
                                IEnumerable<MethodDeclaration> methods,
                                IEnumerable<PropertyDeclaration> properties,
                                Scope scope)
            : base(span, name, scope)
        {
            Constructors = constructors;
            Fields = fields;
            Methods = methods;
            Properties = properties;
        }
        public ClassDeclaration(ClassDeclaration declartion, Scope scope)
            : this(declartion.FilePart, declartion.Name, declartion.Constructors,
                  declartion.Fields, declartion.Methods, declartion.Properties,
                  scope)
        {

        }
        public ClassDeclaration(ClassDeclaration declartion, IEnumerable<FieldDeclaration> fields,
            IEnumerable<PropertyDeclaration> properties, IEnumerable<MethodDeclaration> methods,
            IEnumerable<ConstructorDeclaration> constructors, Scope scope)
            : this(declartion.FilePart, declartion.Name, constructors,
                  fields, methods, properties,
                  scope)
        {

        }
    }
}
