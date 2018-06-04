using System;
using System.Collections.Generic;

namespace Compiler.Parsing.Syntax.Expressions
{
    internal class ReferenceExpression : Expression
    {
        public override SyntaxKind Kind => SyntaxKind.ReferenceExpression;
        public IEnumerable<SyntaxNode> References { get; }

        public ReferenceExpression(SourceFilePart filePart, IEnumerable<SyntaxNode> references) 
            : base(filePart)
        {
            if (references == null)
                throw new ArgumentNullException(nameof(references));

            References = references;
        }
    }
}
