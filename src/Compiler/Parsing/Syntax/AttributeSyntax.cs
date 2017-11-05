using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Parsing.Syntax
{
    internal class AttributeSyntax : SyntaxNode
    {
        public string Name { get; }
        public override SyntaxKind Kind => SyntaxKind.Attribute;
        public override SyntaxCategory Category => SyntaxCategory.AttributeUsage;
        public IEnumerable<Expression> Parameters { get; }

        public AttributeSyntax(SourceFilePart filePart, string name, IEnumerable<Expression> parameters) : base(filePart)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Name = name;
            Parameters = parameters;
        }
    }
}
