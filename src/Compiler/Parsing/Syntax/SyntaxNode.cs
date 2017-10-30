using System;

namespace Compiler.Parsing.Syntax
{
    internal abstract class SyntaxNode
    {
        public abstract SyntaxCategory Category { get; }
        public abstract SyntaxKind Kind { get; }
        public SourceFilePart FilePart { get; }

        public void Accept(SyntaxVisitor visitor) => visitor.Visit(this);
        public T Accept<T>(SyntaxVisitor<T> visitor) where T : SyntaxNode => visitor.Visit(this);

        protected SyntaxNode(SourceFilePart filePart)
        {
            if (filePart == null)
                throw new ArgumentNullException(nameof(filePart));

            FilePart = filePart;
        }
    }
}
