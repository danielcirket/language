using System;

namespace Compiler.Parsing.Syntax.Expressions
{
    internal class IdentifierExpression : Expression
    {
        public string Name { get; }
        public override SyntaxKind Kind => SyntaxKind.IdentifierExpression;

        public IdentifierExpression(SourceFilePart filePart, string name)
            : base(filePart)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }
    }
}
