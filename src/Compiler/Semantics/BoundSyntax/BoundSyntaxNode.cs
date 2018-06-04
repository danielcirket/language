using System;
using Compiler.Parsing.Syntax;

namespace Compiler.Semantics.BoundSyntax
{
    internal abstract class BoundSyntaxNode
    {
        private SyntaxNode _syntaxNode;

        public T SyntaxNode<T>() where T : SyntaxNode => (T)_syntaxNode;

        public void Accept(BoundSyntaxVisitor visitor) => visitor.Visit(this);
        public T Accept<T>(BoundSyntaxVisitor<T> visitor) where T : BoundSyntaxNode => visitor.Visit(this);

        protected BoundSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode == null)
                throw new ArgumentNullException(nameof(syntaxNode));

            _syntaxNode = syntaxNode;
        }
    }
}
