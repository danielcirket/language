﻿using System;
using System.Collections.Generic;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class ModuleDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.ModuleDeclaration;
        public IEnumerable<ClassDeclaration> Classes { get; }
        public IEnumerable<MethodDeclaration> Methods { get; }
        public IEnumerable<EnumDeclaration> Enums { get; }
        public IEnumerable<InterfaceDeclaration> Interfaces { get; }

        public ModuleDeclaration(SourceFilePart filePart, string name, IEnumerable<ClassDeclaration> classes, IEnumerable<InterfaceDeclaration> interfaces, IEnumerable<MethodDeclaration> methods, IEnumerable<EnumDeclaration> enums)
            : base(filePart, name)
        {
            if (classes == null)
                throw new ArgumentNullException(nameof(classes));
            if (interfaces == null)
                throw new ArgumentNullException(nameof(interfaces));
            if (methods == null)
                throw new ArgumentNullException(nameof(methods));
            if (enums == null)
                throw new ArgumentNullException(nameof(enums));

            Classes = classes;
            Interfaces = interfaces;
            Methods = methods;
            Enums = enums;
        }
    }
}
