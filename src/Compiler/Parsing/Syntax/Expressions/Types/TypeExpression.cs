using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler.Parsing.Syntax.Expressions.Types
{
    internal abstract class TypeExpression : Expression
    {
        private readonly IdentifierExpression _identifier;

        public string Name => _identifier.Name;
        public IdentifierExpression Identifier => _identifier;
        public TypeExpressionKind TypeKind { get; }
        public IEnumerable<TypeExpression> GenericParameters { get; }

        protected TypeExpression(TypeExpressionKind typeKind, IdentifierExpression name, IEnumerable<TypeExpression> genericParameters, SourceFilePart filePart) : base(filePart)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (genericParameters == null)
                throw new ArgumentNullException(nameof(genericParameters));

            _identifier = name;
            TypeKind = typeKind;
            GenericParameters = genericParameters;
        }
    }
}
