using System;
using System.Collections.Generic;

namespace Compiler.Parsing.Syntax.Expressions
{
    internal class NewExpression : Expression
    {
        public override SyntaxKind Kind => SyntaxKind.NewExpression;
        public SyntaxNode Reference { get; }
        public IEnumerable<Expression> Arguments { get; }

        public NewExpression(SourceFilePart filePart, SyntaxNode reference, IEnumerable<Expression> arguments) 
            : base(filePart)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));

            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            Reference = reference;
            Arguments = arguments;
        }
    }
}
