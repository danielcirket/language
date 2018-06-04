using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Declarations;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal class BoundModuleDeclaration : BoundDeclaration
    {
        public IDictionary<string, BoundClassDeclaration> Classes { get; }
        public IDictionary<string, BoundMethodDeclaration> Methods { get; }
        public IDictionary<string, BoundEnumDeclaration> Enums { get; }
        public IDictionary<string, BoundInterfaceDeclaration> Interfaces { get; }

        public BoundModuleDeclaration(
            ModuleDeclaration moduleDeclaration,
            ref IDictionary<string, BoundClassDeclaration> classes,
            ref IDictionary<string, BoundInterfaceDeclaration> interfaces,
            ref IDictionary<string, BoundMethodDeclaration> methods,
            ref IDictionary<string, BoundEnumDeclaration> enums,
            Scope scope
        )
            : base(moduleDeclaration, scope)
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
