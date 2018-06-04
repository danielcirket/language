using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Declarations;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal class BoundInterfaceDeclaration : BoundDeclaration
    {
        public IEnumerable<BoundPropertyDeclaration> Properties { get; }
        public IEnumerable<BoundMethodDeclaration> Methods { get; }

        public BoundInterfaceDeclaration(
            InterfaceDeclaration @interface,
            IEnumerable<BoundPropertyDeclaration> properties,
            IEnumerable<BoundMethodDeclaration> methods,
            Scope scope
        ) 
            : base(@interface, scope)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            Methods = methods;
            Properties = properties;
        }
    }
}
