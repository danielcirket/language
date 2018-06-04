using System;
using Compiler.Semantics.BoundSyntax.Declarations;

namespace Compiler.Semantics.Symbols
{
    internal class Symbol
    {
        public string Name { get; }
        public BoundDeclaration Declaration { get; internal set; }

        public Symbol(string name, BoundDeclaration declaration)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            //if (declaration == null)
            //    throw new ArgumentNullException(nameof(declaration));

            Name = name;
            Declaration = declaration;
        }
    }
}
