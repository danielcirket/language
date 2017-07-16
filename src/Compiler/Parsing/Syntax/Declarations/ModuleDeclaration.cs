using System;
using System.Collections.Generic;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class ModuleDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.ModuleDeclaration;
        public IEnumerable<ClassDeclaration> Classes { get; }
        public IEnumerable<MethodDeclaration> Methods { get; }

        public ModuleDeclaration(SourceFilePart filePart, string name, IEnumerable<ClassDeclaration> classes, IEnumerable<MethodDeclaration> methods) 
            : base(filePart, name)
        {
            if (classes == null)
                throw new ArgumentNullException(nameof(classes));

            if (methods == null)
                throw new ArgumentNullException(nameof(methods));

            Classes = classes;
            Methods = methods;
        }
    }
}
