using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal abstract class Declaration : SyntaxNode
    {
        public override SyntaxCategory Category => SyntaxCategory.Declaration;
        public string Name { get; }
        public IEnumerable<AttributeSyntax> Attributes { get; }

        protected Declaration(SourceFilePart filePart, string name)
            : base(filePart)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Attributes = Enumerable.Empty<AttributeSyntax>();
        }
        protected Declaration(SourceFilePart filePart, string name, IEnumerable<AttributeSyntax> attributes)
            : this(filePart, name)
        {
            if (attributes == null)
                throw new ArgumentNullException(nameof(attributes));

            Attributes = attributes;
        }
    }
}
