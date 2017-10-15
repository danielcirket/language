using System;
using Compiler.Semantics;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class PropertyDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.PropertyDeclaration;
        public MethodDeclaration Getter { get; }
        public MethodDeclaration Setter { get; }
        public TypeDeclaration Type { get; }

        public PropertyDeclaration(SourceFilePart span, string name, TypeDeclaration type, MethodDeclaration getMethod, MethodDeclaration setMethod) : base(span, name)
        {
            Type = type;
            Getter = getMethod;
            Setter = setMethod;
        }
        public PropertyDeclaration(SourceFilePart span, string name, TypeDeclaration type, MethodDeclaration getMethod, MethodDeclaration setMethod, Scope scope) : base(span, name, scope)
        {
            Type = type;
            Getter = getMethod;
            Setter = setMethod;
        }
        public PropertyDeclaration(PropertyDeclaration declaration, Scope scope)
            : this(declaration.FilePart, declaration.Name, declaration.Type, declaration.Getter, declaration.Setter, scope)
        {

        }
        public PropertyDeclaration(PropertyDeclaration declaration, MethodDeclaration getMethod, MethodDeclaration setMethod, Scope scope)
            : this(declaration.FilePart, declaration.Name, declaration.Type, getMethod, setMethod, scope)
        {

        }
    }
}
