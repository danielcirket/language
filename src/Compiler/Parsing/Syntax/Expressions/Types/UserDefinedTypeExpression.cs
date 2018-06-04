using System.Collections.Generic;

namespace Compiler.Parsing.Syntax.Expressions.Types
{
    internal class UserDefinedTypeExpression : TypeExpression
    {
        public override SyntaxKind Kind => SyntaxKind.UserDefinedTypeExpression;

        public UserDefinedTypeExpression(TypeExpressionKind typeKind, IdentifierExpression name, IEnumerable<TypeExpression> genericParameters, SourceFilePart filePart) 
            : base(typeKind, name, genericParameters, filePart)
        {

        }
    }
}
