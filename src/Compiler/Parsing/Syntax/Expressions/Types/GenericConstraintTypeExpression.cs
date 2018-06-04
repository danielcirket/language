using System.Collections.Generic;

namespace Compiler.Parsing.Syntax.Expressions.Types
{
    internal class GenericConstraintTypeExpression : TypeExpression
    {
        public override SyntaxKind Kind => SyntaxKind.UserDefinedTypeExpression;

        public GenericConstraintTypeExpression(TypeExpressionKind typeKind, IdentifierExpression name, IEnumerable<TypeExpression> genericParameters, SourceFilePart filePart) 
            : base(typeKind, name, genericParameters, filePart)
        {

        }
    }
}
