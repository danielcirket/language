using System;
using System.Collections.Generic;
using Compiler.Semantics;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class ModuleDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.ModuleDeclaration;
        public IEnumerable<ClassDeclaration> Classes { get; }
        public IEnumerable<MethodDeclaration> Methods { get; }

        public ModuleDeclaration(SourceFilePart span, string name, IEnumerable<ClassDeclaration> classes, IEnumerable<MethodDeclaration> methods) 
            : base(span, name)
        {
            Classes = classes;
            Methods = methods;
        }
        public ModuleDeclaration(SourceFilePart span, string name, IEnumerable<ClassDeclaration> classes, IEnumerable<MethodDeclaration> methods, Scope scope) 
            : base(span, name, scope)
        {
            Classes = classes;
            Methods = methods;
        }
        public ModuleDeclaration(ModuleDeclaration declaration, IEnumerable<ClassDeclaration> classes, IEnumerable<MethodDeclaration> methods, Scope scope)
            : this(declaration.FilePart, declaration.Name, classes, methods, scope)
        {
        }
    }
}
