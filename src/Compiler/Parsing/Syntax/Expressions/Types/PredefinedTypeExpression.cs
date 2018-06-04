using System.Collections.Generic;

namespace Compiler.Parsing.Syntax.Expressions.Types
{
    internal class PredefinedTypeExpression : TypeExpression
    {
        public override SyntaxKind Kind { get; }

        public PredefinedTypeExpression(SyntaxKind kind, TypeExpressionKind typeKind, IdentifierExpression name, IEnumerable<TypeExpression> genericParameters, SourceFilePart filePart)
            : base(typeKind, name, genericParameters, filePart)
        {
            Kind = kind;
        }
    }
}
