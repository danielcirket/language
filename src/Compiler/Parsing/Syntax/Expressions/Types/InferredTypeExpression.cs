using System.Linq;

namespace Compiler.Parsing.Syntax.Expressions.Types
{
    internal class InferredTypeExpression : TypeExpression
    {
        public override SyntaxKind Kind => SyntaxKind.InferredType;

        public InferredTypeExpression(IdentifierExpression name, SourceFilePart filePart) 
            : base(TypeExpressionKind.Inferred, name, Enumerable.Empty<TypeExpression>(), filePart)
        {

        }
    }
}
