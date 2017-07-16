using System;
using System.Collections.Generic;

namespace Compiler.Parsing.Syntax.Expressions
{
    internal class MethodCallExpression : Expression
    {
        public override SyntaxKind Kind => SyntaxKind.MethodCallExpression;
        public Expression Reference { get; }
        public IEnumerable<Expression> Arguments { get; }

        public MethodCallExpression(SourceFilePart filePart, Expression reference, IEnumerable<Expression> arguments) 
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
