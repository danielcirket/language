using System;
using Compiler.Semantics;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal abstract class Declaration : SyntaxNode
    {
        public override SyntaxCategory Category => SyntaxCategory.Declaration;
        public string Name { get; }
        public Scope Scope { get; }

        protected Declaration(SourceFilePart filePart, string name)
            : base(filePart)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }
        protected Declaration(SourceFilePart span, string name, Scope scope) 
            : this(span, name)
        {
            Scope = scope;
        }
    }
}
